using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCompiler
{
    public class BinaryOperationExpression : Expression
    {
        public string Operator { get;  set; }
        public Expression Operand1 { get;  set; }
        public Expression Operand2 { get;  set; }

        public override string ToString()
        {
            return "(" + Operand1 + " " + Operator + " " + Operand2 + ")";
        }

        public override void Parse(TokensStack sTokens)
        {
            Token t=sTokens.Pop(); //(
            if (!(t is Parentheses)|| ((Parentheses)t).Name!='(')
                throw new SyntaxErrorException("Expected parentheses received: " + t, t);
            Operand1= Expression.Create(sTokens);
            Operand1.Parse(sTokens);
            t=sTokens.Pop(); //operator
            if (!(t is Operator)|| ((Operator)t).Name == '!')
                throw new SyntaxErrorException("Expected operator received: " + t, t);
            Operator=char.ToString(((Operator)t).Name);
            Operand2= Expression.Create(sTokens);
            Operand2.Parse(sTokens);
            t=sTokens.Pop(); //)
            if (!(t is Parentheses)|| ((Parentheses)t).Name!=')')
                throw new SyntaxErrorException("Expected parentheses received: " + t, t);
        }
    }
}
