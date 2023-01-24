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

    class NodeVisitor
    {
        // Dynamic method caller:
        // subclasses.Visit_{node type}
        // example: Visit_Num(node)->int

        public Type class_type;

        public NodeVisitor()
        {
            class_type = typeof(NodeVisitor);
        }

        public dynamic Visit(AST node)
        {
            Debug.Trace(Debug.MODULE.INTERPRETER, $"Visit({node.token.Str()}");

            dynamic ret = 0;

            // string method = $"Visit_{node.GetType()}"; -> Visit_SPI.name
            Type node_type = node.GetType();
            string? method_name = node_type.ToString();
            method_name = method_name.Replace("SPI.", "Visit_");
            // Console.WriteLine($"method_name={method_name!}");

            // class_type = typeof(Interpreter); // won't work. Has to be child class to NodeVisitor

            // Console.WriteLine($"class_type {class_type.ToString()}");

            MethodInfo? info = class_type.GetMethod(method_name);
            if(info is null)
                throw new Exception($"ERROR: Visit method missing '{method_name}' in class '{class_type.ToString()}'");
            else
                try {
                    ret = info!.Invoke(this, new object[]{node})!;
                } catch (Exception e) {
                    // because of "Invoke", innerexception has to be used
                    Debug.Error(e.InnerException!.Message);
                }

            return ret;
        }

    }
}
