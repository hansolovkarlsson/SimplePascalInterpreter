using System;
using System.Collections.Generic;

class Parser
{
    Lexer   lexer;
    Token   current_token; // probably can remove the null flag
    
    public Parser(Lexer lexer)
    {
        this.lexer = lexer;
        // set current token to the first token taken from the input.
        this.current_token = lexer.GetNextToken();
    }

    public void Error(Token token)
    {
        throw new Exception($"Parser: Invalid syntax token={token.Str()}");
    }

    public void Eat(Token.TYPE token_type)
    {
        Debug.Trace(Debug.MODULE.PARSER, $"Eat({token_type.ToString()})");

        /* compare the current token type with the passed token type
        // and if they match then "eat" the current token and
        // assign the next token to the current_token,
        // otherwise raise an exception
        */
        if(current_token!.type==token_type)
            current_token = lexer.GetNextToken();
        else 
            Error(current_token);
    }

    public AST Program()
    {
        Debug.Trace(Debug.MODULE.PARSER, $"Program()");

        // program : compound_statement DOT

        AST ret = CompoundStatement();
        Eat(Token.TYPE.DOT);
        return ret;
    }

    public AST CompoundStatement()
    {
        Debug.Trace(Debug.MODULE.PARSER, $"CompoundStatement()");

        // compound_statement : BEGIN statement_list END

        Eat(Token.TYPE.BEGIN);
        List<AST> nodes = StatementList();
        Eat(Token.TYPE.END);

        Compound ret = new Compound();
        foreach( AST node in nodes )
            ret.children.Add(node);

        return ret;
    }

    public List<AST> StatementList()
    {
        Debug.Trace(Debug.MODULE.PARSER, $"StatementList()");

        // statement_list   : statement
        //                  | statement SEMI statement_list

        List<AST>   ret = new List<AST>();

        ret.Add(Statement());
        while (current_token.type == Token.TYPE.SEMI) {
            Eat(Token.TYPE.SEMI);
            ret.Add(Statement());
        }

        // ???
        // if (current_token.type == Token.TYPE.ID)
        //     Error(current_token);

        return ret;
    }

    public AST Statement()
    {
        Debug.Trace(Debug.MODULE.PARSER, $"Statement()");

        // statement        : compound_statement
        //                  | assignment_statement
        //                  | empty

        AST ret;
        if (current_token.type == Token.TYPE.BEGIN)
            ret = CompoundStatement();
        else if (current_token.type == Token.TYPE.ID)
            ret = AssignmentStatement();
        else
            ret = Empty();
        
        return ret;
    }

    public AST AssignmentStatement()
    {
        Debug.Trace(Debug.MODULE.PARSER, $"AssignmentStatement()");

        // assignment_statement : variable ASSIGN expr

        AST ret;

        Var left = Variable();
        Token token = current_token;
        Eat(Token.TYPE.ASSIGN);
        AST right = Expr();

        ret = new Assign(left, token, right);
        return ret;
    }

    public Var Variable()
    {
        Debug.Trace(Debug.MODULE.PARSER, $"Variable()");

        // variable : ID

        Var ret = new Var(current_token);
        Eat(Token.TYPE.ID);
        return ret;
    }

    public AST Empty()
    {
        Debug.Trace(Debug.MODULE.PARSER, $"Empty()");

        // empty : NIL
        return new NoOp();
    }

    public AST Expr()
    {
        Debug.Trace(Debug.MODULE.PARSER, $"Expr()");

        // expr     : term ((PLUS | MINUS) term)*

        AST ret = Term();

        while(true) {
            Token token = current_token;
            if(token.type! == Token.TYPE.PLUS)
                Eat(Token.TYPE.PLUS);
            else if(token.type! == Token.TYPE.MINUS)
                Eat(Token.TYPE.MINUS);
            else
                break; // while PLUS|MINUS continue

            ret = new BinOp(ret, token, Term());
        }

        return ret;
    }

    public AST Term()
    {
        Debug.Trace(Debug.MODULE.PARSER, $"Term()");

        // term: factor ((MUL | DIV) factor)*

        AST ret = Factor();

        while(true) {
            Token token = current_token;
            if(token.type! == Token.TYPE.MUL) {
                Eat(Token.TYPE.MUL);
            } else if(token.type! == Token.TYPE.DIV) {
                Eat(Token.TYPE.DIV);
            } else {
                break; // while MUL|DIV continue
            }

            ret = new BinOp(ret, token, Factor());
        }

        return ret;
    }

    public AST Factor()
    {
        Debug.Trace(Debug.MODULE.PARSER, $"Factor()");

        // factor   : PLUS factor
        //          | MINUS factor
        //          | INTEGER
        //          | LPAR expr RPAR
        //          | variable

        AST ret;
        Token token = current_token;

        if( token.type == Token.TYPE.PLUS ) {           // unary plus
            Eat(Token.TYPE.PLUS);
            ret = new UnaryOp(token, Factor());

        } else if( token.type == Token.TYPE.MINUS ) {   // unary minus
            Eat(Token.TYPE.MINUS);
            ret = new UnaryOp(token, Factor());

        } else if( token.type == Token.TYPE.INTEGER ) { // number
            Eat(Token.TYPE.INTEGER);
            ret = new Num(token);

        } else if( token.type == Token.TYPE.LPAR ) {    // parenthesis
            Eat(Token.TYPE.LPAR);
            ret = Expr();
            Eat(Token.TYPE.RPAR);

        } else
            ret = Variable();

        return ret;
    }

    public AST Parse()
    {
        Debug.Trace(Debug.MODULE.PARSER, $"Parse()");

        /*
            program                 : compound_statement DOT

            compound_statement      : BEGIN statement_list END

            statement_list          : statement
                                    | statement SEMI statement_list

            statement               : compound_statement
                                    | assignment_statement
                                    | empty

            assignment_statement    : variable ASSIGN expr

            empty                   : NIL

            expr                    : term ((PLUS | MINUS) term)*

            term                    : factor ((MUL | DIV) factor)*

            factor                  : PLUS factor
                                    | MINUS factor
                                    | INTEGER
                                    | LPAR expr RPAR
                                    | variable

            variable                : ID
        */

        AST ret = Program();
        Debug.Trace(Debug.MODULE.PARSER, $"Program()=>{ret.token.Str()}");

        if (current_token.type != Token.TYPE.EOF) {
            Debug.Trace(Debug.MODULE.PARSER, "current_token != EOF");
             Error(current_token);
        }

        return ret;
    }
}
