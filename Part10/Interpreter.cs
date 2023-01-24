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

class Interpreter : NodeVisitor
{
    Parser parser;

    public Dictionary<string,dynamic> GLOBAL_SCOPE = new Dictionary<string, dynamic>();

    public Interpreter(Parser parser)
    {
        this.parser = parser;
    }

    public void PrintGlobalScope()
    {
        Console.WriteLine("----- GLOBAL_SCOPE -----");
        foreach(string var in GLOBAL_SCOPE.Keys)
            Console.WriteLine($"{var}={GLOBAL_SCOPE[var]}");
        Console.WriteLine("-----(GLOBAL_SCOPE)-----");
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

    public dynamic Visit_NodeProgram(NodeProgram node)
    {
        return Visit(node.block);
    }

    public dynamic Visit_NodeBlock(NodeBlock node)
    {
        foreach(NodeVarDecl declaration in node.declarations)
            Visit(declaration);
        return Visit(node.compound_statement);
    }

    public dynamic Visit_NodeVarDecl(NodeVarDecl node)
    {
        return 0; // do nothing
    }

    public dynamic Visit_NodeType(NodeType node)
    {
        return 0; // do nothing
    }

    public dynamic Visit_NodeBinOp(NodeBinOp node)
    {
        dynamic ret = 0;

        if(node.op.type==Token.TYPE.PLUS)
            ret = Visit(node.left) + Visit(node.right);
        else if (node.op.type==Token.TYPE.MINUS)
            ret = Visit(node.left) - Visit(node.right);
        else if (node.op.type==Token.TYPE.MUL)
            ret = Visit(node.left) * Visit(node.right);
        else if (node.op.type==Token.TYPE.INTEGER_DIV)
            ret = (int)(Visit(node.left) / Visit(node.right));
        else if (node.op.type==Token.TYPE.FLOAT_DIV)
            ret = (float)Visit(node.left) / (float)Visit(node.right);

        return ret;
    }

    public dynamic Visit_NodeNum(NodeNum node)
    {
        // Num num = (Num)node;
        // return num.value;
        return node.value;
    }

    public dynamic Visit_NodeUnaryOp(NodeUnaryOp node)
    {
        dynamic ret = 0;

        // UnaryOp unaryop = (UnaryOp)node;
        if(node.op.type == Token.TYPE.PLUS)
            return +Visit(node.expr);
        else if (node.op.type == Token.TYPE.MINUS)
            return -Visit(node.expr);

        return ret;
    }

    public dynamic Visit_NodeCompound(NodeCompound node)
    {
        // Compound compound = (Compound)node;
        foreach(AST child in node.children)
            Visit(child);

        return 0;
    }

    public dynamic Visit_NodeAssign(NodeAssign node)
    {
        // Assign assign = (Assign)node;
        // assume left is a NodeVar
        NodeVar left = (NodeVar)node.left;
        string var_name = left.value;
        dynamic value = Visit(node.right);


        if(GLOBAL_SCOPE.ContainsKey(var_name))
            GLOBAL_SCOPE[var_name] = value;
        else
            GLOBAL_SCOPE.Add(var_name, value);

        return 0;
    }

    public dynamic Visit_NodeVar(NodeVar node)
    {
        dynamic ret = 0;
        string var_name = node.value;
        if(GLOBAL_SCOPE.ContainsKey(var_name))
            ret = GLOBAL_SCOPE[var_name];
        else
            NameError(var_name);
        return ret;
    }

    public dynamic Visit_NodeNoOp(AST node)
    {
        // do nothing
        return 0;
    }

    public dynamic Interpret()
    {
        Debug.Trace(Debug.MODULE.INTERPRETER, $"Interpreter()");

        dynamic ret = 0;

        NodeProgram tree = parser.Parse();

        if (tree is not null)
            ret = Visit(tree);            

        return ret;
    }
}