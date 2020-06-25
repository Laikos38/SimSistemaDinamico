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

        public List<Avion> clientes { get; set; }

        

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

        /*
        public IList<StateRow> simulate(int quantity, int from, StateRow anterior)
        {
            Dictionary<string, double> tiempos = new Dictionary<string, double>();
            IList<StateRow> stateRows = new List<StateRow>();

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
                if (menorTiempo.Key == "tiempoProximaLlegada" && (anterior.pista.colaEET.Count + anterior.pista.colaEEV.Count + GetCantidadAvionesEnPermanencia(anterior) >= 30))
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

                if (i >= from - 1 && i <= from + 99 || i == (quantity - 1))
                {
                    if (from == 0 && i == 0) 
                        stateRows.Add(anterior);
                    stateRows.Add(actual);
                }

                anterior = actual;                
            }

            return stateRows;
        }
        */

        public StateRow NextStateRow(StateRow anterior, int i)
        {
            // Creo vector de estado para fila actual
            StateRow actual = new StateRow();

            // Diccionario de eventos (nombre de evento y su tiempo) para determinar el siguiente evento
            Dictionary<string, double> tiempos = new Dictionary<string, double>();
            var siguienteEvento = this.DeterminarSiguienteEvento(tiempos, anterior);

            int avion = 0;
            switch (siguienteEvento.Key)
            {
                case "tiempoProximaLlegada":
                    actual = CrearStateRowLlegadaAvion(anterior, siguienteEvento.Value, i);
                    break;
                case var val when new Regex(@"tiempoFinAterrizaje_*").IsMatch(val):
                    avion = Convert.ToInt32(siguienteEvento.Key.Split('_')[1]);
                    actual = CrearStateRowFinAterrizaje(anterior, siguienteEvento.Value, avion);
                    break;
                case var val when new Regex(@"tiempoFinDeDespegue_*").IsMatch(val):
                    avion = Convert.ToInt32(siguienteEvento.Key.Split('_')[1]);
                    actual = CrearStateRowFinDeDespegue(anterior, siguienteEvento.Value, avion);
                    break;
                case var val when new Regex(@"tiempoPermanencia_*").IsMatch(val):
                    avion = Convert.ToInt32(siguienteEvento.Key.Split('_')[1]);
                    actual = CrearStateRowFinDePermanencia(anterior, siguienteEvento.Value, avion);
                    break;
            }

            actual.iterationNum = i + 1;

            return actual;
        }

        public KeyValuePair<string, double> DeterminarSiguienteEvento(Dictionary<string, double> tiempos, StateRow anterior)
        {
            if (anterior.tiempoProximaLlegada != 0)
                tiempos.Add("tiempoProximaLlegada", anterior.tiempoProximaLlegada);
            for (int j = 0; j < this.clientes.Count; j++)
            {
                if (this.clientes[j].tiempoFinAterrizaje != 0)
                    tiempos.Add("tiempoFinAterrizaje_" + (j + 1).ToString(), this.clientes[j].tiempoFinAterrizaje);
                if (this.clientes[j].tiempoFinDeDespegue != 0)
                    tiempos.Add("tiempoFinDeDespegue_" + (j + 1).ToString(), this.clientes[j].tiempoFinDeDespegue);
                if (this.clientes[j].tiempoPermanencia != 0)
                    tiempos.Add("tiempoPermanencia_" + (j + 1).ToString(), this.clientes[j].tiempoPermanencia);
            }

            var tiemposOrdenados = tiempos.OrderBy(obj => obj.Value).ToDictionary(obj => obj.Key, obj => obj.Value);
            KeyValuePair<string, double> menorTiempo = tiemposOrdenados.First();

            return menorTiempo;
        }

        private int GetCantidadAvionesEnPermanencia()
        {
            int contador = 0;
            for (int i=0; i<this.clientes.Count; i++)
            {
                if (this.clientes[i].estado == "EP")
                    contador += 1;
            }
            return contador;
        }

        private StateRow CrearStateRowLlegadaAvion(StateRow anterior, double tiempoProximoEvento, int i)
        {
            StateRow nuevo = new StateRow();

            // Controlamos que los aviones en tierra sean menores a 30, si lo son, pasamos al siguiente menor tiempo, es decir, el siguiente evento
            int cantAvionesEnPermanencia = GetCantidadAvionesEnPermanencia();
            if (anterior.pista.colaEET.Count + anterior.pista.colaEEV.Count + cantAvionesEnPermanencia >= 5)
            {
                nuevo = this.arrastrarVariablesEst(anterior);
                nuevo.evento = "Rechazo avión";
                nuevo.reloj = tiempoProximoEvento;

                if (anterior.tiempoProximaLlegada != nuevo.reloj)
                {
                    nuevo.tiempoProximaLlegada = anterior.tiempoProximaLlegada;
                }
                else
                {
                    // Calcular siguiente tiempo de llegada de prox avion
                    nuevo.rndLlegada = this.generator.NextRnd();
                    nuevo.tiempoEntreLlegadas = this.exponentialGenerator.Generate(nuevo.rndLlegada);
                    nuevo.tiempoProximaLlegada = nuevo.tiempoEntreLlegadas + nuevo.reloj;
                }

                // Arrastro todos los valores del vector de estado anterior
                nuevo.pista = new Pista();
                nuevo.pista.libre = anterior.pista.libre;
                nuevo.pista.colaEET = new Queue<Avion>(anterior.pista.colaEET);
                nuevo.pista.colaEEV = new Queue<Avion>(anterior.pista.colaEEV);

                nuevo.tiempoFinAterrizaje = anterior.tiempoFinAterrizaje;

                nuevo.tiempoFinDeDespegue = anterior.tiempoFinDeDespegue;

                nuevo.tiempoFinPermanencia = anterior.tiempoFinPermanencia;

                // Se recalculan variables estadísticas
                nuevo.porcAvionesAyDInst = (Convert.ToDouble(nuevo.cantAvionesAyDInst) / Convert.ToDouble(this.clientes.Count)) * 100;
                nuevo.avgEETTime = Convert.ToDouble(nuevo.acumEETTime) / Convert.ToDouble(this.clientes.Count);
                nuevo.avgEEVTime = Convert.ToDouble(nuevo.acumEEVTime) / Convert.ToDouble(this.clientes.Count);

                return nuevo;
            }

            Avion.count += 1;
            Avion avionNuevo = new Avion();
            
            nuevo = this.arrastrarVariablesEst(anterior);
            nuevo.evento = "Llegada Avion (" + Avion.count.ToString() + ")";
            nuevo.reloj = tiempoProximoEvento;

            // Se arrastran variables estadísticas.

            // Calcular siguiente tiempo de llegada de prox avion
            nuevo.rndLlegada = this.generator.NextRnd();
            nuevo.tiempoEntreLlegadas = this.exponentialGenerator.Generate(nuevo.rndLlegada);
            nuevo.tiempoProximaLlegada = nuevo.tiempoEntreLlegadas + nuevo.reloj;


            // Calcular variables de aterrizaje
            // Calculos variables de pista
            nuevo.pista = new Pista();
            nuevo.pista.libre = anterior.pista.libre;
            nuevo.pista.colaEEV = new Queue<Avion>(anterior.pista.colaEEV);
            nuevo.pista.colaEET = new Queue<Avion>(anterior.pista.colaEET);
            if (!nuevo.pista.libre)
            {
                avionNuevo.estado = "EEV";
                nuevo.pista.colaEEV.Enqueue(avionNuevo);
                nuevo.tiempoFinAterrizaje = anterior.tiempoFinAterrizaje;
                avionNuevo.tiempoEEVin = nuevo.reloj;
            }
            else
            {
                avionNuevo.estado = "EA";
                nuevo.rndAterrizaje = this.generator.NextRnd();
                nuevo.tiempoAterrizaje = this.uniformGeneratorAterrizaje.Generate(nuevo.rndAterrizaje);
                nuevo.tiempoFinAterrizaje = nuevo.tiempoAterrizaje + nuevo.reloj;
                avionNuevo.tiempoFinAterrizaje = nuevo.tiempoFinAterrizaje;
                nuevo.pista.libre = false;
                avionNuevo.instantLanding = true;
            }

            // Calcular variables de despegue
            nuevo.tiempoFinDeDespegue = anterior.tiempoFinDeDespegue;

            // Clientes
            this.clientes.Add(avionNuevo);

            // Se recalculan variables estadísticas
            nuevo.porcAvionesAyDInst = (Convert.ToDouble(nuevo.cantAvionesAyDInst) / Convert.ToDouble(this.clientes.Count)) * 100;
            nuevo.avgEETTime = Convert.ToDouble(nuevo.acumEETTime) / Convert.ToDouble(this.clientes.Count);
            nuevo.avgEEVTime = Convert.ToDouble(nuevo.acumEEVTime) / Convert.ToDouble(this.clientes.Count);

            return nuevo;
        }

        private StateRow CrearStateRowFinDeDespegue(StateRow anterior, double tiempoProximoEvento, int avion)
        {
            StateRow nuevo = new StateRow();
            nuevo = this.arrastrarVariablesEst(anterior);
            nuevo.evento = "Fin Despegue (" + avion.ToString() + ")";
            nuevo.reloj = tiempoProximoEvento;

            
            nuevo.tiempoProximaLlegada = anterior.tiempoProximaLlegada;
            // Se arrastran variables estadísticas

            this.clientes[avion - 1].tiempoFinDeDespegue = 0;
            this.clientes[avion - 1].estado = "";
            //this.clientes[avion - 1].disabled = true;

            // Calculos variables de pista
            nuevo.pista = new Pista();
            nuevo.pista.libre = anterior.pista.libre;
            nuevo.pista.colaEEV = new Queue<Avion>(anterior.pista.colaEEV);
            nuevo.pista.colaEET = new Queue<Avion>(anterior.pista.colaEET);

            if (nuevo.pista.colaEEV.Count != 0)
            {
                // Calculos variables aterrizaje
                Avion avionNuevo = nuevo.pista.colaEEV.Dequeue();
                nuevo.rndAterrizaje = this.generator.NextRnd();
                nuevo.tiempoAterrizaje = this.uniformGeneratorAterrizaje.Generate(nuevo.rndAterrizaje);
                nuevo.tiempoFinAterrizaje = nuevo.tiempoAterrizaje + nuevo.reloj;
                nuevo.pista.libre = false;
                this.clientes[avionNuevo.id - 1].tiempoFinAterrizaje = nuevo.tiempoFinAterrizaje;
                this.clientes[avionNuevo.id - 1].estado = "EA";


                // Se chequea si el tiempo de espera en cola del avión desencolado es mayor al máx registrado,
                // de ser así lo asigna como maxEEVTime.
                // Puede que el chequeo no sea necesario
                if (this.clientes[avionNuevo.id - 1].tiempoEEVin != 0)
                {
                    double eevTime = nuevo.reloj - this.clientes[avionNuevo.id - 1].tiempoEEVin;
                    if (eevTime > nuevo.maxEEVTime) nuevo.maxEEVTime = eevTime;

                    nuevo.acumEEVTime += eevTime;
                }
            }
            else if (nuevo.pista.colaEET.Count != 0)
            {
                // Calculos variables de despegue
                Avion avionNuevo = nuevo.pista.colaEET.Dequeue();
                nuevo.rndDespegue = this.generator.NextRnd();
                nuevo.tiempoDeDespegue = this.uniformGeneratorDespegue.Generate(nuevo.rndDespegue);
                nuevo.tiempoFinDeDespegue = nuevo.tiempoDeDespegue + nuevo.reloj;
                nuevo.pista.libre = false;
                this.clientes[avionNuevo.id - 1].tiempoFinDeDespegue = nuevo.tiempoFinDeDespegue;
                this.clientes[avionNuevo.id - 1].estado = "ED";


                // Idem cola en vuelo
                if (this.clientes[avionNuevo.id - 1].tiempoEETin != 0)
                {
                    double eetTime = nuevo.reloj - this.clientes[avionNuevo.id - 1].tiempoEETin;
                    if (eetTime > nuevo.maxEETTime) nuevo.maxEETTime = eetTime;

                    nuevo.acumEETTime += eetTime;
                }
            }
            else
            {
                nuevo.pista.libre = true;
            }

            // Se recalculan variables estadísticas
            nuevo.porcAvionesAyDInst = (Convert.ToDouble(nuevo.cantAvionesAyDInst) / Convert.ToDouble(this.clientes.Count)) * 100;
            nuevo.avgEETTime = nuevo.acumEETTime / Convert.ToDouble(this.clientes.Count);
            nuevo.avgEEVTime = nuevo.acumEEVTime / Convert.ToDouble(this.clientes.Count);

            return nuevo;
        }

        private StateRow CrearStateRowFinAterrizaje(StateRow anterior, double tiempoProximoEvento, int avion)
        {
            StateRow nuevo = new StateRow();
            nuevo = this.arrastrarVariablesEst(anterior);
            nuevo.evento = "Fin Aterrizaje (" + avion.ToString() + ")" ;
            nuevo.reloj = tiempoProximoEvento;
            nuevo.tiempoProximaLlegada = anterior.tiempoProximaLlegada;
            // Se arrastran variables estadísticas


            //Calcular variables tiempo permanencia
            double ac = 0;
            nuevo.tiempoDePermanencia = convolutionGenerator.Generate(out ac);
            nuevo.rndPermanencia = ac;
            nuevo.tiempoFinPermanencia = nuevo.reloj + nuevo.tiempoDePermanencia;
            this.clientes[avion - 1].tiempoPermanencia = nuevo.tiempoFinPermanencia;
            this.clientes[avion - 1].tiempoFinAterrizaje = 0;
            this.clientes[avion - 1].estado = "EP";

            // Calculos variables de pista
            nuevo.pista = new Pista();
            nuevo.pista.libre = anterior.pista.libre;
            nuevo.pista.colaEEV = new Queue<Avion>(anterior.pista.colaEEV);
            nuevo.pista.colaEET = new Queue<Avion>(anterior.pista.colaEET);

            if ( nuevo.pista.colaEEV.Count != 0)
            {
                // Calculos variables aterrizaje
                Avion avionNuevo = nuevo.pista.colaEEV.Dequeue();
                nuevo.rndAterrizaje = this.generator.NextRnd();
                nuevo.tiempoAterrizaje = this.uniformGeneratorAterrizaje.Generate(nuevo.rndAterrizaje);
                nuevo.tiempoFinAterrizaje = nuevo.tiempoAterrizaje + nuevo.reloj;
                nuevo.pista.libre = false;
                this.clientes[avionNuevo.id - 1].tiempoFinAterrizaje = nuevo.tiempoFinAterrizaje;
                this.clientes[avionNuevo.id - 1].estado = "EA";

                // Se chequea si el tiempo de espera en cola del avión desencolado es mayor al máx registrado,
                // de ser así lo asigna como maxEEVTime.
                if(this.clientes[avionNuevo.id - 1].tiempoEEVin != 0)
                {
                    double eevTime = nuevo.reloj - this.clientes[avionNuevo.id - 1].tiempoEEVin;
                    if (eevTime > nuevo.maxEEVTime) nuevo.maxEEVTime = eevTime;

                    nuevo.acumEEVTime += eevTime;
                }
            }
            else if (nuevo.pista.colaEET.Count != 0)
            {
                // Calculos variables de despegue
                Avion avionNuevo = nuevo.pista.colaEET.Dequeue();
                nuevo.rndDespegue = this.generator.NextRnd();
                nuevo.tiempoDeDespegue = this.uniformGeneratorDespegue.Generate(nuevo.rndDespegue);
                nuevo.tiempoFinDeDespegue = nuevo.tiempoDeDespegue + nuevo.reloj;
                nuevo.pista.libre = false;
                this.clientes[avionNuevo.id - 1].tiempoFinDeDespegue = nuevo.tiempoFinDeDespegue;
                this.clientes[avionNuevo.id - 1].estado = "ED";

                if (this.clientes[avionNuevo.id - 1].tiempoEETin != 0)
                {
                    double eetTime = nuevo.reloj - this.clientes[avionNuevo.id - 1].tiempoEETin;
                    if (eetTime > nuevo.maxEETTime) nuevo.maxEETTime = eetTime;

                    nuevo.acumEETTime += eetTime;
                }
            }
            else
            {
                nuevo.pista.libre = true;
            }

            // Se recalculan variables estadísticas
            nuevo.porcAvionesAyDInst = (Convert.ToDouble(nuevo.cantAvionesAyDInst) / Convert.ToDouble(this.clientes.Count)) * 100;
            nuevo.avgEETTime = Convert.ToDouble(nuevo.acumEETTime) / Convert.ToDouble(this.clientes.Count);
            nuevo.avgEEVTime = Convert.ToDouble(nuevo.acumEEVTime) / Convert.ToDouble(this.clientes.Count);

            return nuevo;
        }

        private StateRow CrearStateRowFinDePermanencia(StateRow anterior, double tiempoProximoEvento, int avion)
        {
            StateRow nuevo = new StateRow();
            nuevo = this.arrastrarVariablesEst(anterior);
            nuevo.evento = "Fin permanencia (" + avion.ToString() + ")";
            nuevo.reloj = tiempoProximoEvento;

            // Se arrastran variables estadísticas
            


            // Calcular siguiente tiempo de llegada de prox avion
            nuevo.tiempoProximaLlegada = anterior.tiempoProximaLlegada;

            // Calcular variables de aterrizaje
            nuevo.tiempoFinAterrizaje = anterior.tiempoFinAterrizaje;

            nuevo.tiempoFinDeDespegue = anterior.tiempoFinDeDespegue;

            // Calculos variables de pista
            nuevo.pista = new Pista();
            nuevo.pista.libre = anterior.pista.libre;
            nuevo.pista.colaEEV = new Queue<Avion>(anterior.pista.colaEEV);
            nuevo.pista.colaEET = new Queue<Avion>(anterior.pista.colaEET);


            if (!nuevo.pista.libre)
            {
                this.clientes[avion-1].estado = "EET";
                nuevo.pista.colaEET.Enqueue(this.clientes[avion-1]);
                this.clientes[avion - 1].tiempoEETin = nuevo.reloj;
            }
            else
            {
                // Calcular variables de despegue
                this.clientes[avion - 1].estado = "ED";
                nuevo.rndDespegue = this.generator.NextRnd();
                nuevo.tiempoDeDespegue = this.uniformGeneratorDespegue.Generate(nuevo.rndDespegue);
                nuevo.tiempoFinDeDespegue = nuevo.tiempoDeDespegue + nuevo.reloj;
                this.clientes[avion - 1].tiempoFinDeDespegue = nuevo.tiempoFinDeDespegue;
                nuevo.pista.libre = false;

                if (this.clientes[avion - 1].instantLanding) nuevo.cantAvionesAyDInst++;
            }
            this.clientes[avion - 1].tiempoPermanencia = 0;

            // Se recalculan variables estadísticas
            nuevo.porcAvionesAyDInst = (Convert.ToDouble(nuevo.cantAvionesAyDInst) / Convert.ToDouble(this.clientes.Count)) * 100;

            nuevo.avgEETTime = Convert.ToDouble(nuevo.acumEETTime) / Convert.ToDouble(this.clientes.Count);
            nuevo.avgEEVTime = Convert.ToDouble(nuevo.acumEEVTime) / Convert.ToDouble(this.clientes.Count);

            return nuevo;
        }

        private StateRow arrastrarVariablesEst(StateRow _anterior)
        {
            StateRow nuevo = new StateRow();

            nuevo.maxEEVTime = _anterior.maxEEVTime;
            nuevo.maxEETTime = _anterior.maxEETTime;
            nuevo.porcAvionesAyDInst = _anterior.porcAvionesAyDInst;
            nuevo.cantAvionesAyDInst = _anterior.cantAvionesAyDInst;
            nuevo.acumEETTime = _anterior.acumEETTime;
            nuevo.acumEEVTime = _anterior.acumEEVTime;
            nuevo.avgEETTime = _anterior.avgEETTime;
            nuevo.avgEEVTime = _anterior.avgEEVTime;

            return nuevo;
        }
    }

}
