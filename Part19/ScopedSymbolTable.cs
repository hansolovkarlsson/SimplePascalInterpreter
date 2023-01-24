// SymbolTable

using System;
using System.Collections.Generic;

namespace SPI
{

    // Renamed SymbolTable to ScopedSymbolTable

    class ScopedSymbolTable
    {
        public Dictionary<string,Symbol>    symbols;
        public string                       scope_name;
        public int                          scope_level;
        public ScopedSymbolTable?           enclosing_scope;


        public ScopedSymbolTable(string scope_name, int scope_level, ScopedSymbolTable? enclosing_scope=null)
        {
            this.symbols            = new Dictionary<string,Symbol>();
            this.scope_name         = scope_name;
            this.scope_level        = scope_level;
            this.enclosing_scope    = enclosing_scope;

            // InitBuiltins(); // called by SemanticAnalyzer
        }

        public void InitBuiltins()
        {
            Insert(new BuiltinTypeSymbol("INTEGER"));
            Insert(new BuiltinTypeSymbol("REAL"));
        }

        public string Str()
        {
            string ret = $"\n-----SCOPED SYMBOL TABLE-----\n";
            ret += $"Scope name     : {scope_name}\n";
            ret += $"Scope level    : {scope_level}\n";
            if(enclosing_scope is not null)
                ret += $"Enclosing Scope: {enclosing_scope.scope_name}\n";

            foreach(KeyValuePair<string,Symbol> item in symbols) {
                ret += $"{item.Key,15}";
                ret += $":{item.Value.Str()}";
                // if(item.Value.type is not null)
                //     ret += $": {item.Value.type.Str()}";
                ret += "\n";
            }
            ret += $"-----(SCOPED SYMBOL TABLE)-----\n";
            return ret;
        }

        public void Insert(Symbol symbol)
        {
            Debug.Trace(Debug.MODULE.SYMBOLTABLE, $"Insert({symbol.Str()})");

            symbol.scope_level = scope_level;
            symbols.Add(symbol.name, symbol);

            //------------------
            symbol.scope = this; // addition for Source2Source Compiler
            //------------------
        }

        public Symbol? Lookup(string name, bool current_scope_only=false)
        {
            Debug.Trace(Debug.MODULE.SYMBOLTABLE, $"Lookup: {name}. (Scope name: {scope_name})");

            Symbol? ret = null;

            // 'symbol' is either an instance of the symbol class or none            
            if(symbols.ContainsKey(name))
                ret = symbols[name];

            else if (current_scope_only)
                ret = null; // i.e. don't search deeper

            else if(enclosing_scope is not null)
                // if not found, and search deeper
                ret = enclosing_scope.Lookup(name);

            return ret;
        }
    }
}