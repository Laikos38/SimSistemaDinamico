using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemasDinamicos
{
    class Pista
    {

        public bool libre { get; set; }
        public Queue<Avion> colaEET { get; set; }
        public Queue<Avion> colaEEV { get; set; }


    }
}
