using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCompiler
{
    public class Compiler
    {


        public Compiler()
        {

        }


        public List<VarDeclaration> ParseVarDeclarations(List<string> lVarLines)
        {
            List<VarDeclaration> lVars = new List<VarDeclaration>();
            for(int i = 0; i < lVarLines.Count; i++)
            {
                List<Token> lTokens = Tokenize(lVarLines[i], i);
                TokensStack stack = new TokensStack(lTokens);
                VarDeclaration var = new VarDeclaration();
                var.Parse(stack);
                lVars.Add(var);
            }
            return lVars;
        }


        public List<LetStatement> ParseAssignments(List<string> lLines)
        {
            List<LetStatement> lParsed = new List<LetStatement>();
            List<Token> lTokens = Tokenize(lLines);
            TokensStack sTokens = new TokensStack();
            for (int i = lTokens.Count - 1; i >= 0; i--)
                sTokens.Push(lTokens[i]);
            while(sTokens.Count > 0)
            {
                LetStatement ls = new LetStatement();
                ls.Parse(sTokens);
                lParsed.Add(ls);

            }
            return lParsed;
        }

 

        public List<string> GenerateCode(LetStatement aSimple, Dictionary<string, int> dSymbolTable)
        {
            List<string> lAssembly = new List<string>();
            //add here code for computing a single let statement containing only a simple expression
            Expression e=aSimple.Value;

            if (e is NumericExpression)
            {
                lAssembly.Add("@"+((NumericExpression)e).ToString());
                lAssembly.Add("D=A");
                lAssembly.Add("@RESULT");
                lAssembly.Add("M=D");
            }
            if (e is VariableExpression)
            {
                if (!dSymbolTable.ContainsKey(e.ToString()))
                        throw new SyntaxErrorException("The var " + e.ToString()+" is not exist", new Identifier(e.ToString(),0,0));
                lAssembly.Add("@LCL");
                lAssembly.Add("D=M");
                lAssembly.Add("@"+dSymbolTable[e.ToString()]);
                lAssembly.Add("A=D+A");
                lAssembly.Add("D=M");
                lAssembly.Add("@RESULT");
                lAssembly.Add("M=D");
            }
          /*  if (e is UnaryOperatorExpression)
            {
                String s=e.ToString();
                char op=s[0];
                string op1=s.Substring(1);
                int x;
                if (int.TryParse(op1,out x))
                {
                    lAssembly.Add("@"+op1);
                    lAssembly.Add("D=A");
                    lAssembly.Add("@RESULT");
                    lAssembly.Add("M=D");
                }
                else // if op1 is ID
                {
                    if (!dSymbolTable.ContainsKey(op1))
                        throw new SyntaxErrorException("The var " + op1+" is not exist", op1);
                    lAssembly.Add("@LCL");
                    lAssembly.Add("D=M");
                    lAssembly.Add("@"+dSymbolTable[op1]);
                    lAssembly.Add("A=D+A");
                    lAssembly.Add("@RESULT");
                    lAssembly.Add("M=D");
                }
                lAssembly.Add("@RESULT");
                lAssembly.Add("D=M");
                if (op=='-')
                {
                    lAssembly.Add("M=-D");
                }
                else
                {
                    lAssembly.Add("@IS_FALSE");
                    lAssembly.Add("D;JEQ");
                    lAssembly.Add("@RESULT");
                    lAssembly.Add("M=0");
                    lAssembly.Add("@EDN_NOT");
                    lAssembly.Add("0;JMP");
                    lAssembly.Add("(IS_FALSE)");
                    lAssembly.Add("@RESULT");
                    lAssembly.Add("M=1");
                    lAssembly.Add("(END_NOT");
                }
            }
            */
            if (e is BinaryOperationExpression)
            {
                String s=e.ToString();
                 int i1 = s.IndexOf(' ');
                 String op1 = s.Substring(1, i1-1);
                 int i2 = s.IndexOf(' ', i1+1);
                 String op = s.Substring(i2 - 1, 1);
                 int i3 = s.Length - 2 - i2;
                 String op2 = s.Substring(i2 + 1, i3);
               
                

                //code for operand1
                int x;
                if (int.TryParse(op1,out x))
                {
                    lAssembly.Add("@"+op1);
                    lAssembly.Add("D=A");
                    lAssembly.Add("@RESULT");
                    lAssembly.Add("M=D");
                }
                else //if op1 is ID
                {
                    if (!dSymbolTable.ContainsKey(op1))
                        throw new SyntaxErrorException("The var " + op1+" is not exist", new Identifier(op1,0,0));
                    lAssembly.Add("@LCL");
                    lAssembly.Add("D=M");
                    lAssembly.Add("@"+dSymbolTable[op1]);
                    lAssembly.Add("A=D+A");
                    lAssembly.Add("D=M");
                    lAssembly.Add("@RESULT");
                    lAssembly.Add("M=D");
                }

                lAssembly.Add("@RESULT");
                lAssembly.Add("D=M");
                lAssembly.Add("@OPERAND1");
                lAssembly.Add("M=D");

                //code for operend2
                if (int.TryParse(op2,out x))
                {
                    lAssembly.Add("@"+op2);
                    lAssembly.Add("D=A");
                    lAssembly.Add("@RESULT");
                    lAssembly.Add("M=D");
                }
                else //if OP2 is ID
                {
                    if (!dSymbolTable.ContainsKey(op2))
                        throw new SyntaxErrorException("The var " + op2+" is not exist", new Identifier(op2,0,0));
                    lAssembly.Add("@LCL");
                    lAssembly.Add("D=M");
                    lAssembly.Add("@"+dSymbolTable[op2]);
                    lAssembly.Add("A=D+A");
                    lAssembly.Add("D=M");
                    lAssembly.Add("@RESULT");
                    lAssembly.Add("M=D");
                }

                lAssembly.Add("@RESULT");
                lAssembly.Add("D=M");
                lAssembly.Add("@OPERAND2");
                lAssembly.Add("M=D");


                //compute
                lAssembly.Add("@OPERAND1");
                lAssembly.Add("D=M");
                lAssembly.Add("@OPERAND2");
                lAssembly.Add("A=M");

                if(op=="-")
                    lAssembly.Add("D=D-A");
                else
                    lAssembly.Add("D=D+A");
                lAssembly.Add("@RESULT");
                lAssembly.Add("M=D");
            }

            //let
            if (!dSymbolTable.ContainsKey(aSimple.Variable))
                throw new SyntaxErrorException("The var " + aSimple.Variable+" is not exist",new Identifier(aSimple.Variable,0,0));
            int index=dSymbolTable[aSimple.Variable];
            lAssembly.Add("@"+Convert.ToString(index));
            lAssembly.Add("D=A");
            lAssembly.Add("@LCL");
            lAssembly.Add("D=D+M");
            lAssembly.Add("@ADDRESS");
            lAssembly.Add("M=D");
            lAssembly.Add("@RESULT");
            lAssembly.Add("D=M");
            lAssembly.Add("@ADDRESS");
            lAssembly.Add("A=M");
            lAssembly.Add("M=D");
            return lAssembly;
        }


        public Dictionary<string, int> ComputeSymbolTable(List<VarDeclaration> lDeclerations)
        {
            Dictionary<string, int> dTable = new Dictionary<string, int>();
            //add here code to comptue a symbol table for the given var declarations
            //real vars should come before (lower indexes) than artificial vars (starting with _), and their indexes must be by order of appearance.
            //for example, given the declarations:
            //var int x;
            //var int _1;
            //var int y;
            //the resulting table should be x=0,y=1,_1=2
            //throw an exception if a var with the same name is defined more than once
            int index=0;
            for (int i=0;i<lDeclerations.Count;i++)
            {
                if(lDeclerations[i].Name[0]=='_')
                    continue;
                if(dTable.ContainsKey(lDeclerations[i].Name)==true)
                    throw new SyntaxErrorException("The name of the var " + lDeclerations[i].Name+" is already exist", new Identifier(lDeclerations[i].Name,0,0));
                dTable.Add(lDeclerations[i].Name,index);
                index++;
            }
            for (int i=0;i<lDeclerations.Count;i++)
            {
                if(lDeclerations[i].Name[0]!='_')
                    continue;
                if(dTable.ContainsKey(lDeclerations[i].Name)==true)
                    throw new SyntaxErrorException("The name of the var " + lDeclerations[i].Name+" is already exist", new Identifier(lDeclerations[i].Name,0,0));
                dTable.Add(lDeclerations[i].Name,index);
                index++;
            }
            return dTable;
        }


        public List<string> GenerateCode(List<LetStatement> lSimpleAssignments, List<VarDeclaration> lVars)
        {
            List<string> lAssembly = new List<string>();
            Dictionary<string, int> dSymbolTable = ComputeSymbolTable(lVars);
            foreach (LetStatement aSimple in lSimpleAssignments)
                lAssembly.AddRange(GenerateCode(aSimple, dSymbolTable));
            return lAssembly;
        }
        int index=1;
        public List<LetStatement> SimplifyExpressions(LetStatement s, List<VarDeclaration> lVars)
        {
            //add here code to simply expressins in a statement. 
            //add var declarations for artificial variables.
            List<LetStatement> lSimplified = new List<LetStatement>();
          //  int index=1;
            if (s.Value is BinaryOperationExpression)
            {
            BinaryOperationExpression e=(BinaryOperationExpression)s.Value;
            String str=e.ToString();
            int close,x;
            close=str.IndexOf(')');
            String simple1;
            List<Token> lTokens;
            while (close!=str.Length-1)
            {
                int i=close-1;
                while (str[i]!='(')
                {
                    i--;
                }
                String simple=str.Substring(i,close-i+1);
                simple1="let _"+index+" = "+simple+";";
                lTokens = Tokenize(simple1, 0);
                LetStatement assignment = ParseStatement(lTokens);
                lSimplified.Add(assignment);
                VarDeclaration var=new VarDeclaration("int","_"+index);
                lVars.Add(var);
                str=str.Replace(simple,"_"+index);
                index++;
                close=str.IndexOf(')');
            }
            simple1="let "+s.Variable+" = "+str+";";
            List<Token> lTokens1 = Tokenize(simple1, 0);
            LetStatement assignment1 = ParseStatement(lTokens1);
            lSimplified.Add(assignment1);
            }
            else
                lSimplified.Add(s);
            return lSimplified;
        }
        public List<LetStatement> SimplifyExpressions(List<LetStatement> ls, List<VarDeclaration> lVars)
        {
            List<LetStatement> lSimplified = new List<LetStatement>();
            foreach (LetStatement s in ls)
                lSimplified.AddRange(SimplifyExpressions(s, lVars));
            return lSimplified;
        }

 
        public LetStatement ParseStatement(List<Token> lTokens)
        {
            TokensStack sTokens = new TokensStack();
            for (int i = lTokens.Count - 1; i >= 0; i--)
                sTokens.Push(lTokens[i]);
            LetStatement s = new LetStatement();
            s.Parse(sTokens);
            return s;
        }


        //Splits a string into a list of tokens, separated by delimiters
        private List<string> Split(string s, char[] aDelimiters)
        {
            List<string> lTokens = new List<string>();
            while (s.Length > 0)
            {
                string sToken = "";
                int i = 0;
                for (i = 0; i < s.Length; i++)
                {
                    if (aDelimiters.Contains(s[i]))
                    {
                        if (sToken.Length > 0)
                            lTokens.Add(sToken);
                        lTokens.Add(s[i] + "");
                        break;
                    }
                    else
                        sToken += s[i];
                }
                if (i == s.Length)
                {
                    lTokens.Add(sToken);
                    s = "";
                }
                else
                    s = s.Substring(i + 1);
            }
            return lTokens;
        }

 
        public List<Token> Tokenize(string sLine, int iLine)
        {
            List<Token> lTokens = new List<Token>();
            //your code here
            Token t=new Token();
            int pos=0,x;
            List<string> splitLine=new List<string>();
            
            char [] aDelimiters=new char[21];
            char[] Separators = new char[] {  ',', ';'};
            char[] Parentheses = new char[] { '(', ')', '[', ']', '{', '}' };
            char[] Operators = new char[] { '*', '+', '-', '/', '<', '>', '&', '=', '|', '!' };
            Array.Copy(Operators,aDelimiters,10);
            Array.Copy(Parentheses,0,aDelimiters,10,6);
            Array.Copy(Separators,0,aDelimiters,16,2);
            aDelimiters[18]=' ';
            aDelimiters[19]='\t';
            aDelimiters[20]='.';

            splitLine=Split(sLine,aDelimiters);

      //      if (splitLine[0]=="/"&&splitLine[1]=="/")
       //             continue;

            for(int j=0;j<splitLine.Count;j++)
                {
                    if(splitLine[j]=="/"&&splitLine[j+1]=="/")
                        break;
                    if(t.getStatements().Contains(splitLine[j]))
                    {
                        lTokens.Add(new Statement(splitLine[j],iLine,pos));
                        pos+=splitLine[j].Length;
                    }
                    else if(t.getVarTypes().Contains(splitLine[j]))
                    {
                        lTokens.Add(new VarType(splitLine[j],iLine,pos));
                        pos+=splitLine[j].Length;
                    }
                    else if(t.getConstants().Contains(splitLine[j]))
                    {
                        lTokens.Add(new Constant(splitLine[j],iLine,pos));
                        pos+=splitLine[j].Length;
                    }
                    else if(t.getOperators().Contains(splitLine[j][0]))
                    {
                        lTokens.Add(new Operator(splitLine[j][0],iLine,pos));
                        pos+=splitLine[j].Length;
                    }
                    else if(t.getParentheses().Contains(splitLine[j][0]))
                    {
                        lTokens.Add(new Parentheses(splitLine[j][0],iLine,pos));
                        pos+=splitLine[j].Length;
                    }
                    else if(t.getSeparators().Contains(splitLine[j][0]))
                    {
                        lTokens.Add(new Separator(splitLine[j][0],iLine,pos));
                        pos+=splitLine[j].Length;
                    }
                    else if(int.TryParse(splitLine[j],out x))
                    {
                        lTokens.Add(new Number(splitLine[j],iLine,pos));
                        pos+=splitLine[j].Length;
                    }
                    else if((splitLine[j][0]>=65&&splitLine[j][0]<=90)||(splitLine[j][0]>=97&&splitLine[j][0]<=122)||splitLine[j][0]=='_')
                    {
                        lTokens.Add(new Identifier(splitLine[j],iLine,pos));
                        pos+=splitLine[j].Length;
                    }
                    else if(splitLine[j][0]>=48&&splitLine[j][0]<=57&&!int.TryParse(splitLine[j],out x))
                    {
                        throw new SyntaxErrorException("The identifier is not legal",new Identifier(splitLine[j],j,pos));
                    //    lTokens.Add(e.Token);
                      //  pos+=splitLine[j].Length;
                    }
                    else if(splitLine[j][0]==' '||splitLine[j][0]=='\t')
                    {
                        pos+=splitLine[j].Length;
                        continue;
                    }
                    else if((splitLine[j][0] < 65 || splitLine[j][0] > 90) && (splitLine[j][0] < 97 || splitLine[j][0] > 122) && !int.TryParse(splitLine[j], out iLine))
                    {
                        throw new SyntaxErrorException("The operator is not legal",new Operator(splitLine[j][0],j,pos));
                       // lTokens.Add(e.Token);
                       // pos+=splitLine[j].Length;
                    }
                    
                }
            
            return lTokens;
        }
        

        public List<Token> Tokenize(List<string> lCodeLines)
        {
            List<Token> lTokens = new List<Token>();
            for (int i = 0; i < lCodeLines.Count; i++)
            {
                string sLine = lCodeLines[i];
                List<Token> lLineTokens = Tokenize(sLine, i);
                lTokens.AddRange(lLineTokens);
            }
            return lTokens;
        }

    }
}
