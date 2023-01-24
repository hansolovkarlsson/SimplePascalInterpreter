
using System;

enum TokenType {
    INTEGER,
    PLUS,
    MINUS,
    EOF
}

class Token
{
    public TokenType type;
    public dynamic? value;

    public Token(TokenType type, dynamic? value)
    {
        // token type: INTEGER, PLUS, MINUS, or EOF
        this.type = type;
        // token value: non-negative integer value, '+', '-', or null
        this.value = value;
    }

    public string Str()
    {
        /* string representation of class
        Examples:
            Token(INTEGER, 3)
            Token(PLUS, '+')
        */
        string val = (value is null)?"none":value!.ToString();
        return $"Token({type.ToString()}, {val})";
    }

    public string Repr()
    {
        return Str();
    }
}

class Interpreter
{
    string  text;
    int     pos;
    Token?  current_token;
    char?   current_char;

    public Interpreter(string text)
    {
        // client string input, e.g. "3 + 5", "12 - 5", etc
        this.text = text;
        // pos is an index into text
        this.pos = 0;
        // current token instance
        current_token = null;
        current_char = text[pos];
    }

    public void Error()
    {
        throw new Exception("Error parsing input");
    }

    public void Advance()
    {
        // Advance the 'pos' pionter and set the 'current_char' variable.
        pos++;
        if(pos>(text.Length-1))
            current_char = null; // indicates end of input
        else
            current_char = text[pos];
    }

    public void SkipWhiteSpace()
    {
        while((current_char is not null) && (Char.IsWhiteSpace((char)current_char!)))
            Advance();
    }

    public int Integer()
    {
        // Return a (multidigit) integer consumed from the input.
        int ret = 0;
        while((current_char is not null) && (char.IsDigit((char)current_char!))) {
            ret = (ret * 10)+(int)(current_char!-'0');
            Advance();
        }
        return ret;
    }

    public Token GetNextToken()
    {
        /* Lexical analyzer, also known as scanner or tokenizer
        This method is responsible for breaking a sentence
        apart into tokens. One token at a time.
        */
        Token ret = new Token(TokenType.EOF, null);

        while(current_char is not null) {
            if(char.IsWhiteSpace((char)current_char!))
                SkipWhiteSpace();
            else if(char.IsDigit((char)current_char!)) {
                ret = new Token(TokenType.INTEGER, Integer());
                break;
            } else if (current_char! == '+') {
                Advance();
                ret = new Token(TokenType.PLUS, '+');
                break;
            } else if (current_char! == '-') {
                Advance();
                ret = new Token(TokenType.MINUS, '-');
                break;
            } else
                Error();
        }

        return ret;
    }

    public void Eat(TokenType token_type)
    {
        /* compare the current token type with the passed token type
        // and if they match then "eat" the current token and
        // assigne the next token toi the current_token,
        // otherwise raise an exception
        */
        
        if((current_token is not null) && (current_token!.type==token_type))
            current_token = GetNextToken();
        else 
            Error();
    }

    public int Expr()
    {
        /* Parser / Interpreter
        // expr -> INTEGER PLUS INTEGER
        // expr -> INTEGER MINUS INTEGER
        */

        int ret = 0;

        // set current token to the first toekn taken from the input
        current_token = GetNextToken();

        // we expect the current token to be a single-digit integer
        Token left = current_token;
        Eat(TokenType.INTEGER);

        // we expect the current token to be a '+' token
        Token op = current_token;
        if(op.type! == TokenType.PLUS)
            Eat(TokenType.PLUS);
        else
            Eat(TokenType.MINUS);

        // We expect the current token to be a single-digit integer
        Token right = current_token;
        Eat(TokenType.INTEGER);
        // after the above call the current_token is set to EOF token

        /* at this point either the INTEGER PLUS INTEGER or
        // the INTEGER MINUS INTEGER sequence of tokens
        // has been successfully found and the moethod can just
        // return the result of adding or subtracting two integer,
        // thus effectively interpreting client input
        */

        if(op.type! == TokenType.PLUS)
            ret = left.value + right.value;
        else
            ret = left.value - right.value;

        return ret;
    }
}

class CALCULATOR
{
    static int Main(string[] args)
    {
        Console.WriteLine("CALCULATOR Chapter 2");

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
