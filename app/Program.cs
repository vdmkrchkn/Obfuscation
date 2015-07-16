using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obfuscation;

namespace ConsoleApplication1
{
    class MyExprGen : ExprGen
    {
        public MyExprGen(string sx, int nx, string sy, int ny, int result)
            : base(sx, nx, sy, ny, result)
        { }
        //
        public override string generate(bool isPredTrue)
        {
            return base.generate(isPredTrue); // or suggest your generation method...
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Random rnd = new Random();
            int i = 77,
                j = rnd.Next(10), // 0
                k = 1;            
            ExprGen egen = new MyExprGen("i", i, "j", j, k);
            Console.WriteLine(egen);
            Console.WriteLine(egen.generate(true));
            //            
            Console.ReadLine();
        }
    }
}
