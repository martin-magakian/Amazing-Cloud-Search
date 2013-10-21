using System.Collections.Generic;
using System.Text;
using AmazingCloudSearch.Builder;
using AmazingCloudSearch.Query;
using AmazingCloudSearch.Query.Boolean;
using NUnit.Framework;
using Rhino.Mocks;
using AmazingCloudSearch.Enum;

namespace AmazingCloudSearch.Test.Builder
{
    [TestFixture]
    public class WhenFeedingAKeyword : InteractionContext<QueryBuilder<Movie>>
    {
        QueryBuilder<Movie> _queryBuilder;

        protected override void beforeEach()
        {
            _queryBuilder = new QueryBuilder<Movie>("");
        }

        [Test]
        public void ShouldAppendQToTheUrl()
        {
            var url = new StringBuilder("foo");
            _queryBuilder.FeedKeyword("testKeyword", url);
            url.ToString().ShouldStartWith("fooq=");
        }

        [Test]
        public void ShouldAppendTheKeyword()
        {
            var url = new StringBuilder();
            _queryBuilder.FeedKeyword("testKeyword", url);
            url.ToString().ShouldStartWith("q=testKeyword");
        }

        [Test]
        public void ShouldEscapeTheKeyword()
        {
            var url = new StringBuilder();
            _queryBuilder.FeedKeyword("@", url);
            url.ToString().ShouldStartWith("q=%40");        
        }

        [Test]
        public void ShouldNotDoAnythingIfTheKeywordIsEmpty()
        {
            var url = new StringBuilder("test");
            _queryBuilder.FeedKeyword(string.Empty, url);
            url.ToString().ShouldEqual("test");
        }

        [Test]
        public void ShouldNotDoAnythingIfTheKeywordIsNull()
        {
            var url = new StringBuilder("test");
            _queryBuilder.FeedKeyword(null, url);
            url.ToString().ShouldEqual("test");
        }
    }

    [TestFixture]
    public class WhenSplittingConditions
    {
        QueryBuilder<Movie> _queryBuilder;
        BooleanQuery _booleanQuery;
        List<string> _listOrConditions;
        StringBuilder _andConditions;

        [SetUp]
        public void SetUp()
        {
            _queryBuilder = new QueryBuilder<Movie>("");
            _booleanQuery = new BooleanQuery();
            _listOrConditions = new List<string>();
            _andConditions = new StringBuilder();
        }

        [Test]
        public void ShouldNotDoAnythingIfThereAreNoConditions()
        {            
            _queryBuilder.SplitConditions(_booleanQuery, _listOrConditions, _andConditions);
            _listOrConditions.Count.ShouldEqual(0);
            _andConditions.Length.ShouldEqual(0);
        }

        [Test]
        public void ShouldNotAddToAndConditionIfTheConditionIsAnOrCondition()
        {
            var intBooleanCondition = new IntBooleanCondition("field", 4);
            _booleanQuery.Conditions.Add(intBooleanCondition);
            _queryBuilder.SplitConditions(_booleanQuery, _listOrConditions, _andConditions);
            _andConditions.Length.ShouldEqual(0);
        }

        [Test]
        public void ShouldAddToOrConditionIfTheConditionIsAnOrCondition()
        {
            var intBooleanCondition = new IntBooleanCondition("field", 4);
            _booleanQuery.Conditions.Add(intBooleanCondition);
            _queryBuilder.SplitConditions(_booleanQuery, _listOrConditions, _andConditions);
            _listOrConditions.Count.ShouldEqual(1);
            _listOrConditions[0].ShouldMatch(intBooleanCondition.GetConditionParam());
        }

        [Test]
        public void ShouldAddToAndConditionIfTheConditionIsAnAndCondition()
        {
            var stringBooleanCondition = new StringBooleanCondition("field", "4");
            _booleanQuery.Conditions.Add(stringBooleanCondition);
            _queryBuilder.SplitConditions(_booleanQuery, _listOrConditions, _andConditions);
            _andConditions.Length.ShouldBeGreaterThan(0);
            _andConditions.ToString().ShouldEqual(string.Format("{0}+", stringBooleanCondition.GetConditionParam()));
        }

        [Test]
        public void ShouldNotAddToOrConditionIfTheConditionIsAnAndCondition()
        {
            var stringBooleanCondition = new StringBooleanCondition("field", "4");
            _booleanQuery.Conditions.Add(stringBooleanCondition);
            _queryBuilder.SplitConditions(_booleanQuery, _listOrConditions, _andConditions);
            _listOrConditions.Count.ShouldEqual(0);
        }
    }

    [TestFixture]
    public class WhenAddingConjunctionConditionsToBooleanConditions
    {
        QueryBuilder<Movie> _queryBuilder;
        BooleanQuery _booleanQuery;
        List<string> _listOrConditions;
        StringBuilder _andConditions;

