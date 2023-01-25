using System;
using System.Collections.Generic;

namespace SPI
{

    /* Token class used by Lexer */
    public enum TokenType {
        // NONE,
        // single-character token types
        PLUS,
        MINUS,
        MUL,
        FLOAT_DIV,
        LPAR,
        RPAR,
        SEMI,
        DOT,
        COLON,
        COMMA,
        // block of reserved words
        PROGRAM,
        INTEGER,
        REAL,
        STRING,
        INTEGER_DIV,
        VAR,
        PROCEDURE,
        BEGIN,
        WRITE,
        WRITELN,
        END,
        // misc
        ID,
        INTEGER_CONST,
        REAL_CONST,
        STRING_CONST,
        ASSIGN,
        EOF,
    };

    class Token
    {
        public TokenType?   type;
        public dynamic?     value;
        public int?         lineno;
        public int?         column;

        public static Dictionary<TokenType,string> TokenTypeValue = new Dictionary<TokenType, string>() {
            // single-character
            { TokenType.PLUS,           "+" },
            { TokenType.MINUS,          "-" },
            { TokenType.MUL,            "*" },
            { TokenType.FLOAT_DIV,      "/" },
            { TokenType.LPAR,           "(" },
            { TokenType.RPAR,           ")" },
            { TokenType.SEMI,           ";" },
            { TokenType.DOT,            "." },
            { TokenType.COLON,          ":" },
            { TokenType.COMMA,          "," },

            // reserved words
            { TokenType.PROGRAM,        "PROGRAM"   },
            { TokenType.INTEGER,        "INTEGER"   },
            { TokenType.REAL,           "REAL"      },
            { TokenType.STRING,         "STRING"    },
            { TokenType.INTEGER_DIV,    "DIV"       },
            { TokenType.VAR,            "VAR"       },
            { TokenType.PROCEDURE,      "PROCEDURE" },
            { TokenType.BEGIN,          "BEGIN"     },
            { TokenType.WRITE,          "WRITE"     },
            { TokenType.WRITELN,        "WRITELN"   },
            { TokenType.END,            "END"       },
            
            // misc
            { TokenType.ID,             "ID"            },
            { TokenType.INTEGER_CONST,  "INTEGER_CONST" },
            { TokenType.REAL_CONST,     "REAL_CONST"    },
            { TokenType.STRING_CONST,   "STRING_CONST"  },
            { TokenType.ASSIGN,         ":="            },
            { TokenType.EOF,            ""              },
        };


        public Token(TokenType? type, dynamic? value, int? lineno=null, int? column=null)
        {
            this.type = type;
            this.value = value;

            this.lineno = lineno;
            this.column = column;

            Debug.Trace(Debug.TRACE.TOKEN, Str());
        }

        public string Str()
        {
            /* string representation of the class instance.
            //
            // Examples:
            //      Token(INTEGER, 3)
            //      Token(PLUS, '+')
            //      Token(MUL, '*')    
            */
            string str = $"Token({type.ToString()}";

            if(value is not null)
                str += $", value={value!.ToString()}";
            if(lineno is not null && column is not null)
                str += $", position={lineno}:{column}";
            str += ")";

            // Debug.Trace(Debug.MODULE.TOKEN, $"Str():{str}");

            return str;
        }
    }
}