using System;
using System.Reflection;
using System.Collections.Generic;

namespace SPI
{

    // AST visitors (walkers)
    // Abstract Symbol Tree

    // abstract class NodeVisitor
    // {
    //     public abstract int Visit(AST node);
    // }

    abstract class NodeVisitor
    {
        // Dynamic method caller:
        // subclasses.Visit_{node type}
        // example: Visit_Num(node)->int

        public Type class_type;

        public NodeVisitor(Type class_type)
        {
            // class_type = typeof(NodeVisitor);
            this.class_type = class_type;
        }

        public dynamic Visit(AST node)
        {
            Debug.Trace(Debug.MODULE.VISITOR, $"Visit({node.token.Str()}");

            dynamic ret = 0;

            // class_type = typeof(Interpreter); // won't work. Has to be child class to NodeVisitor
            // therefore each child-class has to assign class_type
            Debug.Trace(Debug.MODULE.VISITOR, $"class_type {class_type.ToString()}");

            Type node_type = node.GetType();
            string? method_name = node_type.ToString();
            method_name = method_name.Replace("SPI.", "Visit_");

            Debug.Trace(Debug.MODULE.VISITOR, $"method_name={method_name!}");

            MethodInfo? info = class_type.GetMethod(method_name);
            if(info is null)
                Debug.Error($"Warning! Method missing {class_type.ToString()}.{method_name}");
            else
                try {
                    ret = info!.Invoke(this, new object[]{node})!;
                } catch (Exception e) {
                    // because of "Invoke", innerexception has to be used
                    Debug.Error(e.InnerException!.Message);
                }

            return ret;
        }

        public abstract dynamic Visit_NodeProgram(NodeProgram node);
        public abstract dynamic Visit_NodeBlock(NodeBlock node);
        public abstract dynamic Visit_NodeVarDecl(NodeVarDecl node);
        public abstract dynamic Visit_NodeVar(NodeVar node);
        public abstract dynamic Visit_NodeType(NodeType node);
        public abstract dynamic Visit_NodeProcedureDecl(NodeProcedureDecl node);
        public abstract dynamic Visit_NodeCompound(NodeCompound node);
        public abstract dynamic Visit_NodeAssign(NodeAssign node);
        public abstract dynamic Visit_NodeBinOp(NodeBinOp node);
        public abstract dynamic Visit_NodeUnaryOp(NodeUnaryOp node);
        public abstract dynamic Visit_NodeNum(NodeNum node);
        public abstract dynamic Visit_NodeNoOp(NodeNoOp node);


    }
}
