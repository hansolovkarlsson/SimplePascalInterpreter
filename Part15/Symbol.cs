// Symbol classes

using System;

namespace SPI
{
    abstract class Symbol
    {
        public string name;
        public Symbol? type; // =null for built-in symbols
        public string class_name;

        //------------------------------
        public ScopedSymbolTable? scope; // addition for SourceToSource Compiler test
        //------------------------------

        public Symbol(string class_name, string name, Symbol? type=null)
        {
            this.class_name = class_name;
            this.name = name;
            this.type = type; // definition of symbol
        }

        public abstract string Str();

    }

    class VarSymbol : Symbol
    {
        public VarSymbol(string name, Symbol type) : base("VarSymbol", name, type)
        {
        }

        public override string Str()
        {
            return $"<{class_name}(name='{name}', type='{type!.Str()}')>";
        }
    }

    class BuiltinTypeSymbol : Symbol
    {
        public BuiltinTypeSymbol(string name) : base("BuiltinTypeSymbol", name)
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
        public List<VarSymbol> parms;

        public ProcedureSymbol(string name, List<VarSymbol>? parms=null) : base("ProcedureSymbol", name)
        {
            if(parms is null)
                this.parms = new List<VarSymbol>();
            else
                this.parms = parms;

        }

        public override string Str()
        {
            string ret = $"<{class_name}(name='{name}', parameters:";
            foreach(VarSymbol parm in parms) {
                ret += $"\n\t\t{parm.Str()}";
            }
            ret += "\n\t\t)>\n";
            return ret;
        }
    }

}