// SemanticAnalyzer

using System;

namespace SPI
{

    class SemanticAnalyzer : NodeVisitor
    {
        // public ScopedSymbolTable symtab;
        public ScopedSymbolTable? global_scope;
        public ScopedSymbolTable? current_scope;

        public SemanticAnalyzer() : base(typeof(SemanticAnalyzer))
        {
            // class_type = typeof(SemanticAnalyzer);
            // symtab = new ScopedSymbolTable("global", 1);
            current_scope = null;
        }

        public void Error(ErrorCode error_code, Token token)
        {
            string s = $"{error_code.ToString()} -> {token.Str()}";

            // throw new SemanticError(
            //     error_code: error_code,
            //     token: token,
            //     message: s
            // );

            Debug.Error(s);
        }

        public void DumpTable()
        {
            // Console.WriteLine(symtab.Str());
            if(current_scope is not null)
                Console.WriteLine(current_scope!.Str());
        }


        public override dynamic Visit_NodeProgram(NodeProgram node)
        {
            Debug.Trace(Debug.TRACE.ANALYZER, $"ENTER scope: global");

            global_scope = new ScopedSymbolTable(
                scope_name: "global", 
                scope_level: 1,
                enclosing_scope: current_scope); // current=null
                 
            global_scope.InitBuiltins();
            current_scope = global_scope;

            // visit subtree
            Visit(node.block);

            if((Debug.dump & Debug.DUMP.SYMBOLS)>0)
                Console.WriteLine(current_scope.Str());

            current_scope = current_scope.enclosing_scope;

            Debug.Trace(Debug.TRACE.ANALYZER, $"LEAVE scope: global");
            return 0;
        }

        public override dynamic Visit_NodeBlock(NodeBlock node)
        {
            foreach(AST declaration in node.declarations) {
                Visit(declaration);
            }
            Visit(node.compound_statement);

            return 0;
        }

        public override dynamic Visit_NodeCompound(NodeCompound node)
        {
            foreach(AST child in node.children)
                Visit(child);
            return 0;
        }

        public override dynamic Visit_NodeBinOp(NodeBinOp node)
        {
            Visit(node.left);
            Visit(node.right);
            return 0;
        }

        public override dynamic Visit_NodeProcedureDecl(NodeProcedureDecl node)
        {
            string proc_name = node.proc_name;
            ProcedureSymbol proc_symbol = new ProcedureSymbol(proc_name);
            current_scope!.Insert(proc_symbol);

            Debug.Trace(Debug.TRACE.ANALYZER, $"ENTER scope: {proc_name}");

            // Scope for parameters and local variables
            ScopedSymbolTable procedure_scope = new ScopedSymbolTable(
                scope_name: proc_name,
                scope_level: current_scope.scope_level+1,
                enclosing_scope: current_scope
            );
            current_scope = procedure_scope;

            // insert parameters into the procedure scope
            foreach(NodeParam param in node.formal_parms) {
                Symbol param_type = current_scope.Lookup(param.type_node.value);
                string param_name = param.var_node.value;
                VarSymbol var_symbol = new VarSymbol(param_name, param_type);
                current_scope.Insert(var_symbol);
                proc_symbol.formal_parms.Add(var_symbol);
            }

            Visit(node.block_node);

            if((Debug.dump & Debug.DUMP.SYMBOLS)>0)
                Console.WriteLine(procedure_scope.Str());

            current_scope = current_scope.enclosing_scope;

            // accessed by the interpreter when executing procedure call
            proc_symbol.block_ast = node.block_node;

            Debug.Trace(Debug.TRACE.ANALYZER, $"LEAVE scope: {proc_name}");

            return 0;
        }

        public override dynamic Visit_NodeVarDecl(NodeVarDecl node)
        {
            string type_name = node.type_node.value;
            Symbol? type_symbol = current_scope!.Lookup(type_name);

            // we have all the information we need to create a variable symbol
            // create the symbol and insert it into the symbol table
            string var_name = node.var_node.value;
            VarSymbol var_symbol = new VarSymbol(var_name, type_symbol!);

            // signal an error if the table already has a symbol
            // with the same name
            if(current_scope!.Lookup(var_name, current_scope_only: true) is not null)
                Error(ErrorCode.DUPLICATE_ID, node.var_node.token);
            else
                current_scope!.Insert(var_symbol);

            return 0;
        }

        public override dynamic Visit_NodeProcedureCall(NodeProcedureCall node)
        {
            foreach(AST param_node in node.actual_parms)
                Visit(param_node);
            
            Symbol proc_symbol = current_scope!.Lookup(node.proc_name)!;
            node.proc_symbol = (ProcedureSymbol)proc_symbol;
            
            return 0;
        }

        public override dynamic Visit_NodeAssign(NodeAssign node)
        {
            // right-hand side
            Visit(node.right);
            // left-hand side
            Visit(node.left);
            return 0;
        }

        public override dynamic Visit_NodeVar(NodeVar node)
        {
            string var_name = node.value;
            Symbol? var_symbol = current_scope!.Lookup(var_name);
            if(var_symbol is null)
                Error(ErrorCode.ID_NOT_FOUND, node.token);
            return 0;
        }

        public override dynamic Visit_NodeNum(NodeNum node) { return 0; }
        public override dynamic Visit_NodeStr(NodeStr node) { return 0; }
        public override dynamic Visit_NodeUnaryOp(NodeUnaryOp node) { return 0; }
        public override dynamic Visit_NodeType(NodeType node) { return 0; }
        public override dynamic Visit_NodeNoOp(NodeNoOp node) { return 0; }

    }

}