using AmazingCloudSearch.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmazingCloudSearch.Query.Boolean
{
    public class GroupedCondition : IBooleanCondition
    {

        private IBooleanCondition conditionA;
        private IBooleanCondition conditionB;
        private ConditionType condition;

        public GroupedCondition(IBooleanCondition conditionA, ConditionType conditionType, IBooleanCondition conditionB)
        {
            condition = conditionType;

            this.conditionA = conditionA;
            this.conditionB = conditionB;
        }

        public string GetConditionParam()
        {
            StringBuilder conds = new StringBuilder();
            conds.Append("(");
            conds.Append(GetConditionType() == ConditionType.AND ?  "and+" : "or+");
            conds.Append(conditionA.GetConditionParam());
            conds.Append("+");
            conds.Append(conditionB.GetConditionParam());
            conds.Append(")");
            return conds.ToString();
        }

        public ConditionType GetConditionType()
        {
            return condition;
        }

        public bool IsList()
        {
            return false;
        }
    }
}
