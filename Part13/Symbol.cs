// Symbol classes

using System;

namespace SPI
{
    abstract class Symbol
    {
        public string name;
        public Symbol? type; // =null for built-in symbols
        public string class_name;

        public Symbol(string name, Symbol? type=null)
        {
            this.name = name;
            this.type = type; // definition of symbol
            this.class_name = "";
        }

        public abstract string Str();

    }

    class VarSymbol : Symbol
    {
        public VarSymbol(string name, Symbol type) : base(name, type)
        {
            class_name = "VarSymbol";
        }

        public override string Str()
        {
            return $"<{class_name}(name='{name}', type='{type}')>";
        }
    }

    class BuiltinTypeSymbol : Symbol
    {
        public BuiltinTypeSymbol(string name) : base(name)
        {
            class_name = "BuiltinTypeSymbol";
        }

        public override string Str()
        {
            return $"<{class_name}(name='{name}')";
        }
    }

}