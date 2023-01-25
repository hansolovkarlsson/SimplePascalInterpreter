using System;
using System.Collections.Generic;

namespace SPI
{

	// Abstract Syntax Tree, part of Parser

	abstract class AST
	{
		// Polymorphic root class for AST nodes

		public Token	token;

		public AST()
		{
			Debug.Trace(Debug.TRACE.AST, $"AST()");
			token = new Token(null, null);
		}

		public AST(Token token)
		{
			Debug.Trace(Debug.TRACE.AST, $"AST(token)");
			this.token = token;
		}

		public abstract string Str();
	}

	class NodeBinOp : AST
	{
		// +, -, *, /

		public AST		left;
		public Token	op;
		public AST		right;

		public NodeBinOp(AST left, Token op, AST right) : base(op)
		{
			Debug.Trace(Debug.TRACE.AST, $"NodeBinOp({left.Str()}, {op.Str()}, {right.Str()}");
			// this.token = op;

			this.left = left;
			this.op = op;
			this.right = right;
		}

		public override string Str()
		{
			return $"NodeBinOp({left.Str()}, {op.Str()}, {right.Str()})";
		}
	}

	// probably would be better to be called NodeConst
	class NodeNum : AST
	{
		// integer / float/real

		public dynamic	value;

		public NodeNum(Token token) : base(token)
		{
			Debug.Trace(Debug.TRACE.AST, $"NodeNum({token.Str()}");
			// this.token = token;
			this.value = token.value!;
		}

		public override string Str()
		{
			return $"NodeNum({token.Str()})";
		}
	}

	class NodeStr : AST
	{
		// string
		public string value;

		public NodeStr(Token token) : base(token)
		{
			Debug.Trace(Debug.TRACE.AST, $"NodeStr({token.Str()}");
			this.value = token.value!;
		}

		public override string Str()
		{
			return $"NodeStr({token.Str()})";
		}
	}

	class NodeUnaryOp : AST
	{
		// +num, -num

		public Token	op;
		public AST		expr;

		public NodeUnaryOp(Token op, AST expr) : base(op)
		{
			Debug.Trace(Debug.TRACE.AST, $"NodeUnaryOp({op.Str()}, {expr.Str()})");
			// this.token = op;
			this.op = token;
			this.expr = expr;
		}

		public override string Str()
		{
			return $"NodeUnaryOp({op.Str()}, {expr.Str()})";
		}
	}

	class NodeCompound : AST
	{
		// Represents a 'BEING' ... 'END' block

		public List<AST> children;

		public NodeCompound() : base()
		{
			Debug.Trace(Debug.TRACE.AST, $"NodeCompound()");
			// this.token = default/empty
			children = new List<AST>();
		}

		// public void AddChild(AST child)
		// {
		// 	Debug.Trace(Debug.TRACE.AST, $"NodeCompound.AddChild({child.Str()})");
		// 	children.Add(child);
		// }

		public override string Str()
		{
			string ret = $"NodeCompound:\n";
			foreach(AST node in children) {
				ret = ret + $"\t{node.Str()}\n";
			}
			return ret;
		}
	}

	class NodeAssign : AST
	{
		// :=

		public NodeVar	left; // most likely a NodeVar
		public Token	op;
		public AST		right; // num, op, ...

		public NodeAssign(NodeVar left, Token op, AST right) : base(op)
		{
			Debug.Trace(Debug.TRACE.AST, $"NodeAssign({left.Str()}, {op.Str()}, {right.Str()})");
			// this.token = op;
			this.left = left;
			this.op = op;
			this.right = right;
		}

		public override string Str()
		{
			return $"NodeAssign({left.Str()}, {op.Str()}, {right.Str()})";
		}
	}

	class NodeVar : AST
	{
		// the var node is constructed out of the ID token

		public dynamic	value;

		public NodeVar(Token token) : base(token)
		{
			Debug.Trace(Debug.TRACE.AST, $"NodeVar({token.Str()})");
			// this.token = token;
			this.value = token.value!;
		}

		public override string Str()
		{
			return $"NodeVar({token.Str()})";
		}
	}

	class NodeNoOp : AST
	{
		public NodeNoOp() : base()
		{
			Debug.Trace(Debug.TRACE.AST, $"NodeNoOp()");
			// do nothing
		}

		public override string Str()
		{
			return $"NodeNoOp()";
		}
	}

	class NodeProgram : AST
	{
		public string name;
		public NodeBlock block;

