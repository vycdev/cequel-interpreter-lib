using Interpreter_lib.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter_lib.Parser
{
    public class Node : ISyntaxNode
    {
        private ERule _rule;
        private List<ISyntaxNode> _syntaxNodes; 
        public bool IsEmpty => _syntaxNodes.Count == 0;
        public int TokenCount => _syntaxNodes.Count(isToken) + _syntaxNodes.Where(isNode).Aggregate(0, (acc, n) => acc + ((Node)n).TokenCount);
        public int NodeCount => 1 + _syntaxNodes.Where(isNode).Aggregate(0, (acc, n) => acc + ((Node)n).NodeCount);
        public int TopTokenCount => _syntaxNodes.Count(isToken);
        public int TopNodeCount => _syntaxNodes.Count(isNode);

        private Func<ISyntaxNode, bool> isNode = x => x.GetType() == typeof(Node);
        private Func<ISyntaxNode, bool> isToken = x => x.GetType() == typeof(Token);

        public Node(ERule rule) 
        {
            _syntaxNodes = new();
            _rule = rule;
        } 

        public ERule GetRule()
        {
            return _rule;
        }

        public List<ISyntaxNode> GetSyntaxNodes()
        {
            return _syntaxNodes.Select(n => (ISyntaxNode)n.Clone()).ToList();
        }

        public List<Node> GetNodes()
        {
            return _syntaxNodes.Where(isNode).Select(n => (Node)((Node)n).Clone()).ToList(); 
        }

        public List<Token> GetTokens()
        {
            return _syntaxNodes.Where(isToken).Select(t => (Token)t).ToList();
        }

        public void Add(Node node)
        {
            _syntaxNodes.Add((Node)node.Clone());
        }

        public void Add(List<Node> nodes)
        {
            _syntaxNodes.AddRange(nodes.Select(n => (Node)n.Clone()));
        }

        public void Add(Token token)
        {
            _syntaxNodes.Add(token);
        }

        public void Add(List<Token> tokens)
        {
            _syntaxNodes.AddRange(tokens);
        }

        public object Clone()
        {
            Node node = new(_rule);

            List<ISyntaxNode> nodes = new();

            node._syntaxNodes.AddRange(_syntaxNodes.Select(n => (ISyntaxNode)n.Clone()));

            return node;
        }
    }
}
