using Interpreter_lib.Tokenizer;
using System.Reflection;

namespace Interpreter_lib.Parser
{
    public class Parser
    {
        // Used for traversing the tokens array.  
        private int _currentTokenIndex = 0;

        // Data from which the syntax tree is created.
        private List<Token> _tokens;
        private Node _tree;

        private const ERule _rootRule = ERule.ROOT;

        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
            _tree = new Node(_rootRule);

            // ROOT 
            Rule.AddRule(new Rule(ERule.ROOT, o => o
                .WithR(ERule.EXPRESSION).NeverHoist().ZeroOrMore()));

            // Expression
            Rule.AddRule(new Rule(ERule.EXPRESSION, o => o
                .WithR(ERule.LESS_THAN).Once()));

            
            // LESS_THAN, SUBSEQUENT_LESS_THAN,
            Rule.AddBinaryOperatorRule(ERule.LESS_THAN, ERule.SUBSEQUENT_LESS_THAN, ERule.LESS_THAN_EQUAL, EToken.LESS_THAN);
            
            // LESS_THAN_EQUAL, SUBSEQUENT_LESS_THAN_EQUAL,
            Rule.AddBinaryOperatorRule(ERule.LESS_THAN_EQUAL, ERule.SUBSEQUENT_LESS_THAN_EQUAL, ERule.GREATER_THAN, EToken.LESS_THAN_EQUAL);

            // GREATER_THAN, SUBSEQUENT_GREATER_THAN,
            Rule.AddBinaryOperatorRule(ERule.GREATER_THAN, ERule.SUBSEQUENT_GREATER_THAN, ERule.GREATER_THAN_EQUAL, EToken.GREATER_THAN);

            // GREATER_THAN_EQUAL, SUBSEQUENT_GREATER_THAN_EQUAL,
            Rule.AddBinaryOperatorRule(ERule.GREATER_THAN_EQUAL, ERule.SUBSEQUENT_GREATER_THAN_EQUAL, ERule.BITWISE_LEFT_SHIFT, EToken.GREATER_THAN_EQUAL);
            
            // BITWISE_LEFT_SHIFT, SUBSEQUENT_BITWISE_LEFT_SHIFT,
            Rule.AddBinaryOperatorRule(ERule.BITWISE_LEFT_SHIFT, ERule.SUBSEQUENT_BITWISE_LEFT_SHIFT, ERule.BITWISE_RIGHT_SHIFT, EToken.BITWISE_LEFT_SHIFT);

            // BITWISE_RIGHT_SHIFT, SUBSEQUENT_BITWISE_RIGHT_SHIFT,
            Rule.AddBinaryOperatorRule(ERule.BITWISE_RIGHT_SHIFT, ERule.SUBSEQUENT_BITWISE_RIGHT_SHIFT, ERule.SUM, EToken.BITWISE_RIGHT_SHIFT);

            // SUM, SUBSEQUENT_SUM, 
            Rule.AddBinaryOperatorRule(ERule.SUM, ERule.SUBSEQUENT_SUM, ERule.SUBTRACT, EToken.PLUS);

            // SUBTRACT, SUBSEQUENT_SUBTRACT, 
            Rule.AddBinaryOperatorRule(ERule.SUBTRACT, ERule.SUBSEQUENT_SUBTRACT, ERule.MULTIPLY, EToken.MINUS);

            // MULTIPLY, SUBSEQUENT_MULTIPLY, 
            Rule.AddBinaryOperatorRule(ERule.MULTIPLY, ERule.SUBSEQUENT_MULTIPLY, ERule.DIVIDE, EToken.MULTIPLY);

            // DIVIDE, SUBSEQUENT_DIVIDE,
            Rule.AddBinaryOperatorRule(ERule.DIVIDE, ERule.SUBSEQUENT_DIVIDE, ERule.MODULUS, EToken.DIVIDE);

            // MODULUS, SUBSEQUENT_MODULUS,
            Rule.AddBinaryOperatorRule(ERule.MODULUS, ERule.SUBSEQUENT_MODULUS, ERule.POWER, EToken.MODULUS);

            // POWER, SUBSEQUENT_POWER,
            Rule.AddBinaryOperatorRule(ERule.POWER, ERule.SUBSEQUENT_POWER, ERule.PRIMARY, EToken.POWER);


            // FLOOR
            Rule.AddRule(new Rule(ERule.FLOOR, o => o
                .WithT(EToken.LEFT_SQUARE_BRACKET).Exclude().Once()
                .ThenR(ERule.EXPRESSION).NeverHoist().Once()
                .ThenT(EToken.RIGHT_SQUARE_BRACKET).Exclude().Once()));
            
            // Grouping
            Rule.AddRule(new Rule(ERule.GROUP, o => o
                .WithT(EToken.LEFT_PARENTHESIS).Exclude().Once()
                .ThenR(ERule.EXPRESSION).NeverHoist().Once()
                .ThenT(EToken.RIGHT_PARENTHESIS).Exclude().Once()));

            // Primary
            Rule.AddRule(new Rule(ERule.PRIMARY, o => o
                .WithT(EToken.NUMBER).Once()));
            Rule.AddRule(new Rule(ERule.PRIMARY, o => o
                .WithT(EToken.IDENTIFIER).Once()));
            Rule.AddRule(new Rule(ERule.PRIMARY, o => o
                .WithR(ERule.GROUP).Hoist().Once()));            
            Rule.AddRule(new Rule(ERule.PRIMARY, o => o
                .WithR(ERule.FLOOR).NeverHoist().Once()));
        }

        public void Parse()
        {
            List<Rule> rules = Rule.GetRules(_rootRule);
            Node node;

            foreach (Rule rule in rules)
            {
                if (_currentTokenIndex > 0)
                    node = rule.Evaluate(_tokens.Skip(_currentTokenIndex).ToList());
                else
                    node = rule.Evaluate(_tokens);

                if(_tree.IsEmpty && !node.IsEmpty)
                {
                    _tree = node;
                    _currentTokenIndex += rule._currentTokenIndex;
                } 
                else if (!node.IsEmpty)
                {
                    _tree.Add(node);
                    _currentTokenIndex += rule._currentTokenIndex;
                }
            }
        }

        public Node GetTree()
        {
            return _tree;
        }
    }
}
