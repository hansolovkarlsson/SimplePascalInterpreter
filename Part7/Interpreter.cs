using System;
using System.Reflection;

// abstract class NodeVisitor
// {
//     public abstract int Visit(AST node);
// }

class NodeVisitor
{
    // Dynamic method caller:
    // subclasses.Visit_{node type}
    // example: Visit_Num(node)->int

    public int Visit(AST node)
    {
        int ret = 0;

        Type type = typeof(Interpreter);
        MethodInfo? info = type.GetMethod($"Visit_{node.GetType()}");
        if(info is not null)
            ret = (int)(info!.Invoke(this, new object[]{node})!);

        return ret;
    }

}

class Interpreter : NodeVisitor
{
    Parser parser;

/*
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

    public Interpreter(Parser parser)
    {
        this.parser = parser;
    }

    public int Visit_BinOp(AST node)
    {
        int ret = 0;

        BinOp binop = (BinOp)node;
        if(binop.op.type==TokenType.PLUS)
            ret = Visit(binop.left) + Visit(binop.right);
        else if (binop.op.type==TokenType.MINUS)
            ret = Visit(binop.left) - Visit(binop.right);
        else if (binop.op.type==TokenType.MUL)
            ret = Visit(binop.left) * Visit(binop.right);
        else if (binop.op.type==TokenType.DIV)
            ret = Visit(binop.left) / Visit(binop.right);

        return ret;
    }

    public int Visit_Num(AST node)
    {
        Num num = (Num)node;
        return num.value;
    }

    public int Interpret()
    {
        AST tree = parser.Parse();
        return Visit(tree);
    }
}