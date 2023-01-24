
class AST
{
	// Polymorphic root class for AST nodes

	public Token	token;

	public AST()
	{
		token = new Token(TokenType.EOF, null);
	}
}

class BinOp : AST
{
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
	public int		value;

	public Num(Token token)
	{
		this.token = token;
		this.value = token.value;
	}
}

class UnaryOp : AST
{
	public Token	op;
	public AST		expr;

	public UnaryOp(Token op, AST expr)
	{
		this.token = op;

		this.op = token;
		this.expr = expr;
	}

}