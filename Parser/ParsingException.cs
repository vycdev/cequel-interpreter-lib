using Interpreter_lib.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter_lib.Parser
{
    internal class ParsingException : Exception
    {
        public Token? Token { get; set; }

        public ParsingException()
        {
        }

        public ParsingException(string? message) : base(message)
        {
        }

        public ParsingException(string? message, Token? token) : base(message)
        {
            Token = token;
        }

        public ParsingException(string? message, Token? token, Exception? innerException) : base(message, innerException)
        {
            Token = token;
        }

        public ParsingException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
