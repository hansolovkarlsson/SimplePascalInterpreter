
using System;
using System.Collections.Generic;

/*
    Part 10 - Closing the gap, adding declaration, real numbers, program block

    program                 : PROGRAM variable SEMI block DOT

    block                   : declarations compound_statement

    declarations            : VAR (variable_declaration SEMI)+
                            | empty

    variable_declaration    : ID (COMMA ID)* COLON type_spec

    type_spec               : INTEGER | REAL

    compound_statement      : BEGIN statement_list END

    statement_list          : statement
                            | statement SEMI statement_list

    statement               : compound_statement
                            | assignment_statement
                            | empty

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
    const string VER = "Part 10: bridge the gap";

    static int Main(string[] args)
    {
        Console.WriteLine("Let's Build a Simple Interpreter.");
        Console.WriteLine($"{VER}");

        Debug.trace = Debug.MODULE.NONE;
        Debug.dump =  Debug.DUMP.NONE;


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
                    if((Debug.dump & Debug.DUMP.PRINT_TEXT)>0)
                        Console.WriteLine($"----- TEXT -----\n{text}-----(TEXT)-----");

                    Lexer lexer = new Lexer(text);
                    Parser parser = new Parser(lexer);
                    Interpreter interpreter = new Interpreter(parser);
                    int result = interpreter.Interpret();
                    Console.WriteLine($"{result}");

                    if((Debug.dump & Debug.DUMP.PRINT_VARS)>0)
                        interpreter.PrintGlobalScope();
                }

            } catch(Exception e) {
                Console.WriteLine($"{e.Message}");
                // Console.WriteLine($"{e.ToString()}");
                break;
            }
        }

        return 0;
    }
}
