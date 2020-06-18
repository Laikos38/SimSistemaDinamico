﻿using System;
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
        public int removed { get; set; }
        public UniformGenerator uniformGeneratorAterrizaje { get; set; }
        public UniformGenerator uniformGeneratorDespegue { get; set; }
        public ConvolutionGenerator convolutionGenerator { get; set; }
        public ExponentialGenerator exponentialGenerator { get; set; }
        public BoxMullerGenerator boxMullerGenerator { get; set; }
        public Generator generator { get; set; }
        public int from { get; set; }
        public int quantity { get; set; }



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


        public IList<StateRow> simulate(int _quantity, int _from, StateRow anterior)
        {
            quantity = _quantity;
            from = _from;
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
                        tiempos.Add("tiempoFinAterrizaje_" + (j + 1 + removed).ToString(), anterior.clientes[j].tiempoFinAterrizaje);
                    if (anterior.clientes[j].tiempoFinDeDespegue != 0)
                        tiempos.Add("tiempoFinDeDespegue_" + (j + 1 + removed).ToString(), anterior.clientes[j].tiempoFinDeDespegue);
                    if (anterior.clientes[j].tiempoPermanencia != 0)
                        tiempos.Add("tiempoPermanencia_" + (j + 1 + removed).ToString(), anterior.clientes[j].tiempoPermanencia);
                }

                // Obtiene diccionario de tiempos ordenado segun menor tiempo
                var tiemposOrdenados = tiempos.OrderBy(obj => obj.Value).ToDictionary(obj => obj.Key, obj => obj.Value);
                KeyValuePair<string, double> menorTiempo = tiemposOrdenados.First();

                int value = GetCantidadAvionesEnPermanencia(anterior);
                // Controlamos que los aviones en tierra sean menores a 30, si lo son, pasamos al siguiente menor tiempo, es decir, el siguiente evento
                while (menorTiempo.Key == "tiempoProximaLlegada" && (anterior.pista.colaEET.Count + anterior.pista.colaEEV.Count + value >= 30))
                {
                    tiemposOrdenados.Remove(tiemposOrdenados.First().Key);
                    menorTiempo = tiemposOrdenados.First();
                }

                int avion=0;
                // Se crea nuevo staterow segun el evento siguiente determinado
                switch (menorTiempo.Key)
                {
                    case "tiempoProximaLlegada":
                        actual = CrearStateRowLlegadaAvion(anterior, menorTiempo.Value);
                        break;
                    case var val when new Regex(@"tiempoFinAterrizaje_*").IsMatch(val):
                        avion = Convert.ToInt32(menorTiempo.Key.Split('_')[1]);
                        actual = CrearStateRowFinAterrizaje(anterior, menorTiempo.Value, avion);
                        break;
                    case var someVal when new Regex(@"tiempoFinDeDespegue_*").IsMatch(someVal):
                        avion = Convert.ToInt32(menorTiempo.Key.Split('_')[1]);
                        actual = CrearStateRowFinDeDespegue(anterior, menorTiempo.Value, avion, i);
                        break;
                    case var someVal when new Regex(@"tiempoPermanencia_*").IsMatch(someVal):
                        avion = Convert.ToInt32(menorTiempo.Key.Split('_')[1]);
                        actual = CrearStateRowFinDePermanencia(anterior, menorTiempo.Value, avion);
                        break;
                }

                actual.iterationNum = i + 1;

                if (i >= from - 1 && i <= from + 99 || i == (quantity - 1))
                {
                    if (from == 0 && i == 0)
                    {
                        stateRows.Add(anterior);
                    }
                    
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
            nuevo = this.arrastrarVariablesEst(_anterior);
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
            nuevo.pista.libre = _anterior.pista.libre;
            nuevo.pista.colaEEV = _anterior.pista.colaEEV;
            nuevo.pista.colaEET = _anterior.pista.colaEET;
            if (!nuevo.pista.libre)
            {
                avionNuevo.estado = "EEV";
                nuevo.pista.colaEEV.Enqueue(avionNuevo);
                nuevo.tiempoFinAterrizaje = _anterior.tiempoFinAterrizaje;
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
            nuevo.tiempoFinDeDespegue = _anterior.tiempoFinDeDespegue;


            // Clientes
            nuevo.clientes = new List<Avion>();
            foreach(Avion avionAnterior in _anterior.clientes)
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
                nuevo.clientes.Add(aux);
            }
            nuevo.clientes.Add(avionNuevo);

            // Se recalculan variables estadísticas
            nuevo.porcAvionesAyDInst = (Convert.ToDouble(nuevo.cantAvionesAyDInst) / Convert.ToDouble(nuevo.clientes.Count)) * 100;
            nuevo.avgEETTime = Convert.ToDouble(nuevo.acumEETTime) / Convert.ToDouble(nuevo.clientes.Count);
            nuevo.avgEEVTime = Convert.ToDouble(nuevo.acumEEVTime) / Convert.ToDouble(nuevo.clientes.Count);

            nuevo.pista.colaEETnum = nuevo.pista.colaEET.Count;
            nuevo.pista.colaEEVnum = nuevo.pista.colaEEV.Count;

            return nuevo;
        }

        private StateRow CrearStateRowFinDeDespegue(StateRow anterior, double tiempoProximoEvento, int avion, int iteration)
        {
            StateRow nuevo = new StateRow();
            StateRow _anterior = anterior;
            nuevo = this.arrastrarVariablesEst(_anterior);
            nuevo.evento = "Fin Despegue (" + (avion+removed).ToString() + ")";
            nuevo.reloj = tiempoProximoEvento;
            nuevo.clientes = new List<Avion>();
            nuevo.tiempoProximaLlegada = _anterior.tiempoProximaLlegada;
            // Se arrastran variables estadísticas

            foreach (Avion avionAnterior in _anterior.clientes)
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
                nuevo.clientes.Add(aux);
            }

            Avion avionDespegado = new Avion();
            avionDespegado = nuevo.clientes[GetIndex(avion)];
            nuevo.clientes[GetIndex(avion)].tiempoFinDeDespegue = 0;
            nuevo.clientes[GetIndex(avion)].estado = "";
            nuevo.clientes[GetIndex(avion)].disabled = true;

            if (!(iteration >= from - 1 && iteration <= from + 99 || iteration == (quantity - 1)))
            {
                if (!(from == 0 && iteration == 0))
                {
                    nuevo.clientes.RemoveAt(GetIndex(avion));
                    this.removed += 1;
                }
            }

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
                nuevo.rndAterrizaje = this.generator.NextRnd();
                nuevo.tiempoAterrizaje = this.uniformGeneratorAterrizaje.Generate(nuevo.rndAterrizaje);
                nuevo.tiempoFinAterrizaje = nuevo.tiempoAterrizaje + nuevo.reloj;
                nuevo.pista.libre = false;
                nuevo.clientes[GetIndex(avionNuevo.id)].tiempoFinAterrizaje = nuevo.tiempoFinAterrizaje;
                nuevo.clientes[GetIndex(avionNuevo.id)].estado = "EA";


                // Se chequea si el tiempo de espera en cola del avión desencolado es mayor al máx registrado,
                // de ser así lo asigna como maxEEVTime.
                // Puede que el chequeo no sea necesario
                if (nuevo.clientes[GetIndex(avionNuevo.id)].tiempoEEVin != 0)
                {
                    double eevTime = nuevo.reloj - nuevo.clientes[GetIndex(avionNuevo.id)].tiempoEEVin;
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
                nuevo.clientes[GetIndex(avionNuevo.id)].tiempoFinDeDespegue = nuevo.tiempoFinDeDespegue;
                nuevo.clientes[GetIndex(avionNuevo.id)].estado = "ED";


                // Idem cola en vuelo
                if (nuevo.clientes[GetIndex(avionNuevo.id)].tiempoEETin != 0)
                {
                    double eetTime = nuevo.reloj - nuevo.clientes[GetIndex(avionNuevo.id)].tiempoEETin;
                    if (eetTime > nuevo.maxEETTime) nuevo.maxEETTime = eetTime;

                    nuevo.acumEETTime += eetTime;
                }
            }
            else
            {
                nuevo.pista.libre = true;
            }

            // Se recalculan variables estadísticas
            nuevo.porcAvionesAyDInst = (Convert.ToDouble(nuevo.cantAvionesAyDInst) / Convert.ToDouble(nuevo.clientes.Count)) * 100;
            nuevo.avgEETTime = nuevo.acumEETTime / Convert.ToDouble(nuevo.clientes.Count);
            nuevo.avgEEVTime = nuevo.acumEEVTime / Convert.ToDouble(nuevo.clientes.Count);

            nuevo.pista.colaEETnum = nuevo.pista.colaEET.Count;
            nuevo.pista.colaEEVnum = nuevo.pista.colaEEV.Count;

            return nuevo;
        }

        private StateRow CrearStateRowFinAterrizaje(StateRow anterior, double tiempoProximoEvento, int avion)
        {
            StateRow nuevo = new StateRow();
            StateRow _anterior = anterior;
            nuevo = this.arrastrarVariablesEst(_anterior);
            nuevo.evento = "Fin Aterrizaje (" + (avion+removed).ToString() + ")" ;
            nuevo.reloj = tiempoProximoEvento;
            nuevo.tiempoProximaLlegada = _anterior.tiempoProximaLlegada;
            // Se arrastran variables estadísticas


            nuevo.clientes = new List<Avion>();
            foreach (Avion avionAnterior in _anterior.clientes)
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
                nuevo.clientes.Add(aux);
            }

            //Calcular variables tiempo permanencia
            double ac = 0;
            nuevo.tiempoDePermanencia = convolutionGenerator.Generate(out ac);
            nuevo.rndPermanencia = ac;
            nuevo.tiempoFinPermanencia = nuevo.reloj + nuevo.tiempoDePermanencia;
            nuevo.clientes[GetIndex(avion)].tiempoPermanencia = nuevo.tiempoFinPermanencia;
            nuevo.clientes[GetIndex(avion)].tiempoFinAterrizaje = 0;
            nuevo.clientes[GetIndex(avion)].estado = "EP";

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
                nuevo.clientes[GetIndex(avionNuevo.id)].tiempoFinAterrizaje = nuevo.tiempoFinAterrizaje;
                nuevo.clientes[GetIndex(avionNuevo.id)].estado = "EA";

                // Se chequea si el tiempo de espera en cola del avión desencolado es mayor al máx registrado,
                // de ser así lo asigna como maxEEVTime.
                if(nuevo.clientes[GetIndex(avionNuevo.id)].tiempoEEVin != 0)
                {
                    double eevTime = nuevo.reloj - nuevo.clientes[GetIndex(avionNuevo.id)].tiempoEEVin;
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
                nuevo.clientes[GetIndex(avionNuevo.id)].tiempoFinDeDespegue = nuevo.tiempoFinDeDespegue;
                nuevo.clientes[GetIndex(avionNuevo.id)].estado = "ED";

                if (nuevo.clientes[GetIndex(avionNuevo.id)].tiempoEETin != 0)
                {
                    double eetTime = nuevo.reloj - nuevo.clientes[GetIndex(avionNuevo.id)].tiempoEETin;
                    if (eetTime > nuevo.maxEETTime) nuevo.maxEETTime = eetTime;

                    nuevo.acumEETTime += eetTime;
                }
            }
            else
            {
                nuevo.pista.libre = true;
            }

            // Se recalculan variables estadísticas
            nuevo.porcAvionesAyDInst = (Convert.ToDouble(nuevo.cantAvionesAyDInst) / Convert.ToDouble(nuevo.clientes.Count)) * 100;
            nuevo.avgEETTime = Convert.ToDouble(nuevo.acumEETTime) / Convert.ToDouble(nuevo.clientes.Count);
            nuevo.avgEEVTime = Convert.ToDouble(nuevo.acumEEVTime) / Convert.ToDouble(nuevo.clientes.Count);

            nuevo.pista.colaEETnum = nuevo.pista.colaEET.Count;
            nuevo.pista.colaEEVnum = nuevo.pista.colaEEV.Count;

            return nuevo;
        }

        private StateRow CrearStateRowFinDePermanencia(StateRow anterior, double tiempoProximoEvento, int avion)
        {
            StateRow nuevo = new StateRow();
            StateRow _anterior = anterior;
            nuevo = this.arrastrarVariablesEst(_anterior);
            nuevo.evento = "Fin permanencia (" + (avion+removed).ToString() + ")";
            nuevo.reloj = tiempoProximoEvento;

            // Se arrastran variables estadísticas
            


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
                    disabled = avionAnterior.disabled,
                    tiempoFinAterrizaje = avionAnterior.tiempoFinAterrizaje,
                    tiempoPermanencia = avionAnterior.tiempoPermanencia,
                    tiempoFinDeDespegue = avionAnterior.tiempoFinDeDespegue,
                    tiempoEETin = avionAnterior.tiempoEETin,
                    tiempoEEVin = avionAnterior.tiempoEEVin,
                    instantLanding = avionAnterior.instantLanding
                };
                nuevo.clientes.Add(aux);
            }


            if (!nuevo.pista.libre)
            {
                nuevo.clientes[GetIndex(avion)].estado = "EET";
                nuevo.pista.colaEET.Enqueue(nuevo.clientes[GetIndex(avion)]);
                nuevo.clientes[GetIndex(avion)].tiempoEETin = nuevo.reloj;
            }
            else
            {
                // Calcular variables de despegue
                nuevo.clientes[GetIndex(avion)].estado = "ED";
                nuevo.rndDespegue = this.generator.NextRnd();
                nuevo.tiempoDeDespegue = this.uniformGeneratorDespegue.Generate(nuevo.rndDespegue);
                nuevo.tiempoFinDeDespegue = nuevo.tiempoDeDespegue + nuevo.reloj;
                nuevo.clientes[GetIndex(avion)].tiempoFinDeDespegue = nuevo.tiempoFinDeDespegue;
                nuevo.pista.libre = false;

                if (nuevo.clientes[GetIndex(avion)].instantLanding) nuevo.cantAvionesAyDInst++;
            }   
            nuevo.clientes[GetIndex(avion)].tiempoPermanencia = 0;

            // Se recalculan variables estadísticas
            nuevo.porcAvionesAyDInst = (Convert.ToDouble(nuevo.cantAvionesAyDInst) / Convert.ToDouble(nuevo.clientes.Count)) * 100;

            nuevo.avgEETTime = Convert.ToDouble(nuevo.acumEETTime) / Convert.ToDouble(nuevo.clientes.Count);
            nuevo.avgEEVTime = Convert.ToDouble(nuevo.acumEEVTime) / Convert.ToDouble(nuevo.clientes.Count);
       
            nuevo.pista.colaEETnum = nuevo.pista.colaEET.Count;
            nuevo.pista.colaEEVnum = nuevo.pista.colaEEV.Count;

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

        private int GetIndex(int idx)
        {
            if (idx - 1 - this.removed < 0)
                return 0;
            return idx - 1 - this.removed;
        }
    }

}
