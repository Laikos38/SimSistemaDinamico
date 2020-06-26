using GeneradorDeNumerosAleatorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomVarGenerator
{
    class ConvolutionGenerator
    {
        public double mean { get; set; }

        public double stDeviation { get; set; }

        public Generator congruentialGenerator = new Generator() { seed = 31767, a = 71561, c = 56822, M = 341157 };
        
        public double Generate(out double ac)
        {
            double rnd = -1.0;
            ac = 0;

            while(rnd < 0)
            {
                ac = 0;
                for (int i = 0; i < 12; i++)
                {
                    ac += congruentialGenerator.NextRnd();
                }
                rnd = (ac - 6) * this.stDeviation + this.mean;
            }

            return rnd;
        }

        public double GenerateFake(double ac)
        {
        
            double rnd = (ac - 6) * this.stDeviation + this.mean;

            return rnd;
        }
    }
}
