using Interpreter_lib.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter_lib.Parser
{
    public class Parser
    {
        // Used for traversing the tokens array.  
        private int _currentTokenIndex = 0;

        // Data from which the syntax tree is created.
        private List<Token> _tokens;
        private List<Node> _tree;

        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
            _tree = new();

            // Add rules to the tree
            Rule.AddRule(new Rule(ERule.SUM, o => o
                    .WithT(EToken.NUMBER).Once()
                    .ThenR(ERule.SUBSEQUENTSUM).Hoist().AtLeastOnce()));

            Rule.AddRule(new Rule(ERule.SUBSEQUENTSUM, o => o
                    .WithT(EToken.PLUS).Exclude().Once()
                    .ThenT(EToken.NUMBER).Once()));
        }

        public void Parse()
        {
            _tree.Add(Rule.GetRule(ERule.SUM).Evaluate(_tokens)); 
        }

        public List<Node> GetTree()
        {
            return _tree;
        }
    }
}
