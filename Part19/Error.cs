using System;
using System.Reflection;

namespace SPI
{
    enum ErrorCode {
        NONE                = 0,
        UNEXPECTED_TOKEN    = 1,
        ID_NOT_FOUND        = 2,
        DUPLICATE_ID        = 3,
    };

    class Error : Exception
    {
        public Dictionary<ErrorCode,string> ErrorText = new Dictionary<ErrorCode, string>() {
            {ErrorCode.NONE,                ""                      },
            {ErrorCode.UNEXPECTED_TOKEN,    "Unexpected token"      },
            {ErrorCode.ID_NOT_FOUND,        "Identifier not found"  },
            {ErrorCode.DUPLICATE_ID,        "Duplicate id found"    },
        };

        public ErrorCode    error_code;
        public Token?       token;
        public string       message;
        public Type         object_type;

        public Error(ErrorCode error_code=ErrorCode.NONE, Token? token=null, string message="")
        {
            this.error_code = error_code;
            this.token = token;
            object_type = this.GetType();
            this.message = $"{object_type.Name}: {message}";
        }
    }

    class LexerError : Error
    {
        public LexerError(string message) : base(ErrorCode.UNEXPECTED_TOKEN, null, message){}
    }

    class ParserError : Error
    {
        public ParserError(ErrorCode error_code, Token token, string message)
            : base(error_code, token, message){}
    }

    class SemanticError : Error
    {
        public SemanticError(ErrorCode error_code, Token token, string message)
            : base(error_code, token, message){}
    }


}