
using System;
using System.Collections.Generic;

// Part5: term-factor precedent

class PASCAL
{
    static int Main(string[] args)
    {
        Console.WriteLine("Let's Build a Simple Interpreter.");
        Console.WriteLine("Part 5 */+- precedent");


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
