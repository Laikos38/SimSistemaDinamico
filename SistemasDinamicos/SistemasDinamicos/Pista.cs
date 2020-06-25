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
        public String state { get; set; }
        public Queue<Avion> colaEET { get; set; }
        public Queue<Avion> colaEEV { get; set; }
        public double tiempoRestanteAoD { get; set; }
        public int colaEETnum { get; set; }
        public int colaEEVnum { get; set; }
        public int idClienteActual { get; set; }
    }
}
