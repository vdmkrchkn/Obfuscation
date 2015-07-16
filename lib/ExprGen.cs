using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obfuscation
{
    public class ExprGen // генератор выражений
    {
        /// <summary>
        /// Создание генератора выражений
        /// </summary>
        /// <param name="sx">имя известной переменной</param>
        /// <param name="nx">значение известной переменной</param>
        /// <param name="sy">имя неизвестной переменной</param>
        /// <param name="ny">значение неизвестной переменной</param>
        /// <param name="result">желаемый результат</param>
        public ExprGen(string sx, int nx, string sy, int ny, int result)
        {
            xId = new IdNode(sx,nx);
            yId = new IdNode(sy,ny);
            nResult = result;
        }        
        /// <summary>
        /// Генерация
        /// </summary>
        /// <param name="isPredTrue">должно ли условие быть истинным</param>
        /// <returns>искомое выражение в виде строки</returns>
        public virtual string generate(bool isPredTrue)
        {
            // формирование условного выражения
            if (!isPredTrue)
                e_lg = new BinOpNode(// x * x * (x + 1) * (x + 1) % 4 == 0
                    new BinOpNode(
                        new BinOpNode(
                            new BinOpNode(yId, yId, BinOpType.MULT),
                            new BinOpNode(yId, new IntNumNode(1), BinOpType.PLUS),
                            BinOpType.MULT),
                        new BinOpNode(yId, new IntNumNode(1), BinOpType.PLUS),
                        BinOpType.MULT),
                    new IntNumNode(4),
                    BinOpType.MOD);
            else
                e_lg = new BinOpNode(// (x [<=|>] 0) && !(2 * y & 1) == 1
                    new BinOpNode(
                        xId, new IntNumNode(0), xId.eval() <= 0 ? BinOpType.LE : BinOpType.GT),
                    new UnOpNode(
                        new BinOpNode(
                            new BinOpNode(new IntNumNode(2), yId, BinOpType.MULT),
                            new IntNumNode(1),
                            BinOpType.bAND),
                        UnOpType.NOT),
                    BinOpType.AND);
            // формирование 1-го выражения        
            if (yId.eval() != 0)
                expr1 = new BinOpNode(// y * (x - (x - nResult)) / y;
                    new BinOpNode(
                        yId,
                        new BinOpNode(
                            xId,
                            new BinOpNode(xId, new IntNumNode(nResult), BinOpType.MINUS),
                            BinOpType.MINUS),
                        BinOpType.MULT),
                    yId,
                    BinOpType.DIV);
            else
                expr1 = new IntNumNode(nResult);
            // формирование 2-го выражения          
            expr2 = new IntNumNode(nResult + new Random().Next(10, 21));
            int a, b;
            if (hasLinearSln(xId.eval(), yId.eval(), nResult, out a, out b))
                if (a != 0 && b != 0)
                    expr2 = new BinOpNode(// a * x + b * y
                        new BinOpNode(new IntNumNode(a), xId, BinOpType.MULT),
                        new BinOpNode(new IntNumNode(b), yId, BinOpType.MULT),
                        BinOpType.PLUS);            
            return String.Format("({0} ? {1} : {2})",
                e_lg, isPredTrue ? expr1 : expr2, isPredTrue ? expr2 : expr1);
        }
        // вывод конфигурации
        public override string ToString()
        {
            return string.Format("{0} = {1}\n", xId, xId.eval()) +
                string.Format("{0} = {1}\n", yId, yId.eval()) +
                string.Format("r = {0}", nResult);
        }
        /// <summary>
        /// Расширенный алгоритм Евклида
        /// </summary>
        /// <param name="a">1-е целое число</param>
        /// <param name="b">2-е целое число</param>
        /// <param name="x">коэффициент при a</param>
        /// <param name="y">коэффициент при b</param>
        /// <returns>наибольший общий делитель двух чисел</returns>          
        public static int gcd(int a, int b, out int x, out int y)
        {
            if (a == 0)
            {
                x = 0; y = b > 0 ? 1 : -1;
                return Math.Abs(b);
            }
            int x1, y1;
            int d = gcd(b % a, a, out x1, out y1);
            x = y1 - (b / a) * x1;
            y = x1;
            return d;
        }
        /// <summary>
        /// Определение наличия решения уравнения ax + by = c
        /// </summary>
        /// <param name="a">1-й коэффициент</param>
        /// <param name="b">2-й коэффициент</param>
        /// <param name="c">результат</param>
        /// <param name="x0">коэффициент при a</param>
        /// <param name="y0">коэффициент при b</param>
        /// <returns>eсли есть, то возвращается единственную пару корней (x0,y0)</returns>
        public static bool hasLinearSln(int a, int b, int c, out int x0, out int y0)
        {
            if (a == 0 && b == 0)
            {
                x0 = y0 = 0;
                return false;
            }
            int g = gcd(Math.Abs(a), Math.Abs(b), out x0, out y0);
            if (c % g != 0)
                return false;
            x0 *= c / g;
            y0 *= c / g;
            if (a < 0)
                x0 *= -1;
            if (b < 0)
                y0 *= -1;
            return true;
        }
        //
        protected IdNode xId, yId;  // входящие в выражение переменные        
        protected int nResult;	    // результат
        public ExprNode e_lg, expr1, expr2;
    }
}
