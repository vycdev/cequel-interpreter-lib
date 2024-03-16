using Interpreter_lib.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter_lib.Parser
{
    public class Node
    {
        private ERule _rule;
        private List<Node> _nodes;
        private List<Token> _tokens;
        public bool IsEmpty => _nodes.Count == 0 && _tokens.Count == 0;

        public Node(ERule rule) 
        {
            _nodes = new();
            _tokens = new();
            _rule = rule;
        }

        public ERule GetRule()
        {
            return _rule;
        }

        public List<Node> GetNodes()
        {
            return _nodes;
        }
        public List<Token> GetTokens()
        {
            return _tokens;
        }

        public void Add(Node node)
        {
            _nodes.Add(node);
        }

        public void Add(List<Node> nodes)
        {
            _nodes.AddRange(nodes);
        }

        public void Add(Token token)
        {
            _tokens.Add(token);
        }

        public void Add(List<Token> tokens)
        {
            _tokens.AddRange(tokens);
        }
    }
}
