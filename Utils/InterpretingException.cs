using Interpreter_lib.Parser;
using Interpreter_lib.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter_lib.Utils
{
    public class InterpretingException : Exception
    {
        public Rule? Rule { get; }

        public InterpretingException(Rule? rule, string? message) : base(message)
        {
            Rule = rule;
        }
    }
}
