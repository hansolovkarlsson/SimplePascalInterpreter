using System;
// using System.Reflection;
using System.Collections.Generic;

namespace SPI
{

    // abstract class NodeVisitor
    // {
    //     public abstract int Visit(AST node);
    // }

    /*
    class NodeVisitor
    {
        // Dynamic method caller:
        // subclasses.Visit_{node type}
        // example: Visit_Num(node)->int

        public dynamic Visit(AST node)
        {
            Debug.Trace(Debug.MODULE.INTERPRETER, $"Visit({node.token.Str()}");

            dynamic ret = 0;

            Type type = typeof(Interpreter);
            string method = $"Visit_{node.GetType()}";
            MethodInfo? info = type.GetMethod(method);
            if(info is null)
                Console.WriteLine($"ERROR: Visit method missing {method}");
            else
                ret = info!.Invoke(this, new object[]{node})!;

            return ret;
        }
    }
    */

    class Interpreter : NodeVisitor
    {
        // Parser parser;
        public NodeProgram tree;

        public Dictionary<string,dynamic> GLOBAL_MEMORY;

        public Interpreter(NodeProgram tree) : base(typeof(Interpreter))
        {
            Debug.Trace(Debug.MODULE.INTERPRETER, $"Interpreter(tree)");

            // class_type = typeof(Interpreter);

            this.tree = tree;
            GLOBAL_MEMORY = new Dictionary<string, dynamic>();
        }

        public void PrintGlobalScope()
        {
            Console.WriteLine("----- GLOBAL_MEMORY -----");
            foreach(string var in GLOBAL_MEMORY.Keys)
                Console.WriteLine($"{var}={GLOBAL_MEMORY[var]}");
            Console.WriteLine("-----(GLOBAL_MEMORY)-----");
        }

    /* -- local visit definition, dynamic and static with "if"

        public override int Visit(AST node)
        {
            int ret = 0;

            // Console.WriteLine($"Visit method to call: Visit_{node.GetType()}");
            Type type = typeof(Interpreter);
            MethodInfo? info = type.GetMethod($"Visit_{node.GetType()}");
            if(info is not null) {
                // Console.WriteLine("Invoking method");
                ret = (int)(info!.Invoke(this, new object[]{node})!);
            }

            // if(node.GetType()==typeof(BinOp))
            //     ret = Visit_BinOp(node);
            // else if(node.GetType()==typeof(Num))
            //     ret = Visit_Num(node);
            
            return ret;
        }
    */

        public void NameError(string name)
        {
            Debug.Error($"Name error: {name}");
        }

        public override dynamic Visit_NodeProgram(NodeProgram node)
        {
            Debug.Trace(Debug.MODULE.INTERPRETER, $"Visit_NodeProgram(node)");

            return Visit(node.block);
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

        public override dynamic Visit_NodeBinOp(NodeBinOp node)
        {
            Debug.Trace(Debug.MODULE.INTERPRETER, $"Visit_NodeBinOp()");

            dynamic ret = 0;

            if(node.op.type==TokenType.PLUS)
                ret = Visit(node.left) + Visit(node.right);
            else if (node.op.type==TokenType.MINUS)
                ret = Visit(node.left) - Visit(node.right);
            else if (node.op.type==TokenType.MUL)
                ret = Visit(node.left) * Visit(node.right);
            else if (node.op.type==TokenType.INTEGER_DIV)
                ret = (int)(Visit(node.left) / Visit(node.right));
            else if (node.op.type==TokenType.FLOAT_DIV)
                ret = (float)Visit(node.left) / (float)Visit(node.right);

            return ret;
        }

        public override dynamic Visit_NodeNum(NodeNum node)
        {
            Debug.Trace(Debug.MODULE.INTERPRETER, $"Visit_NodeNum()");

            // Num num = (Num)node;
            // return num.value;
            return node.value;
        }

        public override dynamic Visit_NodeUnaryOp(NodeUnaryOp node)
        {
            Debug.Trace(Debug.MODULE.INTERPRETER, $"Visit_NodeUnaryOp()");

            dynamic ret = 0;

            // UnaryOp unaryop = (UnaryOp)node;
            if(node.op.type == TokenType.PLUS)
                ret = +Visit(node.expr);
            else if (node.op.type == TokenType.MINUS)
                ret = -Visit(node.expr);

            return ret;
        }

        public override dynamic Visit_NodeCompound(NodeCompound node)
        {
            Debug.Trace(Debug.MODULE.INTERPRETER, $"Visit_NodeCompound()");

            // Compound compound = (Compound)node;
            foreach(AST child in node.children)
                Visit(child);

            return 0;
        }

        public override dynamic Visit_NodeAssign(NodeAssign node)
        {
            Debug.Trace(Debug.MODULE.INTERPRETER, $"Visit_NodeAssign()");

            // Assign assign = (Assign)node;
            // assume left is a NodeVar
            NodeVar left = (NodeVar)node.left;
            string var_name = left.value;
            dynamic value = Visit(node.right);

            if(GLOBAL_MEMORY.ContainsKey(var_name))
                GLOBAL_MEMORY[var_name] = value;
            else
                GLOBAL_MEMORY.Add(var_name, value);

            return 0;
        }

        public override dynamic Visit_NodeVar(NodeVar node)
        {
            Debug.Trace(Debug.MODULE.INTERPRETER, $"Visit_NodeVar()");

            dynamic ret = 0;
            string var_name = node.value;
            if(GLOBAL_MEMORY.ContainsKey(var_name))
                ret = GLOBAL_MEMORY[var_name];
            // else
            //     Debug.Error($"Variable not found '{var_name}'");
            return ret;
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