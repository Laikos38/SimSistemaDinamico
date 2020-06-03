using SistemasDinamicos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulacionMontecarlo
{
    class StateRow
    {
        public double reloj{ get; set; }
        public string evento { get; set; }
        public int iterationNum { get; set; }
        public double rndLlegada { get; set; }
        public double tiempoEntreLlegadas { get; set; }
        public double tiempoProximaLlegada { get; set; }
        public double rndAterrizaje { get; set; }
        public double tiempoAterrizaje { get; set; }
        public double tiempoFinAterrizaje { get; set; }
        public double rndPermanencia { get; set; }
        public double tiempoDePermanencia { get; set; }
        public double tiempoFinPermanencia { get; set; }
        public double rndDespegue { get; set; }
        public double tiempoDeDespegue { get; set; }
        public int tiempoFinDeDespegue { get; set; }
        public Pista pista { get; set; }
        public List<Avion> clientes { get; set; }
    }
}
