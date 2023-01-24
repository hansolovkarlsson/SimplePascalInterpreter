using System;
using System.Collections.Generic;

/* Token class used by Lexer */

class Token
{
    public enum TYPE {
        INTEGER, PLUS, MINUS, MUL, DIV, LPAR, RPAR, ID, ASSIGN,
        BEGIN, END, SEMI, DOT, EOF
    };

    public TYPE type;
    public dynamic? value;

    public Token(TYPE type, dynamic? value)
    {
        this.type = type;
        this.value = value;

        Debug.Trace(Debug.MODULE.TOKEN, Str());
    }

    public string Str()
    {
        /* string representation of the class instance.
        //
        // Examples:
        //      Token(INTEGER, 3)
        //      Token(PLUS, '+')
        //      Token(MUL, '*')    
        */
        string val = (value is null)?"none":value!.ToString();
        string str = $"Token({type.ToString()}, {val})";

        Debug.Trace(Debug.MODULE.TOKEN, $"Str():{str}");

        return str;
    }

    public string Repr()
    {
        return Str();
    }
}
