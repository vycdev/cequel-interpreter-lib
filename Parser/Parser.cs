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
            Rule.AddRule(new Rule(ERule.ARITHMETIC_EXPRESSION, o => o
                .WithR(ERule.EXPRESSION_ATOM).Hoist().Once()
                .ThenR(ERule.ARITHMETIC_EXPRESSION_MIDDLE).Hoist().AtLeastOnce()));
            Rule.AddRule(new Rule(ERule.ARITHMETIC_EXPRESSION, o => o
                .WithT(EToken.LEFT_PARENTHESIS).Exclude().Once()
                .ThenR(ERule.ARITHMETIC_EXPRESSION).Hoist().Once()
                .ThenT(EToken.RIGHT_PARENTHESIS).Exclude().Once()));

            Rule.AddRule(new Rule(ERule.EXPRESSION_ATOM, o => o
                .WithT(EToken.NUMBER).Once()));
            Rule.AddRule(new Rule(ERule.EXPRESSION_ATOM, o => o
                .WithT(EToken.IDENTIFIER).Once()));

            Rule.AddRule(new Rule(ERule.ARITHMETIC_EXPRESSION_MIDDLE, o => o
                .WithR(ERule.SUM).AtLeastOnce()));
            Rule.AddRule(new Rule(ERule.ARITHMETIC_EXPRESSION_MIDDLE, o => o
                .WithR(ERule.DIVIDE).AtLeastOnce()));
            Rule.AddRule(new Rule(ERule.ARITHMETIC_EXPRESSION_MIDDLE, o => o
                .WithR(ERule.MULTIPLY).AtLeastOnce()));
            Rule.AddRule(new Rule(ERule.ARITHMETIC_EXPRESSION_MIDDLE, o => o
                .WithR(ERule.DIVIDE).AtLeastOnce()));
            Rule.AddRule(new Rule(ERule.ARITHMETIC_EXPRESSION_MIDDLE, o => o
                .WithR(ERule.MODULUS).AtLeastOnce()));
            Rule.AddRule(new Rule(ERule.ARITHMETIC_EXPRESSION_MIDDLE, o => o
                .WithR(ERule.POWER).AtLeastOnce()));

            Rule.AddRule(new Rule(ERule.SUM, o => o
                    .WithT(EToken.PLUS).Exclude().Once()
                    .ThenR(ERule.EXPRESSION_ATOM).Hoist().Once()));

            Rule.AddRule(new Rule(ERule.SUBSTRACT, o => o
                    .WithT(EToken.MINUS).Exclude().Once()
                    .ThenR(ERule.EXPRESSION_ATOM).Hoist().Once()));

            Rule.AddRule(new Rule(ERule.MULTIPLY, o => o
                    .WithT(EToken.MULTIPLY).Exclude().Once()
                    .ThenR(ERule.EXPRESSION_ATOM).Hoist().Once()));

            Rule.AddRule(new Rule(ERule.DIVIDE, o => o
                    .WithT(EToken.DIVIDE).Exclude().Once()
                    .ThenR(ERule.EXPRESSION_ATOM).Hoist().Once()));

            Rule.AddRule(new Rule(ERule.MODULUS, o => o
                    .WithT(EToken.MODULUS).Exclude().Once()
                    .ThenR(ERule.EXPRESSION_ATOM).Hoist().Once()));

            Rule.AddRule(new Rule(ERule.POWER, o => o
                    .WithT(EToken.POWER).Exclude().Once()
                    .ThenR(ERule.EXPRESSION_ATOM).Hoist().Once()));

            Rule.AddRule(new Rule(ERule.FLOOR, o => o
                    .WithT(EToken.LEFT_SQUARE_BRACKET).Exclude().Once()
                    .ThenR(ERule.ARITHMETIC_EXPRESSION).Hoist().Once()
                    .ThenT(EToken.RIGHT_SQUARE_BRACKET).Exclude().Once()));
        }

        public void Parse()
        {
            List<Rule> rules = Rule.GetRules(ERule.ARITHMETIC_EXPRESSION); 
            foreach (Rule rule in rules)
            {
                Node node = rule.Evaluate(_tokens);
                if (!node.IsEmpty)
                {
                    _tree.Add(node);
                    _currentTokenIndex += rule._currentTokenIndex;
                }
                rule.Reset();
            }
        }

        public List<Node> GetTree()
        {
            return _tree;
        }
    }
}
