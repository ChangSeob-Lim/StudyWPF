﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculatorApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Calculator calc = new Calculator();
            double a = 4.5, b = 2.5;
            double result = calc.Add(a, b);
            Console.WriteLine($"Result = {result}");
            result = calc.Subtract(a, b);
            Console.WriteLine($"Result = {result}");
            result = calc.Multiple(a, b);
            Console.WriteLine($"Result = {result}");
            result = calc.Divide(a, b);
            Console.WriteLine($"Result = {result}");
        }

        private static double Add(double a, double b)
        {
            return a + b;
        }
    }
}