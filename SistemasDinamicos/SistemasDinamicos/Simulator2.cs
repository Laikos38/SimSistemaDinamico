using GeneradorDeNumerosAleatorios;
using RandomVarGenerator;
using SimulacionMontecarlo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SistemasDinamicos
{
    class Simulator2
    {
        public int removed { get; set; }
        public UniformGenerator uniformGeneratorAterrizaje { get; set; }
        public UniformGenerator uniformGeneratorDespegue { get; set; }
        public ConvolutionGenerator convolutionGenerator { get; set; }
        public ExponentialGenerator exponentialGenerator { get; set; }
        public BoxMullerGenerator boxMullerGenerator { get; set; }
        public Generator generator { get; set; }
        public int from { get; set; }
        public int quantity { get; set; }

        public Simulator2()
        {
            uniformGeneratorAterrizaje = new UniformGenerator();
            uniformGeneratorAterrizaje.a = 3;
            uniformGeneratorAterrizaje.b = 5;
            uniformGeneratorDespegue = new UniformGenerator();
            uniformGeneratorDespegue.a = 2;
            uniformGeneratorDespegue.b = 4;
            convolutionGenerator = new ConvolutionGenerator();
            exponentialGenerator = new ExponentialGenerator();
            exponentialGenerator.lambda = (double)0.1;
            convolutionGenerator.mean = 80;
            convolutionGenerator.stDeviation = 30;
            generator = new Generator();
        }

        public IList<StateRow> simulate(int quantity, int from, int to, StateRow anterior)
        {
            IList<StateRow> rowsToShow = new List<StateRow>();

            for (int i=0; i<quantity; i++)
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
                        actual = CrearStateRowLlegadaAvion(anterior, siguienteEvento.Value);
                        break;
                    case var val when new Regex(@"tiempoFinAterrizaje_*").IsMatch(val):
                        avion = Convert.ToInt32(siguienteEvento.Key.Split('_')[1]);
                        actual = CrearStateRowFinAterrizaje(anterior, siguienteEvento.Value, avion);
                        break;
                    case var val when new Regex(@"tiempoFinDeDespegue_*").IsMatch(val):
                        avion = Convert.ToInt32(siguienteEvento.Key.Split('_')[1]);
                        actual = CrearStateRowFinDeDespegue(anterior, siguienteEvento.Value, avion, i);
                        break;
                    case var val when new Regex(@"tiempoPermanencia_*").IsMatch(val):
                        avion = Convert.ToInt32(siguienteEvento.Key.Split('_')[1]);
                        actual = CrearStateRowFinDePermanencia(anterior, siguienteEvento.Value, avion);
                        break;
                }

                actual.iterationNum = i + 1;

                if (i >= from - 1 && i <= from + 99 || i == (quantity - 1))
                {
                    if (from == 0 && i == 0)
                    {
                        rowsToShow.Add(anterior);
                    }
                    rowsToShow.Add(actual);
                }

                anterior = actual;
            }

            return rowsToShow;
        }

        public KeyValuePair<string, double> DeterminarSiguienteEvento(Dictionary<string, double> tiempos, StateRow anterior)
        {
            if (anterior.tiempoProximaLlegada != 0)
                tiempos.Add("tiempoProximaLlegada", anterior.tiempoProximaLlegada);
            for (int j = 0; j < anterior.clientes.Count; j++)
            {
                if (anterior.clientes[j].tiempoFinAterrizaje != 0)
                    tiempos.Add("tiempoFinAterrizaje_" + (j + 1 + removed).ToString(), anterior.clientes[j].tiempoFinAterrizaje);
                if (anterior.clientes[j].tiempoFinDeDespegue != 0)
                    tiempos.Add("tiempoFinDeDespegue_" + (j + 1 + removed).ToString(), anterior.clientes[j].tiempoFinDeDespegue);
                if (anterior.clientes[j].tiempoPermanencia != 0)
                    tiempos.Add("tiempoPermanencia_" + (j + 1 + removed).ToString(), anterior.clientes[j].tiempoPermanencia);
            }

            var tiemposOrdenados = tiempos.OrderBy(obj => obj.Value).ToDictionary(obj => obj.Key, obj => obj.Value);
            KeyValuePair<string, double> menorTiempo = tiemposOrdenados.First();

            return menorTiempo;
        }

        private StateRow CrearStateRowLlegadaAvion(StateRow anterior, double reloj)
        {
            // Creo nuevo stateRow
            StateRow nuevoStateRow = new StateRow();
            Avion.count += 1;
            Avion avionNuevo = new Avion();

            // Seteo nombre de evento y reloj
            nuevoStateRow.evento = "Llegada Avion (" + Avion.count.ToString() + ")";
            nuevoStateRow.reloj = reloj;

            // Calcular siguiente tiempo de llegada de prox avion
            nuevoStateRow.rndLlegada = this.generator.NextRnd();
            nuevoStateRow.tiempoEntreLlegadas = this.exponentialGenerator.Generate(nuevoStateRow.rndLlegada);
            nuevoStateRow.tiempoProximaLlegada = nuevoStateRow.tiempoEntreLlegadas + nuevoStateRow.reloj;


            // Controlamos que los aviones en tierra sean menores a 30, si lo son, pasamos al siguiente menor tiempo, es decir, el siguiente evento
            int cantAvionesEnPermanencia = GetCantidadAvionesEnPermanencia(anterior);
            if (anterior.pista.colaEET.Count + anterior.pista.colaEEV.Count + cantAvionesEnPermanencia >= 30)
            {
                // Arrastro todos los valores del vector de estado anterior
                nuevoStateRow.pista.libre = anterior.pista.libre;
                nuevoStateRow.pista.colaEET = new Queue<Avion>(anterior.pista.colaEET);
                nuevoStateRow.pista.colaEEV = new Queue<Avion>(anterior.pista.colaEEV);
            }

            // Calculos para aterrizaje
            nuevoStateRow.pista.libre = anterior.pista.libre;
            nuevoStateRow.pista.colaEET = new Queue<Avion>(anterior.pista.colaEET);
            nuevoStateRow.pista.colaEEV = new Queue<Avion>(anterior.pista.colaEEV);
            if (!nuevoStateRow.pista.libre)
            {
                // Pista ocupada
                avionNuevo.estado = "EEV";
                nuevoStateRow.pista.colaEEV.Enqueue(avionNuevo);
                nuevoStateRow.tiempoFinAterrizaje = anterior.tiempoFinAterrizaje;
                avionNuevo.tiempoEEVin = nuevoStateRow.reloj;
            }
            else
            {
                // Pista desocupada
                avionNuevo.estado = "EA";
                nuevoStateRow.rndAterrizaje = this.generator.NextRnd();
                nuevoStateRow.tiempoAterrizaje = this.uniformGeneratorAterrizaje.Generate(nuevoStateRow.rndAterrizaje);
                nuevoStateRow.tiempoFinAterrizaje = nuevoStateRow.tiempoAterrizaje + nuevoStateRow.reloj;
                avionNuevo.tiempoFinAterrizaje = nuevoStateRow.tiempoFinAterrizaje;
                nuevoStateRow.pista.libre = false;
                avionNuevo.instantLanding = true;
            }

            // Calcular variables de despegue
            nuevoStateRow.tiempoFinDeDespegue = anterior.tiempoFinDeDespegue;

            // Clientes
            // Copio clientes anteriores
            nuevoStateRow.clientes = CopiarClientes(anterior.clientes);
            nuevoStateRow.clientes.Add(avionNuevo);

            // Se recalculan variables estadísticas
            nuevoStateRow.porcAvionesAyDInst = (Convert.ToDouble(nuevoStateRow.cantAvionesAyDInst) / Convert.ToDouble(nuevoStateRow.clientes.Count)) * 100;
            nuevoStateRow.avgEETTime = Convert.ToDouble(nuevoStateRow.acumEETTime) / Convert.ToDouble(nuevoStateRow.clientes.Count);
            nuevoStateRow.avgEEVTime = Convert.ToDouble(nuevoStateRow.acumEEVTime) / Convert.ToDouble(nuevoStateRow.clientes.Count);

            return nuevoStateRow;
        }

        private StateRow CrearStateRowFinAterrizaje(StateRow anterior, double tiempoProximoEvento, int avion)
        {
            // Creo nuevo stateRow
            StateRow nuevoStateRow = new StateRow();

            // Estadísticas
            nuevoStateRow = this.arrastrarVariablesEst(anterior);

            // Seteo nombre de evento y reloj
            nuevoStateRow.evento = "Fin Aterrizaje (" + (avion + removed).ToString() + ")";
            nuevoStateRow.reloj = tiempoProximoEvento;

            // Seteo valores de llegada de avion
            nuevoStateRow.tiempoProximaLlegada = anterior.tiempoProximaLlegada;

            // Copio clientes anteriores
            nuevoStateRow.clientes = CopiarClientes(anterior.clientes);

            // Seteo tiempo de permanencia
            double ac = 0;
            nuevoStateRow.tiempoDePermanencia = convolutionGenerator.Generate(out ac);
            nuevoStateRow.rndPermanencia = ac;
            nuevoStateRow.tiempoFinPermanencia = nuevoStateRow.reloj + nuevoStateRow.tiempoDePermanencia;
            nuevoStateRow.clientes[GetIndex(avion)].tiempoPermanencia = nuevoStateRow.tiempoFinPermanencia;
            nuevoStateRow.clientes[GetIndex(avion)].tiempoFinAterrizaje = 0;
            nuevoStateRow.clientes[GetIndex(avion)].estado = "EP";

            // Seteo pista
            nuevoStateRow.pista.libre = anterior.pista.libre;
            nuevoStateRow.pista.colaEET = new Queue<Avion>(anterior.pista.colaEET);
            nuevoStateRow.pista.colaEEV = new Queue<Avion>(anterior.pista.colaEEV);
            
            if (nuevoStateRow.pista.colaEEV.Count != 0)
            {
                Avion avionNuevo = nuevoStateRow.pista.colaEEV.Dequeue();
                nuevoStateRow.rndAterrizaje = this.generator.NextRnd();
                nuevoStateRow.tiempoAterrizaje = this.uniformGeneratorAterrizaje.Generate(nuevoStateRow.rndAterrizaje);
                nuevoStateRow.tiempoFinAterrizaje = nuevoStateRow.tiempoAterrizaje + nuevoStateRow.reloj;
                nuevoStateRow.pista.libre = false;
                nuevoStateRow.clientes[GetIndex(avionNuevo.id)].tiempoFinAterrizaje = nuevoStateRow.tiempoFinAterrizaje;
                nuevoStateRow.clientes[GetIndex(avionNuevo.id)].estado = "EA";

                // Se chequea si el tiempo de espera en cola del avión desencolado es mayor al máx registrado,
                // de ser así lo asigna como maxEEVTime.
                if (nuevoStateRow.clientes[GetIndex(avionNuevo.id)].tiempoEEVin != 0)
                {
                    double eevTime = nuevoStateRow.reloj - nuevoStateRow.clientes[GetIndex(avionNuevo.id)].tiempoEEVin;
                    if (eevTime > nuevoStateRow.maxEEVTime) nuevoStateRow.maxEEVTime = eevTime;

                    nuevoStateRow.acumEEVTime += eevTime;
                }
            }
            else if (nuevoStateRow.pista.colaEET.Count != 0)
            {
                // Calculos variables de despegue
                Avion avionNuevo = nuevoStateRow.pista.colaEET.Dequeue();
                nuevoStateRow.rndDespegue = this.generator.NextRnd();
                nuevoStateRow.tiempoDeDespegue = this.uniformGeneratorDespegue.Generate(nuevoStateRow.rndDespegue);
                nuevoStateRow.tiempoFinDeDespegue = nuevoStateRow.tiempoDeDespegue + nuevoStateRow.reloj;
                nuevoStateRow.pista.libre = false;
                nuevoStateRow.clientes[GetIndex(avionNuevo.id)].tiempoFinDeDespegue = nuevoStateRow.tiempoFinDeDespegue;
                nuevoStateRow.clientes[GetIndex(avionNuevo.id)].estado = "ED";

                if (nuevoStateRow.clientes[GetIndex(avionNuevo.id)].tiempoEETin != 0)
                {
                    double eetTime = nuevoStateRow.reloj - nuevoStateRow.clientes[GetIndex(avionNuevo.id)].tiempoEETin;
                    if (eetTime > nuevoStateRow.maxEETTime) nuevoStateRow.maxEETTime = eetTime;

                    nuevoStateRow.acumEETTime += eetTime;
                }
            }
            else
            {
                nuevoStateRow.pista.libre = true;
            }

            // Se recalculan variables estadísticas
            nuevoStateRow.porcAvionesAyDInst = (Convert.ToDouble(nuevoStateRow.cantAvionesAyDInst) / Convert.ToDouble(nuevoStateRow.clientes.Count)) * 100;
            nuevoStateRow.avgEETTime = Convert.ToDouble(nuevoStateRow.acumEETTime) / Convert.ToDouble(nuevoStateRow.clientes.Count);
            nuevoStateRow.avgEEVTime = Convert.ToDouble(nuevoStateRow.acumEEVTime) / Convert.ToDouble(nuevoStateRow.clientes.Count);

            return nuevoStateRow;
        }

        private StateRow CrearStateRowFinDePermanencia(StateRow anterior, double tiempoProximoEvento, int avion)
        {
            // Creo nuevo stateRow
            StateRow nuevoStateRow = new StateRow();

            // Estadísticas
            nuevoStateRow = this.arrastrarVariablesEst(anterior);

            // Seteo nombre de evento y reloj
            nuevoStateRow.evento = "Fin permanencia (" + (avion + removed).ToString() + ")";
            nuevoStateRow.reloj = tiempoProximoEvento;

            // Seteo siguiente tiempo de llegada de prox avion
            nuevoStateRow.tiempoProximaLlegada = anterior.tiempoProximaLlegada;

            // Seteo variables de aterrizaje
            nuevoStateRow.tiempoFinAterrizaje = anterior.tiempoFinAterrizaje;

            // Seteo variables de despegue
            nuevoStateRow.tiempoFinDeDespegue = anterior.tiempoFinDeDespegue;

            // Seteo de pista
            nuevoStateRow.pista.libre = anterior.pista.libre;
            nuevoStateRow.pista.colaEET = new Queue<Avion>(anterior.pista.colaEET);
            nuevoStateRow.pista.colaEEV = new Queue<Avion>(anterior.pista.colaEEV);

            // Copio clientes anteriores
            nuevoStateRow.clientes = CopiarClientes(anterior.clientes);

            if (!nuevoStateRow.pista.libre)
            {
                nuevoStateRow.clientes[GetIndex(avion)].estado = "EET";
                nuevoStateRow.pista.colaEET.Enqueue(nuevoStateRow.clientes[GetIndex(avion)]);
                nuevoStateRow.clientes[GetIndex(avion)].tiempoEETin = nuevoStateRow.reloj;
            }
            else
            {
                // Calcular variables de despegue
                nuevoStateRow.clientes[GetIndex(avion)].estado = "ED";
                nuevoStateRow.rndDespegue = this.generator.NextRnd();
                nuevoStateRow.tiempoDeDespegue = this.uniformGeneratorDespegue.Generate(nuevoStateRow.rndDespegue);
                nuevoStateRow.tiempoFinDeDespegue = nuevoStateRow.tiempoDeDespegue + nuevoStateRow.reloj;
                nuevoStateRow.clientes[GetIndex(avion)].tiempoFinDeDespegue = nuevoStateRow.tiempoFinDeDespegue;
                nuevoStateRow.pista.libre = false;

                if (nuevoStateRow.clientes[GetIndex(avion)].instantLanding) nuevoStateRow.cantAvionesAyDInst++;
            }

            nuevoStateRow.clientes[GetIndex(avion)].tiempoPermanencia = 0;

            // Se recalculan variables estadísticas
            nuevoStateRow.porcAvionesAyDInst = (Convert.ToDouble(nuevoStateRow.cantAvionesAyDInst) / Convert.ToDouble(nuevoStateRow.clientes.Count)) * 100;
            nuevoStateRow.avgEETTime = Convert.ToDouble(nuevoStateRow.acumEETTime) / Convert.ToDouble(nuevoStateRow.clientes.Count);
            nuevoStateRow.avgEEVTime = Convert.ToDouble(nuevoStateRow.acumEEVTime) / Convert.ToDouble(nuevoStateRow.clientes.Count);

            return nuevoStateRow;
        }

        private StateRow CrearStateRowFinDeDespegue(StateRow anterior, double tiempoProximoEvento, int avion, int iteration)
        {
            // Creo nuevo stateRow
            StateRow nuevoStateRow = new StateRow();

            // Estadísticas
            nuevoStateRow = this.arrastrarVariablesEst(anterior);

            // Seteo nombre de evento y reloj
            nuevoStateRow.evento = "Fin Despegue (" + (avion + removed).ToString() + ")";
            nuevoStateRow.reloj = tiempoProximoEvento;

            // Seteo siguiente tiempo de llegada de prox avion
            nuevoStateRow.tiempoProximaLlegada = anterior.tiempoProximaLlegada;

            // Clientes
            nuevoStateRow.clientes = CopiarClientes(anterior.clientes);

            // Modifico clientes que despegó
            Avion avionDespegado = new Avion();
            avionDespegado = nuevoStateRow.clientes[GetIndex(avion)];
            nuevoStateRow.clientes[GetIndex(avion)].tiempoFinDeDespegue = 0;
            nuevoStateRow.clientes[GetIndex(avion)].estado = "";
            nuevoStateRow.clientes[GetIndex(avion)].disabled = true;

            // Si el cliente esta dentro de los vectores a mostrar el grilla NO lo remuevo del arreglo.
            if (!(iteration >= from - 1 && iteration <= from + 99 || iteration == (quantity - 1)))
            {
                if (!(from == 0 && iteration == 0))
                {
                    nuevoStateRow.clientes.RemoveAt(GetIndex(avion));
                    this.removed += 1;
                }
            }

            // Pista
            nuevoStateRow.pista.libre = anterior.pista.libre;
            nuevoStateRow.pista.colaEET = new Queue<Avion>(anterior.pista.colaEET);
            nuevoStateRow.pista.colaEEV = new Queue<Avion>(anterior.pista.colaEEV);

            if (nuevoStateRow.pista.colaEEV.Count != 0)
            {
                // Calculos variables aterrizaje
                Avion avionNuevo = nuevoStateRow.pista.colaEEV.Dequeue();
                nuevoStateRow.rndAterrizaje = this.generator.NextRnd();
                nuevoStateRow.tiempoAterrizaje = this.uniformGeneratorAterrizaje.Generate(nuevoStateRow.rndAterrizaje);
                nuevoStateRow.tiempoFinAterrizaje = nuevoStateRow.tiempoAterrizaje + nuevoStateRow.reloj;
                nuevoStateRow.pista.libre = false;
                nuevoStateRow.clientes[GetIndex(avionNuevo.id)].tiempoFinAterrizaje = nuevoStateRow.tiempoFinAterrizaje;
                nuevoStateRow.clientes[GetIndex(avionNuevo.id)].estado = "EA";

                // Se chequea si el tiempo de espera en cola del avión desencolado es mayor al máx registrado,
                // de ser así lo asigna como maxEEVTime.
                // Puede que el chequeo no sea necesario
                if (nuevoStateRow.clientes[GetIndex(avionNuevo.id)].tiempoEEVin != 0)
                {
                    double eevTime = nuevoStateRow.reloj - nuevoStateRow.clientes[GetIndex(avionNuevo.id)].tiempoEEVin;
                    if (eevTime > nuevoStateRow.maxEEVTime) nuevoStateRow.maxEEVTime = eevTime;

                    nuevoStateRow.acumEEVTime += eevTime;

                }
            }
            else if (nuevoStateRow.pista.colaEET.Count != 0)
            {
                // Calculos variables de despegue
                Avion avionNuevo = nuevoStateRow.pista.colaEET.Dequeue();
                nuevoStateRow.rndDespegue = this.generator.NextRnd();
                nuevoStateRow.tiempoDeDespegue = this.uniformGeneratorDespegue.Generate(nuevoStateRow.rndDespegue);
                nuevoStateRow.tiempoFinDeDespegue = nuevoStateRow.tiempoDeDespegue + nuevoStateRow.reloj;
                nuevoStateRow.pista.libre = false;
                nuevoStateRow.clientes[GetIndex(avionNuevo.id)].tiempoFinDeDespegue = nuevoStateRow.tiempoFinDeDespegue;
                nuevoStateRow.clientes[GetIndex(avionNuevo.id)].estado = "ED";

                // Idem cola en vuelo
                if (nuevoStateRow.clientes[GetIndex(avionNuevo.id)].tiempoEETin != 0)
                {
                    double eetTime = nuevoStateRow.reloj - nuevoStateRow.clientes[GetIndex(avionNuevo.id)].tiempoEETin;
                    if (eetTime > nuevoStateRow.maxEETTime) nuevoStateRow.maxEETTime = eetTime;

                    nuevoStateRow.acumEETTime += eetTime;
                }
            }
            else
            {
                nuevoStateRow.pista.libre = true;
            }

            // Se recalculan variables estadísticas
            nuevoStateRow.porcAvionesAyDInst = (Convert.ToDouble(nuevoStateRow.cantAvionesAyDInst) / Convert.ToDouble(nuevoStateRow.clientes.Count)) * 100;
            nuevoStateRow.avgEETTime = nuevoStateRow.acumEETTime / Convert.ToDouble(nuevoStateRow.clientes.Count);
            nuevoStateRow.avgEEVTime = nuevoStateRow.acumEEVTime / Convert.ToDouble(nuevoStateRow.clientes.Count);

            return nuevoStateRow;
        }

        private int GetIndex(int idx)
        {
            if (idx - 1 - this.removed < 0)
                return 0;
            return idx - 1 - this.removed;
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

        private List<Avion> CopiarClientes(List<Avion> clientesPrevios)
        {
            List<Avion> clientes = new List<Avion>();
            foreach (Avion avionAnterior in clientesPrevios)
            {
                Avion aux = new Avion()
                {
                    estado = avionAnterior.estado,
                    id = avionAnterior.id,
                    disabled = avionAnterior.disabled,
                    tiempoFinAterrizaje = avionAnterior.tiempoFinAterrizaje,
                    tiempoPermanencia = avionAnterior.tiempoPermanencia,
                    tiempoFinDeDespegue = avionAnterior.tiempoFinDeDespegue,
                    tiempoEETin = avionAnterior.tiempoEETin,
                    tiempoEEVin = avionAnterior.tiempoEEVin,
                    instantLanding = avionAnterior.instantLanding
                };
                clientes.Add(aux);
            }

            return clientes;
        }

        private int GetCantidadAvionesEnPermanencia(StateRow vector)
        {
            int contador = 0;
            for (int i = 0; i < vector.clientes.Count; i++)
            {
                if (vector.clientes[i].estado == "EP")
                    contador += 1;
            }
            return contador;
        }
    }
}
