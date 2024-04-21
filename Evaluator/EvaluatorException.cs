using Interpreter_lib.Parser;
using Interpreter_lib.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter_lib.Evaluator
{
    public class EvaluatorException : Exception
    {
        ISyntaxNode Node { get; }

        public EvaluatorException(ISyntaxNode node, string? message) : base(BetterMessage(node, message))
        {
            this.Node = node;
        }

        private static string BetterMessage(ISyntaxNode node, string? message)
        {
            if (!string.IsNullOrEmpty(message))
                message = "\n" + message;

            if(node.GetType() == typeof(Node))
            {
                return $"Error at line {((Node)node).Line} in node {((Node)node)._rule}.{message}";
            }
            else
            {
                return $"Error at line {((Token)node).Line} in token {((Token)node).Type} with value {((Token)node).Value}.{message}";
            }
        }
    }
}
