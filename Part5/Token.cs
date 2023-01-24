using System;

enum TokenType {
    INTEGER, PLUS, MINUS, MUL, DIV, EOF
};

class Token
{
    // public enum Type {
    //     INTEGER, PLUS, MINUS, MUL, DIV, EOF
    // };

    public TokenType type;
    public dynamic? value;

    // public List<TokenType> operators = new List<TokenType>(){ TokenType.MUL, TokenType.DIV };

    public Token(TokenType type, dynamic? value)
    {
        // token type: INTEGER, PLUS, MINUS, MUL, DIV, or EOF
        this.type = type;

        // token value: non-negative integer value, '*', '/', or null
        this.value = value;
    }

    public string Str()
    {   /* string representation of the class instance.
        //
        // Examples:
        //      Token(INTEGER, 3)
        //      Token(PLUS, '+')
        //      Token(MUL, '*')    
        */
        string val = (value is null)?"none":value!.ToString();
        return $"Token({type.ToString()}, {val})";
    }

    public string Repr()
    {
        return Str();
    }
}
