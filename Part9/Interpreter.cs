using System;
using System.Reflection;
using System.Collections.Generic;

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
        Debug.Trace(Debug.MODULE.INTERPRETER, $"Visit({node.token.Str()}");

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

    public Dictionary<string,int> GLOBAL_SCOPE = new Dictionary<string, int>();

    public Interpreter(Parser parser)
    {
        this.parser = parser;
    }

    public void PrintGlobalScope()
    {
        foreach(string var in GLOBAL_SCOPE.Keys)
            Console.WriteLine($"{var}={GLOBAL_SCOPE[var]}");
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
        throw new Exception($"Name error: {name}");
    }

    public int Visit_BinOp(BinOp node)
    {
        int ret = 0;

        // BinOp binop = (BinOp)node;
        if(node.op.type==Token.TYPE.PLUS)
            ret = Visit(node.left) + Visit(node.right);
        else if (node.op.type==Token.TYPE.MINUS)
            ret = Visit(node.left) - Visit(node.right);
        else if (node.op.type==Token.TYPE.MUL)
            ret = Visit(node.left) * Visit(node.right);
        else if (node.op.type==Token.TYPE.DIV)
            ret = Visit(node.left) / Visit(node.right);

        return ret;
    }

    public int Visit_Num(Num node)
    {
        // Num num = (Num)node;
        // return num.value;
        return node.value;
    }

    public int Visit_UnaryOp(UnaryOp node)
    {
        int ret = 0;

        // UnaryOp unaryop = (UnaryOp)node;
        if(node.op.type == Token.TYPE.PLUS)
            return +Visit(node.expr);
        else if (node.op.type == Token.TYPE.MINUS)
            return -Visit(node.expr);

        return ret;
    }

    public int Visit_Compound(Compound node)
    {
        // Compound compound = (Compound)node;
        foreach(AST child in node.children)
            Visit(child);

        return 0;
    }

    public int Visit_Assign(Assign node)
    {
        // Assign assign = (Assign)node;
        string var_name = node.left.value;
        int value = Visit(node.right);

Console.WriteLine($"Visit_Assign: var_name={var_name} value={value}");

        if(GLOBAL_SCOPE.ContainsKey(var_name))
            GLOBAL_SCOPE[var_name] = value;
        else
            GLOBAL_SCOPE.Add(var_name, value);

        return 0;
    }

    public int Visit_Var(Var node)
    {
        int ret = 0;
        string var_name = node.value;
        if(GLOBAL_SCOPE.ContainsKey(var_name))
            ret = GLOBAL_SCOPE[var_name];
        else
            NameError(var_name);
        return ret;
    }

    public int Visit_NoOp(AST node)
    {
        // do nothing
        return 0;
    }

    public int Interpret()
    {
        Debug.Trace(Debug.MODULE.INTERPRETER, $"Interpreter()");

        int ret = 0;

        AST? tree = parser.Parse();

        if (tree is not null)
            ret = Visit(tree);            

        return ret;
    }
}