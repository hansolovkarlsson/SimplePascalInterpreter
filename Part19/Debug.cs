
class Debug
{
    public enum MODULE {
        NONE        = 0b0000_0000,
        TOKEN       = 0b0000_0001,
        LEXER       = 0b0000_0010,
        AST         = 0b0000_0100,
        PARSER      = 0b0000_1000,
        INTERPRETER = 0b0001_0000,
        SYMBOLTABLE = 0b0010_0000,
        VISITOR     = 0b0100_0000,
        ANALYZER    = 0b1000_0000,
    }

    public enum DUMP {
        NONE            = 0b0000_0000,
        PRINT_VARS      = 0b0000_0001,
        PRINT_AST       = 0b0000_0010,
        PRINT_TEXT      = 0b0000_0100,
        PRINT_SYMBOLS   = 0b0000_1000,
        PRINT_ALL       = 0b1111_1111,
    }

    static public MODULE trace; // ex: TOKEN | LEXER
    static public DUMP dump;

    static public void Trace(MODULE module, string msg)
    {
        if((trace & module)>0)
            Console.WriteLine($"Trace {module.ToString()}: {msg}");
    }

    static public void Error(string msg)
    {
        Console.WriteLine(msg);
        // throw new Exception(msg);
    }


}