using System;
using System.Collections.Generic;

namespace SPI
{

    class Lexer
    {
        // probably should redesign it so no moving a Token as parameter but rather a TokenType
        // get program working with the latest changes first, then redesign it for TokenType

        public string  text;
        public int     pos;
        public char?   current_char;
        public int     peek_pos;

        // token line number and column number
        public int      lineno;
        public int      column;

        public Dictionary<string,TokenType> RESERVED_KEYWORDS;


        public Lexer(string text)
        {
            Debug.Trace(Debug.TRACE.LEXER, "Lexer.Lexer()");

            // client string input, e.g. "3 * 5", "12 / 3 * 4", etc
            this.text = text;

            // pos is an index into text
            pos = 0;
            current_char = text[pos];

            lineno = 1;
            column = 1;

            RESERVED_KEYWORDS = BuildReservedKeywords();
        }

        public Dictionary<string,TokenType> BuildReservedKeywords()
        {
            Debug.Trace(Debug.TRACE.LEXER, "BuildReservedKeywords()");

            /*  Build a dictionary of reserved keywords
                The function relies on the fact theat in the TokeType
                enumeration the beginning of the block of reserved keywords is
                marked with PROGRAM and the end of the block is marked with
                the END keyword
            */

            Dictionary<string,TokenType> reserved_keywords = new Dictionary<string, TokenType>();

            int start_index = (int)TokenType.PROGRAM;
            int end_index = (int)TokenType.END;
            for(int ix=start_index; ix<=end_index; ix++) {
                string word = Token.TokenTypeValue[(TokenType)ix];
                reserved_keywords.Add(word, (TokenType)ix);
            }

            return reserved_keywords;
        }

        public void Error()
        {
            string s = $"Lexer error on '{current_char}' line: {lineno} column: {column}";
            Debug.Error(s);
            throw new LexerError(s);
        }

        public void Advance()
        {
            Debug.Trace(Debug.TRACE.LEXER, $"Lexer.Advance()");

            // Advance the 'pos' pointer and set the 'current_char' variable
            if(current_char=='\n') {
                lineno++;
                column = 0;
            }

            pos++;
            if(pos>(text.Length-1))
                current_char = null; // indicates end of input
            else {
                current_char = text[pos];
                column++;
            }
        }

        public char? Peek()
        {
            Debug.Trace(Debug.TRACE.LEXER, $"Lexer.Peek()");

            char? ret = null;
            peek_pos = pos+1;
            if(peek_pos>(text.Length-1))
                ret = null;
            else
                ret = text[peek_pos];
            return ret;
        }

        public void SkipWhiteSpace()
        {
            Debug.Trace(Debug.TRACE.LEXER, $"Lexer.SkipWhiteSpace()");

            while((current_char is not null) && (char.IsWhiteSpace((char)current_char!)))
                Advance();
        }

        public void SkipComment()
        {
            Debug.Trace(Debug.TRACE.LEXER, $"Lexer.SkipComment()");

            while((current_char is not null) && (current_char! != '}'))
                Advance();
            Advance(); // closing curly brace
        }

        public Token Number()
        {
            Debug.Trace(Debug.TRACE.LEXER, $"Lexer.Number()");

            // Return a (multidigit) integer or float consumed from the input
            Token token = new Token(type: null, value: null, lineno: lineno, column: column);

            string num = "";
            while((current_char is not null) && (char.IsDigit((char)current_char!))) {
                num = num + (char)current_char!;
                Advance();
            }

            if(current_char! == '.') {
                num = num + (char)current_char!;
                Advance();

                while((current_char is not null) && (char.IsDigit((char)current_char!))) {
                    num = num + (char)current_char!;
                    Advance();
                }

                token.type = TokenType.REAL_CONST;
                token.value = float.Parse(num);
            } else {
                token.type = TokenType.INTEGER_CONST;
                token.value = int.Parse(num);
            }

            return token;   // ret;
        }

        public Token Id()
        {
            Debug.Trace(Debug.TRACE.LEXER, $"Lexer.Id()");

            // Handle identifiers and reserved keywords
            Token token = new Token(type: null, value: null, lineno: lineno, column: column);

            string word = "";
            while((current_char is not null) && (char.IsLetterOrDigit((char)current_char!))) {
                word += char.ToUpper((char)current_char!);
                Advance();
            }

            TokenType token_type;
            if(RESERVED_KEYWORDS.TryGetValue(word, out token_type)) {
                // reserved keyword
                token.type = token_type;
                token.value = word;
            } else {
                token.type = TokenType.ID;
                token.value = word;
            }

            return token;
        }

        // string constants start with single quote, 'my string'
        public Token StringConstant()
        {
            string result = "";
            // create a new token with current line and column number
            Token token = new Token(
                type:   TokenType.STRING_CONST,
                value:  null,
                lineno: lineno,
                column: column
            );
            while((current_char is not null) && (current_char !='\'')) {
                result += (char)(current_char);
                Advance();
            }
            Advance(); // skip closing "'"

            token.value = result;

            return token;
        }

        public Token GetNextToken()
        {
            Debug.Trace(Debug.TRACE.LEXER, $"Lexer.GetNextToken()");

            // Lexical analyzer (also known as scanner or tokenizer)
            //
            // This method is responsible for breaking a sentence
            // apart into tokens. One token at a time.
            Token ret = new Token(TokenType.EOF, null);

            while (current_char is not null) {

                if (char.IsWhiteSpace((char)current_char)) {
                    SkipWhiteSpace();
                    // continue

                } else if (current_char == '{') {
                    Advance();
                    SkipComment();
                    // continue

                } else if (char.IsLetter((char)current_char!)) {
                    ret = Id();
                    break; // return

                } else if (char.IsDigit((char)current_char!)) {
                    ret = Number();
                    break; // return

                } else if (current_char == '\'') {
                    Advance(); // skip "'"
                    ret = StringConstant();
                    break; // return

                } else if ((current_char == ':') && (Peek()! == '=')) {
                    ret = new Token(
                        type: TokenType.ASSIGN,
                        value: Token.TokenTypeValue[TokenType.ASSIGN],
                        lineno: lineno,
                        column: column
                        );
                    Advance();
                    Advance();
                    break; // return

                // Single Character token
                } else if(Token.TokenTypeValue.ContainsValue(current_char.ToString()!)) {
                    // get enum member by value, e.g.
                    // Find ';' => TokenType.SEMI
                    TokenType token_type = 
                        Token.TokenTypeValue.First(x => x.Value == current_char.ToString()!).Key;
                    ret = new Token(
                        type: token_type,
                        value: Token.TokenTypeValue[token_type],
                        lineno: lineno,
                        column: column
                    );
                    Advance();
                    break; // return token

                } else {
                    // Error((char)current_char!);
                    // Debug.Error($"Lexer.GetNextToken invalid character '{current_char}'");
                    Error();
                }
            }
            return ret;
        } 
    }
}