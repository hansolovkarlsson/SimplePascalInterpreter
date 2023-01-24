""" SPI - Simple Pascal Interpreter. Part 10."""

###############################################################################
#                                                                             #
#  LEXER                                                                      #
#                                                                             #
###############################################################################

# Token NodeTypes
#
# EOF (end-of-file) token is used to indicate that
# there is no more input left for lexical analysis
INTEGER       = 'INTEGER'
REAL          = 'REAL'
INTEGER_CONST = 'INTEGER_CONST'
REAL_CONST    = 'REAL_CONST'
PLUS          = 'PLUS'
MINUS         = 'MINUS'
MUL           = 'MUL'
INTEGER_DIV   = 'INTEGER_DIV'
FLOAT_DIV     = 'FLOAT_DIV'
LPAREN        = 'LPAREN'
RPAREN        = 'RPAREN'
ID            = 'ID'
NodeAssign        = 'NodeAssign'
BEGIN         = 'BEGIN'
END           = 'END'
SEMI          = 'SEMI'
DOT           = 'DOT'
NodeProgram       = 'NodeProgram'
NodeVar           = 'NodeVar'
COLON         = 'COLON'
COMMA         = 'COMMA'
EOF           = 'EOF'


class Token(object):
    def __init__(self, NodeType, value):
        self.NodeType = NodeType
        self.value = value

    def __str__(self):
        """String representation of the class instance.

        Examples:
            Token(INTEGER_CONST, 3)
            Token(PLUS, '+')
            Token(MUL, '*')
        """
        return 'Token({NodeType}, {value})'.format(
            NodeType=self.NodeType,
            value=repr(self.value)
        )

    def __repr__(self):
        return self.__str__()


RESERVED_KEYWORDS = {
    'NodeProgram': Token('NodeProgram', 'NodeProgram'),
    'NodeVar': Token('NodeVar', 'NodeVar'),
    'DIV': Token('INTEGER_DIV', 'DIV'),
    'INTEGER': Token('INTEGER', 'INTEGER'),
    'REAL': Token('REAL', 'REAL'),
    'BEGIN': Token('BEGIN', 'BEGIN'),
    'END': Token('END', 'END'),
}


class Lexer(object):
    def __init__(self, text):
        # client string input, e.g. "4 + 2 * 3 - 6 / 2"
        self.text = text
        # self.pos is an index into self.text
        self.pos = 0
        self.current_char = self.text[self.pos]

    def error(self):
        raise Exception('Invalid character')

    def advance(self):
        """Advance the `pos` pointer and set the `current_char` NodeVariable."""
        self.pos += 1
        if self.pos > len(self.text) - 1:
            self.current_char = None  # Indicates end of input
        else:
            self.current_char = self.text[self.pos]

    def peek(self):
        peek_pos = self.pos + 1
        if peek_pos > len(self.text) - 1:
            return None
        else:
            return self.text[peek_pos]

    def skip_whitespace(self):
        while self.current_char is not None and self.current_char.isspace():
            self.advance()

    def skip_comment(self):
        while self.current_char != '}':
            self.advance()
        self.advance()  # the closing curly brace

    def NodeNumber(self):
        """Return a (multidigit) integer or float consumed from the input."""
        result = ''
        while self.current_char is not None and self.current_char.isdigit():
            result += self.current_char
            self.advance()

        if self.current_char == '.':
            result += self.current_char
            self.advance()

            while (
                self.current_char is not None and
                self.current_char.isdigit()
            ):
                result += self.current_char
                self.advance()

            token = Token('REAL_CONST', float(result))
        else:
            token = Token('INTEGER_CONST', int(result))

        return token

    def _id(self):
        """Handle identifiers and reserved keywords"""
        result = ''
        while self.current_char is not None and self.current_char.isalNodeNum():
            result += self.current_char
            self.advance()

        token = RESERVED_KEYWORDS.get(result, Token(ID, result))
        return token

    def get_next_token(self):
        """Lexical analyzer (also known as scanner or tokenizer)

        This method is responsible for breaking a sentence
        apart into tokens. One token at a time.
        """
        while self.current_char is not None:

            if self.current_char.isspace():
                self.skip_whitespace()
                continue

            if self.current_char == '{':
                self.advance()
                self.skip_comment()
                continue

            if self.current_char.isalpha():
                return self._id()

            if self.current_char.isdigit():
                return self.NodeNumber()

            if self.current_char == ':' and self.peek() == '=':
                self.advance()
                self.advance()
                return Token(NodeAssign, ':=')

            if self.current_char == ';':
                self.advance()
                return Token(SEMI, ';')

            if self.current_char == ':':
                self.advance()
                return Token(COLON, ':')

            if self.current_char == ',':
                self.advance()
                return Token(COMMA, ',')

            if self.current_char == '+':
                self.advance()
                return Token(PLUS, '+')

            if self.current_char == '-':
                self.advance()
                return Token(MINUS, '-')

            if self.current_char == '*':
                self.advance()
                return Token(MUL, '*')

            if self.current_char == '/':
                self.advance()
                return Token(FLOAT_DIV, '/')

            if self.current_char == '(':
                self.advance()
                return Token(LPAREN, '(')

            if self.current_char == ')':
                self.advance()
                return Token(RPAREN, ')')

            if self.current_char == '.':
                self.advance()
                return Token(DOT, '.')

            self.error()

        return Token(EOF, None)


