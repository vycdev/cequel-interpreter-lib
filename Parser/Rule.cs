﻿using Interpreter_lib.Tokenizer;
using System.Data;
using System.Numerics;
using System.Timers;

namespace Interpreter_lib.Parser
{
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
        private List<EToken> _currentTokensToMatch;
        private List<ERule> _currentRulesToMatch;
        private bool _hasMatchedW;
        private bool _isWSide = false;
        private bool _isTSide = false;
        private bool _lastToMatch = false;

        // Used for additional behavior when creating the nodes.
        private bool _isHoisted = false;
        private bool _isHoistImmune = false;
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
            _currentTokensToMatch = new();
            _currentRulesToMatch = new();
        }

        public Node Evaluate(List<Token> tokens, int? currentTokenIndex = 0)
        {
            Reset();

            if (currentTokenIndex > 0)
                _tokens = tokens.Skip(currentTokenIndex.Value).ToList();
            else
                _tokens = tokens;

            _definition(this);

            return _tree;
        }

        public void Reset()
        {
            _currentTokenIndex = 0;
            _isHoisted = false;
            _isHoistImmune = false;
            _isExcluded = false;
            _tree = new(_rule);
            _hasMatchedW = false;
            _isWSide = false;
            _isTSide = false;
            _currentTokensToMatch.Clear();
            _currentRulesToMatch.Clear();
        }

        #region WITH 

        // Match the first token in the sequence
        IRuleTokenConfiguration IRuleConfiguration.WithT(params EToken[] tokens)
        {
            _isExcluded = false;
            _hasMatchedW = false;
            _currentRulesToMatch.Clear();
            _currentTokensToMatch.Clear();
            _currentTokensToMatch.AddRange(tokens);
            _isWSide = true;
            _isTSide = false;

            return this;
        }

        // Match the first rule in the sequence
        IRuleRuleConfiguration IRuleConfiguration.WithR(params ERule[] rules)
        {
            _isHoisted = false;
            _isHoistImmune = false;
            _hasMatchedW = false;
            _currentRulesToMatch.Clear();
            _currentTokensToMatch.Clear();
            _currentRulesToMatch.AddRange(rules);
            _isWSide = true;
            _isTSide = false;

            return this;
        }

        #endregion

        #region THEN
        IRuleTokenConfiguration IRuleContinuationConfiguration.ThenT(params EToken[] token)
        {
            _isExcluded = false;
            _currentRulesToMatch.Clear();
            _currentTokensToMatch.Clear();
            _currentTokensToMatch.AddRange(token);
            _isWSide = false;
            _isTSide = true;

            return this;
        }

        IRuleRuleConfiguration IRuleContinuationConfiguration.ThenR(params ERule[] rule)
        {
            _isHoisted = false;
            _isHoistImmune = false;
            _currentRulesToMatch.Clear();
            _currentTokensToMatch.Clear();
            _currentRulesToMatch.AddRange(rule);
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

            bool hasMatchedOnce = false;

            if (_currentTokensToMatch.Count > 0)
            {
                foreach (EToken token in _currentTokensToMatch)
                {
                    if (_tokens[_currentTokenIndex].Type == token)
                    {
                        AddToTree(_tokens[_currentTokenIndex]);
                        _currentTokenIndex++;
                        hasMatchedOnce = true;
                        break;
                    }
                }
            }
            else if (_currentRulesToMatch.Count > 0)
            {
                List<Rule> currentRules = GetRules(_currentRulesToMatch);
                Node node;
                Rule currentRule;

                for (int i = 0; i < currentRules.Count; i++)
                {
                    currentRule = currentRules[i];

                    if (i == currentRules.Count - 1)
                        currentRule._lastToMatch = true;

                    node = currentRule.Evaluate(_tokens, _currentTokenIndex);

                    if (!node.IsEmpty)
                    {
                        AddToTree(node);
                        _currentTokenIndex += currentRule._currentTokenIndex;
                        hasMatchedOnce = true;
                        break;
                    }
                }

                if (_hasMatchedW && !hasMatchedOnce && _lastToMatch)
                    throw new ParsingException(this, "Rule has matched less than once.");

                if (hasMatchedOnce && _isWSide)
                    _hasMatchedW = true;
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

            bool hasMatchedAtLeastOnce = false;

            if (_currentTokensToMatch.Count > 0)
            {
                foreach (EToken token in _currentTokensToMatch)
                {
                    while (_tokens[_currentTokenIndex].Type == token)
                    {
                        AddToTree(_tokens[_currentTokenIndex]);
                        _currentTokenIndex++;
                        hasMatchedAtLeastOnce = true;
                    }

                    if (hasMatchedAtLeastOnce)
                        break;
                }
            }
            else if (_currentRulesToMatch.Count > 0)
            {
                List<Rule> currentRules = GetRules(_currentRulesToMatch);
                Node node;
                Rule currentRule;

                for (int i = 0; i < currentRules.Count; i++)
                {
                    do
                    {
                        currentRule = currentRules[i];
                        if (i == currentRules.Count - 1)
                            currentRule._lastToMatch = true;

                        node = currentRule.Evaluate(_tokens, _currentTokenIndex);

                        if (!node.IsEmpty)
                        {
                            AddToTree(node);
                            _currentTokenIndex += currentRule._currentTokenIndex;
                            hasMatchedAtLeastOnce = true;
                        }
                    } while (!node.IsEmpty);
                    
                    if (hasMatchedAtLeastOnce)
                        break;
                }
            }

            if (_hasMatchedW && !hasMatchedAtLeastOnce && _lastToMatch)
                throw new ParsingException(this, $"{(_currentRulesToMatch.Count > 0 ? "Rule" : "Token")} has matched less than once.");

            if (hasMatchedAtLeastOnce && _isWSide)
                _hasMatchedW = true;

            return this;
        }

        // Match zero or one time at most
        IRuleContinuationConfiguration IRuleFrequencyConfiguration.AtMostOnce()
        {
            if (_tokens.Count == 0)
                return this;

            if (_isTSide && !_hasMatchedW)
                return this;

            if (_currentTokensToMatch.Count > 0)
            {
                foreach (EToken token in _currentTokensToMatch)
                {
                    if (_tokens[_currentTokenIndex].Type == token)
                    {
                        AddToTree(_tokens[_currentTokenIndex]);
                        _currentTokenIndex++;
                        break;
                    }
                }
            }
            else if (_currentRulesToMatch.Count > 0)
            {
                List<Rule> currentRules = GetRules(_currentRulesToMatch);
                Node node;
                foreach (Rule currentRule in currentRules)
                {
                    node = currentRule.Evaluate(_tokens, _currentTokenIndex);

                    if (!node.IsEmpty)
                    {
                        AddToTree(node);
                        _currentTokenIndex += currentRule._currentTokenIndex;
                        break;
                    }
                }
            }

            if (_isWSide)
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

            bool hasMatchedAtLeastOnce = false;

            if (_currentTokensToMatch.Count > 0)
            {
                foreach (EToken token in _currentTokensToMatch)
                {
                    while (_tokens[_currentTokenIndex].Type == token)
                    {
                        AddToTree(_tokens[_currentTokenIndex]);
                        _currentTokenIndex++;
                        hasMatchedAtLeastOnce = true;
                    }

                    if (hasMatchedAtLeastOnce)
                        break;
                }
            }
            else if (_currentRulesToMatch.Count > 0)
            {
                List<Rule> currentRules = GetRules(_currentRulesToMatch);
                Node node;

                foreach (Rule currentRule in currentRules)
                {
                    do
                    {
                        node = currentRule.Evaluate(_tokens, _currentTokenIndex);

                        if (!node.IsEmpty)
                        {
                            AddToTree(node);
                            _currentTokenIndex += currentRule._currentTokenIndex;
                            hasMatchedAtLeastOnce = true;
                        }
                    } while (!node.IsEmpty);

                    if (hasMatchedAtLeastOnce)
                        break;
                }
            }

            if (_isWSide)
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

        IRuleRuleConfiguration IRuleRuleConfiguration.NeverHoist()
        {
            _isHoistImmune = true;

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
            if ((_isHoisted
                || node.GetRule() == _rule
                || (node.TokenCount == 1 && node.NodeCount == 1)
                || (node.TopNodeCount == 1 && node.TopTokenCount == 0))
                && !_isHoistImmune)
            {
                _tree.Add(node.GetSyntaxNodes());
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

        public static List<Rule> GetRules(List<ERule> rules)
        {
            return new List<Rule>(_rules.Where(r => rules.Contains(r._rule)).Select(r => (Rule)r.Clone()).ToList());
        }

        public static void AddRule(Rule rule)
        {
            _rules.Add(rule);
        }

        public static void AddLTRBinaryOperatorRule(ERule ruleName, ERule subsequentRuleName, ERule nextRuleName, EToken token)
        {
            AddRule(new Rule(ruleName, o => o
                .WithR(nextRuleName).Once()
                .ThenR(subsequentRuleName).Hoist().ZeroOrMore()));
            AddRule(new Rule(subsequentRuleName, o => o
                .WithT(token).Exclude().Once()
                .ThenR(nextRuleName).Once()));
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
