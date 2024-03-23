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

            Rule.AddRule(new Rule(ERule.SUBSTRACT, o => o
                    .WithT(EToken.NUMBER).Once()
                    .ThenR(ERule.SUBSEQUENTSUBSTRACT).Hoist().AtLeastOnce()));
            Rule.AddRule(new Rule(ERule.SUBSEQUENTSUBSTRACT, o => o
                    .WithT(EToken.MINUS).Exclude().Once()
                    .ThenT(EToken.NUMBER).Once()));

            Rule.AddRule(new Rule(ERule.MULTIPLY, o => o
                    .WithT(EToken.NUMBER).Once()
                    .ThenR(ERule.SUBSEQUENTMULTIPLY).Hoist().AtLeastOnce()));
            Rule.AddRule(new Rule(ERule.SUBSEQUENTMULTIPLY, o => o
                    .WithT(EToken.MULTIPLY).Exclude().Once()
                    .ThenT(EToken.NUMBER).Once()));

            Rule.AddRule(new Rule(ERule.DIVIDE, o => o
                    .WithT(EToken.NUMBER).Once()
                    .ThenR(ERule.SUBSEQUENTDIVIDE).Hoist().AtLeastOnce()));
            Rule.AddRule(new Rule(ERule.SUBSEQUENTDIVIDE, o => o
                    .WithT(EToken.DIVIDE).Exclude().Once()
                    .ThenT(EToken.NUMBER).Once()));

            Rule.AddRule(new Rule(ERule.MODULUS, o => o
                    .WithT(EToken.NUMBER).Once()
                    .ThenR(ERule.SUBSEQUENTMODULUS).Hoist().AtLeastOnce()));
            Rule.AddRule(new Rule(ERule.SUBSEQUENTMODULUS, o => o
                    .WithT(EToken.MODULUS).Exclude().Once()
                    .ThenT(EToken.NUMBER).Once()));

            Rule.AddRule(new Rule(ERule.POWER, o => o
                    .WithT(EToken.NUMBER).Once()
                    .ThenR(ERule.SUBSEQUENTPOWER).Hoist().AtLeastOnce()));
            Rule.AddRule(new Rule(ERule.SUBSEQUENTPOWER, o => o
                    .WithT(EToken.POWER).Exclude().Once()
                    .ThenT(EToken.NUMBER).Once()));

            Rule.AddRule(new Rule(ERule.FLOOR, o => o
                    .WithT(EToken.LEFT_SQUARE_BRACKET).Exclude().Once()
                    .ThenR(ERule.ARITHMETIC_EXPRESSION).Hoist().Once()
                    .ThenT(EToken.RIGHT_SQUARE_BRACKET).Exclude().Once()));
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
