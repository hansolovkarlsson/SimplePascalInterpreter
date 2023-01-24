// SourceToSourceCompiler

using System;

namespace SPI
{
    class SourceToSourceCompiler : NodeVisitor
    {
       public ScopedSymbolTable?   current_scope;
       public string?              output;

        public SourceToSourceCompiler() : base(typeof(SourceToSourceCompiler))
        {
            current_scope = null;
            output = null;
        }

        public override dynamic Visit_NodeProgram(NodeProgram node)
        {
            string program_name = node.name;
            string result_str = $"program {program_name};\n";

            ScopedSymbolTable global_scope = new ScopedSymbolTable(
                scope_name: "global",
                scope_level: 1,
                enclosing_scope: current_scope
            );
            global_scope.InitBuiltins();
            current_scope = global_scope;

            // visit subtree
            result_str += Visit(node.block);
            result_str += ".";
            result_str += $" {{END OF {program_name}}}";

            output = result_str;
            return result_str;
        }

        public override dynamic Visit_NodeBlock(NodeBlock node)
        {
            List<string> results = new List<string>();
            string result = "";
            foreach(AST declaration in node.declarations) {
                result = Visit(declaration);
                results.Add(result);
            }
            results.Add("\nbegin");
            result = Visit(node.compound_statement);
            result = "    " + result;
            results.Add(result);
            results.Add("end");
            return string.Join("\n", results);
        }

        public override dynamic Visit_NodeVarDecl(NodeVarDecl node)
        {
            string ret = "";

            string type_name = node.type_node.value;
            Symbol type_symbol = current_scope!.Lookup(type_name)!;


            // we have all the informatino we need to create a variable symbol
            // create the symbol and insert it into the symbol table
            string var_name = node.var_node.value;
            VarSymbol var_symbol = new VarSymbol(var_name, type_symbol);

            // signal an error if the table already has a symbol
            // with the same name
            if(current_scope.Lookup(var_name, current_scope_only:true) is not null)
                Debug.Error($"Error: Duplicate identifier {var_name} found");
            else {
                current_scope.Insert(var_symbol);
                string scope_level = current_scope.scope_level.ToString();
                ret = $"   var {var_name}{scope_level} : {type_name};";
            }
            
            return ret;
        }

        public override dynamic Visit_NodeVar(NodeVar node)
        {
            string ret = "";

            string var_name = node.value;
            Symbol? var_symbol = current_scope!.Lookup(var_name);
            if(var_symbol is null)
                Debug.Error("Error: Symbole(identifier) not found '{var_name}'");
            else {
                string scope_level = var_symbol!.scope!.scope_level.ToString();
                ret = $"<{var_name}{scope_level}:{var_symbol!.type!.name}>";
            }

            return ret;
        }
        
        public override dynamic Visit_NodeType(NodeType node) { return ""; }

        public override dynamic Visit_NodeProcedureDecl(NodeProcedureDecl node)
        {
            string proc_name = node.proc_name;
            ProcedureSymbol proc_symbol = new ProcedureSymbol(proc_name);
            current_scope!.Insert(proc_symbol);

            string result_str = $"procedure {proc_name}{current_scope.scope_level}";

            // scope for parameters and local variables
            ScopedSymbolTable procedure_scope = new ScopedSymbolTable(
                scope_name: proc_name,
                scope_level: current_scope.scope_level+1,
                enclosing_scope: current_scope
            );
            current_scope = procedure_scope;

            if(node.formal_parms is not null) {
                result_str += "(";
                List<string> formal_params = new List<string>();
                foreach(NodeParam param in node.formal_parms!) {
                    Symbol param_type = current_scope.Lookup(param.type_node.value);
                    string param_name = param.var_node.value;

                    VarSymbol var_symbol = new VarSymbol(param_name, param_type);
                    current_scope.Insert(var_symbol);
                    proc_symbol.formal_parms.Add(var_symbol);

                    string scope_level = current_scope.scope_level.ToString();
                    formal_params.Add($"{param_name}{scope_level} : {param_type.name}");
                }

                result_str += string.Join("; ", formal_params);
                result_str += ")";
            }
            result_str += ";\n";

            result_str += Visit(node.block_node);
            result_str += $"; {{END OF {proc_name}}}";

            // result_str = join and split, not sure what supposed outcome

            current_scope = current_scope.enclosing_scope;

            return result_str;
        }

        public override dynamic Visit_NodeCompound(NodeCompound node)
        {
            List<string> results = new List<string>();

            foreach(AST child in node.children) {
                string result = Visit(child);
                if(result!="")
                    results.Add(result);
            }
            return string.Join("\n", results);
        }

        public override dynamic Visit_NodeProcedureCall(NodeProcedureCall node)
        {
            List<string> results = new List<string>();

            results.Add($"{node.proc_name}(");

            foreach(AST node_param in node.actual_parms) {
                string result = Visit(node_param);
                if(result!="")
                    results.Add(result);
            }
            results.Add(");");
            return string.Join("\n", results);
        }

        public override dynamic Visit_NodeAssign(NodeAssign node)
        {
            string t2 = Visit(node.right);
            string t1 = Visit(node.left);
            return $"{t1} := {t2};";
        }

        public override dynamic Visit_NodeBinOp(NodeBinOp node)
        {
            string t1 = Visit(node.left);
            string t2 = Visit(node.right);
            return $"{t1} {node.op.value} {t2}";
        }

        public override dynamic Visit_NodeUnaryOp(NodeUnaryOp node) { return ""; }
        public override dynamic Visit_NodeNum(NodeNum node) { return ""; }
        public override dynamic Visit_NodeStr(NodeStr node) { return ""; }
        public override dynamic Visit_NodeNoOp(NodeNoOp node) { return ""; }


    }
}