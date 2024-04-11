using Interpreter_lib.Tokenizer;

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
                .WithR(ERule.ADDITIVE).Once()));

            // Binary Operations 
            Rule.AddRule(new Rule(ERule.ADDITIVE, o => o
                .WithR(ERule.MULTIPLICATIVE).Once()
                .ThenR(ERule.SUBSEQUENT_ADDITIVE).Hoist().ZeroOrMore()));
            Rule.AddRule(new Rule(ERule.SUBSEQUENT_ADDITIVE, o => o
                .WithT(EToken.PLUS, EToken.MINUS).Once()
                .ThenR(ERule.MULTIPLICATIVE).Once()));

            Rule.AddRule(new Rule(ERule.MULTIPLICATIVE, o => o
                .WithR(ERule.EXPONENTIAL).Once()
                .ThenR(ERule.SUBSEQUENT_MULTIPLICATIVE).Hoist().ZeroOrMore()));
            Rule.AddRule(new Rule(ERule.SUBSEQUENT_MULTIPLICATIVE, o => o
                .WithT(EToken.MULTIPLY, EToken.DIVIDE, EToken.MODULUS).Once()
                .ThenR(ERule.EXPONENTIAL).Once()));

            Rule.AddRule(new Rule(ERule.EXPONENTIAL, o => o
                .WithR(ERule.PRIMARY).Hoist().Once()
                .ThenR(ERule.SUBSEQUENT_EXPONENTIAL).Hoist().ZeroOrMore()));
            Rule.AddRule(new Rule(ERule.SUBSEQUENT_EXPONENTIAL, o => o
                .WithT(EToken.POWER).Once()
                .ThenR(ERule.PRIMARY).Hoist().Once()));

            // Unary Operations
            Rule.AddRule(new Rule(ERule.FLOOR, o => o
                .WithT(EToken.LEFT_SQUARE_BRACKET).Exclude().Once()
                .ThenR(ERule.EXPRESSION).Once()
                .ThenT(EToken.RIGHT_SQUARE_BRACKET).Exclude().Once()));
        }

        public void Parse()
        {
            List<Rule> rules = Rule.GetRules(ERule.EXPRESSION);
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
