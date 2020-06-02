using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimulacionMontecarlo;
using SistemasDinamicos;

namespace SimulacionMontecarlo
{
    class Simulator
    {
        public IList<StateRow> simulate(int quantity, int from, StateRow initialize)
        {
        /*public double tiempoLlegada { get; set; }
        public double tiempoAterrizada { get; set; }
        public double tiempoPermanencia { get; set; }
        public int tiempoDespegue { get; set; }*/

        IList<StateRow> stateRows = new List<StateRow>();


            Dictionary<string, double> tiempos = new Dictionary<string, double>();
            tiempos.Add("tiempoLlegada", initialize.tiempoLlegada);
            for (int i = 0; i<initialize.clientes.Count; i++)
            {
                tiempos.Add("tiempoPermanencia"+(i+1).ToString(), initialize.clientes[i].tiempoPermanencia);
            }

            KeyValuePair<string, double> tiempoProximoEvento = tiempos.FirstOrDefault(x => x.Value == tiempos.Values.Min());


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

        private int getCurrentPassengers(double rnd, int maxReservations)
        {
            switch (maxReservations)
            {
                case 31:
                    //Para el caso de 31 reservaciones máx
                    if (rnd < 0.10) return 28;

                    if (rnd < 0.35) return 29;

                    if (rnd < 0.85) return 30;

                    return 31;


                case 32:
                    //Para el caso de 32 reservaciones máx
                    if (rnd < 0.05) return 28;

                    if (rnd < 0.3) return 29;

                    if (rnd < 0.8) return 30;

                    if (rnd < 0.95) return 31;

                    return 32;


                case 33:
                    //Para el caso de 33 reservaciones máx
                    if (rnd < 0.05) return 29;

                    if (rnd < 0.25) return 30;

                    if (rnd < 0.70) return 31;

                    if (rnd < 0.90) return 32;

                    return 33;


                case 34:
                    //Para el caso de 34 reservaciones máx
                    if (rnd < 0.05) return 29;

                    if (rnd < 0.15) return 30;

                    if (rnd < 0.55) return 31;

                    if (rnd < 0.85) return 32;

                    if (rnd < 0.95) return 33;

                    return 34;

            }   
            
            return 30;
        }
    }
}
