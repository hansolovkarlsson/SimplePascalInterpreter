
using System;
using System.Collections.Generic;

/*
    Part 9 - Compound statements

    program                 : compound_statement DOT

    compound_statement      : BEGIN statement_list END

    statement_list          : statement | statement SEMI statement_list

    statement               : compound_statement
                            | assignment_statement
                            | empty

    assignment_statement    : variable ASSIGN expr

    empty                   :

    expr                    : term ((PLUS | MINUS) term)*

    term                    : factor ((MUL | DIV) factor)*

    factor                  : PLUS factor
                            | MINUS factor
                            | INTEGER
                            | LPAR expr RPAR
                            | variable

    variable                : ID
    
*/

class PASCAL
{
    const string VER = "Part 9: Compound statements and variables";

    static int Main(string[] args)
    {
        Console.WriteLine("Let's Build a Simple Interpreter.");
        Console.WriteLine($"{VER}");

        Debug.level = Debug.MODULE.NONE;

        while(true) {
            try {
                Console.Write("spi> ");
                string text = Console.ReadLine()!;
                if(text!="") {
                    Lexer lexer = new Lexer(text);
                    Parser parser = new Parser(lexer);
                    Interpreter interpreter = new Interpreter(parser);
                    int result = interpreter.Interpret();
                    Console.WriteLine($"{result}");

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
