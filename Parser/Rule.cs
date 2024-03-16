using Interpreter_lib.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter_lib.Parser
{
    public enum ERule
    {
        SUM,
    }

    public class Rule : IRuleConfiguration, IRuleContinuationConfiguration, IRuleFrequencyConfiguration, IRuleTokenConfiguration, IRuleRuleConfiguration
    {
        private int _currentTokenIndex = 0;
        private EToken _currentTokenToMatch; 
        private int _frequencyMethodsPassed = 0; 
        
        private bool _isHoisted = false;
        private bool _isExcluded = false;

        private Action<IRuleConfiguration> _definition;
        private ERule _rule { get; }

        private List<Token> _tokens = new();
        private Node _tree = new();

        // TODO: More information about the exception (give the token to the exception, better message, etc.)

        public Rule(Action<IRuleConfiguration> definition)
        {
            _definition = definition;
            // Example:
            // new Rule(o => o
            //         .WithT(EToken.REPEAT).Exclude().Once()
            //         .ThenR(logicalExpressionRule).Hoist().Once()
            //      );
        } 

        public Node Evaluate(List<Token> tokens)
        {
            _tokens = tokens;
            _definition(this);

            return _tree;
        }

        private void Reset()
        {
            _currentTokenIndex = 0;
            _isHoisted = false;
            _isExcluded = false;
            _tokens.Clear();
            _tree = new();
        }

        #region WITH 

        // Match the first token in the sequence
        IRuleTokenConfiguration IRuleConfiguration.WithT(EToken token)
        {
            _isExcluded = false;
            _frequencyMethodsPassed = 0;
            _currentTokenToMatch = token;

            return this; 
        }

        // Match the first rule in the sequence
        IRuleRuleConfiguration IRuleConfiguration.WithR(Rule rule)
        {
            _isHoisted = false;
            _frequencyMethodsPassed = 0;
            _tree.Add(rule.Evaluate(_tokens)); 

            return this; 
        }

        #endregion

        #region THEN
        IRuleTokenConfiguration IRuleContinuationConfiguration.ThenT(EToken token)
        {
            _isExcluded = false; 
            _currentTokenToMatch = token;

            return this;
        }

        IRuleRuleConfiguration IRuleContinuationConfiguration.ThenR(Rule rule)
        {
            _isHoisted = false;
            if(_currentTokenIndex > 0)
                _tree.Add(rule.Evaluate(_tokens.Skip(_currentTokenIndex).ToList()));
            else 
                _tree.Add(rule.Evaluate(_tokens));

            return this; 
        }

        #endregion

        #region FREQUENCY

        // Match exactly once
        IRuleContinuationConfiguration IRuleFrequencyConfiguration.Once()
        {
            if (_tokens[_currentTokenIndex].Type == _currentTokenToMatch)
            {
                _currentTokenIndex++;
                _frequencyMethodsPassed++;

                // TODO: Add the token to the tree

                return this; 
            }

            if(_frequencyMethodsPassed > 0)
                throw new ParsingException("Token not matched.");
            
            return this; 
        }

        // Match at least once
        IRuleContinuationConfiguration IRuleFrequencyConfiguration.AtLeastOnce()
        {
            bool ok = false;
            while (_tokens[_currentTokenIndex].Type == _currentTokenToMatch)
            {
                _currentTokenToMatch++;
                // TODO: Add the token to the tree
                
                ok = true;
            }

            if(_frequencyMethodsPassed > 0 && !ok)
                throw new ParsingException("Token matched less than once.");
            

            _frequencyMethodsPassed++;
            return this; 
        }

        // Match zero or one time at most
        IRuleContinuationConfiguration IRuleFrequencyConfiguration.AtMostOnce()
        {
            if (_tokens[_currentTokenIndex].Type == _currentTokenToMatch)
            {
                _currentTokenToMatch++;
                // TODO: Add the token to the tree  

                if (_tokens[_currentTokenIndex].Type == _currentTokenToMatch && _frequencyMethodsPassed > 0)
                    throw new ParsingException("Token matched more than once.");
            }


            _frequencyMethodsPassed++;
            return this; 
        }

        // Match zero or more times
        IRuleContinuationConfiguration IRuleFrequencyConfiguration.ZeroOrMore()
        {
            while (_tokens[_currentTokenIndex].Type == _currentTokenToMatch)
            {
                _currentTokenToMatch++;

                // TODO: Add the token to the tree
            }

            _frequencyMethodsPassed++;
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
    }
}