###############################################################################
#                                                                             #
#  PARSER                                                                     #
#                                                                             #
###############################################################################

class AST(object):
    pass


class NodeBinOp(AST):
    def __init__(self, left, op, right):
        self.left = left
        self.token = self.op = op
        self.right = right


class NodeNum(AST):
    def __init__(self, token):
        self.token = token
        self.value = token.value


class NodeUnaryOp(AST):
    def __init__(self, op, expr):
        self.token = self.op = op
        self.expr = expr


class NodeCompound(AST):
    """Represents a 'BEGIN ... END' block"""
    def __init__(self):
        self.children = []


class NodeAssign(AST):
    def __init__(self, left, op, right):
        self.left = left
        self.token = self.op = op
        self.right = right


class NodeVar(AST):
    """The NodeVar node is constructed out of ID token."""
    def __init__(self, token):
        self.token = token
        self.value = token.value


class NodeNoOp(AST):
    pass


class NodeProgram(AST):
    def __init__(self, name, block):
        self.name = name
        self.block = block


class Block(AST):
    def __init__(self, declarations, NodeCompound_statement):
        self.declarations = declarations
        self.NodeCompound_statement = NodeCompound_statement


class NodeVarDecl(AST):
    def __init__(self, NodeVar_node, NodeType_node):
        self.NodeVar_node = NodeVar_node
        self.NodeType_node = NodeType_node


class NodeType(AST):
    def __init__(self, token):
        self.token = token
        self.value = token.value


