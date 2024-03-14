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
        public ParsingException()
        {
        }

        public ParsingException(string? message) : base(message)
        {
        }

        public ParsingException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected ParsingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
