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
        public double tiempoLlegada { get; set; }
        public double tiempoAterrizada { get; set; }
        public double tiempoPermanencia { get; set; }
        public int tiempoDespegue { get; set; }
        public Pista pista { get; set; }
        public List<Avion> clientes { get; set; }


    }
}
