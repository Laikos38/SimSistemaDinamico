using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneradorDeNumerosAleatorios
{
    public class Generator
    {
        public double seed { get; set; }
        public double a { get; set; }
        public double c { get; set; }
        public double M { get; set; }
        public double type { get; set; }
        public Queue<double> fakeRnds { get; set; }

        public Generator()
        {
            this.seed = 31767;
            this.a = 71561;
            this.c = 56822;
            this.M = 341157;

            fakeRnds = new Queue<double>();
            fakeRnds.Enqueue(0.87);
            fakeRnds.Enqueue(0.32);
            fakeRnds.Enqueue(4.89);
            fakeRnds.Enqueue(0.63);
            fakeRnds.Enqueue(0.82);
            fakeRnds.Enqueue(0.09);
            fakeRnds.Enqueue(0.28);
            fakeRnds.Enqueue(0.85);
            fakeRnds.Enqueue(0.2);
            fakeRnds.Enqueue(6);
            fakeRnds.Enqueue(0.63);
            fakeRnds.Enqueue(0.23);
            fakeRnds.Enqueue(0.53);
            fakeRnds.Enqueue(0.93);
            fakeRnds.Enqueue(0.69);
        }

        public List<double> Generate(int quantity)
        {
            List<double> result = new List<double>();

            for (int i = 0; i < quantity; i++)
            {
                double rnd = ((this.a * this.seed) + this.c) % this.M;
                this.seed = rnd;
                rnd = rnd / (this.M - 1);
                rnd = Math.Truncate(rnd * 10000) / 10000;
                result.Add(rnd);
            }

            return result;
        }

        public double NextRnd()
        {
            double rnd = ((this.a * this.seed) + this.c) % this.M;
            this.seed = rnd;
            rnd = rnd / (this.M - 1);
            return rnd;
        }

        public double NextFakeRnd()
        {
            return fakeRnds.Dequeue();
        }
    }
}