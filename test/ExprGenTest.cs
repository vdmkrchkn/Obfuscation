using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Obfuscation;

namespace UnitTestObfuscation
{
    [TestClass]
    public class GcdTest
    {
        [TestMethod]
        public void gcdTest1()
        {
            int x, y;
            int d = ExprGen.gcd(7, 3, out x, out y);
            Assert.AreEqual(1, d, "(7,3)!=1");
            Assert.AreEqual(1, x, "значение x неверно");
            Assert.AreEqual(-2, y, "значение y неверно");
        }

        [TestMethod]
        public void gcdTest2()
        {
            int x, y;
            int d = ExprGen.gcd(12, 30, out x, out y);
            Assert.AreEqual(6, d, "(12,30)!=6");
            Assert.AreEqual(-2, x, "значение x неверно");
            Assert.AreEqual(1, y, "значение y неверно");
        }

        [TestMethod]
        public void gcdTest3()
        {
            int x, y;
            int d = ExprGen.gcd(991, 981, out x, out y);
            Assert.AreEqual(1, d, "(991,981)!=1");
            Assert.AreEqual(-98, x, "значение x неверно");
            Assert.AreEqual(99, y, "значение y неверно");
        }

        [TestMethod]
        public void gcdTest4()
        {
            int x, y;
            int d = ExprGen.gcd(5, 0, out x, out y);
            Assert.AreEqual(5, d, "(5,0)!=5");
            Assert.AreEqual(1, x, "значение x неверно");
            Assert.AreEqual(0, y, "значение y неверно");
        }

        [TestMethod]
        public void gcdTest5()
        {
            int x, y;
            int d = ExprGen.gcd(-7, 3, out x, out y);
            Assert.AreEqual(1, d, "(-7,3)!=1");
            Assert.AreEqual(-1, x, "значение x неверно");
            Assert.AreEqual(-2, y, "значение y неверно");
        }
    }
    
    [TestClass]
    public class LinEqTest
    {
        [TestMethod]
        public void LinEqTest1()
        {
            int x, y;            
            bool b = ExprGen.hasLinearSln(5, 4, 1, out x, out y);
            Assert.IsTrue(b, "5*x + 4*y != 1");
            Assert.AreEqual(1, x, "значение x неверно");
            Assert.AreEqual(-1, y, "значение y неверно");
        }

        [TestMethod]
        public void LinEqTest2()
        {
            int x, y;
            bool b = ExprGen.hasLinearSln(-5, 5, -4, out x, out y);
            Assert.IsFalse(b, "-5*x + 5*y == -4");            
        }
    }

    [TestClass]
    public class ExprGenTest
    {
        [TestMethod]
        public void ExprGenTest1()
        {
            int res = 5;
            ExprGen egen = new ExprGen("i", 11, "j", 7, res);            
            egen.generate(true);
            Assert.AreEqual(1, egen.Cond.eval(), "неверный предикат");
            Assert.AreEqual(res, egen.Expr1.eval(), "неверный результат вычисления");
            Assert.AreEqual(res, egen.Expr2.eval(), "неверный результат вычисления");
        }

        [TestMethod]
        public void ExprGenTest2()
        {
            int res = 7;
            ExprGen egen = new ExprGen("i", 13, "j", 11, res);
            egen.generate(false);
            Assert.AreEqual(0, egen.Cond.eval(), "неверный предикат");
            Assert.AreEqual(res, egen.Expr1.eval(), "неверный результат вычисления");
            Assert.AreEqual(res, egen.Expr2.eval(), "неверный результат вычисления");
        }
    }
}
