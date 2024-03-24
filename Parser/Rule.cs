using Interpreter_lib.Tokenizer;

namespace Interpreter_lib.Parser
{
    public enum ERule
    {
        GROUP,

        // Arithmetic expressions
        ARITHMETIC_EXPRESSION,
        EXPRESSION_ATOM, ARITHMETIC_EXPRESSION_MIDDLE,

        SUM, SUBSEQUENT_SUM, SUBSTRACT, MULTIPLY, DIVIDE, POWER, MODULUS,
        FLOOR,
     
        // Logical expressions
        LOGICAL_EXPRESSION,
    }

    public class Rule : IRuleConfiguration, 
        IRuleContinuationConfiguration, 
        IRuleFrequencyConfiguration, 
        IRuleTokenConfiguration, 
        IRuleRuleConfiguration, 
        ICloneable
    {
        // Rules
        private static List<Rule> _rules = new();

        // Used for traversing the tokens array.  
        public int _currentTokenIndex { get; private set; } = 0;
        private EToken? _currentTokenToMatch;
        private ERule? _currentRuleToMatch;
        private bool _hasMatchedW;
        private bool _isWSide = false;
        private bool _isTSide = false;

        // Used for additional behavior when creating the nodes.
        private bool _isHoisted = false;
        private bool _isExcluded = false;

        // Used for defining the rule.
        public ERule _rule { get; }
        private Action<IRuleConfiguration> _definition;

        // Data from which the syntax tree is created.
        private List<Token> _tokens;
        private Node _tree;

        public Rule(ERule rule, Action<IRuleConfiguration> definition)
        {
            _definition = definition;
            _rule = rule;
            _tree = new(_rule);
            _tokens = new();
            _hasMatchedW = false;
        }

        public Node Evaluate(List<Token> tokens)
        {
            this.Reset();
            _tokens = tokens;
            _definition(this);

            return _tree;
        }

        public void Reset()
        {
            _currentTokenIndex = 0;
            _isHoisted = false;
            _isExcluded = false;
            _tree = new(_rule);
            _hasMatchedW = false;
            _isWSide = false;
            _isTSide = false;
        }

        #region WITH 

        // Match the first token in the sequence
        IRuleTokenConfiguration IRuleConfiguration.WithT(EToken token)
        {
            _isExcluded = false;
            _hasMatchedW = false;
            _currentTokenToMatch = token;
            _currentRuleToMatch = null;
            _isWSide = true;
            _isTSide = false;

            return this;
        }

        // Match the first rule in the sequence
        IRuleRuleConfiguration IRuleConfiguration.WithR(ERule rule)
        {
            _isHoisted = false;
            _hasMatchedW = false;
            _currentRuleToMatch = rule;
            _currentTokenToMatch = null;
            _isWSide = true;
            _isTSide = false;

            return this;
        }

        #endregion

        #region THEN
        IRuleTokenConfiguration IRuleContinuationConfiguration.ThenT(EToken token)
        {
            _isExcluded = false;
            _currentTokenToMatch = token;
            _currentRuleToMatch = null;
            _isWSide = false;
            _isTSide = true;

            return this;
        }

        IRuleRuleConfiguration IRuleContinuationConfiguration.ThenR(ERule rule)
        {
            _isHoisted = false;
            _currentRuleToMatch = rule;
            _currentTokenToMatch = null;
            _isWSide = false;
            _isTSide = true;

            return this;
        }

        #endregion

        #region FREQUENCY

        // Match exactly once
        IRuleContinuationConfiguration IRuleFrequencyConfiguration.Once()
        {
            if (_tokens.Count == 0)
                return this;

            if (_isTSide && !_hasMatchedW)
                return this; 

            if (_currentTokenToMatch != null && _tokens[_currentTokenIndex].Type == _currentTokenToMatch)
            {
                AddToTree(_tokens[_currentTokenIndex]);
                _currentTokenIndex++;
                
                if(_isWSide)
                    _hasMatchedW = true;
            }
            else if (_currentRuleToMatch != null)
            {
                List<Rule> currentRules = GetRules(_currentRuleToMatch.Value);
                Node node;
                var ok = false; 
                
                foreach (Rule currentRule in currentRules)
                {
                    if (_currentTokenIndex > 0)
                        node = currentRule.Evaluate(_tokens.Skip(_currentTokenIndex).ToList());
                    else
                        node = currentRule.Evaluate(_tokens);

                    if (!node.IsEmpty)
                    {
                        AddToTree(node);
                        _currentTokenIndex += currentRule._currentTokenIndex;
                        ok = true;
                        break;
                    }
                }

                if (_hasMatchedW && !ok)
                    throw new ParsingException(this, "Rule has matched less than once.");
                
                if(ok && _isWSide)
                    _hasMatchedW = true;
            }
            else if (_hasMatchedW)
            {
                throw new ParsingException(this, "Token has matched less than once.");
            }

            return this;
        }

        // Match at least once
        IRuleContinuationConfiguration IRuleFrequencyConfiguration.AtLeastOnce()
        {
            if (_tokens.Count == 0)
                return this;

            if (_isTSide && !_hasMatchedW)
                return this;

            bool ok = false;

            if (_currentTokenToMatch != null)
            {
                while (_tokens[_currentTokenIndex].Type == _currentTokenToMatch)
                {
                    AddToTree(_tokens[_currentTokenIndex]);
                    _currentTokenToMatch++;
                    ok = true;
                }

                if (_hasMatchedW && !ok)
                    throw new ParsingException(this, "Token has matched less than once.");
                
                if(ok && _isWSide)
                    _hasMatchedW = true;
            }
            else if (_currentRuleToMatch != null)
            {
                List<Rule> currentRules = GetRules(_currentRuleToMatch.Value);
                Node node;

                foreach (Rule currentRule in currentRules)
                {
                    do
                    {
                        if (_currentTokenIndex > 0)
                            node = currentRule.Evaluate(_tokens.Skip(_currentTokenIndex).ToList());
                        else
                            node = currentRule.Evaluate(_tokens);

                        if (!node.IsEmpty)
                        {
                            AddToTree(node);
                            _currentTokenIndex += currentRule._currentTokenIndex;
                            ok = true;
                        }
                    } while (!node.IsEmpty);
                }

                if (_hasMatchedW && !ok)
                    throw new ParsingException(this, "Rule has matched less than once.");
                
                if(ok && _isWSide)
                    _hasMatchedW = true;
            }

            return this;
        }

        // Match zero or one time at most
        IRuleContinuationConfiguration IRuleFrequencyConfiguration.AtMostOnce()
        {
            if (_tokens.Count == 0)
                return this;

            if (_isTSide && !_hasMatchedW)
                return this;

            if (_currentTokenToMatch != null)
            {
                if (_tokens[_currentTokenIndex].Type == _currentTokenToMatch)
                {
                    AddToTree(_tokens[_currentTokenIndex]);
                    _currentTokenToMatch++;

                    if (_tokens[_currentTokenIndex].Type == _currentTokenToMatch && _hasMatchedW)
                        throw new ParsingException(this, "Token has matched more than once.");
                }
            }
            else if (_currentRuleToMatch != null)
            {
                List<Rule> currentRules = GetRules(_currentRuleToMatch.Value);
                Node node;
                var ok = false; 
                foreach (Rule currentRule in currentRules)
                {
                    if (_currentTokenIndex > 0)
                        node = currentRule.Evaluate(_tokens.Skip(_currentTokenIndex).ToList());
                    else
                        node = currentRule.Evaluate(_tokens);

                    if (!node.IsEmpty)
                    {
                        AddToTree(node);
                        _currentTokenIndex += currentRule._currentTokenIndex;
                        ok = true;
                        break;
                    }
                }

                if(ok)
                {
                    foreach (Rule currentRule in currentRules)
                    {
                        if (_currentTokenIndex > 0)
                            node = currentRule.Evaluate(_tokens.Skip(_currentTokenIndex).ToList());
                        else
                            node = currentRule.Evaluate(_tokens);

                        if (!node.IsEmpty && _hasMatchedW)
                        {
                            throw new ParsingException(this, "Rule has matched more than once.");
                        }
                    }
                }
            }

            if(_isWSide)
                _hasMatchedW = true;

            return this;
        }

        // Match zero or more times
        IRuleContinuationConfiguration IRuleFrequencyConfiguration.ZeroOrMore()
        {
            if (_tokens.Count == 0)
                return this;

            if (_isTSide && !_hasMatchedW)
                return this;

            if (_currentTokenToMatch != null)
            {
                while (_tokens[_currentTokenIndex].Type == _currentTokenToMatch)
                {
                    AddToTree(_tokens[_currentTokenIndex]);
                    _currentTokenToMatch++;
                }
            }
            else if (_currentRuleToMatch != null)
            {
                List<Rule> currentRules = GetRules(_currentRuleToMatch.Value);
                Node node;

                foreach (Rule currentRule in currentRules)
                {
                    do
                    {
                        if (_currentTokenIndex > 0)
                            node = currentRule.Evaluate(_tokens.Skip(_currentTokenIndex).ToList());
                        else
                            node = currentRule.Evaluate(_tokens);

                        if (!node.IsEmpty)
                        {
                            AddToTree(node);
                            _currentTokenIndex += currentRule._currentTokenIndex;
                        }
                    } while (!node.IsEmpty);
                }
            }

            if(_isWSide)
                _hasMatchedW = true;

            return this;
        }

        #endregion

        #region ADDITIONAL BEHAVIOR

        // This can be applied to token elements (such as Add) and will cause that token to be matched
        // but not included in the resultant tree
        IRuleTokenConfiguration IRuleTokenConfiguration.Exclude()
        {
            _isExcluded = true;

            return this;
        }

        // This can be applied to rule elements (such as SubsequentSum) and will cause the element to be replaced
        // by it's content causing the content to be hoisted up a level
        IRuleRuleConfiguration IRuleRuleConfiguration.Hoist()
        {
            _isHoisted = true;

            return this;
        }

        #endregion

        #region TREE MANIPULATION
        private void AddToTree(Token token)
        {
            if (!_isExcluded)
                _tree.Add(token);
        }

        private void AddToTree(Node node)
        {
            if (_isHoisted)
            {
                _tree.Add(node.GetNodes());
                _tree.Add(node.GetTokens());
            }
            else
            {
                _tree.Add(node);
            }
        }
        #endregion

        #region STATIC METHODS
        public static List<Rule> GetRules()
        {
            return _rules;
        }

        public static List<Rule> GetRules(ERule rule)
        {
            return new List<Rule>(_rules.Where(r => r._rule == rule).Select(r => (Rule)r.Clone()).ToList());
        }

        public static Rule GetRule(ERule rule)
        {
            return (Rule)_rules.First(r => r._rule == rule).Clone();
        }

        public static void AddRule(Rule rule)
        {
            _rules.Add(rule);
        }

        #endregion

        #region OTHER

        public object Clone()
        {
            return new Rule(_rule, _definition);
        }

        #endregion
    }
}
