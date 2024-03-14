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
        public long _currentTokenIndex = 0;
        public bool _isHoisted = false;
        public bool _isExcluded = false;

        private Action<IRuleConfiguration> _definition;
        private List<Token> _tokens = new();

        public Rule(Action<IRuleConfiguration> definition)
        {
            _definition = definition;
            // Example:
            // new Rule(o => o
            //         .WithT(EToken.REPEAT).Once()
            //         .GotoT(EToken.UNTIL).Once()
            //         .ThenR(logicalExpressionRule)
            //      );
        } 

        public void Evaluate(List<Token> tokens)
        {
            _tokens = tokens;
            _definition(this);
        }

        private void Reset()
        {
            _currentTokenIndex = 0;
            _isHoisted = false;
            _isExcluded = false;
            _tokens.Clear();
        }

        #region WITH 

        // Match the first token in the sequence
        IRuleTokenConfiguration IRuleConfiguration.WithT(EToken token)
        {
            throw new NotImplementedException();
        }

        // Match the first rule in the sequence
        IRuleRuleConfiguration IRuleConfiguration.WithR(Action<IRuleConfiguration> configuration)
        {
            throw new NotImplementedException();
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

        #region GOTO
        
        // Skip tokens until matching the token 
        IRuleTokenConfiguration IRuleContinuationConfiguration.GotoT(EToken token)
        {
            throw new NotImplementedException();
        }

        // Skip tokens until matching the rule
        IRuleRuleConfiguration IRuleContinuationConfiguration.GotoR(Action<IRuleConfiguration> configuration)
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
