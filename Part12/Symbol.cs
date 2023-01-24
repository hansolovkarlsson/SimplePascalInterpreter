// Symbol classes

using System;

namespace SPI
{
    abstract class Symbol
    {
        public string name;
        public Symbol? type; // =null for built-in symbols

        public Symbol(string name, Symbol? type=null)
        {
            this.name = name;
            this.type = type; // definition of symbol
        }

        public abstract string Str();

    }

    class VarSymbol : Symbol
    {
        public VarSymbol(string name, Symbol type) : base(name, type)
        {
        }

        public override string Str()
        {
            return $"<{name}:{type}>";
        }
    }

    class BuiltinTypeSymbol : Symbol
    {
        public BuiltinTypeSymbol(string name) : base(name)
        {
        }

        public override string Str()
        {
            return $"{name}";
        }
    }

}