class Parser(object):
    def __init__(self, lexer):
        self.lexer = lexer
        # set current token to the first token taken from the input
        self.current_token = self.lexer.get_next_token()

    def error(self):
        raise Exception('Invalid syntax')

    def eat(self, token_NodeType):
        # compare the current token NodeType with the passed token
        # NodeType and if they match then "eat" the current token
        # and NodeAssign the next token to the self.current_token,
        # otherwise raise an exception.
        if self.current_token.NodeType == token_NodeType:
            self.current_token = self.lexer.get_next_token()
        else:
            self.error()

    def NodeProgram(self):
        """NodeProgram : NodeProgram NodeVariable SEMI block DOT"""
        self.eat(NodeProgram)
        NodeVar_node = self.NodeVariable()
        prog_name = NodeVar_node.value
        self.eat(SEMI)
        block_node = self.block()
        NodeProgram_node = NodeProgram(prog_name, block_node)
        self.eat(DOT)
        return NodeProgram_node

    def block(self):
        """block : declarations NodeCompound_statement"""
        declaration_nodes = self.declarations()
        NodeCompound_statement_node = self.NodeCompound_statement()
        node = Block(declaration_nodes, NodeCompound_statement_node)
        return node

    def declarations(self):
        """declarations : NodeVar (NodeVariable_declaration SEMI)+
                        | empty
        """
        declarations = []
        if self.current_token.NodeType == NodeVar:
            self.eat(NodeVar)
            while self.current_token.NodeType == ID:
                NodeVar_decl = self.NodeVariable_declaration()
                declarations.extend(NodeVar_decl)
                self.eat(SEMI)

        return declarations

    def NodeVariable_declaration(self):
        """NodeVariable_declaration : ID (COMMA ID)* COLON NodeType_spec"""
        NodeVar_nodes = [NodeVar(self.current_token)]  # first ID
        self.eat(ID)

        while self.current_token.NodeType == COMMA:
            self.eat(COMMA)
            NodeVar_nodes.append(NodeVar(self.current_token))
            self.eat(ID)

        self.eat(COLON)

        NodeType_node = self.NodeType_spec()
        NodeVar_declarations = [
            NodeVarDecl(NodeVar_node, NodeType_node)
            for NodeVar_node in NodeVar_nodes
        ]
        return NodeVar_declarations

    def NodeType_spec(self):
        """NodeType_spec : INTEGER
                     | REAL
        """
        token = self.current_token
        if self.current_token.NodeType == INTEGER:
            self.eat(INTEGER)
        else:
            self.eat(REAL)
        node = NodeType(token)
        return node

    def NodeCompound_statement(self):
        """
        NodeCompound_statement: BEGIN statement_list END
        """
        self.eat(BEGIN)
        nodes = self.statement_list()
        self.eat(END)

        root = NodeCompound()
        for node in nodes:
            root.children.append(node)

        return root

    def statement_list(self):
        """
        statement_list : statement
                       | statement SEMI statement_list
        """
        node = self.statement()

        results = [node]

        while self.current_token.NodeType == SEMI:
            self.eat(SEMI)
            results.append(self.statement())

        return results

    def statement(self):
        """
        statement : NodeCompound_statement
                  | NodeAssignment_statement
                  | empty
        """
        if self.current_token.NodeType == BEGIN:
            node = self.NodeCompound_statement()
        elif self.current_token.NodeType == ID:
            node = self.NodeAssignment_statement()
        else:
            node = self.empty()
        return node

    def NodeAssignment_statement(self):
        """
        NodeAssignment_statement : NodeVariable NodeAssign expr
        """
        left = self.NodeVariable()
        token = self.current_token
        self.eat(NodeAssign)
        right = self.expr()
        node = NodeAssign(left, token, right)
        return node

    def NodeVariable(self):
        """
        NodeVariable : ID
        """
        node = NodeVar(self.current_token)
        self.eat(ID)
        return node

    def empty(self):
        """An empty production"""
        return NodeNoOp()

    def expr(self):
        """
        expr : term ((PLUS | MINUS) term)*
        """
        node = self.term()

        while self.current_token.NodeType in (PLUS, MINUS):
            token = self.current_token
            if token.NodeType == PLUS:
                self.eat(PLUS)
            elif token.NodeType == MINUS:
                self.eat(MINUS)

            node = NodeBinOp(left=node, op=token, right=self.term())

        return node

    def term(self):
        """term : factor ((MUL | INTEGER_DIV | FLOAT_DIV) factor)*"""
        node = self.factor()

        while self.current_token.NodeType in (MUL, INTEGER_DIV, FLOAT_DIV):
            token = self.current_token
            if token.NodeType == MUL:
                self.eat(MUL)
            elif token.NodeType == INTEGER_DIV:
                self.eat(INTEGER_DIV)
            elif token.NodeType == FLOAT_DIV:
                self.eat(FLOAT_DIV)

            node = NodeBinOp(left=node, op=token, right=self.factor())

        return node

    def factor(self):
        """factor : PLUS factor
                  | MINUS factor
                  | INTEGER_CONST
                  | REAL_CONST
                  | LPAREN expr RPAREN
                  | NodeVariable
        """
        token = self.current_token
        if token.NodeType == PLUS:
            self.eat(PLUS)
            node = NodeUnaryOp(token, self.factor())
            return node
        elif token.NodeType == MINUS:
            self.eat(MINUS)
            node = NodeUnaryOp(token, self.factor())
            return node
        elif token.NodeType == INTEGER_CONST:
            self.eat(INTEGER_CONST)
            return NodeNum(token)
        elif token.NodeType == REAL_CONST:
            self.eat(REAL_CONST)
            return NodeNum(token)
        elif token.NodeType == LPAREN:
            self.eat(LPAREN)
            node = self.expr()
            self.eat(RPAREN)
            return node
        else:
            node = self.NodeVariable()
            return node

    def parse(self):
        """
        NodeProgram : NodeProgram NodeVariable SEMI block DOT

        block : declarations NodeCompound_statement

        declarations : NodeVar (NodeVariable_declaration SEMI)+
                     | empty

        NodeVariable_declaration : ID (COMMA ID)* COLON NodeType_spec

        NodeType_spec : INTEGER | REAL

        NodeCompound_statement : BEGIN statement_list END

        statement_list : statement
                       | statement SEMI statement_list

        statement : NodeCompound_statement
                  | NodeAssignment_statement
                  | empty

        NodeAssignment_statement : NodeVariable NodeAssign expr

        empty :

        expr : term ((PLUS | MINUS) term)*

        term : factor ((MUL | INTEGER_DIV | FLOAT_DIV) factor)*

        factor : PLUS factor
               | MINUS factor
               | INTEGER_CONST
               | REAL_CONST
               | LPAREN expr RPAREN
               | NodeVariable

        NodeVariable: ID
        """
        node = self.NodeProgram()
        if self.current_token.NodeType != EOF:
            self.error()

        return node


