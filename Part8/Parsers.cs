using System;

class Parser
{
    Lexer   lexer;
    Token?  current_token; // probably can remove the null flag
    
    public Parser(Lexer lexer)
    {
        this.lexer = lexer;
        this.current_token = lexer.GetNextToken();
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

    public AST Factor()
    {
        // factor : (PLUS|MINUS) factor | INTEGER | LPAREN expr RPAREN
        AST ret = new AST();
        Token token = current_token!;

        if( token.type == TokenType.PLUS ) { // unary plus
            Eat(TokenType.PLUS);
            ret = new UnaryOp(token, Factor());
        } else if( token.type == TokenType.MINUS ) { // unary minus
            Eat(TokenType.MINUS);
            ret = new UnaryOp(token, Factor());
        } else if( token.type == TokenType.INTEGER ) {
            Eat(TokenType.INTEGER);
            ret = new Num(token);
        } else if( token.type == TokenType.LPAR ) {
            Eat(TokenType.LPAR);
            ret = Expr();
            Eat(TokenType.RPAR);
        }

        return ret; // ret;
    }

    public AST Term()
    {
        // term: factor ((MUL | DIV) factor)*
        AST ret = Factor();

        while(current_token is not null) {
            Token token = current_token!;
            if(token.type! == TokenType.MUL) {
                Eat(TokenType.MUL);
            } else if(token.type! == TokenType.DIV) {
                Eat(TokenType.DIV);
            } else { // break loop when token not in (*, /)
                break;
            }

            ret = new BinOp(ret, token, Factor());
        }

        return ret;
    }

    public AST Expr()
    {   // Arithmetic expression parser/interpreter
        //
        // calc> 14+2*3-6/2
        // 17
        //
        // expr     : term ((PLUS | MINUS) term)*
        // term     : factor ((MUL | DIV) factor)*
        // factor   : INTEGER

        AST ret = Term();

        while(current_token is not null) {
            Token token = current_token!;
            if(token.type! == TokenType.PLUS)
                Eat(TokenType.PLUS);
            else if(token.type! == TokenType.MINUS)
                Eat(TokenType.MINUS);
            else
                break;

            ret = new BinOp(ret, token, Term());
        }
        return ret;
    }

    public AST Parse()
    {
        return Expr();
    }
}
