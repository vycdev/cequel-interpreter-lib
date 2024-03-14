using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter_lib.Parser
{
    internal class Node
    {
        private List<Node> nodes = new();

        #region Constructors
        public Node() { }
 
        public Node(Node node)
        {
            nodes.Add(node);
        }

        public Node(List<Node> nodes)
        {
            this.nodes = nodes;
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