###############################################################################
#                                                                             #
#  INTERPRETER                                                                #
#                                                                             #
###############################################################################

class NodeVisitor(object):
    def visit(self, node):
        method_name = 'visit_' + NodeType(node).__name__
        visitor = getattr(self, method_name, self.generic_visit)
        return visitor(node)

    def generic_visit(self, node):
        raise Exception('No visit_{} method'.format(NodeType(node).__name__))


class Interpreter(NodeVisitor):
    def __init__(self, parser):
        self.parser = parser
        self.GLOBAL_SCOPE = {}

    def visit_NodeProgram(self, node):
        self.visit(node.block)

    def visit_Block(self, node):
        for declaration in node.declarations:
            self.visit(declaration)
        self.visit(node.NodeCompound_statement)

    def visit_NodeVarDecl(self, node):
        # Do nothing
        pass

    def visit_NodeType(self, node):
        # Do nothing
        pass

    def visit_NodeBinOp(self, node):
        if node.op.NodeType == PLUS:
            return self.visit(node.left) + self.visit(node.right)
        elif node.op.NodeType == MINUS:
            return self.visit(node.left) - self.visit(node.right)
        elif node.op.NodeType == MUL:
            return self.visit(node.left) * self.visit(node.right)
        elif node.op.NodeType == INTEGER_DIV:
            return self.visit(node.left) // self.visit(node.right)
        elif node.op.NodeType == FLOAT_DIV:
            return float(self.visit(node.left)) / float(self.visit(node.right))

    def visit_NodeNum(self, node):
        return node.value

    def visit_NodeUnaryOp(self, node):
        op = node.op.NodeType
        if op == PLUS:
            return +self.visit(node.expr)
        elif op == MINUS:
            return -self.visit(node.expr)

    def visit_NodeCompound(self, node):
        for child in node.children:
            self.visit(child)

    def visit_NodeAssign(self, node):
        NodeVar_name = node.left.value
        self.GLOBAL_SCOPE[NodeVar_name] = self.visit(node.right)

    def visit_NodeVar(self, node):
        NodeVar_name = node.value
        NodeVar_value = self.GLOBAL_SCOPE.get(NodeVar_name)
        if NodeVar_value is None:
            raise NameError(repr(NodeVar_name))
        else:
            return NodeVar_value

    def visit_NodeNoOp(self, node):
        pass

    def interpret(self):
        tree = self.parser.parse()
        if tree is None:
            return ''
        return self.visit(tree)


def main():
    import sys
    text = open(sys.argv[1], 'r').read()

    lexer = Lexer(text)
    parser = Parser(lexer)
    interpreter = Interpreter(parser)
    result = interpreter.interpret()

    for k, v in sorted(interpreter.GLOBAL_SCOPE.items()):
        print('{} = {}'.format(k, v))


if __name__ == '__main__':
    main()

