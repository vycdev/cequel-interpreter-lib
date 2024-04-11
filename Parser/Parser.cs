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

            // Expression
            Rule.AddRule(new Rule(ERule.EXPRESSION, o => o
                .WithR(ERule.SUM).Once()));

            // SUM, SUBSEQUENT_SUM, 
            Rule.AddRule(new Rule(ERule.SUM, o => o
                .WithR(ERule.SUBTRACT).Once()
                .ThenR(ERule.SUBSEQUENT_SUM).Hoist().ZeroOrMore()));
            Rule.AddRule(new Rule(ERule.SUBSEQUENT_SUM, o => o
                .WithT(EToken.PLUS).Exclude().Once()
                .ThenR(ERule.SUBTRACT).Once()));
            // SUBTRACT, SUBSEQUENT_SUBTRACT, 
            Rule.AddRule(new Rule(ERule.SUBTRACT, o => o
                .WithR(ERule.MULTIPLY).Once()
                .ThenR(ERule.SUBSEQUENT_MULTIPLY).Hoist().ZeroOrMore()));
            Rule.AddRule(new Rule(ERule.SUBSEQUENT_MULTIPLY, o => o
                .WithT(EToken.MINUS).Exclude().Once()
                .ThenR(ERule.MULTIPLY).Once()));
            // MULTIPLY, SUBSEQUENT_MULTIPLY, 
            Rule.AddRule(new Rule(ERule.MULTIPLY, o => o
                .WithR(ERule.DIVIDE).Once()
                .ThenR(ERule.SUBSEQUENT_MULTIPLY).Hoist().ZeroOrMore()));
            Rule.AddRule(new Rule(ERule.SUBSEQUENT_MULTIPLY, o => o
                .WithT(EToken.MULTIPLY).Exclude().Once()
                .ThenR(ERule.DIVIDE).Once()));
            // DIVIDE, SUBSEQUENT_DIVIDE,
            Rule.AddRule(new Rule(ERule.DIVIDE, o => o
                .WithR(ERule.MODULUS).Once()
                .ThenR(ERule.SUBSEQUENT_DIVIDE).Hoist().ZeroOrMore()));
            Rule.AddRule(new Rule(ERule.SUBSEQUENT_DIVIDE, o => o
                .WithT(EToken.DIVIDE).Exclude().Once()
                .ThenR(ERule.MODULUS).Once()));
            // MODULUS, SUBSEQUENT_MODULUS,
            Rule.AddRule(new Rule(ERule.MODULUS, o => o
                .WithR(ERule.POWER).Once()
                .ThenR(ERule.SUBSEQUENT_MODULUS).Hoist().ZeroOrMore()));
            Rule.AddRule(new Rule(ERule.SUBSEQUENT_MODULUS, o => o
                .WithT(EToken.MODULUS).Exclude().Once()
                .ThenR(ERule.POWER).Once()));
            // POWER, SUBSEQUENT_POWER,
            Rule.AddRule(new Rule(ERule.POWER, o => o
                .WithR(ERule.PRIMARY).Once()
                .ThenR(ERule.SUBSEQUENT_POWER).Hoist().ZeroOrMore()));
            Rule.AddRule(new Rule(ERule.SUBSEQUENT_POWER, o => o
                .WithT(EToken.POWER).Exclude().Once()
                .ThenR(ERule.PRIMARY).Once()));

            // Unary Operations
            Rule.AddRule(new Rule(ERule.FLOOR, o => o
                .WithT(EToken.LEFT_SQUARE_BRACKET).Exclude().Once()
                .ThenR(ERule.EXPRESSION).NeverHoist().Once()
                .ThenT(EToken.RIGHT_SQUARE_BRACKET).Exclude().Once()));
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
