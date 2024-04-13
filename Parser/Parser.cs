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
                .WithT(EToken.END_OF_LINE).Exclude().ZeroOrMore()
                .ThenR(ERule.STATEMENT).ZeroOrMore()
                .ThenT(EToken.END_OF_FILE).Exclude().Once()));

            // STATEMENT
            Rule.AddRule(new Rule(ERule.STATEMENT, o => o
                .WithR(
                    ERule.EXPRESSION, 
                    ERule.SIMPLE_CONDITIONAL
                ).NeverHoist().Once()
                .ThenT(EToken.END_OF_LINE).Exclude().AtLeastOnce()));          
            
            // SIMPLE_CONDITIONAL
            Rule.AddRule(new Rule(ERule.SIMPLE_CONDITIONAL, o => o
                .WithT(EToken.IF).Exclude().Once()));

            // PRINT
            Rule.AddRule(new Rule(ERule.PRINT, o => o
                .WithT(EToken.WRITE).Exclude().Once()
                .ThenR(ERule.EXPRESSION).NeverHoist().Once()));

            // Expression
            Rule.AddRule(new Rule(ERule.EXPRESSION, o => o
                .WithR(ERule.LOGICAL_OR).Once()));

            // LOGICAL_OR, SUBSEQUENT_LOGICAL_OR,
            Rule.AddLTRBinaryOperatorRule(ERule.LOGICAL_OR, ERule.SUBSEQUENT_LOGICAL_OR, ERule.LOGICAL_AND, EToken.OR);

            // LOGICAL_AND, SUBSEQUENT_LOGICAL_AND,
            Rule.AddLTRBinaryOperatorRule(ERule.LOGICAL_AND, ERule.SUBSEQUENT_LOGICAL_AND, ERule.BITWISE_OR, EToken.AND);

            // BITWISE_OR, SUBSEQUENT_BITWISE_OR,
            Rule.AddLTRBinaryOperatorRule(ERule.BITWISE_OR, ERule.SUBSEQUENT_BITWISE_OR, ERule.BITWISE_XOR, EToken.BITWISE_OR);

            // BITWISE_XOR, SUBSEQUENT_BITWISE_XOR,
            Rule.AddLTRBinaryOperatorRule(ERule.BITWISE_XOR, ERule.SUBSEQUENT_BITWISE_XOR, ERule.BITWISE_AND, EToken.BITWISE_XOR);

            // BITWISE_AND, SUBSEQUENT_BITWISE_AND,
            Rule.AddLTRBinaryOperatorRule(ERule.BITWISE_AND, ERule.SUBSEQUENT_BITWISE_AND, ERule.NOT_EQUAL, EToken.BITWISE_AND);

            // NOT_EQUAL, SUBSEQUENT_NOT_EQUAL,
            Rule.AddLTRBinaryOperatorRule(ERule.NOT_EQUAL, ERule.SUBSEQUENT_NOT_EQUAL, ERule.EQUAL, EToken.NOT_EQUAL);

            // EQUAL, SUBSEQUENT_EQUAL,
            Rule.AddLTRBinaryOperatorRule(ERule.EQUAL, ERule.SUBSEQUENT_EQUAL, ERule.LESS_THAN, EToken.EQUAL);

            // LESS_THAN, SUBSEQUENT_LESS_THAN,
            Rule.AddLTRBinaryOperatorRule(ERule.LESS_THAN, ERule.SUBSEQUENT_LESS_THAN, ERule.LESS_THAN_EQUAL, EToken.LESS_THAN);
            
            // LESS_THAN_EQUAL, SUBSEQUENT_LESS_THAN_EQUAL,
            Rule.AddLTRBinaryOperatorRule(ERule.LESS_THAN_EQUAL, ERule.SUBSEQUENT_LESS_THAN_EQUAL, ERule.GREATER_THAN, EToken.LESS_THAN_EQUAL);

            // GREATER_THAN, SUBSEQUENT_GREATER_THAN,
            Rule.AddLTRBinaryOperatorRule(ERule.GREATER_THAN, ERule.SUBSEQUENT_GREATER_THAN, ERule.GREATER_THAN_EQUAL, EToken.GREATER_THAN);

            // GREATER_THAN_EQUAL, SUBSEQUENT_GREATER_THAN_EQUAL,
            Rule.AddLTRBinaryOperatorRule(ERule.GREATER_THAN_EQUAL, ERule.SUBSEQUENT_GREATER_THAN_EQUAL, ERule.BITWISE_LEFT_SHIFT, EToken.GREATER_THAN_EQUAL);
            
            // BITWISE_LEFT_SHIFT, SUBSEQUENT_BITWISE_LEFT_SHIFT,
            Rule.AddLTRBinaryOperatorRule(ERule.BITWISE_LEFT_SHIFT, ERule.SUBSEQUENT_BITWISE_LEFT_SHIFT, ERule.BITWISE_RIGHT_SHIFT, EToken.BITWISE_LEFT_SHIFT);

            // BITWISE_RIGHT_SHIFT, SUBSEQUENT_BITWISE_RIGHT_SHIFT,
            Rule.AddLTRBinaryOperatorRule(ERule.BITWISE_RIGHT_SHIFT, ERule.SUBSEQUENT_BITWISE_RIGHT_SHIFT, ERule.SUM, EToken.BITWISE_RIGHT_SHIFT);

            // SUM, SUBSEQUENT_SUM, 
            Rule.AddLTRBinaryOperatorRule(ERule.SUM, ERule.SUBSEQUENT_SUM, ERule.SUBTRACT, EToken.PLUS);

            // SUBTRACT, SUBSEQUENT_SUBTRACT, 
            Rule.AddLTRBinaryOperatorRule(ERule.SUBTRACT, ERule.SUBSEQUENT_SUBTRACT, ERule.MULTIPLY, EToken.MINUS);

            // MULTIPLY, SUBSEQUENT_MULTIPLY, 
            Rule.AddLTRBinaryOperatorRule(ERule.MULTIPLY, ERule.SUBSEQUENT_MULTIPLY, ERule.DIVIDE, EToken.MULTIPLY);

            // DIVIDE, SUBSEQUENT_DIVIDE,
            Rule.AddLTRBinaryOperatorRule(ERule.DIVIDE, ERule.SUBSEQUENT_DIVIDE, ERule.MODULUS, EToken.DIVIDE);

            // MODULUS, SUBSEQUENT_MODULUS,
            Rule.AddLTRBinaryOperatorRule(ERule.MODULUS, ERule.SUBSEQUENT_MODULUS, ERule.POWER, EToken.MODULUS);

            // POWER, SUBSEQUENT_POWER,
            Rule.AddLTRBinaryOperatorRule(ERule.POWER, ERule.SUBSEQUENT_POWER, ERule.UNARY_MINUS, EToken.POWER);

            // UNARY
            //Rule.AddRule(new Rule(ERule.UNARY_MINUS, o => o
            //    .WithT(EToken.MINUS).Once()));
            //Rule.AddRule(new Rule(ERule.UNARY_PLUS, o => o
            //    .WithT(EToken.PLUS).Once()));
            //Rule.AddRule(new Rule(ERule.UNARY_NOT, o => o
            //    .WithT(EToken.NOT).Once()));
            //Rule.AddRule(new Rule(ERule.UNARY_BITWISE_NOT, o => o
            //    .WithT(EToken.BITWISE_NOT).Once()));

            //Rule.AddRule(new Rule(ERule.UNARY, o => o
            //    .WithR(ERule.UNARY_MINUS, ERule.UNARY_PLUS, ERule.UNARY_BITWISE_NOT, ERule.UNARY_NOT).NeverHoist().Once()
            //    .ThenR(ERule.UNARY).Once()));
            //Rule.AddRule(new Rule(ERule.UNARY, o => o
            //    .WithR(ERule.PRIMARY).Once()));

            //Rule.AddRule(new Rule(ERule.UNARY, o => o
            //    .WithT(EToken.MINUS, EToken.PLUS, EToken.NOT, EToken.BITWISE_NOT).Once()
            //    .ThenR(ERule.UNARY).NeverHoist().Once()));
            //Rule.AddRule(new Rule(ERule.UNARY, o => o
            //    .WithR(ERule.PRIMARY).Once())); 

            // UNARY_MINUS
            Rule.AddRTLUnaryOperator(ERule.UNARY_MINUS, ERule.UNARY_PLUS, EToken.MINUS);

            // UNARY_PLUS,
            Rule.AddRTLUnaryOperator(ERule.UNARY_PLUS, ERule.UNARY_NOT, EToken.PLUS);

            // UNARY_NOT,
            Rule.AddRTLUnaryOperator(ERule.UNARY_NOT, ERule.UNARY_BITWISE_NOT, EToken.NOT);

            // UNARY_BITWISE_NOT,
            Rule.AddRTLUnaryOperator(ERule.UNARY_BITWISE_NOT, ERule.PRIMARY, EToken.BITWISE_NOT);

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
