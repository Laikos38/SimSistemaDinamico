﻿using GeneradorDeNumerosAleatorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomVarGenerator
{
    class UniformGenerator
    {
        public double a { get; set; }
        public double b { get; set; }
        
        public Generator congruentialGenerator = new Generator() { seed = 31767, a = 71561, c = 56822, M = 341157 };

        public double Generate(double rnd)
        {
            double num = a + rnd * (b - a);
            return num;
        }
    }
}
