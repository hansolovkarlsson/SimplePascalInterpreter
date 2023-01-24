
class Debug
{
    public enum TRACE {
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
        NONE        = 0b0000_0000,
        VARS        = 0b0000_0001, // replaced by STACK
        AST         = 0b0000_0010,
        TEXT        = 0b0000_0100,
        SYMBOLS     = 0b0000_1000,
        STACK       = 0b0001_0000,
        ALL         = 0b1111_1111,
    }

    public enum RUN_LEVEL {
        PARSER      = 1, // run lexer+parser
        ANALYZER    = 2,
        INTERPRETER = 3,
    }

    static public TRACE         trace; // ex: TOKEN | LEXER
    static public DUMP          dump;
    static public RUN_LEVEL     run_level;

    static Debug()
    {
        trace = TRACE.NONE;
        dump = DUMP.NONE;
        run_level = RUN_LEVEL.INTERPRETER;
    }

    static public void Trace(TRACE trace_flags, string msg)
    {
        if((trace & trace_flags)>0)
            Console.WriteLine($"Trace {trace.ToString()}: {msg}");
    }

    static public bool IsDump(DUMP dump_flags)
    {
        return (dump & dump_flags)>0;
    }

    static public void Error(string msg)
    {
        Console.WriteLine(msg);
        // throw new Exception(msg);
    }


}