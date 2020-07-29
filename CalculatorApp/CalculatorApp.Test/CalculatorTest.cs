using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CalculatorApp.Test
{
    [TestClass]
    public class CalculatorTest
    {
        [TestMethod]
        public void TestAdd3and7()
        {
            // 3 + 7 = 10
            double a = 3;
            double b = 7;
            double expected = 10.0;

            Calculator calc = new Calculator();
            double actual = calc.Add(a, b);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestAdd5and23()
        {
            // 5 + 23 = 28
            double a = 5;
            double b = 23;
            double expected = 28;

            Calculator calc = new Calculator();
            double actual = calc.Add(a, b);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestMultiple4and5()
        {
            // 4 * 5 = 20
            double a = 4.0;
            double b = 5.0;
            double expected = 20;

            Calculator calc = new Calculator();
            double actual = calc.Multiple(a, b);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDivide10and2()
        {
            // 10 / 2 = 5
            double a = 10;
            double b = 2;
            double expected = 5.0;

            Calculator calc = new Calculator();
            double actual = calc.Divide(a, b);

            Assert.AreEqual(expected, actual);
        }
    }
}
