using Interpreter_lib.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
            //Rule.AddRule(new Rule(ERule.ARITHMETIC_EXPRESSION, o => o
            //    .WithR(ERule.EXPRESSION_ATOM).Once()
            //    .ThenR(ERule.SUBSEQUENT_EXPRESSION).AtLeastOnce()));

            // Grouping
            Rule.AddRule(new Rule(ERule.GROUP, o => o
                .WithT(EToken.LEFT_PARENTHESIS).Exclude().Once()
                .ThenR(ERule.ARITHMETIC_EXPRESSION).Once()
                .ThenT(EToken.RIGHT_PARENTHESIS).Exclude().Once()));

            // Atoms
            Rule.AddRule(new Rule(ERule.EXPRESSION_ATOM, o => o
                .WithT(EToken.NUMBER).Once()));
            Rule.AddRule(new Rule(ERule.EXPRESSION_ATOM, o => o
                .WithT(EToken.IDENTIFIER).Once()));
            Rule.AddRule(new Rule(ERule.EXPRESSION_ATOM, o => o
                .WithR(ERule.GROUP).Hoist().Once()));            
            Rule.AddRule(new Rule(ERule.EXPRESSION_ATOM, o => o
                .WithR(ERule.FLOOR).Once()));

            // Arithmetic expression
            //Rule.AddRule(new Rule(ERule.ARITHMETIC_EXPRESSION, o => o
            //    .WithR(ERule.SUM, ERule.SUBTRACT, ERule.MULTIPLY, ERule.DIVIDE, ERule.POWER, ERule.MODULUS).Once()));

            Rule.AddRule(new Rule(ERule.ARITHMETIC_EXPRESSION, o => o
                    .WithR(ERule.EXPRESSION_ATOM).Hoist().Once()
                    .ThenR(ERule.SUBSEQUENT_SUM,
                            ERule.SUBSEQUENT_SUBTRACT,
                            ERule.SUBSEQUENT_MULTIPLY,
                            ERule.SUBSEQUENT_DIVIDE,
                            ERule.SUBSEQUENT_POWER,
                            ERule.MODULUS).AtLeastOnce()));

            // Expressions
            Rule.AddRule(new Rule(ERule.SUM, o => o
                    .WithR(ERule.EXPRESSION_ATOM).Hoist().Once()
                    .ThenR(ERule.SUBSEQUENT_SUM).Hoist().AtLeastOnce()));

            Rule.AddRule(new Rule(ERule.SUBTRACT, o => o
                    .WithR(ERule.EXPRESSION_ATOM).Hoist().Once()
                    .ThenR(ERule.SUBSEQUENT_SUBTRACT).Hoist().AtLeastOnce()));
            
            Rule.AddRule(new Rule(ERule.MULTIPLY, o => o
                    .WithR(ERule.EXPRESSION_ATOM).Hoist().Once()
                    .ThenR(ERule.SUBSEQUENT_MULTIPLY).Hoist().AtLeastOnce()));
            
            Rule.AddRule(new Rule(ERule.DIVIDE, o => o
                    .WithR(ERule.EXPRESSION_ATOM).Hoist().Once()
                    .ThenR(ERule.SUBSEQUENT_DIVIDE).Hoist().AtLeastOnce()));
            
            Rule.AddRule(new Rule(ERule.POWER, o => o
                    .WithR(ERule.EXPRESSION_ATOM).Hoist().Once()
                    .ThenR(ERule.SUBSEQUENT_POWER).Hoist().AtLeastOnce()));
            
            Rule.AddRule(new Rule(ERule.MODULUS, o => o
                    .WithR(ERule.EXPRESSION_ATOM).Hoist().Once()
                    .ThenR(ERule.SUBSEQUENT_MODULUS).Hoist().AtLeastOnce()));

            Rule.AddRule(new Rule(ERule.FLOOR, o => o
                    .WithT(EToken.LEFT_SQUARE_BRACKET).Exclude().Once()
                    .ThenR(ERule.ARITHMETIC_EXPRESSION).Hoist().Once()
                    .ThenT(EToken.RIGHT_SQUARE_BRACKET).Exclude().Once()));

            // Subsequent expressions
            Rule.AddRule(new Rule(ERule.SUBSEQUENT_SUM, o => o
                    .WithT(EToken.PLUS).Exclude().Once()
                    .ThenR(ERule.EXPRESSION_ATOM).Hoist().Once()));

            Rule.AddRule(new Rule(ERule.SUBSEQUENT_SUBTRACT, o => o
                    .WithT(EToken.MINUS).Exclude().Once()
                    .ThenR(ERule.EXPRESSION_ATOM).Hoist().Once()));

            Rule.AddRule(new Rule(ERule.SUBSEQUENT_MULTIPLY, o => o
                    .WithT(EToken.MULTIPLY).Exclude().Once()
                    .ThenR(ERule.EXPRESSION_ATOM).Hoist().Once()));

            Rule.AddRule(new Rule(ERule.SUBSEQUENT_DIVIDE, o => o
                    .WithT(EToken.DIVIDE).Exclude().Once()
                    .ThenR(ERule.EXPRESSION_ATOM).Hoist().Once()));

            Rule.AddRule(new Rule(ERule.SUBSEQUENT_MODULUS, o => o
                    .WithT(EToken.MODULUS).Exclude().Once()
                    .ThenR(ERule.EXPRESSION_ATOM).Hoist().Once()));

            Rule.AddRule(new Rule(ERule.SUBSEQUENT_POWER, o => o
                    .WithT(EToken.POWER).Exclude().Once()
                    .ThenR(ERule.EXPRESSION_ATOM).Hoist().Once()));
        }

        public void Parse()
        {
            List<Rule> rules = Rule.GetRules(ERule.ARITHMETIC_EXPRESSION);
            Node node;
            foreach (Rule rule in rules)
            {
                if (_currentTokenIndex > 0)
                    node = rule.Evaluate(_tokens.Skip(_currentTokenIndex).ToList());
                else
                    node = rule.Evaluate(_tokens);

                if (!node.IsEmpty)
                {
                    _tree.Add(node);
                    _currentTokenIndex += rule._currentTokenIndex;
                }
            }
        }

        public List<Node> GetTree()
        {
            return _tree;
        }
    }
}
