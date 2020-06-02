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

        public int id { get; set; }

        public static int count { get; set; }

        public Avion()
        {
            count += 1;
            this.id = count;
        }

    }
}