		public NodeProgram(string name, NodeBlock block) : base()
		{
			Debug.Trace(Debug.TRACE.AST, $"NodeProgram({name}, {block.Str()})");
			this.name = name;
			this.block = block;
		}

		public override string Str()
		{
			return $"NodeProgram: {name}\n{block.Str()}";
		}
	}

	class NodeBlock : AST
	{
		public List<AST>	declarations; // using compound maybe?
		public NodeCompound			compound_statement;

		public NodeBlock(List<AST> declarations, NodeCompound compound_statement) : base()
		{
			Debug.Trace(Debug.TRACE.AST, $"NodeBlock(declarations, {compound_statement.Str()} )");
			this.declarations = declarations;
			this.compound_statement = compound_statement;
		}

		public override string Str()
		{
			string ret = $"NodeBlock:\n";
			foreach(AST node in declarations) {
				ret = ret + $"\t{node.Str()}\n";
			}
			// shit, forgot the print compound, simple as that!!
			ret += compound_statement.Str();
			return ret;
		}
	}

	class NodeVarDecl : AST
	{
		public NodeVar var_node;
		public NodeType type_node;

		public NodeVarDecl(NodeVar var_node, NodeType type_node) : base()
		{
			Debug.Trace(Debug.TRACE.AST, $"NodeVarDecl({var_node.Str()}, {type_node.Str()})");
			this.var_node = var_node;
			this.type_node = type_node;
		}

		public override string Str()
		{
			return $"NodeVarDecl({var_node.Str()}, {type_node.Str()})";
		}
	}

	class NodeType : AST
	{
		public dynamic value;

		public NodeType(Token token) : base(token)
		{
			Debug.Trace(Debug.TRACE.AST, $"NodeType({token.Str()})");
			// this.token = token;
			this.value = token.value!;
		}

		public override string Str()
		{
			return $"NodeType({value})";
		}
	}
	
	class NodeParam : AST
	{
		public NodeVar		var_node;
		public NodeType		type_node;

		public NodeParam(NodeVar var_node, NodeType type_node) : base()
		{
			Debug.Trace(Debug.TRACE.AST, "NodeParam()");
			this.var_node = var_node;
			this.type_node = type_node;
		}

		public override string Str()
		{
			return $"NodeParam({var_node.Str()}, {type_node.Str()})";
		}
	}

	class NodeProcedureDecl : AST
	{
		public string			proc_name;
		public List<NodeParam>	formal_parms;
		public NodeBlock		block_node;

		public NodeProcedureDecl(string proc_name, List<NodeParam> formal_parms, NodeBlock block_node) : base()
		{
			Debug.Trace(Debug.TRACE.AST, "NodeProcedureDecl()");
			this.proc_name = proc_name;
			this.formal_parms = formal_parms;
			this.block_node = block_node;
		}

		public override string Str()
		{
			string ret = $"ProcedureDecl({proc_name}):";
			foreach(NodeParam formal_parm in formal_parms)
				ret += $"\n\t{formal_parm.Str()}";
			ret += $"\n\t{block_node.Str()}";
			return ret;
		}
	}

	class NodeProcedureCall : AST
	{
		public string 				proc_name;
		public List<AST>			actual_parms; // each one a branch of expr
		public ProcedureSymbol?		proc_symbol;

		public NodeProcedureCall(string proc_name, List<AST> actual_parms, Token token) : base(token)
		{
			Debug.Trace(Debug.TRACE.AST, "NodeProcedureCall()");
			// this.token = token;
			this.proc_name = proc_name;
			this.actual_parms = actual_parms;

			// a reference to procedure declaration symbol
			this.proc_symbol = null;
		}

		public override string Str()
		{
			string ret = $"ProcedureDecl({proc_name}):";
			foreach(AST parm in actual_parms)
				ret += $"\n\t{parm.Str()}";
			if(proc_symbol is not null)
				ret += $"\n\t{proc_symbol!.Str()}";
			return ret;
		}
	}

	class NodeWriteStatement : AST
	{
		public bool new_line;
		public List<AST> expressions;

		public NodeWriteStatement() : base()
		{
			this.new_line  = false;
			this.expressions = new List<AST>();
		}

		public override string Str()
		{
			string ret =$"NodeWriteStatement(new_line:{new_line}):";
			foreach(AST expression in expressions)
				ret += $"\n\t{expression.Str()}";
			return ret;
		}

	}

}
