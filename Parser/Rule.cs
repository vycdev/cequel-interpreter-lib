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
        private bool _isHoisted = false;
        private bool _isExcluded = false;

        private Action<IRuleConfiguration> _definition;
        private List<Token> _tokens = new();
        private Node _tree = new();

        private bool _isMatched = true;

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
        // test
        #region WITH 

        // Match the first token in the sequence
        IRuleTokenConfiguration IRuleConfiguration.WithT(EToken token)
        {
            if (_tokens[_currentTokenIndex].Type == token)
            {
                _currentTokenIndex++;
                return this;
            }
            else
                _isMatched = false; 

            // TODO: If I dont match the first token then I want to skip the rule without throwing an exception. 

            return this; 
        }

        // Match the first rule in the sequence
        IRuleRuleConfiguration IRuleConfiguration.WithR(Action<IRuleConfiguration> configuration)
        {
            configuration(this); 

            return this; 
        }

        #endregion

        #region THEN
        IRuleRuleConfiguration IRuleContinuationConfiguration.ThenR(Action<IRuleConfiguration> configuration)
        {
            throw new NotImplementedException();
        }

        IRuleTokenConfiguration IRuleContinuationConfiguration.ThenT(EToken token)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region FREQUENCY

        // Match exactly once
        IRuleContinuationConfiguration IRuleFrequencyConfiguration.Once()
        {
            throw new NotImplementedException();
        }

        // Match at least once
        IRuleContinuationConfiguration IRuleFrequencyConfiguration.AtLeastOnce()
        {
            throw new NotImplementedException();
        }

        // Match zero or one time at most
        IRuleContinuationConfiguration IRuleFrequencyConfiguration.AtMostOnce()
        {
            throw new NotImplementedException();
        }

        // Match zero or more times
        IRuleContinuationConfiguration IRuleFrequencyConfiguration.ZeroOrMore()
        {
            throw new NotImplementedException();
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
