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
			Debug.Trace(Debug.MODULE.AST, $"AST()");

			token = new Token(null, null);
		}

		public abstract string Str();
	}

	class NodeBinOp : AST
	{
		// +, -, *, /

		public AST		left;
		public Token	op;
		public AST		right;

		public NodeBinOp(AST left, Token op, AST right)
		{
			Debug.Trace(Debug.MODULE.AST, $"NodeBinOp({left.Str()}, {op.Str()}, {right.Str()}");

			this.token = op;

			this.left = left;
			this.op = op;
			this.right = right;
		}

		public override string Str()
		{
			return $"NodeBinOp({left.Str()}, {op.Str()}, {right.Str()})";
		}
	}

	class NodeNum : AST
	{
		// integer

		public dynamic	value;

		public NodeNum(Token token)
		{
			Debug.Trace(Debug.MODULE.AST, $"NodeNum({token.Str()}");

			this.token = token;
			this.value = token.value!;
		}

		public override string Str()
		{
			return $"NodeNum({token.Str()})";
		}

	}

	class NodeUnaryOp : AST
	{
		// +num, -num

		public Token	op;
		public AST		expr;

		public NodeUnaryOp(Token op, AST expr)
		{
			Debug.Trace(Debug.MODULE.AST, $"NodeUnaryOp({op.Str()}, {expr.Str()})");

			this.token = op;

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

		public NodeCompound()
		{
			Debug.Trace(Debug.MODULE.AST, $"NodeCompound()");
			// this.token = default/empty

			children = new List<AST>();
		}

		public void AddChild(AST child)
		{
			Debug.Trace(Debug.MODULE.AST, $"NodeCompound.AddChild({child.Str()})");
			children.Add(child);
		}

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

		public NodeAssign(NodeVar left, Token op, AST right)
		{
			Debug.Trace(Debug.MODULE.AST, $"NodeAssign({left.Str()}, {op.Str()}, {right.Str()})");

			this.token = op;

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

		public NodeVar(Token token)
		{
			Debug.Trace(Debug.MODULE.AST, $"NodeVar({token.Str()})");

			this.token = token;
			this.value = token.value!;
		}

		public override string Str()
		{
			return $"NodeVar({token.Str()})";
		}
	}

	class NodeNoOp : AST
	{
		public NodeNoOp()
		{
			Debug.Trace(Debug.MODULE.AST, $"NodeNoOp()");
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

		public NodeProgram(string name, NodeBlock block)
		{
			Debug.Trace(Debug.MODULE.AST, $"NodeProgram({name}, {block.Str()})");

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

		public NodeBlock(List<AST> declarations, NodeCompound compound_statement)
		{
			Debug.Trace(Debug.MODULE.AST, $"NodeBlock(declarations, {compound_statement.Str()} )");

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

		public NodeVarDecl(NodeVar var_node, NodeType type_node)
		{
			Debug.Trace(Debug.MODULE.AST, $"NodeVarDecl({var_node.Str()}, {type_node.Str()})");

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

		public NodeType(Token token)
		{
			Debug.Trace(Debug.MODULE.AST, $"NodeType({token.Str()})");

			this.token = token;
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

		public NodeParam(NodeVar var_node, NodeType type_node)
		{
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
		public List<NodeParam>	parms;
		public NodeBlock		block_node;

		public NodeProcedureDecl(string proc_name, List<NodeParam> parms, NodeBlock block_node)
		{
			this.proc_name = proc_name;
			this.parms = parms;
			this.block_node = block_node;
		}

		public override string Str()
		{
			string ret = $"ProcedureDecl({proc_name}):";
			foreach(NodeParam parm in parms)
				ret += $"\n\t{parm.Str()}";
			ret += $"\n\t{block_node.Str()}";
			return ret;
		}
	}

	class NodeProcedureCall : AST
	{
		public string 		proc_name;
		public List<AST>	actual_parms; // probably expr

		public NodeProcedureCall(string proc_name, List<AST> actual_parms, Token token)
		{
			this.proc_name = proc_name;
			this.actual_parms = actual_parms;
			this.token = token;
		}

		public override string Str()
		{
			string ret = $"ProcedureDecl({proc_name}):";
			foreach(AST parm in actual_parms)
				ret += $"\n\t{parm.Str()}";
			return ret;
		}
	}

}
