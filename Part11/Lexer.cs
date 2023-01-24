using System;
using System.Collections.Generic;

namespace SPI
{

    class Lexer
    {
        string  text;
        int     pos;
        char?   current_char;
        int     peek_pos;

        public Dictionary<string,Token> RESERVED_KEYWORDS = new Dictionary<string,Token>() {
            {"PROGRAM",     new Token(Token.TYPE.PROGRAM,       "PROGRAM")},
            {"VAR",         new Token(Token.TYPE.VAR,           "VAR")},
            {"DIV",         new Token(Token.TYPE.INTEGER_DIV,   "DIV")},
            {"INTEGER",     new Token(Token.TYPE.INTEGER,       "INTEGER")},
            {"REAL",        new Token(Token.TYPE.REAL,          "REAL")},
            {"BEGIN",       new Token(Token.TYPE.BEGIN,         "BEGIN")},
            {"END",         new Token(Token.TYPE.END,           "END")}
        };

        public Lexer(string text)
        {
            // client string input, e.g. "3 * 5", "12 / 3 * 4", etc
            this.text = text;

            // pos is an index into text
            pos = 0;
            current_char = text[pos];
        }

        public void Error(char ch)
        {
            throw new Exception($"Lexer: Invalid character '{ch}'");
        }

        public void Advance()
        {
            // Advance the 'pos' pointer and set the 'current_char' variable
            pos++;
            if(pos>(text.Length-1))
                current_char = null;
            else
                current_char = text[pos];
        }

        public char? Peek()
        {
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
            while((current_char is not null) && (char.IsWhiteSpace((char)current_char!)))
                Advance();
        }

        public void SkipComment()
        {
            while((current_char is not null) && (current_char! != '}'))
                Advance();
            Advance(); // closing curly brace
        }

        public Token Number()
        {
            // Return a (multidigit) integer or float consumed from the input
            Token ret;

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
                ret = new Token(Token.TYPE.REAL_CONST, float.Parse(num));
            } else {
                ret = new Token(Token.TYPE.INTEGER_CONST, int.Parse(num));
            }
            return ret;
        }

        public Token Id()
        {
            // Handle identifiers and reserved keywords

            string result = "";
            while((current_char is not null) && (char.IsLetterOrDigit((char)current_char!))) {
                result += char.ToUpper((char)current_char!);
                Advance();
            }

            return RESERVED_KEYWORDS.GetValueOrDefault(result, new Token(Token.TYPE.ID, result));
        }

        public Token GetNextToken()
        {
            // Lexical analyzer (also known as scanner or tokenizer)
            //
            // This method is responsible for breaking a sentence
            // apart into tokens. One token at a time.
            Token ret = new Token(Token.TYPE.EOF, null);

            while (current_char is not null) {

                if (char.IsWhiteSpace((char)current_char!)) {
                    SkipWhiteSpace();
                    // continue

                } else if (current_char! == '{') {
                    Advance();
                    SkipComment();
                    // continue

                } else if (char.IsLetter((char)current_char!)) {
                    ret = Id();
                    break; // return

                } else if (char.IsDigit((char)current_char!)) {
                    ret = Number();
                    break; // return

                } else if ((current_char! == ':') && (Peek()! == '=')) {
                    Advance();
                    Advance();
                    ret = new Token(Token.TYPE.ASSIGN, ":=");
                    break; // return

                } else if (current_char! == ';') {
                    Advance();
                    ret = new Token(Token.TYPE.SEMI, ';');
                    break; // return

                } else if (current_char! == ':') {
                    Advance();
                    ret = new Token(Token.TYPE.COLON, ':');
                    break; // return

                } else if (current_char! == ',') {
                    Advance();
                    ret = new Token(Token.TYPE.COMMA, ',');
                    break; // return

                } else if (current_char! == '+') {
                    Advance();
                    ret = new Token(Token.TYPE.PLUS, '+');
                    break; // return

                } else if (current_char! == '-') {
                    Advance();
                    ret = new Token(Token.TYPE.MINUS, '-');
                    break; // return

                } else if (current_char! == '*') {
                    Advance();
                    ret = new Token(Token.TYPE.MUL, '*');
                    break; // return

                } else if (current_char! == '/') {
                    Advance();
                    ret = new Token(Token.TYPE.FLOAT_DIV, '/');
                    break; // return

                } else if (current_char! == '(') {
                    Advance();
                    ret = new Token(Token.TYPE.LPAR, '(');
                    break; // return

                } else if (current_char! == ')') {
                    Advance();
                    ret = new Token(Token.TYPE.RPAR, ')');
                    break; // return

                } else if (current_char! == '.') {
                    Advance();
                    ret = new Token(Token.TYPE.DOT, '.');
                    break; // return

                } else {
                    Error((char)current_char!);
                }
            }
            return ret;
        } 
    }
}