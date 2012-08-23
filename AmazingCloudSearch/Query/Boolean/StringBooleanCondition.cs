using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AmazingCloudSearch.Query.Boolean;

namespace AmazingCloudSearch.Query.Boolean
{
    public class StringBooleanCondition : IBooleanCondition
    {
        public string Field { get; set; }
        public string Condition { get; set; }

        public StringBooleanCondition(string field, string condition)
        {
            Field = field;
            Condition = condition;
        }

        public string GetCondictionParam()
        {
            return Field + ":" + "'" + Condition + "'"; ;
        }
    }
}