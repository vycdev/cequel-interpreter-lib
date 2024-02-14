using Interpreter_lib.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter_lib.Parser.Rules
{
    // Inspiration from:
    // https://gwerren.com/Blog/Posts/simpleCSharpParser

    internal interface IRuleConfiguration
    {
        IRuleTokenConfiguration WithT(EToken token);
        IRuleRuleConfiguration WithR(ERule rule);
    }
    
    internal interface IRuleContinuationConfiguration
    {
        IRuleTokenConfiguration ThenT(EToken token);
        IRuleRuleConfiguration ThenR(ERule rule);
    }

    internal interface IRuleFrequencyConfiguration
    {
        IRuleContinuationConfiguration Once();
        IRuleContinuationConfiguration AtMostOnce();
        IRuleContinuationConfiguration AtLeastOnce();
        IRuleContinuationConfiguration Optional();
    }

    internal interface IRuleTokenConfiguration : IRuleFrequencyConfiguration
    {
        IRuleTokenConfiguration Exclude();
    }

    internal interface IRuleRuleConfiguration : IRuleFrequencyConfiguration
    {
        IRuleTokenConfiguration Hoist();
    }

    internal class RuleDefinition : IRuleConfiguration
    {

    }

    internal class Rule
    {
        public ERule ruleType { get; private set; }


        public Rule(ERule ruleType, params Action<IRuleConfiguration>[] options)
        {
            this.ruleType = ruleType;

            // example of how it should work
            new Rule(ERule.Sum, o => o
                    .WithT(EToken.PLUS).Once()
                    .ThenR(ERule.Sum).Hoist().AtLeastOnce());
        }

    }
}
