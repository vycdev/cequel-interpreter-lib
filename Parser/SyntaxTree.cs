using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter_lib.Parser
{
    public class Node
    {
        private ERule _rule { get; } 
        private List<Node> nodes = new();

        #region Constructors
        public Node() { }
 
        public Node(Node node, ERule rule)
        {
            nodes.Add(node);
            _rule = rule;
        }

        public Node(List<Node> nodes, ERule rule)
        {
            this.nodes = nodes;
            _rule = rule;
        }

        #endregion

        public void Add(Node node)
        {
            nodes.Add(node);
        }

        public void Add(List<Node> nodes)
        {
            this.nodes.AddRange(nodes);
        }
    }
}
