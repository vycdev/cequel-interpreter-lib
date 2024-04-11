using Interpreter_lib.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter_lib.Parser
{
    public class Node : ICloneable
    {
        private ERule _rule;
        private List<Node> _nodes;
        private List<Token> _tokens;
        public bool IsEmpty => _nodes.Count == 0 && _tokens.Count == 0;
        public int TokenCount => _tokens.Count + _nodes.Aggregate(0, (acc, n) => acc + n.TokenCount);
        public int NodeCount => 1 + _nodes.Aggregate(0, (acc, n) => acc + n.NodeCount);
        public int TopTokenCount => _tokens.Count;
        public int TopNodeCount => _nodes.Count;

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
            return new List<Node>(_nodes.Select(n => (Node)n.Clone())); 
        }

        public List<Token> GetTokens()
        {
            return _tokens;
        }

        public void Add(Node node)
        {
            _nodes.Add((Node)node.Clone());
        }

        public void Add(List<Node> nodes)
        {
            _nodes.AddRange(nodes.Select(n => (Node)n.Clone()));
        }

        public void Add(Token token)
        {
            _tokens.Add(token);
        }

        public void Add(List<Token> tokens)
        {
            _tokens.AddRange(tokens);
        }

        public object Clone()
        {
            Node node = new(_rule);
            node._nodes = new(_nodes);
            node._tokens = new(_tokens);

            return node;
        }
    }
}
