using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimulacionMontecarlo;

namespace SimulacionMontecarlo
{
    class Simulator
    {
        public IList<StateRow> simulate(int quantity, int from, int maxReservations)
        {
            Random random = new Random();

            IList<StateRow> stateRows = new List<StateRow>();
            int currentPassengers;
            long acumProfit = 0;
            int deniedSeats;
            int totalProfit;
            int extraPassengersCost;

            for (int i=0; i<quantity; i++)
            {
                deniedSeats = 0;
                totalProfit = 0;
                extraPassengersCost = 0;

                double rndNumber = random.NextDouble();

                currentPassengers = this.getCurrentPassengers(rndNumber, maxReservations);

                if (currentPassengers > 30)
                {
                    deniedSeats = currentPassengers - 30;
                    extraPassengersCost = deniedSeats * 150;
                }

                totalProfit = (currentPassengers * 100) - (extraPassengersCost);
                acumProfit += totalProfit;

                if ((i >= from-1 && i <= from + 99) || i == (quantity - 1))
                {
                    StateRow row = new StateRow { currentPassengers = currentPassengers, deniedSeats = deniedSeats, iterationNum = i + 1, extraPassengersCost = extraPassengersCost, rndNumber = Math.Truncate(rndNumber * 10000) / 10000, totalEarnings = currentPassengers * 100, totalProfit = totalProfit, acumProfit = acumProfit};

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
