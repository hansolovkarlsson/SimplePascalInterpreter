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

        public void Error(ErrorCode error_code, Token token)
        {
            string s = $"{error_code.ToString()} -> {token.Str()}";

            // throw new ParserError(
            //     error_code: error_code,
            //     token: token,
            //     message: s
            // );

            Debug.Error(s);
        }

        public void Eat(TokenType token_type)
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
                // Debug.Error(
                //     $"Surprise: 'eat' wrong token type {current_token.Str()}!={token_type.ToString()} @ {lexer.pos}");
                Error(ErrorCode.UNEXPECTED_TOKEN, current_token);
        }

        public NodeProgram Program()
        {
            Debug.Trace(Debug.MODULE.PARSER, $"Program()");

            // program : PROGRAM variable SEMI block DOT

            Eat(TokenType.PROGRAM);
            NodeVar var_node = Variable();
            string prog_name = var_node.value;
            Eat(TokenType.SEMI);
            NodeBlock block_node = Block();
            NodeProgram program_node = new NodeProgram(prog_name, block_node);
            Eat(TokenType.DOT);
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
            // declaration  : (VAR (variable_declaration SEMI)+)? procedure_declaration

            // List<NodeVarDecl> declarations = new List<NodeVarDecl>();
            List<AST> declarations = new List<AST>();

            // could also be a while instead of if
            if(current_token.type == TokenType.VAR) {
                Eat(TokenType.VAR);
                while(current_token.type == TokenType.ID) {
                    List<NodeVarDecl> var_decl = VariableDeclaration();
                    declarations.AddRange(var_decl);
                    Eat(TokenType.SEMI);
                }
            }

            while(current_token.type == TokenType.PROCEDURE) {
                NodeProcedureDecl proc_decl = ProcedureDeclaration();
                declarations.Add(proc_decl);
            }

            return declarations;
        }

        public NodeProcedureDecl ProcedureDeclaration()
        {
            // procedure_declaration : PROCEDURE ID (LPAR formal_parameter_list RPAR)? SEMI block SEMI
            Eat(TokenType.PROCEDURE);
            string proc_name = current_token.value!;
            Eat(TokenType.ID);

            // parameter list
            List<NodeParam> formal_parms = new List<NodeParam>();

            if(current_token.type == TokenType.LPAR) {
                Eat(TokenType.LPAR);
                formal_parms = FormalParameterList();
                Eat(TokenType.RPAR);
            }

            Eat(TokenType.SEMI);
            NodeBlock block_node = Block();
            NodeProcedureDecl proc_decl = new NodeProcedureDecl(proc_name, formal_parms, block_node);
            Eat(TokenType.SEMI);

            return proc_decl;
        }

        public List<NodeParam> FormalParameters()
        {
            Debug.Trace(Debug.MODULE.PARSER, $"FormalParameters()");

            // formal_parameters    : ID (COMMA ID)* COLON type_spec

            List<NodeParam>     param_nodes = new List<NodeParam>();

            List<Token> param_tokens = new List<Token>(){current_token};
            Eat(TokenType.ID);

            while(current_token.type == TokenType.COMMA) {
                Eat(TokenType.COMMA);
                param_tokens.Add(current_token);
                Eat(TokenType.ID);
            }

            Eat(TokenType.COLON);
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
            if(current_token.type == TokenType.ID) {
                param_nodes = FormalParameters();
                while(current_token.type == TokenType.SEMI) {
                    Eat(TokenType.SEMI);
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
            Eat(TokenType.ID);

            while (current_token.type == TokenType.COMMA) {
                Eat(TokenType.COMMA);
                var_nodes.Add(new NodeVar(current_token));
                Eat(TokenType.ID);
            }

            Eat(TokenType.COLON);

            NodeType type_node = TypeSpec();
            List<NodeVarDecl> var_declarations = new List<NodeVarDecl>();
            foreach(NodeVar var_node in var_nodes) {
                var_declarations.Add(new NodeVarDecl(var_node, type_node));
            }

            return var_declarations;
        }

        public NodeType TypeSpec()
        {
            Debug.Trace(Debug.MODULE.PARSER, $"TypeSpec()");

            // type_spec : INTEGER | REAL

            Token token = current_token;
            if(current_token.type == TokenType.INTEGER)
                Eat(TokenType.INTEGER);
            else
                Eat(TokenType.REAL);
            return new NodeType(token);
        }



        public NodeCompound CompoundStatement()
        {
            Debug.Trace(Debug.MODULE.PARSER, $"CompoundStatement()");

            // compound_statement : BEGIN statement_list END

            Eat(TokenType.BEGIN);
            List<AST> nodes = StatementList();
            Eat(TokenType.END);

            NodeCompound root = new NodeCompound();
            foreach( AST node in nodes )
                root.AddChild(node);

            return root;
        }

        public List<AST> StatementList()
        {
            Debug.Trace(Debug.MODULE.PARSER, $"StatementList()");

            // statement_list   : statement
            //                  | statement SEMI statement_list

            List<AST>   ret = new List<AST>();

            ret.Add(Statement());
            while (current_token.type == TokenType.SEMI) {
                Eat(TokenType.SEMI);
                ret.Add(Statement());
            }

            // ???
            // if (current_token.type == TokenType.ID)
            //     Error(current_token);

            return ret;
        }

        public AST Statement()
        {
            Debug.Trace(Debug.MODULE.PARSER, $"Statement()");

            // statement        : compound_statement
            //                  | proccall_statement
            //                  | assignment_statement
            //                  | empty

            AST node;
            if (current_token.type == TokenType.BEGIN)
                node = CompoundStatement();
            else if (current_token.type == TokenType.ID && lexer.current_char=='(')
                node = ProcCallStatement();
            else if (current_token.type == TokenType.ID)
                node = AssignmentStatement();
            else
                node = Empty();
            
            return node;
        }

        public NodeProcedureCall ProcCallStatement()
        {
            Debug.Trace(Debug.MODULE.PARSER, "ProcCallStatement()");

            // proccall_statement : ID LPAR (expr (COMMA expr)*)?
            Token token = current_token;

            string proc_name = current_token.value!;
            Eat(TokenType.ID);
            Eat(TokenType.LPAR);
            List<AST> actual_parms = new List<AST>();
            if(current_token.type != TokenType.RPAR)
                actual_parms.Add(Expr());
            
            while(current_token.type == TokenType.COMMA){
                Eat(TokenType.COMMA);
                actual_parms.Add(Expr());
            }

            Eat(TokenType.RPAR);

            return new NodeProcedureCall(proc_name, actual_parms, token);
        }

        public NodeAssign AssignmentStatement()
        {
            Debug.Trace(Debug.MODULE.PARSER, "AssignmentStatement()");

            // assignment_statement : variable ASSIGN expr

            NodeVar left = (NodeVar)Variable();
            Token token = current_token;
            Eat(TokenType.ASSIGN);
            AST right = Expr();

            // Debug.Trace(Debug.MODULE.PARSER, $"AssignmentStatement=>NodeAssign({left.Str(), ")
            return new NodeAssign(left, token, right);
        }

        public NodeVar Variable()
        {
            Debug.Trace(Debug.MODULE.PARSER, "Variable()");

            // variable : ID

            NodeVar ret = new NodeVar(current_token);
            Eat(TokenType.ID);
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
                if(token.type! == TokenType.PLUS)
                    Eat(TokenType.PLUS);
                else if(token.type! == TokenType.MINUS)
                    Eat(TokenType.MINUS);
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
                if(token.type! == TokenType.MUL) {
                    Eat(TokenType.MUL);
                } else if(token.type! == TokenType.INTEGER_DIV) {
                    Eat(TokenType.INTEGER_DIV);
                } else if(token.type! == TokenType.FLOAT_DIV) {
                    Eat(TokenType.FLOAT_DIV);
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

            if( token.type == TokenType.PLUS ) {           // unary plus
                Eat(TokenType.PLUS);
                ret = new NodeUnaryOp(token, Factor());

            } else if( token.type == TokenType.MINUS ) {   // unary minus
                Eat(TokenType.MINUS);
                ret = new NodeUnaryOp(token, Factor());

            } else if( token.type == TokenType.INTEGER_CONST ) { // integer number
                Eat(TokenType.INTEGER_CONST);
                ret = new NodeNum(token);

            } else if( token.type == TokenType.REAL_CONST ) { // real/float number
                Eat(TokenType.REAL_CONST);
                ret = new NodeNum(token);

            } else if( token.type == TokenType.LPAR ) {    // parenthesis
                Eat(TokenType.LPAR);
                ret = Expr();
                Eat(TokenType.RPAR);

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

            if (current_token.type != TokenType.EOF) {
                // Debug.Trace(Debug.MODULE.PARSER, "current_token != EOF");
                Debug.Error($"Parse: suprising not EOF");
            }

            if((Debug.dump & Debug.DUMP.PRINT_AST)>0)
                Console.WriteLine($"----- AST Tree -----\n{ret.Str()}-----(AST Tree)-----");

            return ret;
        }

    }
}