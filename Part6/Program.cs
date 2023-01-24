
using System;
using System.Collections.Generic;

/*
    expr    : term (( PLUS|MINUS ) term)*
    term    : factor (( MUL|DIV ) factor)*
    factor  : INTEGER | LPAR expr RPAR
*/

class PASCAL
{
    const string VER = "Part 6-parantheses";

    static int Main(string[] args)
    {
        Console.WriteLine("Let's Build a Simple Interpreter.");
        Console.WriteLine($"{VER}");


        while(true) {
            try {
                Console.Write("calc> ");
                string text = Console.ReadLine()!;
                if(text!="") {
                    Lexer lexer = new Lexer(text);
                    Interpreter interpreter = new Interpreter(lexer);
                    int result = interpreter.Expr();
                    Console.WriteLine($"{result}");
                }
            } catch(Exception e) {
                Console.WriteLine($"{e.Message}");
                break;
            }
        }

        return 0;
    }
}
