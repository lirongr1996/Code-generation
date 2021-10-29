using System;
using System.Collections.Generic;

namespace SimpleCompiler
{
    public class FunctionCallExpression : Expression
    {
        public string FunctionName { get; private set; }
        public List<Expression> Args { get; private set; }

        public FunctionCallExpression()
        {
            Args=new List<Expression>();
        }

        public override void Parse(TokensStack sTokens)
        {
            Token t=sTokens.Pop(); //name of the function
            if(!(t is Identifier))
                throw new SyntaxErrorException("Expected identifier received: " + t, t);
            FunctionName=((Identifier)t).Name;
            
            t=sTokens.Pop(); //(
            if (!(t is Parentheses)|| ((Parentheses)t).Name!='(')
                throw new SyntaxErrorException("Expected parentheses received: " + t, t);
            while (sTokens.Count>0 && !((sTokens.Peek() is Parentheses) && (((Parentheses)sTokens.Peek()).Name == ')')))
            {
               Expression e=Expression.Create(sTokens);
                e.Parse(sTokens);
                Args.Add(e);
                
                if (sTokens.Count > 0 && sTokens.Peek() is Separator)//,
                {
                    t=sTokens.Pop();
                    if (!(t is Separator)|| ((Separator)t).Name!=',')
                        throw new SyntaxErrorException("Expected Separator received: " + t, t);
                }
            }
            
            t=sTokens.Pop(); //)
            if (!(t is Parentheses)|| ((Parentheses)t).Name!=')')
                throw new SyntaxErrorException("Expected parentheses received: " + t, t);
     //       t=sTokens.Pop(); //;
       //     if (!(t is Separator)|| ((Separator)t).Name!=';')
        //        throw new SyntaxErrorException("Expected separator received: " + t, t);
        }

        public override string ToString()
        {
            string sFunction = FunctionName + "(";
            for (int i = 0; i < Args.Count - 1; i++)
                sFunction += Args[i] + ",";
            if (Args.Count > 0)
                sFunction += Args[Args.Count - 1];
            sFunction += ")";
            return sFunction;
        }
    }
}