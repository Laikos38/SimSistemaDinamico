using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemasDinamicos
{
    class Avion
    {
        public String estado { get; set; }
        public double tiempoPermanencia { get; set; }
        public double tiempoFinAterrizaje { get; set; }
        public double tiempoFinDeDespegue { get; set; }
        public int id { get; set; }
        public double tiempoEEVin { get; set; }
        public double tiempoEETin { get; set; }
        public static int count { get; set; }
        public bool instantLanding { get; set; }

        public bool disabled { get; set; }

        public Avion()
        {
            this.id = count;
            this.instantLanding = false;
        }
    }
}
