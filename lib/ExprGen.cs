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
            xId = new IdNode(sx, nx);
            yId = new IdNode(sy, ny);
            nResult = result;
            //
            List<ExprNode> eList = new List<ExprNode>(2);            
            eList.Add(new BinOpNode(// x > 0
                        xId, new IntNumNode(0), BinOpType.GT));            
            eList.Add(new UnOpNode(// !(2 * y & 1)
                        new BinOpNode(
                            new BinOpNode(new IntNumNode(2), yId, BinOpType.MULT),
                            new IntNumNode(1),
                            BinOpType.bAND),
                        UnOpType.NOT));            
            prGen = new PredGen(eList, 2);            
        }        
        /// <summary>
        /// Генерация
        /// </summary>
        /// <param name="isPredTrue">должно ли условие быть истинным</param>
        /// <returns>искомое выражение в виде строки</returns>
        public virtual string generate(bool isPredTrue)
        {
            // формирование условного выражения
            Cond = prGen.getPred(isPredTrue);                            
            // формирование 1-го выражения
            if(nResult == 0 && ((xId.eval() & 1) == 0 || (yId.eval() & 1) != 0) 
				|| (nResult == 1 && ((xId.eval() & 1) != 0) && (yId.eval() & 1) == 0))
                Expr1 = new BinOpNode(// x * x * (x + y) * (x + y) % 4
                    new BinOpNode(
                        new BinOpNode(
                            new BinOpNode(xId, xId, BinOpType.MULT),
                            new BinOpNode(yId, xId, BinOpType.PLUS),
                            BinOpType.MULT),
                        new BinOpNode(yId, xId, BinOpType.PLUS),
                        BinOpType.MULT),
                    new IntNumNode(4),
                    BinOpType.MOD);
            else if (yId.eval() != 0)
                Expr1 = new BinOpNode(// y * (x - (x - nResult)) / y;
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
                Expr1 = new BinOpNode(yId, new IntNumNode(nResult),BinOpType.PLUS);
            // формирование 2-го выражения          
            Expr2 = new BinOpNode(// x * x - c * x + nResult
                new BinOpNode(
                    new BinOpNode(xId,xId,BinOpType.MULT),
                    new BinOpNode(new IntNumNode(xId.eval()),xId,BinOpType.MULT),
                    BinOpType.MINUS),
                new IntNumNode(nResult), BinOpType.PLUS);
            int a, b;
            if (hasLinearSln(xId.eval(), yId.eval(), nResult, out a, out b))
                if (a != 0 && b != 0) // есть неоднородные решения
                    Expr2 = new BinOpNode(// a * x + b * y
                        new BinOpNode(new IntNumNode(a), xId, BinOpType.MULT),
                        new BinOpNode(new IntNumNode(b), yId, BinOpType.MULT),
                        BinOpType.PLUS);            
            return String.Format("({0} ? {1} : {2})",
                Cond, isPredTrue ? Expr1 : Expr2, isPredTrue ? Expr2 : Expr1);
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
        public ExprNode Cond { get; protected set; }
        public ExprNode Expr1 { get; protected set; }
        public ExprNode Expr2 { get; protected set; }
        protected PredGen prGen;
    }

    public class PredGen // генератор предикатов
    {
        /// <summary>
        /// Построение таблицы предикатов
        /// </summary>
        /// <param name="simpleExprs">набор различных простых предикатов</param>        
        /// <param name="k">уровень таблицы</param>
        public PredGen(List<ExprNode> simpleExprs, int k)
        {
            table = new List<List<ExprNode>>(k);
            // age = 1
            List<ExprNode> exprList = new List<ExprNode>();
            foreach (ExprNode e in simpleExprs)
            {
                exprList.Add(e);
                exprList.Add(new UnOpNode(e, UnOpType.NOT));
            }
            table.Add(exprList);
            // 2 <= age <= k
            for (int age = 1; age < k; ++age)
            {
                exprList = new List<ExprNode>();
                for (int i = 0; i < table[age-1].Count; ++i)
                    for (int j = i + 1; j < table[age - 1].Count; ++j)
                    {
                        exprList.Add(new BinOpNode(table[age - 1][i], table[age - 1][j],
                            BinOpType.OR));
                        exprList.Add(new BinOpNode(table[age - 1][i], table[age - 1][j],
                            BinOpType.AND));
                    }
                table.Add(exprList);
            }
        }
        /// <summary>
        /// Вывод таблицы на экран
        /// </summary>
        public void printTable()
        {
            for(int i = 0; i < table.Count; ++i)
            {
                Console.WriteLine("Age = " + (i + 1));
                Console.WriteLine();
                foreach (ExprNode expr in table[i])
                    Console.WriteLine(expr);
            }
        }
        // получение случайного предиката, удовлетворяющего pred
        public ExprNode getPred(bool pred)
        {            
            var res = table.Last().Where(expr => expr.eval() == (pred ? 1 : 0));
            Random rnd = new Random();
            return res.ElementAt(rnd.Next(res.Count()));
        }
        //
        List<List<ExprNode>> table; // таблица предикатов
    }
}
