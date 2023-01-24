
using System;
using System.Collections.Generic;

enum TokenType {
    INTEGER,
    PLUS,
    MINUS,
    EOF
}

// List<TokenType> operators; // = new List<TokenType>() { PLUS, MINUS };

class Token
{
    public TokenType type;
    public dynamic? value;

    public Token(TokenType type, dynamic? value)
    {   // token type: INTEGER, PLUS, MINUS, or EOF
        this.type = type;

        // token value: non-negative integer value, '+', '-', or null
        this.value = value;
    }

    public string Str()
    {   /* string representation of class
        // Examples:
        //    Token(INTEGER, 3)
        //    Token(PLUS, '+')
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
    {   // client string input, e.g. "3 + 5", "12 - 5", etc
        this.text = text;

        // pos is an index into text
        this.pos = 0;

        // current token instance
        current_token = null;
        current_char = text[pos];
    }

    /****************************************************
    ** LEXER CODE                                       *
    *****************************************************/
    public void Error()
    {
        throw new Exception("Invalid syntax");
    }

    public void Advance()
    {   // Advance the 'pos' pionter and set the 'current_char' variable.
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
    {   // Return a (multidigit) integer consumed from the input.
        int ret = 0;
        while((current_char is not null) && (char.IsDigit((char)current_char!))) {
            ret = (ret * 10)+(int)(current_char!-'0');
            Advance();
        }
        return ret;
    }

    public Token GetNextToken()
    {   /* Lexical analyzer, also known as scanner or tokenizer
        // This method is responsible for breaking a sentence
        // apart into tokens. One token at a time.
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

    /************************************************************/
    /* PARSER / INTERPRETER CODE                                */
    /************************************************************/
    public void Eat(TokenType token_type)
    {
        /* compare the current token type with the passed token type
        // and if they match then "eat" the current token and
        // assign the next token to the current_token,
        // otherwise raise an exception
        */
        if((current_token is not null) && (current_token!.type==token_type))
            current_token = GetNextToken();
        else 
            Error();
    }

    public int Term()
    {   // Return an INTEGER token value
        Token ret = current_token!;
        Eat(TokenType.INTEGER);
        return ret.value;
    }

    public int Expr()
    {   // Arithmetic expression parser/interpreter
        int ret = 0;

        // set current token to the first token taken from the input
        current_token = GetNextToken();

        ret = Term();
        while(true) {
            Token token = current_token;
            if(token.type! == TokenType.PLUS) {
                Eat(TokenType.PLUS);
                ret += Term();
            } else if(token.type! == TokenType.MINUS) {
                Eat(TokenType.MINUS);
                ret -= Term();
            } else
                break;
        }
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
