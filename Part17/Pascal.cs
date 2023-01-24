
using System;
using System.Collections.Generic;

namespace SPI
{
    //    Part 17 - Call stack

    /*
        Source Code -> Lexer for lexical analysis
        ->  Tokens  -> Parser for syntax analysis (grammar)
        ->  AST     -> Semantic analyzer for semantic analysis (<->symbol table)
        ->  AST     -> Interpreter for program evaluation (<-symbol table)
        ->  Program output


        SYNTAX:

        program                 : PROGRAM variable SEMI block DOT

        block                   : declarations compound_statement

        declarations            : (VAR (variable_declaration SEMI)+)*
                                | (PROCEDURE ID (LPAR formal_parameter_list RPAR)? SEMI block SEMI)*
                                | empty

        variable_declaration    : ID (COMMA ID)* COLON type_spec

        formal_params_list      : formal_parameters
                                | formal_parameters SEMI formal_params_list

        formal_parameters       : ID (COMMA ID)* COLON type_spec

        type_spec               : INTEGER | REAL

        compound_statement      : BEGIN statement_list END

        statement_list          : statement
                                | statement SEMI statement_list

        statement               : compound_statement
                                | proccall_statement
                                | assignment_statement
                                | empty

        proccall_statement      : ID LPAR (expr (COMMA expr)*)? RPAR

        assignment_statement    : variable ASSIGN expr

        empty                   :

        expr                    : term ((PLUS | MINUS) term)*

        term                    : factor ((MUL | INTEGER_DIV | FLOAT_DIV) factor)*

        factor                  : PLUS factor
                                | MINUS factor
                                | INTEGER_CONT
                                | REAL_CONST
                                | LPAR expr RPAR
                                | variable

        variable                : ID
        
    */

    class PASCAL
    {
        const string VER = "Part 17: Call stack";

        static int Main(string[] args)
        {
            Console.WriteLine("Let's Build a Simple Interpreter.");
            Console.WriteLine($"{VER}");

            Debug.trace = Debug.MODULE.INTERPRETER;
            Debug.dump =  Debug.DUMP.NONE; // PRINT_SYMBOLS | Debug.DUMP.PRINT_TEXT;

            if(args.Length>0) { 
                // file to run
                Console.WriteLine($"File: {args[0]}");
                string text = File.ReadAllText(args[0]);
                RunText(text);
            } else {
                while(true) {
                    try {
                        Console.Write("Simple Pascal Interpreter");

                        string text = "";
                        string input = "";
                        do {
                            Console.Write("> ");
                            input = Console.ReadLine()!;
                            text += input + '\n';
                        } while( input != "" );


                        if(text=="\n") {
                            Console.WriteLine("QUIT");
                            break;
                        } else {
                            if(RunText(text)==false)
                                break;
                        }

                    } catch(Exception e) {
                        Console.WriteLine($"{e.Message}");
                        break;
                    }
                }
            }

            return 0;
        }


        // return true if successful
        // false if error
        static bool RunText(string text)
        {
            bool ret = false;

            try {
                if((Debug.dump & Debug.DUMP.PRINT_TEXT)>0)
                    Console.WriteLine($"-----TEXT-----\n{text}----(TEXT)----");

                Lexer lexer = new Lexer(text);
                Parser parser = new Parser(lexer);
                NodeProgram tree = parser.Parse();

                // SymbolTableBuilder symtab_builder = new SymbolTableBuilder();
                // symtab_builder.Visit(tree);
                // if((Debug.dump & Debug.DUMP.PRINT_SYMBOLS)>0)
                //     symtab_builder.DumpTable();

                // SourceToSourceCompiler source_compiler = new SourceToSourceCompiler();
                // source_compiler.Visit(tree);
                // Console.WriteLine(source_compiler.output);

                SemanticAnalyzer semantic_analyzer = new SemanticAnalyzer();
                try {
                    semantic_analyzer.Visit(tree);
                    // if((Debug.dump & Debug.DUMP.PRINT_SYMBOLS)>0)
                    //     semantic_analyzer.DumpTable();
                } catch (Exception e) {
                    Console.WriteLine(e.ToString());
                }

                Interpreter interpreter = new Interpreter(tree);
                dynamic result = interpreter.Interpret();
                Console.WriteLine($"{result}");
                if((Debug.dump & Debug.DUMP.PRINT_VARS)>0)
                    interpreter.PrintGlobalScope();

                ret = true;
            } catch(Exception e) {
                Console.WriteLine($"{e.Message}");
                // Console.WriteLine($"{e.ToString()}");
            }

            return ret;
        }
    }

}