
using System;

namespace PASCAL
{
    enum TOKEN_TYPE {
        INTEGER,
        PLUS,
        EOF
    }

    class Token
    {
        public TOKEN_TYPE type;
        public dynamic? value;

        public Token(TOKEN_TYPE type, dynamic? value)
        {
            this.type = type;
            this.value = value;
        }

        public string Str()
        {
            /* string representation of class
            Examples:
                Token(INTEGER, 3)
                Token(PLUS, '+')
            */
            string val = "none";
            if(!(value is null))
                val = $"{value!.ToString()}";
            return $"Token({type.ToString()}, {val})";
        }

        public string Repr()
        {
            return Str();
        }
    }

    class Interpreter
    {
        string text;
        int pos;
        Token? current_token;

        public Interpreter(string text)
        {
            this.text = text;
            this.pos = 0;
            current_token = null;
        }

        public void Error()
        {
            throw new Exception("Error parsing input");
        }

        public Token GetNextToken()
        {
            /* Lexical analyzer, also known as scanner or tokenizer
            This method is responsible for breaking a sentence
            apart into tokens. One token at a time.
            */
            Token ret = new Token(TOKEN_TYPE.EOF, null);

            if(pos<text.Length) {

                char current_char = text[pos];

                if(char.IsDigit(current_char)) {
                    ret = new Token(TOKEN_TYPE.INTEGER, (int)(current_char-'0'));
Console.WriteLine($"Integer token: {current_char}->{ret.value}");
                    pos++;
                } else if(current_char=='+') {
                    ret = new Token(TOKEN_TYPE.PLUS, current_char);
Console.WriteLine($"Plus token: {current_char}->{ret.value}");
                    pos++;
                } else {
                    Error();
                }
            }
            return ret;
        }

        public void Eat(TOKEN_TYPE token_type)
        {
            
            if((current_token is not null) && (current_token!.type==token_type))
                current_token = GetNextToken();
            else 
                Error();
        }

        public int Expr()
        {
            int ret = 0;
            // expr -> INTEGER PLUS INTEGER

            // set current token to the first toekn taken from the input
            current_token = GetNextToken();

            // we expect the current token to be a single-digit integer
            Token left = current_token;
            Eat(TOKEN_TYPE.INTEGER);

            // we expect the current token to be a '+' token
            Token op = current_token;
            Eat(TOKEN_TYPE.PLUS);

            // We expect the current token to be a single-digit integer
            Token right = current_token;
            Eat(TOKEN_TYPE.INTEGER);

            // after this point, INTEGER PLUS INTEGER sequence of tokens
            // has been successfully found and the method can just
            // return the result of adding two integer
            // thus effectively interpreting client input

            ret = left.value + right.value;

            return ret;
        }
    }

    class CALCULATOR
    {
        static int Main(string[] args)
        {
            Console.WriteLine("CALCULATOR Chapter 1");

            string text;

            while(true) {
                try {
                    Console.Write("calc> ");
                    text = Console.ReadLine()!;
                    if(text!="") {
                        Interpreter interpreter = new Interpreter(text);
                        int result = interpreter.Expr();
                        Console.WriteLine($"{result}");
                    }
                } catch(Exception e) {
                    Console.WriteLine($"{e.Message}");
                    break;
                }
            }

            return 0;
        }
    }
}

