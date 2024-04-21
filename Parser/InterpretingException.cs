using Interpreter_lib.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter_lib.Parser
{
    public class InterpretingException : Exception
    {
        public Rule Rule { get; }

        public InterpretingException(Rule rule, string? message) : base(BetterMessage(rule, message))
        {
            Rule = rule;
        }

        private static string BetterMessage(Rule rule, string? message)
        {
            var expectedTokens = rule._expectedTokens;
            var expectedRules = rule._expectedRules;
            var tokens = rule._tokens;
            var currentTokenIndex = rule._currentTokenIndex;
            var eRule = rule._rule;

            string expectedString = string.Empty;
            string expectedTypeName = string.Empty;
            string currentTokenName = string.Empty;
            string currentTokenValue = string.Empty;
            int line = 0;

            if (expectedTokens != null && expectedTokens.Count > 0)
            {
                expectedString = string.Join(", ", expectedTokens.Select(t => t.ToString()));
                expectedTypeName = "token";
            }

            if (expectedRules != null && expectedRules.Count > 0)
            {
                expectedString = string.Join(", ", expectedRules.Select(r => r.ToString()));
                expectedTypeName = "rule";
            }

            if (tokens != null && currentTokenIndex < tokens.Count)
            {
                currentTokenName = tokens[currentTokenIndex].Type.ToString();
                currentTokenValue = tokens[currentTokenIndex].Value;
                line = tokens[currentTokenIndex].Line;
            }

            if (!string.IsNullOrEmpty(message))
                message = "\n" + message;

            return $"Error at line {line}, expected {expectedTypeName} {expectedString}, but instead got {currentTokenName} \"{currentTokenValue}\" inside rule {eRule}.{message}";
        }
    }
}
