// SymbolTable

using System;
using System.Collections.Generic;

namespace SPI
{

    class SymbolTable
    {
        Dictionary<string,Symbol> symbols;

        public SymbolTable()
        {
            symbols = new Dictionary<string,Symbol>();
            InitBuiltins();
        }

        public void InitBuiltins()
        {
            Define(new BuiltinTypeSymbol("INTEGER"));
            Define(new BuiltinTypeSymbol("REAL"));
        }

        public string Str()
        {
            string ret = $"Symbols:\n";
            foreach(KeyValuePair<string,Symbol> item in symbols) {
                ret += $"\t{item.Key}";
                if(item.Value.type is not null)
                    ret += $":{item.Value.type}";
                ret += "\n";
            }
            return ret;
        }

        public void Define(Symbol symbol)
        {
            Debug.Trace(Debug.MODULE.SYMBOLTABLE, $"Define({symbol.Str()})");
            symbols.Add(symbol.name, symbol);
            // this would be done slightly different if context
            // then it needs to be some form of context stack
        }

        public Symbol? Lookup(string name)
        {
            Debug.Trace(Debug.MODULE.SYMBOLTABLE, $"Lookup({name})");
            Symbol? ret = null;
            if(symbols.ContainsKey(name))
                ret = symbols[name];
            return ret;
        }
    }


    class SymbolTableBuilder : NodeVisitor
    {
        SymbolTable symtab;

        public SymbolTableBuilder()
        {
            class_type = typeof(SymbolTableBuilder);
            symtab = new SymbolTable();
        }

        public void Visit_NodeProgram(NodeProgram node)
        {
            Visit(node.block);
        }

        public void Visit_NodeBlock(NodeBlock node)
        {
            foreach(NodeVarDecl declaration in node.declarations)
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
            symtab.Define(var_symbol);
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

    }

}