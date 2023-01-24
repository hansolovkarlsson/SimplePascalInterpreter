// SemanticAnalyzer

using System;

namespace SPI
{

    class SemanticAnalyzer : NodeVisitor
    {
        public SymbolTable symtab;

        public SemanticAnalyzer()
        {
            class_type = typeof(SemanticAnalyzer);
            symtab = new SymbolTable();
        }

        public void Visit_NodeBlock(NodeBlock node)
        {
            foreach(AST declaration in node.declarations) {
                Visit(declaration);
            }
            Visit(node.compound_statement);
        }

        public void Visit_NodeProgram(NodeProgram node)
        {
            Visit(node.block);
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
            // // for now, manually create a symbol for the integer built-in type
            // // and insert the type symbol in the symbol table
            // Symbol type_symbol = new BuiltinTypeSymbol("INTEGER");
            // symtab.Insert(type_symbol);

            string type_name = node.type_node.value;
            Symbol? type_symbol = symtab.Lookup(type_name);

            if(type_symbol is null)
                Debug.Error($"SemanticAnalyzer.Visit_NodeVarDecl type_symbol missing '{type_name}'");
            else {
                // we have all the information we need to create a variable symbol
                // create the symbol and insert it into the symbol table
                string var_name = node.var_node.value;
                Symbol var_symbol = new VarSymbol(var_name, type_symbol);

                // signal an error if the table already has a symbol
                // with the same name
                if(symtab.Lookup(var_name) is not null)
                    Debug.Error($"Error: Duplicate identifier '{var_name}'");
                else
                    symtab.Insert(var_symbol);
            }
        }

        public void Visit_NodeVar(NodeVar node)
        {
            string var_name = node.value;
            Symbol? var_symbol = symtab.Lookup(var_name);
            if(var_symbol is null)
                Debug.Error($"Error: Symbol(identifier) not found '{var_name}'");
        }

        public void Visit_NodeAssign(NodeAssign node)
        {
            // right-hand side
            Visit(node.right);
            // left-hand side
            Visit(node.left);
        }

        public void Visit_NodeBinOp(NodeBinOp node)
        {
            Visit(node.left);
            Visit(node.right);
        }
    }

}