using System;
// using System.Reflection;
using System.Collections.Generic;

namespace SPI
{

    class Interpreter : NodeVisitor
    {
        public NodeProgram tree;

        // public Dictionary<string,dynamic> GLOBAL_MEMORY;
        public CallStack call_stack;

        public Interpreter(NodeProgram tree) : base(typeof(Interpreter))
        {
            Debug.Trace(Debug.MODULE.INTERPRETER, $"Interpreter(tree)");

            // class_type = typeof(Interpreter);

            this.tree = tree;

            // GLOBAL_MEMORY = new Dictionary<string, dynamic>();
            this.call_stack = new CallStack();
        }

        public void PrintGlobalScope()
        {
            Console.WriteLine("----- GLOBAL_MEMORY -----");
            // foreach(string var in GLOBAL_MEMORY.Keys)
            //     Console.WriteLine($"{var}={GLOBAL_MEMORY[var]}");
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

            // return Visit(node.block);

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

        public override dynamic Visit_NodeVarDecl(NodeVarDecl node)
        {
            Debug.Trace(Debug.MODULE.INTERPRETER, $"Visit_NodeVarDecl()");
            // pass
            return 0;
        }

        public override dynamic Visit_NodeType(NodeType node)
        {
            Debug.Trace(Debug.MODULE.INTERPRETER, $"Visit_NodeType()");
            // pass
            return 0;
        }


        public override dynamic Visit_NodeCompound(NodeCompound node)
        {
            Debug.Trace(Debug.MODULE.INTERPRETER, $"Visit_NodeCompound()");

            // Compound compound = (Compound)node;
            foreach(AST child in node.children)
                Visit(child);

            return 0;
        }

        public override dynamic Visit_NodeProcedureCall(NodeProcedureCall node)
        {
            // pass
            return 0;
        }

        public override dynamic Visit_NodeAssign(NodeAssign node)
        {
            Debug.Trace(Debug.MODULE.INTERPRETER, $"Visit_NodeAssign()");

            // Assign assign = (Assign)node;
            // assume left is a NodeVar
            NodeVar left = (NodeVar)node.left;
            string var_name = left.value;
            dynamic var_value = Visit(node.right);

            // if(GLOBAL_MEMORY.ContainsKey(var_name))
            //     GLOBAL_MEMORY[var_name] = value;
            // else
            //     GLOBAL_MEMORY.Add(var_name, value);

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

            // dynamic ret = 0;
            string var_name = node.value;

            // if(GLOBAL_MEMORY.ContainsKey(var_name))
            //     ret = GLOBAL_MEMORY[var_name];
            // else
            //     Debug.Error($"Variable not found '{var_name}'");

            ActivationRecord ar = call_stack.Peek();
            dynamic var_value = ar.GetItem(var_name)!;

            Debug.Trace(Debug.MODULE.INTERPRETER, $"Var: {var_name} is {var_value}");

            return var_value;
        }

        public override dynamic Visit_NodeNum(NodeNum node)
        {
            Debug.Trace(Debug.MODULE.INTERPRETER, $"Visit_NodeNum()");

            // Num num = (Num)node;
            // return num.value;

            Debug.Trace(Debug.MODULE.INTERPRETER, $"Num: {node.value}");

            return node.value;
        }

        public override dynamic Visit_NodeNoOp(NodeNoOp node)
        {
            Debug.Trace(Debug.MODULE.INTERPRETER, $"Visit_NodeNoOp()");

            // do nothing
            return 0;
        }

        public override dynamic Visit_NodeProcedureDecl(NodeProcedureDecl node)
        {
            Debug.Trace(Debug.MODULE.INTERPRETER, $"Visit_ProcedureDecl()");

            // pass
            return 0;
        }

    
        public dynamic Interpret()
        {
            Debug.Trace(Debug.MODULE.INTERPRETER, $"Interpreter()");

            dynamic ret = "";

            // NodeProgram tree = parser.Parse();

            if (tree is not null)
                ret = Visit(tree);            

            return ret;
        }

    }
}