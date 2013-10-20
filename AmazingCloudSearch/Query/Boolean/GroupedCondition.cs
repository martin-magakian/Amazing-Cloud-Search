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

        public enum BoolOperator
        {
            _and,
            _or
        }

        public GroupedCondition(BoolOperator theBoolOperator, IBooleanCondition conditionA, IBooleanCondition conditionB)
        {
            switch (theBoolOperator)
            {
                case BoolOperator._and:
                    this.boolOperator = "and+";
                    break;
                case BoolOperator._or:
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

        public bool IsOrCondition()
        {
            return false;
        }

        public bool IsList()
        {
            return false;
        }
    }
}
