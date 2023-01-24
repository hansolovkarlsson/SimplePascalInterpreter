
using System;
using System.Collections.Generic;

/*  Part 8 - Unary operators (+/-)

    factor : (PLUS|MINUS) factor | INTEGER | LPAR expr RPAR
    
*/

class PASCAL
{
    const string VER = "Part 8: Unary operators +/-";

    static int Main(string[] args)
    {
        Console.WriteLine("Let's Build a Simple Interpreter.");
        Console.WriteLine($"{VER}");


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
                }
            } catch(Exception e) {
                Console.WriteLine($"{e.Message}");
                break;
            }
        }

        return 0;
    }
}
