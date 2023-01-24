
using System;
using System.Collections.Generic;

// Part4: multiply and divide, factor

enum TokenType {
    INTEGER,
    MUL,
    DIV,
    EOF
};

class Token
{
    public TokenType type;
    public dynamic? value;

    public List<TokenType> operators = new List<TokenType>(){ TokenType.MUL, TokenType.DIV };

    public Token(TokenType type, dynamic? value)
    {   // token type: INTEGER, MUL, DIV, or EOF
        this.type = type;

        // token value: non-negative integer value, '*', '/', or null
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

class Lexer
{
    string  text;
    int     pos;
    char?   current_char;

    public Lexer(string text)
    {
        // client string input, e.g. "3 * 5", "12 / 3 * 4", etc
        this.text = text;

        // pos is an index into text
        pos = 0;
        current_char = text[pos];
    }

    public void Error()
    {
        throw new Exception("Invalid character");
    }

    public void Advance()
    {
        // Advance the 'pos' pointer and set the 'current_char' variable
        pos++;
        if(pos>(text.Length-1))
            current_char = null;
        else
            current_char = text[pos];
    }

    public void SkipWhiteSpace()
    {
        while((current_char is not null) && (char.IsWhiteSpace((char)current_char!)))
            Advance();
    }

    public int Integer()
    {
        // Return a (multidigit) integer consumed from the input
        int ret = 0;
        while((current_char is not null) && (char.IsDigit((char)current_char!))) {
            ret = (ret * 10) + (int)(current_char! - '0');
            Advance();
        }
        return ret;
    }

    public Token GetNextToken()
    {
        // Lexical analyzer (also known as scanner or tokenizer)
        //
        // This method is responsible for breaking a sentence
        // apart into tokens. One token at a time.
        Token ret = new Token(TokenType.EOF, null);

        while(current_char is not null) {
            if(char.IsWhiteSpace((char)current_char!)) {
                SkipWhiteSpace(); // Advances on its own
            } else if (char.IsDigit((char)current_char!)) {
                ret = new Token(TokenType.INTEGER, Integer()); // Advances by itself
                break;
            } else if (current_char! == '*') {
                Advance();
                ret = new Token(TokenType.MUL, '*');
                break;
            } else if (current_char! == '/') {
                Advance();
                ret = new Token(TokenType.DIV, '/');
                break;
            } else {
                Error();
            }
        }
        return ret;
    }
}

class Interpreter
{
    Lexer lexer;
    Token?  current_token;

    // string  text;
    // int     pos;
    // char?   current_char;
    

    public Interpreter(Lexer lexer)
    {   // client string input, e.g. "3 + 5", "12 - 5", etc
        this.lexer = lexer;

        // set current token to the first token taken from the input
        current_token = lexer.GetNextToken();
    }

    public void Error()
    {
        throw new Exception("Invalid syntax");
    }

    public void Eat(TokenType token_type)
    {
        /* compare the current token type with the passed token type
        // and if they match then "eat" the current token and
        // assign the next token to the current_token,
        // otherwise raise an exception
        */
        if((current_token is not null) && (current_token!.type==token_type))
            current_token = lexer.GetNextToken();
        else 
            Error();
    }

    public int Factor()
    {   // Return an INTEGER token value
        // factor : INTEGER
        Token ret = current_token!;
        Eat(TokenType.INTEGER);
        return ret.value;
    }

    public int Expr()
    {   // Arithmetic expression parser/interpreter
        //
        // expr     : factor ((MUL | DIV) factor)*
        // factor   : INTEGER

        int ret = Factor();

        while(current_token is not null) {
            Token token = current_token!;
            if(token.type! == TokenType.MUL) {
                Eat(TokenType.MUL);
                ret = ret * Factor();
            } else if(token.type! == TokenType.DIV) {
                Eat(TokenType.DIV);
                ret = ret / Factor();
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
        Console.WriteLine("CALCULATOR Chapter 4 (mul/div,lex/syntax");

        string text;

        while(true) {
            try {
                Console.Write("calc> ");
                text = Console.ReadLine()!;
                if(text!="") {
                    Lexer lexer = new Lexer(text);
                    Interpreter interpreter = new Interpreter(lexer);
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
