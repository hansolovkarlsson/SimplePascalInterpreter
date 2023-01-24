
class Debug
{
    public enum MODULE {
        NONE        = 0b0000_0000,
        TOKEN       = 0b0000_0001,
        LEXER       = 0b0000_0010,
        AST         = 0b0000_0100,
        PARSER      = 0b0000_1000,
        INTERPRETER = 0b0001_0000,
    }

    static public MODULE level; // ex: TOKEN | LEXER

    static public void Trace(MODULE module, string msg)
    {
        if((level & module)>0)
            Console.WriteLine($"Trace {module.ToString()}: {msg}");
    }

}