using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCompiler
{
    public class LetStatement : StatetmentBase
    {
        public string Variable { get; set; }
        public Expression Value { get; set; }

        public override string ToString()
        {
            return "let " + Variable + " = " + Value + ";";
        }

        public override void Parse(TokensStack sTokens)
        {
            Token t=sTokens.Pop();//let
            if (!(t is Statement)|| ((Statement)t).Name != "let")
                throw new SyntaxErrorException("Expected let received: " + t, t);
            t=sTokens.Pop();//variable
            if(!(t is Identifier))
                throw new SyntaxErrorException("Expected identifier received: " + t, t);
            Variable=((Identifier)t).Name;
            t=sTokens.Pop();//=
            if (!(t is Operator)|| ((Operator)t).Name != '=')
                throw new SyntaxErrorException("Expected operator received: " + t, t);
            Value=Expression.Create(sTokens);
            Value.Parse(sTokens);
            t=sTokens.Pop();//;
            if(!(t is Separator)|| ((Separator)t).Name != ';')
                throw new SyntaxErrorException("Expected identifier received: " + t, t);
        }

    }
}
