using Interpreter_lib.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter_lib.Parser;

public class Node(ERule rule) : ISyntaxNode
{
    public int Line { get; set; } = 0;

    public ERule _rule { get; private set; } = rule;
    private List<ISyntaxNode> _syntaxNodes = new(); 

    public bool IsEmpty => _syntaxNodes.Count == 0;
    public int TokenCount => _syntaxNodes.Count(isToken) + _syntaxNodes.Where(isNode).Aggregate(0, (acc, n) => acc + ((Node)n).TokenCount);
    public int NodeCount => 1 + _syntaxNodes.Where(isNode).Aggregate(0, (acc, n) => acc + ((Node)n).NodeCount);
    public int TopTokenCount => _syntaxNodes.Count(isToken);
    public int TopNodeCount => _syntaxNodes.Count(isNode);

    private Func<ISyntaxNode, bool> isNode = x => x.GetType() == typeof(Node);
    private Func<ISyntaxNode, bool> isToken = x => x.GetType() == typeof(Token);

    public List<ISyntaxNode> GetSyntaxNodes()
    {
        return _syntaxNodes.Select(n => (ISyntaxNode)n.Clone()).ToList();
    }

    public void Add(ISyntaxNode node)
    {
        _syntaxNodes.Add((ISyntaxNode)node.Clone());
    }

    public void Add(List<ISyntaxNode> nodes)
    {
        _syntaxNodes.AddRange(nodes.Select(n => (ISyntaxNode)n.Clone()));
    }

    public object Clone()
    {
        Node node = new(_rule);
        node.Line = Line;
        node.Add(_syntaxNodes);

        return node;
    }

    public string Print()
    {
        return $"{_rule} | Line: {Line}";
    }
}
