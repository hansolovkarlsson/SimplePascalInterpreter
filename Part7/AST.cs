
class AST
{
	// empty class, root for polymorphic classes below

}

class BinOp : AST
{
	public AST		left;
	public AST		right;
	public Token	token;
	public Token	op;

	public BinOp(AST left, Token op, AST right)
	{
		this.left = left;
		this.token = op;
		this.op = op;
		this.right = right;
	}
}

class Num : AST
{
	public Token	token;
	public int		value;

	public Num(Token token)
	{
		this.token = token;
		this.value = token.value;
	}
}