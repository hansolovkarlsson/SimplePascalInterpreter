using System;
// using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace SPI
{

    class Interpreter : NodeVisitor
    {
        public NodeProgram tree;

        public CallStack call_stack;

        public Interpreter(NodeProgram tree) : base(typeof(Interpreter))
        {
            Debug.Trace(Debug.MODULE.INTERPRETER, $"Interpreter(tree)");

            this.tree = tree;
            this.call_stack = new CallStack();
        }

        public void PrintGlobalScope()
        {
            Console.WriteLine("----- GLOBAL_MEMORY -----");
            Console.WriteLine(call_stack.Str());
            Console.WriteLine("-----(GLOBAL_MEMORY)-----");
        }

        public void NameError(string name)
        {
            Debug.Error($"Name error: {name}");
        }

        public override dynamic Visit_NodeProgram(NodeProgram node)
        {
            string program_name = node.name;
            Debug.Trace(Debug.MODULE.INTERPRETER, $"ENTER: PROGRAM {program_name}");

            ActivationRecord ar = new ActivationRecord(
                name: program_name,
                type: ARType.PROGRAM,
                nesting_level: 1
            );
            call_stack.Push(ar);

            Debug.Trace(Debug.MODULE.INTERPRETER, $"{call_stack.Str()}");

            Visit(node.block);

            Debug.Trace(Debug.MODULE.INTERPRETER, $"LEAVE: PROGRAM {program_name}");
            Debug.Trace(Debug.MODULE.INTERPRETER, $"{call_stack.Str()}");

            call_stack.Pop();

            return 0;
        }

        public override dynamic Visit_NodeBlock(NodeBlock node)
        {
            Debug.Trace(Debug.MODULE.INTERPRETER, $"Visit_NodeBlock(node)");

            foreach(AST declaration in node.declarations)
                Visit(declaration);
            return Visit(node.compound_statement);
        }

        public override dynamic Visit_NodeVarDecl(NodeVarDecl node) { return 0; }
        public override dynamic Visit_NodeType(NodeType node) { return 0; }


        public override dynamic Visit_NodeCompound(NodeCompound node)
        {
            Debug.Trace(Debug.MODULE.INTERPRETER, $"Visit_NodeCompound()");

            foreach(AST child in node.children)
                Visit(child);

            return 0;
        }

        public override dynamic Visit_NodeProcedureCall(NodeProcedureCall node)
        {
            string proc_name = node.proc_name;
            ProcedureSymbol proc_symbol = node.proc_symbol!;

            ActivationRecord ar = new ActivationRecord(
                name: proc_name,
                type: ARType.PROCEDURE,
                // nesting_level: 2 // should probably be above record level + 1
                nesting_level: proc_symbol.scope_level+1
            );


            List<VarSymbol> formal_params = proc_symbol.formal_parms;
            List<AST> actual_params = node.actual_parms;

            var zip = formal_params.Zip(actual_params);
            foreach(var item in zip) {
                string param_symbol = item.First.name;
                AST argument_node = item.Second;
                ar.SetItem(param_symbol, Visit(argument_node));
            }

            call_stack.Push(ar);

            Debug.Trace(Debug.MODULE.INTERPRETER, $"ENTER: PROCEDURE {proc_name}");
            Debug.Trace(Debug.MODULE.INTERPRETER, $"{call_stack.Str()}");

            // evaluate procedure body
            Visit(proc_symbol.block_ast!);

            Debug.Trace(Debug.MODULE.INTERPRETER, $"LEAVE: PROCEDURE {proc_name}");
            Debug.Trace(Debug.MODULE.INTERPRETER, $"{call_stack.Str()}");

            call_stack.Pop();
            
            return 0;
        }

        public override dynamic Visit_NodeAssign(NodeAssign node)
        {
            Debug.Trace(Debug.MODULE.INTERPRETER, $"Visit_NodeAssign()");

            NodeVar left = (NodeVar)node.left;
            string var_name = left.value;
            dynamic var_value = Visit(node.right);

            ActivationRecord ar = call_stack.Peek();
            ar.SetItem(var_name, var_value);

            Debug.Trace(Debug.MODULE.INTERPRETER, $"Assign: {var_name}={var_value.ToString()}");
            return 0;
        }

        public override dynamic Visit_NodeUnaryOp(NodeUnaryOp node)
        {
            Debug.Trace(Debug.MODULE.INTERPRETER, $"Visit_NodeUnaryOp()");

            dynamic ret = 0;
            dynamic val = Visit(node.expr);

            // UnaryOp unaryop = (UnaryOp)node;
            if(node.op.type == TokenType.PLUS)
                ret = +val;
            else if (node.op.type == TokenType.MINUS)
                ret = -val;

            Debug.Trace(Debug.MODULE.INTERPRETER, $"UnaryOp: {node.op.type.ToString()} {val} => {ret}");

            return ret;
        }

        public override dynamic Visit_NodeBinOp(NodeBinOp node)
        {
            Debug.Trace(Debug.MODULE.INTERPRETER, $"Visit_NodeBinOp()");

            dynamic ret = 0;
            dynamic left = Visit(node.left);
            dynamic right = Visit(node.right);

            if(node.op.type==TokenType.PLUS)
                ret = left + right;
            else if (node.op.type==TokenType.MINUS)
                ret = left - right;
            else if (node.op.type==TokenType.MUL)
                ret = left * right;
            else if (node.op.type==TokenType.INTEGER_DIV)
                ret = (int)left / right;
            else if (node.op.type==TokenType.FLOAT_DIV)
                ret = (float)left / (float)right;

            Debug.Trace(Debug.MODULE.INTERPRETER, 
                $"BinOp: {left.ToString()} {node.op.type.ToString()} {right.ToString()} => {ret}");

            return ret;
        }

        public override dynamic Visit_NodeVar(NodeVar node)
        {
            Debug.Trace(Debug.MODULE.INTERPRETER, $"Visit_NodeVar()");

            string var_name = node.value;

            ActivationRecord ar = call_stack.Peek();
            dynamic var_value = ar.GetItem(var_name)!;

            Debug.Trace(Debug.MODULE.INTERPRETER, $"Var: {var_name} is {var_value}");

            return var_value;
        }

        public override dynamic Visit_NodeNum(NodeNum node)
        {
            Debug.Trace(Debug.MODULE.INTERPRETER, $"Visit_NodeNum()");

            Debug.Trace(Debug.MODULE.INTERPRETER, $"Num: {node.value}");

            return node.value;
        }

        public override dynamic Visit_NodeNoOp(NodeNoOp node) { return 0; }
        public override dynamic Visit_NodeProcedureDecl(NodeProcedureDecl node) { return 0; }

    
        public dynamic Interpret()
        {
            Debug.Trace(Debug.MODULE.INTERPRETER, $"Interpreter()");

            dynamic ret = "";

            if (tree is not null)
                ret = Visit(tree);            

            return ret;
        }

    }
}