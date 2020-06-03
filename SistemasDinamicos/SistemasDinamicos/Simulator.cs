using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GeneradorDeNumerosAleatorios;
using RandomVarGenerator;
using SimulacionMontecarlo;
using SistemasDinamicos;

namespace SimulacionMontecarlo
{
    class Simulator
    {
        bool DEBUG = true;
        public UniformGenerator uniformGeneratorAterrizaje { get; set; }
        public UniformGenerator uniformGeneratorDespegue { get; set; }
        public ConvolutionGenerator convolutionGenerator { get; set; }
        public ExponentialGenerator exponentialGenerator { get; set; }
        public BoxMullerGenerator boxMullerGenerator { get; set; }
        public Generator generator { get; set; }

        

        public Simulator()
        {
            uniformGeneratorAterrizaje = new UniformGenerator();
            uniformGeneratorAterrizaje.a = 3;
            uniformGeneratorAterrizaje.b = 5;
            uniformGeneratorDespegue = new UniformGenerator();
            uniformGeneratorDespegue.a = 2;
            uniformGeneratorDespegue.b = 4;
            convolutionGenerator = new ConvolutionGenerator();
            exponentialGenerator = new ExponentialGenerator();
            exponentialGenerator.lambda = (double) 0.1;
            convolutionGenerator.mean = 80;
            convolutionGenerator.stDeviation = 30;
            generator = new Generator();
        }


