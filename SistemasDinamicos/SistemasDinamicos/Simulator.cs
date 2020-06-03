using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneradorDeNumerosAleatorios;
using RandomVarGenerator;
using SimulacionMontecarlo;
using SistemasDinamicos;

namespace SimulacionMontecarlo
{
    class Simulator
    {
        public UniformGenerator uniformGenerator { get; set; }
        public ConvolutionGenerator convolutionGenerator { get; set; }
        public ExponentialGenerator exponentialGenerator { get; set; }
        public BoxMullerGenerator boxMullerGenerator { get; set; }
        public Generator generator { get; set; }

        public Simulator()
        {
            uniformGenerator = new UniformGenerator();
            uniformGenerator.a = 3;
            uniformGenerator.b = 5;
            convolutionGenerator = new ConvolutionGenerator();
            exponentialGenerator = new ExponentialGenerator();
            exponentialGenerator.lambda = (double) 0.1;
            boxMullerGenerator = new BoxMullerGenerator();
            generator = new Generator();
        }


        public IList<StateRow> simulate(int quantity, int from, StateRow initialize)
        {
            IList<StateRow> stateRows = new List<StateRow>();

            Dictionary<string, double> tiempos = new Dictionary<string, double>();
            tiempos.Add("tiempoLlegada", initialize.tiempoProximaLlegada);
            for (int i = 0; i<initialize.clientes.Count; i++)
            {
                tiempos.Add("tiempoPermanencia"+(i+1).ToString(), initialize.clientes[i].tiempoPermanencia);
            }

            KeyValuePair<string, double> tiempoProximoEvento = tiempos.FirstOrDefault(x => x.Value == tiempos.Values.Min());

            switch (tiempoProximoEvento.Key)
            {
                case "tiempoLlegada":
                    StateRow actual = CrearStateRowLlegadaAvion(initialize, tiempoProximoEvento.Value);
                    break;
                default:
                    break;
            }

            for (int i=0; i<quantity; i++)
            {
               
                if ((i >= from-1 && i <= from + 99) || i == (quantity - 1))
                {
                    StateRow row = new StateRow { };

                    stateRows.Add(row);
                }
            }

            return stateRows;
        }

        private StateRow CrearStateRowLlegadaAvion(StateRow anterior, double tiempoProximoEvento)
        {
            Avion avionNuevo = new Avion();
            /*
             *              Solo tener en cuenta cuando la suma de los aviones esperando en tierra y en las colas (o como sea que diga
             *              la consigna) sea menor a 30.
             */

            StateRow nuevo = new StateRow();

            nuevo.evento = "Llegada Avion (" + Avion.count.ToString() + ")";
            nuevo.reloj = tiempoProximoEvento;

            // Calcular siguiente tiempo de llegada de prox avion
            nuevo.rndLlegada = this.generator.NextRnd();
            nuevo.tiempoEntreLlegadas = this.exponentialGenerator.Generate(nuevo.rndLlegada);
            nuevo.tiempoProximaLlegada = nuevo.tiempoEntreLlegadas + nuevo.reloj;

            // Calcular variables de aterrizaje
            // Calculos variables de pista
            nuevo.pista = anterior.pista;
            if (!nuevo.pista.libre)
            {
                avionNuevo.estado = "EEV";
                nuevo.pista.colaEEV.Enqueue(avionNuevo);
                nuevo.tiempoFinAterrizaje = anterior.tiempoFinAterrizaje;
            }
            else
            {
                avionNuevo.estado = "EA";
                nuevo.rndAterrizaje = this.generator.NextRnd();
                nuevo.tiempoAterrizaje = this.uniformGenerator.Generate(nuevo.rndAterrizaje);
                nuevo.tiempoFinAterrizaje = nuevo.tiempoAterrizaje + nuevo.reloj;
                nuevo.pista.libre = false;
            }

            // Calcular variables de despegue
            /*
             *              CHEQUEAR
             */
            nuevo.tiempoFinDeDespegue = anterior.tiempoFinDeDespegue;

            // Clientes
            nuevo.clientes = anterior.clientes;
            nuevo.clientes.Add(avionNuevo);
            

            return nuevo;
        }
    }
}
