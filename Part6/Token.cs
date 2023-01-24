using System;

enum TokenType {
    INTEGER, PLUS, MINUS, MUL, DIV, LPAR, RPAR, EOF
};

class Token
{
    // public enum Type {
    //     INTEGER, PLUS, MINUS, MUL, DIV, EOF
    // };

    public TokenType type;
    public dynamic? value;

    public Token(TokenType type, dynamic? value)
    {
        this.type = type;
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
