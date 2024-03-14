using Interpreter_lib.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter_lib.Parser
{
    internal class Rule : IRuleConfiguration, IRuleContinuationConfiguration, IRuleFrequencyConfiguration, IRuleTokenConfiguration, IRuleRuleConfiguration
    {
        private int _currentTokenIndex = 0;
        private EToken _currentTokenToMatch; 
        
        private bool _isHoisted = false;
        private bool _isExcluded = false;

        private Action<IRuleConfiguration> _definition;
        private List<Token> _tokens = new();
        private Node _tree = new();

        private bool _isMatched = false;
        private int _frequencyMethodsPassed = 0; 

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
        IRuleRuleConfiguration IRuleConfiguration.WithR(Action<IRuleConfiguration> configuration)
        {
            _isHoisted = false;
            _frequencyMethodsPassed = 0;
            configuration(this); 

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

        IRuleRuleConfiguration IRuleContinuationConfiguration.ThenR(Action<IRuleConfiguration> configuration)
        {
            _isHoisted = false;
            configuration(this);

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


            _isMatched = false; 
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

            _isMatched = ok; 
            if(_frequencyMethodsPassed > 0 && !_isMatched)
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

                if (_tokens[_currentTokenIndex].Type == _currentTokenToMatch)
                {
                    _isMatched = false;
                    if (_frequencyMethodsPassed > 0)
                        throw new ParsingException("Token matched more than once.");
                }
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
