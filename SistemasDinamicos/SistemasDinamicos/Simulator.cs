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


        public IList<StateRow> simulate(int quantity, int from, StateRow anterior)
        {
            IList<StateRow> stateRows = new List<StateRow>();
            StateRow actual = new StateRow();

            for (int i=0; i<quantity; i++)
            {
                // Se arma diccionario con todos los tiempos del vector para determinar el menor, es decir, el siguiente evento
                Dictionary<string, double> tiempos = new Dictionary<string, double>();
                if (anterior.tiempoProximaLlegada != 0)
                    tiempos.Add("tiempoProximaLlegada", anterior.tiempoProximaLlegada);
                if (anterior.tiempoFinAterrizaje != 0)
                    tiempos.Add("tiempoFinAterrizaje", anterior.tiempoFinAterrizaje);
                if (anterior.tiempoFinDeDespegue != 0)
                    tiempos.Add("tiempoFinDeDespegue", anterior.tiempoFinDeDespegue);
                for (int j = 0; j < anterior.clientes.Count; j++)
                {
                    if (anterior.clientes[j].tiempoPermanencia != 0)
                        tiempos.Add("tiempoPermanencia_" + (j + 1).ToString(), anterior.clientes[j].tiempoPermanencia);
                }

                // Obtiene diccionario de tiempos ordenado segun menor tiempo
                var tiemposOrdenados = tiempos.OrderBy(obj => obj.Value).ToDictionary(obj => obj.Key, obj => obj.Value);
                KeyValuePair<string, double> menorTiempo = tiemposOrdenados.First();
                
                // Controlamos que los aviones en tierra sean menores a 30, si lo son, pasamos al siguiente menor tiempo, es decir, el siguiente evento
                while (menorTiempo.Key == "tiempoProximaLlegada" && (anterior.pista.colaEET.Count + GetCantidadAvionesEnPermanencia(anterior) >= 30))
                {
                    tiemposOrdenados.Remove(tiemposOrdenados.First().Key);
                    menorTiempo = tiemposOrdenados.First();
                }

                // Se crea nuevo staterow segun el evento siguiente determinado
                switch (menorTiempo.Key)
                {
                    case "tiempoProximaLlegada":
                        actual = CrearStateRowLlegadaAvion(anterior, menorTiempo.Value);
                        actual.iterationNum = i + 1;
                        break;
                    case "tiempoFinAterrizaje":
                        actual = CrearStateRowFinAterrizaje(anterior, menorTiempo.Value);
                        break;
                    case "tiempoFinDeDespegue":
                        actual = CrearStateRowFinDeDespegue(anterior, menorTiempo.Value);
                        break;
                    default:
                        int avion = Convert.ToInt32(menorTiempo.Key.Split('_')[1]);
                        actual = CrearStateRowFinDePermanencia(anterior, menorTiempo.Value, avion);
                        break;
                }

                #region Agregar al showed
                /*
                if ((i >= from-1 && i <= from + 99) || i == (quantity - 1))
                {
                    StateRow row = new StateRow { };

                    stateRows.Add(row);
                }
                */
                #endregion

                anterior = actual;
            }

            return stateRows;
        }

        private int GetCantidadAvionesEnPermanencia(StateRow vector)
        {
            int contador = 0;
            for (int i=0; i<vector.clientes.Count; i++)
            {
                if (vector.clientes[i].estado == "EP")
                    contador += 1;
            }
            return contador;
        }

        private StateRow CrearStateRowLlegadaAvion(StateRow anterior, double tiempoProximoEvento)
        {
            Avion avionNuevo = new Avion();
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

        private StateRow CrearStateRowFinDeDespegue(StateRow anterior, double value)
        {
            return new StateRow();
        }

        private StateRow CrearStateRowFinAterrizaje(StateRow anterior, double value)
        {
            return new StateRow();
        }

        private StateRow CrearStateRowFinDePermanencia(StateRow anterior, double value, int avion)
        {
            return new StateRow();
        }
    }
}
