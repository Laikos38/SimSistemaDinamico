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

            stateRows.Add(anterior);

            for (int i=0; i<quantity; i++)
            {
                StateRow actual = new StateRow();

                // Se arma diccionario con todos los tiempos del vector para determinar el menor, es decir, el siguiente evento
                tiempos.Clear();
                if (anterior.tiempoProximaLlegada != 0)
                    tiempos.Add("tiempoProximaLlegada", anterior.tiempoProximaLlegada);
                for (int j = 0; j < anterior.clientes.Count; j++)
                {
                    if (anterior.clientes[j].tiempoFinAterrizaje != 0)
                        tiempos.Add("tiempoFinAterrizaje_" + (j + 1).ToString(), anterior.clientes[j].tiempoFinAterrizaje);
                    if (anterior.clientes[j].tiempoFinDeDespegue != 0)
                        tiempos.Add("tiempoFinDeDespegue_" + (j + 1).ToString(), anterior.clientes[j].tiempoFinDeDespegue);
                    if (anterior.clientes[j].tiempoPermanencia != 0)
                        tiempos.Add("tiempoPermanencia_" + (j + 1).ToString(), anterior.clientes[j].tiempoPermanencia);
                }

                // Obtiene diccionario de tiempos ordenado segun menor tiempo
                var tiemposOrdenados = tiempos.OrderBy(obj => obj.Value).ToDictionary(obj => obj.Key, obj => obj.Value);
                KeyValuePair<string, double> menorTiempo = tiemposOrdenados.First();
                
                // Controlamos que los aviones en tierra sean menores a 30, si lo son, pasamos al siguiente menor tiempo, es decir, el siguiente evento
                //if (menorTiempo.Key == "tiempoProximaLlegada" && (anterior.pista.colaEET.Count + anterior.pista.colaEEV.Count + GetCantidadAvionesEnPermanencia(anterior) >= 30))
                //{
                //    tiemposOrdenados.Remove(tiemposOrdenados.First().Key);
                //    menorTiempo = tiemposOrdenados.First();
                //}

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
                    case var someVal when new Regex(@"tiempoFinDeDespegue_*").IsMatch(someVal):
                        int avionD = Convert.ToInt32(menorTiempo.Key.Split('_')[1]);
                        actual = CrearStateRowFinDeDespegue(anterior, menorTiempo.Value, avionD);
                        break;
                    case var someVal when new Regex(@"tiempoPermanencia_*").IsMatch(someVal):
                        int avion = Convert.ToInt32(menorTiempo.Key.Split('_')[1]);
                        actual = CrearStateRowFinDePermanencia(anterior, menorTiempo.Value, avion);
                        break;
                }

                actual.iterationNum = i + 1;

                if ((i >= from-1 && i <= from + 99) || i == (quantity - 1))
                {
                    stateRows.Add(actual);
                }

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
            Avion.count += 1;
            Avion avionNuevo = new Avion();
            StateRow nuevo = new StateRow();
            StateRow _anterior = anterior;

            nuevo.evento = "Llegada Avion (" + Avion.count.ToString() + ")";
            nuevo.reloj = tiempoProximoEvento;

            // Calcular siguiente tiempo de llegada de prox avion
            nuevo.rndLlegada = this.generator.NextRnd();
            nuevo.tiempoEntreLlegadas = this.exponentialGenerator.Generate(nuevo.rndLlegada);
            nuevo.tiempoProximaLlegada = nuevo.tiempoEntreLlegadas + nuevo.reloj;

            // Calcular variables de aterrizaje
            // Calculos variables de pista
            nuevo.pista = new Pista();
            nuevo.pista.libre = _anterior.pista.libre;
            nuevo.pista.colaEEV = _anterior.pista.colaEEV;
            nuevo.pista.colaEET = _anterior.pista.colaEET;
            if (!nuevo.pista.libre)
            {
                avionNuevo.estado = "EEV";
                nuevo.pista.colaEEV.Enqueue(avionNuevo);
                nuevo.tiempoFinAterrizaje = _anterior.tiempoFinAterrizaje;
            }
            else
            {
                avionNuevo.estado = "EA";
                nuevo.rndAterrizaje = this.generator.NextRnd();
                nuevo.tiempoAterrizaje = this.uniformGeneratorAterrizaje.Generate(nuevo.rndAterrizaje);
                nuevo.tiempoFinAterrizaje = nuevo.tiempoAterrizaje + nuevo.reloj;
                avionNuevo.tiempoFinAterrizaje = nuevo.tiempoFinAterrizaje;
                nuevo.pista.libre = false;
            }

            // Calcular variables de despegue
            nuevo.tiempoFinDeDespegue = _anterior.tiempoFinDeDespegue;

            // Clientes
            nuevo.clientes = new List<Avion>();
            foreach(Avion avionAnterior in _anterior.clientes)
            {
                Avion aux = new Avion() {
                    estado = avionAnterior.estado,
                    id = avionAnterior.id,
                    tiempoFinAterrizaje = avionAnterior.tiempoFinAterrizaje,
                    tiempoPermanencia = avionAnterior.tiempoPermanencia,
                    tiempoFinDeDespegue = avionAnterior.tiempoFinDeDespegue
                };
                nuevo.clientes.Add(aux);
            }
            nuevo.clientes.Add(avionNuevo);

            nuevo.pista.colaEETnum = nuevo.pista.colaEET.Count;
            nuevo.pista.colaEEVnum = nuevo.pista.colaEEV.Count;

            return nuevo;
        }

        private StateRow CrearStateRowFinDeDespegue(StateRow anterior, double tiempoProximoEvento, int avion)
        {
            StateRow nuevo = new StateRow();
            StateRow _anterior = anterior;

            nuevo.evento = "Fin Despegue (" + avion.ToString() + ")";
            nuevo.reloj = tiempoProximoEvento;
            nuevo.clientes = new List<Avion>();
            nuevo.tiempoProximaLlegada = _anterior.tiempoProximaLlegada;

            foreach (Avion avionAnterior in _anterior.clientes)
            {
                Avion aux = new Avion()
                {
                    estado = avionAnterior.estado,
                    id = avionAnterior.id,
                    tiempoFinAterrizaje = avionAnterior.tiempoFinAterrizaje,
                    tiempoPermanencia = avionAnterior.tiempoPermanencia,
                    tiempoFinDeDespegue = avionAnterior.tiempoFinDeDespegue
                };
                nuevo.clientes.Add(aux);
            }

            Avion avionDespegado = new Avion();
            avionDespegado = nuevo.clientes[avion - 1];
            nuevo.clientes[avion - 1].tiempoFinDeDespegue = 0;
            nuevo.clientes[avion - 1].estado = "";

            // Calculos variables de pista
            nuevo.pista = new Pista();
            nuevo.pista.colaEET = new Queue<Avion>();
            nuevo.pista.colaEEV = new Queue<Avion>();
            nuevo.pista.libre = _anterior.pista.libre;
            nuevo.pista.colaEEV = _anterior.pista.colaEEV;
            nuevo.pista.colaEET = _anterior.pista.colaEET;

            if (nuevo.pista.colaEEV.Count != 0)
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

            nuevo.pista.colaEETnum = nuevo.pista.colaEET.Count;
            nuevo.pista.colaEEVnum = nuevo.pista.colaEEV.Count;

            return nuevo;
        }

        private StateRow CrearStateRowFinAterrizaje(StateRow anterior, double tiempoProximoEvento, int avion)
        {
            StateRow nuevo = new StateRow();
            StateRow _anterior = anterior;

            nuevo.evento = "Fin Aterrizaje (" + avion.ToString() + ")" ;
            nuevo.reloj = tiempoProximoEvento;
            nuevo.tiempoProximaLlegada = _anterior.tiempoProximaLlegada;
            nuevo.clientes = new List<Avion>();
            foreach (Avion avionAnterior in _anterior.clientes)
            {
                Avion aux = new Avion()
                {
                    estado = avionAnterior.estado,
                    id = avionAnterior.id,
                    tiempoFinAterrizaje = avionAnterior.tiempoFinAterrizaje,
                    tiempoPermanencia = avionAnterior.tiempoPermanencia,
                    tiempoFinDeDespegue = avionAnterior.tiempoFinDeDespegue
                };
                nuevo.clientes.Add(aux);
            }

            //Calcular variables tiempo permanencia
            double ac = 0;
            nuevo.tiempoDePermanencia = convolutionGenerator.Generate(out ac);
            nuevo.rndPermanencia = ac;
            nuevo.tiempoFinPermanencia = nuevo.reloj + nuevo.tiempoDePermanencia;
            nuevo.clientes[avion - 1].tiempoPermanencia = nuevo.tiempoFinPermanencia;
            nuevo.clientes[avion - 1].tiempoFinAterrizaje = 0;
            nuevo.clientes[avion - 1].estado = "EP";

            // Calculos variables de pista
            nuevo.pista = new Pista();
            nuevo.pista.colaEET = new Queue<Avion>();
            nuevo.pista.colaEEV = new Queue<Avion>();
            nuevo.pista.libre = _anterior.pista.libre;
            nuevo.pista.colaEEV = _anterior.pista.colaEEV;
            nuevo.pista.colaEET = _anterior.pista.colaEET;
            if ( nuevo.pista.colaEEV.Count != 0)
            {
                // Calculos variables aterrizaje
                Avion avionNuevo = nuevo.pista.colaEEV.Dequeue();
                nuevo.rndAterrizaje = this.generator.NextRnd();
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
                nuevo.rndDespegue = this.generator.NextRnd();
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

            nuevo.pista.colaEETnum = nuevo.pista.colaEET.Count;
            nuevo.pista.colaEEVnum = nuevo.pista.colaEEV.Count;

            return nuevo;
        }

        private StateRow CrearStateRowFinDePermanencia(StateRow anterior, double tiempoProximoEvento, int avion)
        {
            StateRow nuevo = new StateRow();
            StateRow _anterior = anterior;

            nuevo.evento = "Fin permanencia (" + avion.ToString() + ")";
            nuevo.reloj = tiempoProximoEvento;

            // Calcular siguiente tiempo de llegada de prox avion
            nuevo.tiempoProximaLlegada = _anterior.tiempoProximaLlegada;

            // Calcular variables de aterrizaje
            nuevo.tiempoFinAterrizaje = _anterior.tiempoFinAterrizaje;

            nuevo.tiempoFinDeDespegue = _anterior.tiempoFinDeDespegue;

            // Calculos variables de pista
            nuevo.pista = new Pista();
            nuevo.pista.colaEET = new Queue<Avion>();
            nuevo.pista.colaEEV = new Queue<Avion>();
            nuevo.pista.libre = _anterior.pista.libre;
            nuevo.pista.colaEEV = _anterior.pista.colaEEV;
            nuevo.pista.colaEET = _anterior.pista.colaEET;

            nuevo.clientes = new List<Avion>();
            foreach (Avion avionAnterior in _anterior.clientes)
            {
                Avion aux = new Avion()
                {
                    estado = avionAnterior.estado,
                    id = avionAnterior.id,
                    tiempoFinAterrizaje = avionAnterior.tiempoFinAterrizaje,
                    tiempoPermanencia = avionAnterior.tiempoPermanencia,
                    tiempoFinDeDespegue = avionAnterior.tiempoFinDeDespegue
                };
                nuevo.clientes.Add(aux);
            }


            if (!nuevo.pista.libre)
            {
                nuevo.clientes[avion-1].estado = "EET";
                nuevo.pista.colaEET.Enqueue(nuevo.clientes[avion-1]);
            }
            else
            {
                // Calcular variables de despegue
                nuevo.clientes[avion - 1].estado = "ED";
                nuevo.rndDespegue = this.generator.NextRnd();
                nuevo.tiempoDeDespegue = this.uniformGeneratorDespegue.Generate(nuevo.rndDespegue);
                nuevo.tiempoFinDeDespegue = nuevo.tiempoDeDespegue + nuevo.reloj;
                nuevo.pista.libre = false;
            }
            nuevo.clientes[avion - 1].tiempoPermanencia = 0;

            nuevo.pista.colaEETnum = nuevo.pista.colaEET.Count;
            nuevo.pista.colaEEVnum = nuevo.pista.colaEEV.Count;

            return nuevo;
        }
    }
}
