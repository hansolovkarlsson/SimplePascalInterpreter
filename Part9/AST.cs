using System;
using System.Collections.Generic;

// Abstract Syntax Tree, part of Parser

class AST
{
	// Polymorphic root class for AST nodes

	public Token	token;

	public AST()
	{
		token = new Token(Token.TYPE.EOF, null);
	}
}

class BinOp : AST
{
	// +, -, *, /

	public AST		left;
	public Token	op;
	public AST		right;

	public BinOp(AST left, Token op, AST right)
	{
		this.token = op;

		this.left = left;
		this.op = op;
		this.right = right;
	}
}

class Num : AST
{
	// integer

	public int		value;

	public Num(Token token)
	{
		this.token = token;
		this.value = token.value;
	}
}

class UnaryOp : AST
{
	// +num, -num

	public Token	op;
	public AST		expr;

	public UnaryOp(Token op, AST expr)
	{
		this.token = op;

		this.op = token;
		this.expr = expr;
	}
}

class Compound : AST
{
	// Represents a 'BEING' ... 'END' block

	public List<AST> children;

	public Compound()
	{
		// this.token = default/empty

		children = new List<AST>();
	}
}

class Assign : AST
{
	// :=

	public Var left;
	public Token op;
	public AST right;

	public Assign(Var left, Token op, AST right)
	{
		this.token = op;

		this.left = left;
		this.op = op;
		this.right = right;
	}
}

class Var : AST
{
	// the var node is constructed out of the ID token

	public string	value;

	public Var(Token token)
	{
		this.token = token;
		this.value = token.value!;
	}
}

class NoOp : AST
{
	public NoOp()
	{
		// do nothing
	}
}