        [SetUp]
        public void SetUp()
        {
            _queryBuilder = new QueryBuilder<Movie>("");
            _booleanQuery = new BooleanQuery();
            _listOrConditions = new List<string>();
            _andConditions = new StringBuilder();
        }        
    }

    [TestFixture]
    public class WhenAddingOrderBy
    { 
        SearchQuery<Movie> _searchQuery;
        QueryBuilder<Movie> _queryBuilder;

        [SetUp]
        public void SetUp()
        {
            _queryBuilder = new QueryBuilder<Movie>("");
        }

        [Test]
        public void ShouldAddRankIfOrderByNotNullOrEmpty()
        {
            string orderByField = "year";
            _searchQuery = new SearchQuery<Movie> { OrderByField = orderByField, Order = Order.ASCENDING };

            _queryBuilder.BuildSearchQuery(_searchQuery).ShouldContain("rank=" + orderByField);
        }

        [Test]
        public void ShouldNotAddRankIfOrderByNullOrEmpty()
        {
            string orderByField = "year";
            _searchQuery = new SearchQuery<Movie> ();

            _queryBuilder.BuildSearchQuery(_searchQuery).ShouldNotContain("rank=" + orderByField);
        }

        [Test]
        public void ShouldOrderByAscIfOrderByAscTrue()
        {
            string orderByField = "year";
            _searchQuery = new SearchQuery<Movie> { OrderByField = orderByField, Order = Order.ASCENDING };

            _queryBuilder.BuildSearchQuery(_searchQuery).ShouldNotContain("rank=-" + orderByField);
            _queryBuilder.BuildSearchQuery(_searchQuery).ShouldContain("rank=" + orderByField);
        }

        [Test]
        public void ShouldOrderByDescIfOrderByAscFalse()
        {
            string orderByField = "year";
            _searchQuery = new SearchQuery<Movie> { OrderByField = orderByField, Order = Order.DESCENDING };

            _queryBuilder.BuildSearchQuery(_searchQuery).ShouldContain("rank=-" + orderByField);
            _queryBuilder.BuildSearchQuery(_searchQuery).ShouldNotContain("rank=" + orderByField);
        }

        [Test]
        public void ShouldntAddRanking()
        {
            _searchQuery = new SearchQuery<Movie> {  };

            _queryBuilder.BuildSearchQuery(_searchQuery).ShouldNotContain("rank=");
        }
    }

    [TestFixture]
    public class WhenGrouping
    {
        SearchQuery<Movie> _searchQuery;
        QueryBuilder<Movie> _queryBuilder;

        [SetUp]
        public void SetUp()
        {
            _queryBuilder = new QueryBuilder<Movie>("");
        }

        [Test]
        public void AndCondition()
        {
            var conditionA = new StringBooleanCondition("genre", "Sci-Fi");
            var conditionB = new IntBooleanCondition("year");
            conditionB.SetFrom(2013);

            var groupCondition = new GroupedCondition(conditionA, ConditionType.AND, conditionB);
            var bQuery = new BooleanQuery();
            bQuery.Conditions.Add(groupCondition);

            _searchQuery = new SearchQuery<Movie> {BooleanQuery = bQuery };
            string query = _queryBuilder.BuildSearchQuery(_searchQuery);
            query.ShouldContain("(and+(and+genre%3A'Sci-Fi'+year%3A2013..))");
        }

        [Test]
        public void OrCondition()
        {
            var conditionA = new StringBooleanCondition("genre", "Sci-Fi");
            var conditionB = new IntBooleanCondition("year");
            conditionB.SetFrom(2013);

            var groupCondition = new GroupedCondition(conditionA, ConditionType.OR, conditionB);
            var bQuery = new BooleanQuery();
            bQuery.Conditions.Add(groupCondition);

            _searchQuery = new SearchQuery<Movie> { BooleanQuery = bQuery };
            string query = _queryBuilder.BuildSearchQuery(_searchQuery);
            query.ShouldContain("(and+(or+genre%3A'Sci-Fi'+year%3A2013..))");
        }

        [Test]
        public void ComplexAnd()
        {
            var condition1A = new StringBooleanCondition("genre", "Sci-Fi");
            var condition1B = new IntBooleanCondition("year");
            condition1B.SetFrom(1990);
            var groupCondition1 = new GroupedCondition(condition1A, ConditionType.AND, condition1B);

            var condition2A = new StringBooleanCondition("genre", "Fantasy");
            var condition2B = new IntBooleanCondition("year");
            condition2B.SetFrom(2013);
            var groupCondition2 = new GroupedCondition(condition2A, ConditionType.AND, condition2B);

            var groupConditionAll = new GroupedCondition(groupCondition1, ConditionType.AND, groupCondition2);

            var bQuery = new BooleanQuery();
            bQuery.Conditions.Add(groupConditionAll);

            _searchQuery = new SearchQuery<Movie> { BooleanQuery = bQuery };
            string query = _queryBuilder.BuildSearchQuery(_searchQuery);
            query.ShouldContain("(and+(and+(and+genre%3A'Sci-Fi'+year%3A1990..)+(and+genre%3A'Fantasy'+year%3A2013..)))");
        }

