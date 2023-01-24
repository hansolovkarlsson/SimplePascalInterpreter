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
            symbols.Add(symbol.name, symbol);

            //------------------
            symbol.scope = this; // addition for Source2Source Compiler
            //------------------
        }

        public Symbol? Lookup(string name, bool current_scope_only=false)
        {
            Debug.Trace(Debug.MODULE.SYMBOLTABLE, $"Lookup({name})");

            Symbol? ret = null;
            
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

/* Replaced by SemanticAnalyzer?

    class SymbolTableBuilder : NodeVisitor
    {
        public SymbolTable symtab;

        public SymbolTableBuilder()
        {
            class_type = typeof(SymbolTableBuilder);
            symtab = new SymbolTable();
        }

        public void DumpTable()
        {
            Console.WriteLine(symtab.Str());
        }

        public void Visit_NodeProgram(NodeProgram node)
        {
            Visit(node.block);
        }

        public void Visit_NodeBlock(NodeBlock node)
        {
            foreach(AST declaration in node.declarations)
                Visit(declaration);
            Visit(node.compound_statement);
        }

        public void Visit_NodeBinOp(NodeBinOp node)
        {
            Visit(node.left);
            Visit(node.right);
        }

        public void Visit_NodeNum(NodeNum node)
        {
            // pass
        }

        public void Visit_NodeUnaryOp(NodeUnaryOp node)
        {
            Visit(node.expr);
        }

        public void Visit_NodeCompound(NodeCompound node)
        {
            foreach(AST child in node.children)
                Visit(child);
        }

        public void Visit_NodeNoOp(NodeNoOp node)
        {
            // pass
        }

        public void Visit_NodeVarDecl(NodeVarDecl node)
        {
            string type_name = node.type_node.value;
            Symbol? type_symbol = symtab.Lookup(type_name);
            string var_name = node.var_node.value;
            Symbol var_symbol = new VarSymbol(var_name, type_symbol!);
            symtab.Insert(var_symbol);
        }

        public void Visit_NodeAssign(NodeAssign node)
        {
            string var_name = node.left.value;
            Symbol var_symbol = symtab.Lookup(var_name)!;
            if(var_symbol is null)
                Debug.Error($"Symbol missing '{var_name}'");
            Visit(node.right);
        }

        public void Visit_NodeVar(NodeVar node)
        {
            string var_name = node.value;
            Symbol? var_symbol = symtab.Lookup(var_name);
            if(var_symbol is null)
                // throw new Exception($"Ooops");
                Debug.Error($"Symbol missing '{var_name}'");
        }

        public void Visit_NodeProcedureDecl(NodeProcedureDecl node)
        {
            // pass
        }

    }
*/

}