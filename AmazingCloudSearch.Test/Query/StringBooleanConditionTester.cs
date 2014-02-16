using System;
using AmazingCloudSearch.Query.Boolean;
using NUnit.Framework;
using AmazingCloudSearch.Enum;

namespace AmazingCloudSearch.Test.Query
{
    [TestFixture]
    public class StringBooleanConditionTester
    {
        const string Field = "_field";
        const string Condition = "_condition";

        [Test]
        public void ShouldStartWithTheFieldName()
        {
            var stringBooleanCondition = new StringBooleanCondition(Field, Condition);
            var output = stringBooleanCondition.GetConditionParam();
            output.ShouldStartWith(Field);
        }

        [Test]
        public void ShouldUrlEscapeAColonAfterTheField()
        {            
            var stringBooleanCondition = new StringBooleanCondition(Field, Condition);
            var output = stringBooleanCondition.GetConditionParam();
            output.ShouldStartWith(string.Format("{0}{1}", Field, Uri.EscapeDataString(":")));            
        }

        [Test]
        public void ShouldUrlEscapeTheValue()
        {            
            var stringBooleanCondition = new StringBooleanCondition(Field, " & ");
            var output = stringBooleanCondition.GetConditionParam();
            output.ShouldEndWith(string.Format("'%20%26%20'"));
        }

        [Test]
        public void ShouldEncloseTheConditionInQuotes()
        {            
            var stringBooleanCondition = new StringBooleanCondition(Field, Condition);
            var output = stringBooleanCondition.GetConditionParam();
            output.ShouldEndWith(string.Format("'{0}'", Condition));
        }

        [Test]
        public void FormatShouldBeFieldColonQuoteConditionQuote()
        {
            var stringBooleanCondition = new StringBooleanCondition(Field, Condition);
            var output = stringBooleanCondition.GetConditionParam();
            output.ShouldEndWith(string.Format("{0}{1}'{2}'", Field, Uri.EscapeDataString(":"), Condition));            
        }

        [Test]
        public void NegatedFormatShouldBeWrappedWithNot()
        {
            var stringBooleanCondition = new StringBooleanCondition(Field, Condition, true);
            var output = stringBooleanCondition.GetConditionParam();
            output.ShouldEndWith(string.Format("(not+{0}{1}'{2}')", Field, Uri.EscapeDataString(":"), Condition));
        }

        [Test]
        public void NullFieldShouldNotIncludeColon()
        {
            var stringBooleanCondition = new StringBooleanCondition(null, Condition);
            var output = stringBooleanCondition.GetConditionParam();
            output.ShouldEqual(string.Format("'{0}'", Condition));
        }

        [Test]
        public void IsOrConditionShouldBeFalse()
        {
            new StringBooleanCondition(Field, Condition).GetConditionType().ShouldEqual(ConditionType.AND);
        }

        [Test]
        public void IsListShouldBeFalse()
        {
            new StringBooleanCondition(Field, Condition).IsList().ShouldBeFalse();
        }
    }
}