using System;
using System.Collections.Generic;

class Lexer
{
    string  text;
    int     pos;
    char?   current_char;
    int     peek_pos;

    public Dictionary<string,Token> RESERVED_KEYWORDS = new Dictionary<string,Token>() {
        {"BEGIN", new Token(Token.TYPE.BEGIN, "BEGIN")},
        {"END", new Token(Token.TYPE.END, "END")}
    };

    public Lexer(string text)
    {
        // client string input, e.g. "3 * 5", "12 / 3 * 4", etc
        this.text = text;

        // pos is an index into text
        pos = 0;
        current_char = text[pos];
    }

    public void Error(char ch)
    {
        throw new Exception($"Lexer: Invalid character '{ch}'");
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

    public char? Peek()
    {
        char? ret = null;
        peek_pos = pos+1;
        if(peek_pos>(text.Length-1))
            ret = null;
        else
            ret = text[peek_pos];
        return ret;
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

    public Token Id()
    {
        // Handle identifiers and reserved keywords

        string result = "";
        while((current_char is not null) && (char.IsLetterOrDigit((char)current_char!))) {
            result += char.ToUpper((char)current_char!);
            Advance();
        }

        return RESERVED_KEYWORDS.GetValueOrDefault(result, new Token(Token.TYPE.ID, result));
    }

    public Token GetNextToken()
    {
        // Lexical analyzer (also known as scanner or tokenizer)
        //
        // This method is responsible for breaking a sentence
        // apart into tokens. One token at a time.
        Token ret = new Token(Token.TYPE.EOF, null);

        while (current_char is not null) {

            if (char.IsWhiteSpace((char)current_char!)) {
                // SkipWhiteSpace() Advances on its own
                SkipWhiteSpace();

            } else if (char.IsLetter((char)current_char!)) {
                ret = Id();
                break;

            } else if (char.IsDigit((char)current_char!)) {
                // Integer() Advances by itself
                ret = new Token(Token.TYPE.INTEGER, Integer()); 
                break;

            } else if ((current_char! == ':') && (Peek()! == '=')) {
                Advance();
                Advance();
                ret = new Token(Token.TYPE.ASSIGN, ":=");
                break;

            } else if (current_char! == ';') {
                Advance();
                ret = new Token(Token.TYPE.SEMI, ';');
                break;

            } else if (current_char! == '+') {
                Advance();
                ret = new Token(Token.TYPE.PLUS, '+');
                break;

            } else if (current_char! == '-') {
                Advance();
                ret = new Token(Token.TYPE.MINUS, '-');
                break;

            } else if (current_char! == '*') {
                Advance();
                ret = new Token(Token.TYPE.MUL, '*');
                break;

            } else if (current_char! == '/') {
                Advance();
                ret = new Token(Token.TYPE.DIV, '/');
                break;

            } else if (current_char! == '(') {
                Advance();
                ret = new Token(Token.TYPE.LPAR, '(');
                break;

            } else if (current_char! == ')') {
                Advance();
                ret = new Token(Token.TYPE.RPAR, ')');
                break;

            } else if (current_char! == '.') {
                Advance();
                ret = new Token(Token.TYPE.DOT, '.');
                break;

            } else {
                Error((char)current_char!);
            }
        }
        return ret;
    } 
}
