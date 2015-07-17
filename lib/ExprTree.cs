using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obfuscation
{
    public enum BinOpType
    {
        NONE = 0,
        PLUS, MINUS, DIV, MULT, MOD, SHL, SHR,
        LT, GT, EQ, NE, LE, GE,
        OR, AND, bOR, bAND, XOR
    };

    public enum UnOpType
    {
        NONE = 0,
        PLUS, MINUS, INC, DEC,
        NOT, bNOT
    };

    public abstract class ExprNode // базовый класс для всех выражений
    {
        public abstract int eval();
    }

    public class IdNode : ExprNode
    {
        string Name { get; set; }
        int value;

        public IdNode(string name, int val = 0)
        {
            Name = name;
            value = val;
        }        

        public override string ToString()
        {
            return Name;
        }

        public override int eval()
        {
            return value;
        }
    }

    public class IntNumNode : ExprNode
    {
        int Num { get; set; }
        public IntNumNode(int num) { Num = num; }

        public override string ToString()
        {
            return Num.ToString();
        }

        public override int eval()
        {
            return Num;
        }
    }

    public class UnOpNode : ExprNode
    {        
        ExprNode expr { get; set; }
        UnOpType Op { get; set; }

        public UnOpNode(ExprNode e, UnOpType operation)
        {            
            expr = e;
            Op = operation;
        }        

        public override string ToString()
        {
            string strOp;
            switch (Op)
            {
                case UnOpType.PLUS:
                    strOp = "+";
                    break;
                case UnOpType.MINUS:
                    strOp = "-";
                    break;
                case UnOpType.INC:
                    strOp = "++";
                    break;
                case UnOpType.DEC:
                    strOp = "--";
                    break;
                case UnOpType.NOT:
                    strOp = "!";
                    break;
                case UnOpType.bNOT:
                    strOp = "~";
                    break;
                default:
                    strOp = "None";
                    break;
            }
            return String.Format("({0}{1})", strOp, expr.ToString());
        }

        public override int eval()
        {
            switch (Op)
            {
                case UnOpType.PLUS:
                    return expr.eval();
                case UnOpType.MINUS:
                    return -expr.eval();
                case UnOpType.INC:
                    return expr.eval() + 1;
                case UnOpType.DEC:
                    return expr.eval() - 1;
                case UnOpType.NOT:
                    return expr.eval() == 0 ? 1 : 0;
                case UnOpType.bNOT:
                    return expr.eval() == 0 ? 1 : 0;
                default:
                    throw new ArgumentException("недопустимая операция");
            }            
        }
    }

    public class BinOpNode : ExprNode
    {
        ExprNode Left { get; set; }
        ExprNode Right { get; set; }
        BinOpType Op { get; set; }

        public BinOpNode(ExprNode left, ExprNode right, BinOpType operation)
        {
            Left = left;
            Right = right;
            Op = operation;
        }        

        public override string ToString()
        {
            string strOp;
            switch (Op)
            {
                case BinOpType.PLUS:
                    strOp = "+";
                    break;
                case BinOpType.MINUS:
                    strOp = "-";
                    break;
                case BinOpType.MULT:
                    strOp = "*";
                    break;
                case BinOpType.DIV:
                    strOp = "/";
                    break;
                case BinOpType.MOD:
                    strOp = "%";
                    break;
                case BinOpType.SHL:
                    strOp = "<<";
                    break;
                case BinOpType.SHR:
                    strOp = ">>";
                    break;
                case BinOpType.LT:
                    strOp = "<";
                    break;
                case BinOpType.GT:
                    strOp = ">";
                    break;
                case BinOpType.EQ:
                    strOp = "==";
                    break;
                case BinOpType.NE:
                    strOp = "!=";
                    break;
                case BinOpType.LE:
                    strOp = "<=";
                    break;
                case BinOpType.GE:
                    strOp = ">=";
                    break;
                case BinOpType.bOR:
                    strOp = "|";
                    break;
                case BinOpType.OR:
                    strOp = "||";
                    break;
                case BinOpType.bAND:
                    strOp = "&";
                    break;
                case BinOpType.AND:
                    strOp = "&&";
                    break;
                case BinOpType.XOR:
                    strOp = "^";
                    break;
                default:
                    strOp = "None";
                    break;
            }
            return String.Format("({0} {1} {2})", Left.ToString(), strOp, Right.ToString());
        }

        public override int eval()
        {            
            switch (Op)
            {
                case BinOpType.PLUS:
                    return Left.eval() + Right.eval();                    
                case BinOpType.MINUS:
                    return Left.eval() - Right.eval();
                case BinOpType.MULT:
                    return Left.eval() * Right.eval();
                case BinOpType.DIV:
                    return Left.eval() / Right.eval();
                case BinOpType.MOD:
                    return Left.eval() % Right.eval();
                case BinOpType.SHL:
                    return Left.eval() << Right.eval();
                case BinOpType.SHR:
                    return Left.eval() >> Right.eval();
                case BinOpType.LT:
                    return Left.eval() < Right.eval() ? 1 : 0;
                case BinOpType.GT:
                    return Left.eval() > Right.eval() ? 1 : 0;
                case BinOpType.EQ:
                    return Left.eval() == Right.eval() ? 1 : 0;
                case BinOpType.NE:
                    return Left.eval() != Right.eval() ? 1 : 0;
                case BinOpType.LE:
                    return Left.eval() <= Right.eval() ? 1 : 0;                    
                case BinOpType.GE:
                    return Left.eval() >= Right.eval() ? 1 : 0;                    
                case BinOpType.bOR:
                    return Left.eval() | Right.eval();                    
                case BinOpType.OR:
                    if (Left.eval() != 0)
                        return 1;
                    else
                        return Right.eval();                    
                case BinOpType.bAND:
                    return Left.eval() & Right.eval();
                case BinOpType.AND:
                    if (Left.eval() == 0)
                        return 0;
                    else
                        return Right.eval();
                case BinOpType.XOR:
                    return Left.eval() ^ Right.eval();                    
                default:
                    throw new ArgumentException("недопустимая операция");
            }
        }
    }
}
