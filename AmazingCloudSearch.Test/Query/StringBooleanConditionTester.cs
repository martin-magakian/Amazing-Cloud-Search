using System;
using AmazingCloudSearch.Query.Boolean;
using NUnit.Framework;
using AmazingCloudSearch.Enum;

namespace AmazingCloudSearch.Test.Query
{
    [TestFixture]
    public class StringBooleanConditionTester
    {
        string _field = "_field";
        string _condition = "_condition";

        [Test]
        public void ShouldStartWithTheFieldName()
        {
            var stringBooleanCondition = new StringBooleanCondition(_field, _condition);
            var output = stringBooleanCondition.GetConditionParam();
            output.ShouldStartWith(_field);
        }

        [Test]
        public void ShouldUrlEscapeAColonAfterTheField()
        {            
            var stringBooleanCondition = new StringBooleanCondition(_field, _condition);
            var output = stringBooleanCondition.GetConditionParam();
            output.ShouldStartWith(string.Format("{0}{1}", _field, Uri.EscapeDataString(":")));            
        }

        [Test]
        public void ShouldEncloseTheConditionInQuotes()
        {            
            var stringBooleanCondition = new StringBooleanCondition(_field, _condition);
            var output = stringBooleanCondition.GetConditionParam();
            output.ShouldEndWith(string.Format("'{0}'", _condition));
        }

        [Test]
        public void FormatShouldBeFieldColonQuoteConditionQuote()
        {            
            var stringBooleanCondition = new StringBooleanCondition(_field, _condition);
            var output = stringBooleanCondition.GetConditionParam();
            output.ShouldEndWith(string.Format("{0}{1}'{2}'", _field, Uri.EscapeDataString(":"), _condition));            
        }

        [Test]
        public void IsOrConditionShouldBeFalse()
        {
            new StringBooleanCondition(_field, _condition).GetConditionType().ShouldEqual(ConditionType.AND);
        }

        [Test]
        public void IsListShouldBeFalse()
        {
            new StringBooleanCondition(_field, _condition).IsList().ShouldBeFalse();
        }
    }
}