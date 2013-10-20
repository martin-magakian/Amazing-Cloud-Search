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
        private string boolOperator;

        public GroupedCondition(IBooleanCondition conditionA, ConditionType conditionType, IBooleanCondition conditionB)
        {
            switch (conditionType)
            {
                case ConditionType.AND:
                    this.boolOperator = "and+";
                    break;
                case ConditionType.OR:
                    this.boolOperator = "or+";
                    break;
                default:
                    break;
            }
            this.conditionA = conditionA;
            this.conditionB = conditionB;
        }

        public string GetConditionParam()
        {
            StringBuilder conds = new StringBuilder();
            conds.Append("(");
            conds.Append(boolOperator);
            conds.Append(conditionA.GetConditionParam());
            conds.Append("+");
            conds.Append(conditionB.GetConditionParam());
            conds.Append(")");
            return conds.ToString();
        }

        public ConditionType GetConditionType()
        {
            return ConditionType.AND;
        }

        public bool IsList()
        {
            return false;
        }
    }
}
