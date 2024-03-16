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
        public Rule? Rule { get; }

        public ParsingException(Rule? rule, string? message) : base(message)
        {
            Rule = rule;
        }
    }
}
