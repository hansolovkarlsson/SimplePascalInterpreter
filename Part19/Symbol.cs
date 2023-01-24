// Symbol classes

using System;

namespace SPI
{
    abstract class Symbol
    {
        public string   name;
        public Symbol?  type; // =null for built-in symbols
        public string   class_name;
        public int      scope_level;

        //------------------------------
        public ScopedSymbolTable? scope; // addition for SourceToSource Compiler test
        //------------------------------

        public Symbol(string class_name, string name, Symbol? type=null)
        {
            this.class_name = class_name;
            this.name = name;
            this.type = type; // definition of symbol
            this.scope_level = 0;
        }

        public abstract string Str();

    }

    class VarSymbol : Symbol
    {
        public VarSymbol(string name, Symbol type)
            : base("VarSymbol", name, type)
        {
        }

        public override string Str()
        {
            return $"<{class_name}(name='{name}', type='{type!.Str()}')>";
        }
    }

    class BuiltinTypeSymbol : Symbol
    {
        public BuiltinTypeSymbol(string name)
            : base("BuiltinTypeSymbol", name)
        {
        }

        public override string Str()
        {
            return $"<{class_name}(name='{name}')";
        }
    }

    class ProcedureSymbol : Symbol
    {
        // public List<NodeParam> parms;
        public List<VarSymbol>  formal_parms;
        public NodeBlock?       block_ast;

        public ProcedureSymbol(string name, List<VarSymbol>? formal_parms=null)
            : base("ProcedureSymbol", name)
        {
            if(formal_parms is null)
                this.formal_parms = new List<VarSymbol>();
            else
                this.formal_parms = formal_parms;

            // a reference to procedure's body (AST sub-tree)
            block_ast = null;
        }

        public override string Str()
        {
            string ret = $"<{class_name}(name='{name}', parameters:";
            foreach(VarSymbol formal_parm in formal_parms) {
                ret += $"\n\t\t{formal_parm.Str()}";
            }
            ret += "\n\t\t)>\n";
            return ret;
        }
    }

}