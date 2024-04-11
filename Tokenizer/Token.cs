using Interpreter_lib.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter_lib.Tokenizer
{
    public class TokensLanguage
    {
        public string READ { get; set; } = "read";
        public string WRITE { get; set; } = "write";
        public string IF { get; set; } = "if";
        public string THEN { get; set; } = "then";
        public string ELSE { get; set; } = "else";
        public string WHILE { get; set; } = "while";
        public string DO { get; set; } = "do";
        public string REPEAT { get; set; } = "repeat";
        public string UNTIL { get; set; } = "until";
        public string FOR { get; set; } = "for";
    }

    public class Token : ISyntaxNode
    {
        public EToken Type { get; private set; }
        public string Value { get; private set; }

        public Token(EToken type, string value)
        {
            Type = type;
            Value = value;
        }

        public object Clone()
        {
            return new Token(Type, Value);
        }

        public string Print()
        {
            return $"{Type}: {Value}";
        }
    }
}
