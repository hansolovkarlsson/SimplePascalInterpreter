using System;

class Interpreter
{
    Lexer   lexer;
    Token?  current_token;
    
    public Interpreter(Lexer lexer)
    {
        // client string input, e.g. "3 + 5", "12 - 5", etc
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
    {
        // factor : INTEGER | LPAREN expr RPAREN
        int ret = 0;
        Token token = current_token!;
        if( token.type == TokenType.INTEGER ) {
            Eat(TokenType.INTEGER);
            ret = token.value;
        } else if( token.type == TokenType.LPAR ) {
            Eat(TokenType.LPAR);
            ret = Expr();
            Eat(TokenType.RPAR);
        }
        return ret;
    }

    public int Term()
    {
        // term: factor ((MUL | DIV) factor)*
        int ret = Factor();

        while(current_token is not null) {
            Token token = current_token!;
            if(token.type! == TokenType.MUL) {
                Eat(TokenType.MUL);
                ret = ret * Factor();

            } else if(token.type! == TokenType.DIV) {
                Eat(TokenType.DIV);
                ret = ret / Factor();
            } else { // break loop when token not in (*, /)
                break;
            }
        }

        return ret;
    }

    public int Expr()
    {   // Arithmetic expression parser/interpreter
        //
        // calc> 14+2*3-6/2
        // 17
        //
        // expr     : term ((PLUS | MINUS) term)*
        // term     : factor ((MUL | DIV) factor)*
        // factor   : INTEGER

        int ret = Term();

        while(current_token is not null) {
            Token token = current_token!;
            if(token.type! == TokenType.PLUS) {
                Eat(TokenType.PLUS);
                ret = ret + Term();
            } else if(token.type! == TokenType.MINUS) {
                Eat(TokenType.MINUS);
                ret = ret - Term();
            } else { // end loop for any token that's not + or -
                break;
            }
        }
        return ret;
    }
}