        [Test]
        public void ComplexOr()
        {
            var condition1A = new StringBooleanCondition("genre", "Sci-Fi");
            var condition1B = new IntBooleanCondition("year");
            condition1B.SetFrom(1990);
            var groupCondition1 = new GroupedCondition(condition1A, ConditionType.AND, condition1B);

            var condition2A = new StringBooleanCondition("genre", "Fantasy");
            var condition2B = new IntBooleanCondition("year");
            condition2B.SetFrom(2013);
            var groupCondition2 = new GroupedCondition(condition2A, ConditionType.AND, condition2B);

            var groupConditionAll = new GroupedCondition(groupCondition1, ConditionType.OR, groupCondition2);

            var bQuery = new BooleanQuery();
            bQuery.Conditions.Add(groupConditionAll);

            _searchQuery = new SearchQuery<Movie> { BooleanQuery = bQuery };
            string query = _queryBuilder.BuildSearchQuery(_searchQuery);
            query.ShouldContain("(and+(or+(and+genre%3A'Sci-Fi'+year%3A1990..)+(and+genre%3A'Fantasy'+year%3A2013..)))");
        }

        [Test]
        public void ComplexOrWithList()
        {
            var genres = new List<string> { "Sci-Fi", "Fantasy" };
            var condition1A = new StringListBooleanCondition("genre", genres, ConditionType.OR);
            var years = new List<int> { 1987, 1990, 2010 };
            var condition1B = new IntListBooleanCondition("year", years, ConditionType.OR);
            var groupCondition1 = new GroupedCondition(condition1A, ConditionType.AND, condition1B);

            var condition2 = new StringBooleanCondition("director", "doduck");

            var groupConditionAll = new GroupedCondition(groupCondition1, ConditionType.AND, condition2);

            var bQuery = new BooleanQuery();
            bQuery.Conditions.Add(groupConditionAll);

            _searchQuery = new SearchQuery<Movie> { BooleanQuery = bQuery };
            string query = _queryBuilder.BuildSearchQuery(_searchQuery);
            query.ShouldNotContain("(and+(and+(and+genre%3A'Sci-Fi'+genre%3A'Fantasy'+year%3A1987+year%3A1990+year%3A2010)+director%3A'doduck'))");
        }
    }


    [TestFixture]
    public class ListBooleanCondition
    {
        SearchQuery<Movie> _searchQuery;
        QueryBuilder<Movie> _queryBuilder;

        [SetUp]
        public void SetUp()
        {
            _queryBuilder = new QueryBuilder<Movie>("");
        }

        [Test]
        public void StringListOr()
        {
            var list = new List<string> { "aValue1", "aValue2", "aValue3" };
            var stringList = new StringListBooleanCondition("myField", list, ConditionType.OR);
            var bQuery = new BooleanQuery();
            bQuery.Conditions.Add(stringList);

            _searchQuery = new SearchQuery<Movie> { BooleanQuery = bQuery };
            string query = _queryBuilder.BuildSearchQuery(_searchQuery);
            query.ShouldContain("(or+myField%3A'aValue1'+myField%3A'aValue2'+myField%3A'aValue3')");
        }

        [Test]
        public void StringListAnd()
        {
            var list = new List<string> { "aValue1", "aValue2", "aValue3" };
            var stringList = new StringListBooleanCondition("myField", list, ConditionType.AND);
            var bQuery = new BooleanQuery();
            bQuery.Conditions.Add(stringList);

            _searchQuery = new SearchQuery<Movie> { BooleanQuery = bQuery };
            string query = _queryBuilder.BuildSearchQuery(_searchQuery);
            query.ShouldContain("(and+myField%3A'aValue1'+myField%3A'aValue2'+myField%3A'aValue3')");
        }


        [Test]
        public void intListOr()
        {
            var list = new List<int> { 1, 2, 3 };
            var stringList = new IntListBooleanCondition("myField", list, ConditionType.OR);
            var bQuery = new BooleanQuery();
            bQuery.Conditions.Add(stringList);

            _searchQuery = new SearchQuery<Movie> { BooleanQuery = bQuery };
            string query = _queryBuilder.BuildSearchQuery(_searchQuery);
            query.ShouldContain("(or+myField%3A1+myField%3A2+myField%3A3)") ;
        }

        [Test]
        public void IntListAnd()
        {
            var list = new List<int> { 1, 2, 3 };
            var stringList = new IntListBooleanCondition("myField", list, ConditionType.AND);
            var bQuery = new BooleanQuery();
            bQuery.Conditions.Add(stringList);

            _searchQuery = new SearchQuery<Movie> { BooleanQuery = bQuery };
            string query = _queryBuilder.BuildSearchQuery(_searchQuery);
            query.ShouldContain("(and+myField%3A1+myField%3A2+myField%3A3)");
        }
    }
}