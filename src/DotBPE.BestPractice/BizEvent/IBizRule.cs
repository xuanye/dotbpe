using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotBPE.BestPractice
{
    public interface IBizRule
    {
        Task<BizRuleValidateResult> ValidateAsync();
    }

    public class BizRule : IBizRule
    {
        private readonly List<IBizRule> rules = new List<IBizRule>();
        public async Task<BizRuleValidateResult> ValidateAsync()
        {
            foreach (var rule in rules)
            {
                var result = await rule.ValidateAsync();
                if (result.Code != 0)
                {
                    return result;
                }
            }
            return BizRuleValidateResult.Success;
        }

        public BizRule AddBizRule(IBizRule rule)
        {
            rules.Add(rule);
            return this;
        }

    }

    public class BizRuleValidateResult
    {
        public static BizRuleValidateResult Success = new BizRuleValidateResult();
        public int Code { get; set; }

        public string Message { get; set; }
    }
}
