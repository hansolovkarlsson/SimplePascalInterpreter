#Part 2: write, writelin

sample program:
```Pascal
program Main;
begin
    writeln('Hello World!');
end.
```

1. Token: TokenType, reserved keywords, write and writeln
2. AST: NodeWriteStatement
3. Parser: statement method modified
4. Factor return string data
5. SemanticAnalyzer: Visit_NodeWriteStatement
6. Interpreter: string and write



