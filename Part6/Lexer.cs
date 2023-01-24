using System;

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
                // SkipWhiteSpace() Advances on its own
                SkipWhiteSpace();
            } else if (char.IsDigit((char)current_char!)) {
                // Integer() Advances by itself
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
            } else if (current_char! == '*') {
                Advance();
                ret = new Token(TokenType.MUL, '*');
                break;
            } else if (current_char! == '/') {
                Advance();
                ret = new Token(TokenType.DIV, '/');
                break;
            } else if (current_char! == '(') {
                Advance();
                ret = new Token(TokenType.LPAR, '(');
                break;
            } else if (current_char! == ')') {
                Advance();
                ret = new Token(TokenType.RPAR, ')');
                break;
            } else {
                Error();
            }
        }
        return ret;
    }
}