        public IList<StateRow> simulate(int quantity, int from, StateRow anterior)
        {
            Dictionary<string, double> tiempos = new Dictionary<string, double>();
            IList<StateRow> stateRows = new List<StateRow>();
            StateRow actual = new StateRow();

            for (int i=0; i<quantity; i++)
            {
                // Se arma diccionario con todos los tiempos del vector para determinar el menor, es decir, el siguiente evento
                tiempos.Clear();
                if (anterior.tiempoProximaLlegada != 0)
                    tiempos.Add("tiempoProximaLlegada", anterior.tiempoProximaLlegada);
                for (int j = 0; j < anterior.clientes.Count; j++)
                {
                    if (anterior.clientes[j].tiempoFinAterrizaje != 0)
                        tiempos.Add("tiempoFinAterrizaje_" + (j + 1).ToString(), anterior.clientes[j].tiempoFinAterrizaje);
                }
                /*
                if (anterior.tiempoFinAterrizaje != 0)
                    tiempos.Add("tiempoFinAterrizaje", anterior.tiempoFinAterrizaje);
                */
                for (int j = 0; j < anterior.clientes.Count; j++)
                {
                    if (anterior.clientes[j].tiempoFinDeDespegue != 0)
                        tiempos.Add("tiempoFinDeDespegue_" + (j + 1).ToString(), anterior.clientes[j].tiempoFinDeDespegue);
                }
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
                        break;
                    case var val when new Regex(@"tiempoFinAterrizaje_*").IsMatch(val):
                        int avionFA = Convert.ToInt32(menorTiempo.Key.Split('_')[1]);
                        actual = CrearStateRowFinAterrizaje(anterior, menorTiempo.Value, avionFA);
                        break;
                    case "tiempoFinDeDespegue":
                        actual = CrearStateRowFinDeDespegue(anterior, menorTiempo.Value);
                        break;
                    case var someVal when new Regex(@"tiempoPermanencia_*").IsMatch(someVal):
                        int avion = Convert.ToInt32(menorTiempo.Key.Split('_')[1]);
                        actual = CrearStateRowFinDePermanencia(anterior, menorTiempo.Value, avion);
                        break;
                }

                actual.iterationNum = i + 1;

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

                // Esto esta aca para propositos de debug
                stateRows.Add(actual);
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
            nuevo.rndLlegada = this.generator.NextFakeRnd();
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
                nuevo.rndAterrizaje = this.generator.NextFakeRnd();
                nuevo.tiempoAterrizaje = this.uniformGeneratorAterrizaje.Generate(nuevo.rndAterrizaje);
                nuevo.tiempoFinAterrizaje = nuevo.tiempoAterrizaje + nuevo.reloj;
                avionNuevo.tiempoFinAterrizaje = nuevo.tiempoFinAterrizaje;
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

        private StateRow CrearStateRowFinAterrizaje(StateRow anterior, double tiempoProximoEvento, int avion)
        {
            StateRow nuevo = new StateRow();

            nuevo.evento = "Fin Aterrizaje (" + avion.ToString() + ")" ;
            nuevo.reloj = tiempoProximoEvento;
            nuevo.tiempoProximaLlegada = anterior.tiempoProximaLlegada;
            nuevo.clientes = anterior.clientes;

            //Calcular variables tiempo permanencia
            /*
            double ac;
            nuevo.tiempoDePermanencia = convolutionGenerator.Generate(out ac);
            nuevo.rndPermanencia = ac;
            */
            nuevo.rndPermanencia = generator.NextFakeRnd();
            nuevo.tiempoDePermanencia = convolutionGenerator.GenerateFake(nuevo.rndPermanencia);
            nuevo.tiempoFinPermanencia = nuevo.reloj + nuevo.tiempoDePermanencia;
            nuevo.clientes[avion - 1].tiempoPermanencia = nuevo.tiempoFinPermanencia;
            nuevo.clientes[avion - 1].tiempoFinAterrizaje = 0;
            nuevo.clientes[avion - 1].estado = "EP";

            // Calculos variables de pista
            nuevo.pista = anterior.pista;
            if( nuevo.pista.colaEEV.Count != 0)
            {
                // Calculos variables aterrizaje
                Avion avionNuevo = nuevo.pista.colaEEV.Dequeue();
                nuevo.rndAterrizaje = this.generator.NextFakeRnd();
                nuevo.tiempoAterrizaje = this.uniformGeneratorAterrizaje.Generate(nuevo.rndAterrizaje);
                nuevo.tiempoFinAterrizaje = nuevo.tiempoAterrizaje + nuevo.reloj;
                nuevo.pista.libre = false;
                nuevo.clientes[avionNuevo.id - 1].tiempoFinAterrizaje = nuevo.tiempoFinAterrizaje;
                nuevo.clientes[avionNuevo.id - 1].estado = "EA";
            }
            else if (nuevo.pista.colaEET.Count != 0)
            {
                // Calculos variables de despegue
                Avion avionNuevo = nuevo.pista.colaEET.Dequeue();
                nuevo.rndDespegue = this.generator.NextFakeRnd();
                nuevo.tiempoDeDespegue = this.uniformGeneratorDespegue.Generate(nuevo.rndDespegue);
                nuevo.tiempoFinDeDespegue = nuevo.tiempoDeDespegue + nuevo.reloj;
                nuevo.pista.libre = false;
                nuevo.clientes[avionNuevo.id - 1].tiempoFinDeDespegue = nuevo.tiempoFinDeDespegue;
                nuevo.clientes[avionNuevo.id - 1].estado = "ED";
            }
            else
            {
                nuevo.pista.libre = true;
            }

            return nuevo;
        }

        private StateRow CrearStateRowFinDePermanencia(StateRow anterior, double tiempoProximoEvento, int avion)
        {
            StateRow nuevo = new StateRow();

            nuevo.evento = "Fin permanencia (" + avion.ToString() + ")";
            nuevo.reloj = tiempoProximoEvento;

            // Calcular siguiente tiempo de llegada de prox avion
            nuevo.tiempoProximaLlegada = anterior.tiempoProximaLlegada;

            // Calcular variables de aterrizaje
            nuevo.tiempoFinAterrizaje = anterior.tiempoFinAterrizaje;

            nuevo.tiempoFinDeDespegue = anterior.tiempoFinDeDespegue;

            // Calculos variables de pista
            nuevo.pista = anterior.pista;
            nuevo.clientes = anterior.clientes;
            if (!nuevo.pista.libre)
            {
                nuevo.clientes[avion-1].estado = "EET";
                nuevo.pista.colaEET.Enqueue(nuevo.clientes[avion-1]);
            }
            else
            {
                // Calcular variables de despegue
                nuevo.clientes[avion - 1].estado = "ED";
                nuevo.rndDespegue = this.generator.NextFakeRnd();
                nuevo.tiempoDeDespegue = this.uniformGeneratorDespegue.Generate(nuevo.rndDespegue);
                nuevo.tiempoFinDeDespegue = nuevo.tiempoDeDespegue + nuevo.reloj;
                nuevo.pista.libre = false;
            }
            nuevo.clientes[avion - 1].tiempoPermanencia = 0;

            return nuevo;
        }
    }
}
