using System;
using System.Collections.Generic;

namespace SPI
{

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
                Debug.Error(
                    $"Surprise: 'eat' wrong token type {current_token.Str()}!={token_type.ToString()} @ {lexer.pos}");
        }

        public NodeProgram Program()
        {
            Debug.Trace(Debug.MODULE.PARSER, $"Program()");

            // program : PROGRAM variable SEMI block DOT

            Eat(Token.TYPE.PROGRAM);
            NodeVar var_node = Variable();
            string prog_name = var_node.value;
            Eat(Token.TYPE.SEMI);
            NodeBlock block_node = Block();
            NodeProgram program_node = new NodeProgram(prog_name, block_node);
            Eat(Token.TYPE.DOT);
            return program_node;
        }

        public NodeBlock Block()
        {
            Debug.Trace(Debug.MODULE.PARSER, $"Block()");

            // block : declarations compound_statement

            List<AST> declaration_nodes = Declarations();
            NodeCompound compound_statement_node = CompoundStatement();

            return new NodeBlock(declaration_nodes, compound_statement_node);
        }

        // returning list of Var and Proc declarations
        public List<AST> Declarations()
        {
            Debug.Trace(Debug.MODULE.PARSER, $"Declarations()");

            //  ; changed def, check source code for proper structure
            // declarations : (VAR (variable_declaration SEMI)+)*
            //              | (PROCEDURE ID (LPAR formal_parameter_list RPAR)? SEMI block SEMI)*
            //              | empty

            // List<NodeVarDecl> declarations = new List<NodeVarDecl>();
            List<AST> declarations = new List<AST>();

            // actually, the syntax for follow code is:
            //
            // declarations : declaration*
            //
            // declaration  : VAR (variable_declaration SEMI)+
            //              | PROCEDURE ID (LPAR format_parameter_list RPAR)? SEMI block SEMI
            while(true)
            {
                if(current_token.type == Token.TYPE.VAR) {
                    Eat(Token.TYPE.VAR);
                    while(current_token.type == Token.TYPE.ID) {
                        List<NodeVarDecl> var_decl = VariableDeclaration();
                        declarations.AddRange(var_decl);
                        Eat(Token.TYPE.SEMI);
                    }
                } else if (current_token.type == Token.TYPE.PROCEDURE) {
                    Eat(Token.TYPE.PROCEDURE);
                    string proc_name = current_token.value!;
                    Eat(Token.TYPE.ID);

                    // parameter list
                    List<NodeParam> parms = new List<NodeParam>();

                    if(current_token.type == Token.TYPE.LPAR) {
                        Eat(Token.TYPE.LPAR);
                        parms = FormalParameterList();
                        Eat(Token.TYPE.RPAR);
                    }

                    Eat(Token.TYPE.SEMI);
                    NodeBlock block_node = Block();
                    NodeProcedureDecl proc_decl = new NodeProcedureDecl(proc_name, parms, block_node);
                    declarations.Add(proc_decl);
                    Eat(Token.TYPE.SEMI);
                } else
                    break;
            }

            return declarations;
        }

        public List<NodeParam> FormalParameters()
        {
            Debug.Trace(Debug.MODULE.PARSER, $"FormalParameters()");

            // formal_parameters    : ID (COMMA ID)* COLON type_spec

            List<NodeParam>     param_nodes = new List<NodeParam>();

            List<Token> param_tokens = new List<Token>(){current_token};
            Eat(Token.TYPE.ID);

            while(current_token.type == Token.TYPE.COMMA) {
                Eat(Token.TYPE.COMMA);
                param_tokens.Add(current_token);
                Eat(Token.TYPE.ID);
            }

            Eat(Token.TYPE.COLON);
            NodeType type_node = TypeSpec();

            // distribute to individual var-spec's
            foreach(Token param_token in param_tokens) {
                NodeParam param_node = new NodeParam(new NodeVar(param_token), type_node);
                param_nodes.Add(param_node);
            }

            return param_nodes;
        }

        public List<NodeParam> FormalParameterList()
        {
            Debug.Trace(Debug.MODULE.PARSER, $"FormalParameterList()");

            // format_parameter_list    : format_parameters
            //                          | format_parameters SEMI formal_parameter_list

            List<NodeParam> param_nodes = new List<NodeParam>();

            // procedure Foo();
            if(current_token.type == Token.TYPE.ID) {
                param_nodes = FormalParameters();
                while(current_token.type == Token.TYPE.SEMI) {
                    Eat(Token.TYPE.SEMI);
                    param_nodes.AddRange(FormalParameters());
                }
            }

            return param_nodes;
        }


        // break down A,B,C:int to A:int, B:int, C:int
        public List<NodeVarDecl> VariableDeclaration()
        {
            Debug.Trace(Debug.MODULE.PARSER, $"VariableDeclaration()");

            // variable_declaration : ID (COMMA ID)* COLON type_spec

            // break up the IDs, then the declaration, recombine IDs each with a declaration

            List<NodeVar> var_nodes = new List<NodeVar>(){ new NodeVar(current_token) };
            Eat(Token.TYPE.ID);

            while (current_token.type == Token.TYPE.COMMA) {
                Eat(Token.TYPE.COMMA);
                var_nodes.Add(new NodeVar(current_token));
                Eat(Token.TYPE.ID);
            }

            Eat(Token.TYPE.COLON);

            NodeType type_node = TypeSpec();
            List<NodeVarDecl> var_declarations = new List<NodeVarDecl>();
            foreach(NodeVar var in var_nodes) {
                var_declarations.Add(new NodeVarDecl(var, type_node));
            }

            return var_declarations;
        }

        public NodeType TypeSpec()
        {
            Debug.Trace(Debug.MODULE.PARSER, $"TypeSpec()");

            // type_spec : INTEGER | REAL

            Token token = current_token;
            if(current_token.type == Token.TYPE.INTEGER)
                Eat(Token.TYPE.INTEGER);
            else
                Eat(Token.TYPE.REAL);
            return new NodeType(token);
        }



        public NodeCompound CompoundStatement()
        {
            Debug.Trace(Debug.MODULE.PARSER, $"CompoundStatement()");

            // compound_statement : BEGIN statement_list END

            Eat(Token.TYPE.BEGIN);
            List<AST> nodes = StatementList();
            Eat(Token.TYPE.END);

            NodeCompound ret = new NodeCompound();
            foreach( AST node in nodes )
                // ret.children.Add(node);
                ret.AddChild(node);

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

        public NodeAssign AssignmentStatement()
        {
            Debug.Trace(Debug.MODULE.PARSER, $"AssignmentStatement()");

            // assignment_statement : variable ASSIGN expr

            NodeVar left = (NodeVar)Variable();
            Token token = current_token;
            Eat(Token.TYPE.ASSIGN);
            AST right = Expr();

            // Debug.Trace(Debug.MODULE.PARSER, $"AssignmentStatement=>NodeAssign({left.Str(), ")
            return new NodeAssign(left, token, right);
        }

        public NodeVar Variable()
        {
            Debug.Trace(Debug.MODULE.PARSER, $"Variable()");

            // variable : ID

            NodeVar ret = new NodeVar(current_token);
            Eat(Token.TYPE.ID);
            return ret;
        }

        public NodeNoOp Empty()
        {
            Debug.Trace(Debug.MODULE.PARSER, $"Empty()");

            // empty : NIL
            return new NodeNoOp();
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

                ret = new NodeBinOp(ret, token, Term());
            }

            return ret;
        }

        // returns factor or NodeBinOp
        public AST Term()
        {
            Debug.Trace(Debug.MODULE.PARSER, $"Term()");

            // term: factor ((MUL | DIV) factor)*

            AST ret = Factor();

            while(true) {
                Token token = current_token;
                if(token.type! == Token.TYPE.MUL) {
                    Eat(Token.TYPE.MUL);
                } else if(token.type! == Token.TYPE.INTEGER_DIV) {
                    Eat(Token.TYPE.INTEGER_DIV);
                } else if(token.type! == Token.TYPE.FLOAT_DIV) {
                    Eat(Token.TYPE.FLOAT_DIV);
                } else {
                    break;
                }
                // continue MUL|INTDIV|FLTDIV continue

                ret = new NodeBinOp(ret, token, Factor());
            }

            return ret;
        }

        // returns NodeUnaryOp, NodeNum, expression or variable
        public AST Factor()
        {
            Debug.Trace(Debug.MODULE.PARSER, $"Factor()");

            // factor   : PLUS factor
            //          | MINUS factor
            //          | INTEGER_CONST
            //          | REAL_CONST
            //          | LPAR expr RPAR
            //          | variable

            AST ret;
            Token token = current_token;

            if( token.type == Token.TYPE.PLUS ) {           // unary plus
                Eat(Token.TYPE.PLUS);
                ret = new NodeUnaryOp(token, Factor());

            } else if( token.type == Token.TYPE.MINUS ) {   // unary minus
                Eat(Token.TYPE.MINUS);
                ret = new NodeUnaryOp(token, Factor());

            } else if( token.type == Token.TYPE.INTEGER_CONST ) { // integer number
                Eat(Token.TYPE.INTEGER_CONST);
                ret = new NodeNum(token);

            } else if( token.type == Token.TYPE.REAL_CONST ) { // real/float number
                Eat(Token.TYPE.REAL_CONST);
                ret = new NodeNum(token);

            } else if( token.type == Token.TYPE.LPAR ) {    // parenthesis
                Eat(Token.TYPE.LPAR);
                ret = Expr();
                Eat(Token.TYPE.RPAR);

            } else
                ret = Variable();

            return ret;
        }

        public NodeProgram Parse()
        {
            /*
                program             : PROGRAM variable SEMI block DOT
                block               : declarations compound_statement
                declarations        : (VAR (variable_declaration SEMI)+)*
                                    | (PROCEDURE ID (LPAREN formal_parameter_list RPAREN)? SEMI block SEMI)*
                                    | empty
                variable_declaration: ID (COMMA ID)* COLON type_spec
                formal_params_list  : formal_parameters
                                    | formal_parameters SEMI formal_parameter_list
                formal_parameters   : ID (COMMA ID)* COLON type_spec
                type_spec           : INTEGER
                compound_statement  : BEGIN statement_list END
                statement_list      : statement
                                    | statement SEMI statement_list
                statement           : compound_statement
                                    | assignment_statement
                                    | empty
                assignment_statement: variable ASSIGN expr
                empty               :
                expr                : term ((PLUS | MINUS) term)*
                term                : factor ((MUL | INTEGER_DIV | FLOAT_DIV) factor)*
                factor              : PLUS factor
                                    | MINUS factor
                                    | INTEGER_CONST
                                    | REAL_CONST
                                    | LPAREN expr RPAREN
                                    | variable
                variable: ID
            */

            Debug.Trace(Debug.MODULE.PARSER, $"Parse()");

            NodeProgram ret = Program();
            Debug.Trace(Debug.MODULE.PARSER, $"Program()=>{ret.token.Str()}");

            if (current_token.type != Token.TYPE.EOF) {
                // Debug.Trace(Debug.MODULE.PARSER, "current_token != EOF");
                Debug.Error($"Parse: suprising not EOF");
            }

            if((Debug.dump & Debug.DUMP.PRINT_AST)>0)
                Console.WriteLine($"----- AST Tree -----\n{ret.Str()}-----(AST Tree)-----");

            return ret;
        }

    }
}