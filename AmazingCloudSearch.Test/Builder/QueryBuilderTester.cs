using System.Collections.Generic;
using System.Text;
using AmazingCloudSearch.Builder;
using AmazingCloudSearch.Query;
using AmazingCloudSearch.Query.Boolean;
using NUnit.Framework;
using Rhino.Mocks;

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
            _searchQuery = new SearchQuery<Movie> { OrderByField = orderByField, OrderByAsc = true };

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
            _searchQuery = new SearchQuery<Movie> { OrderByField = orderByField, OrderByAsc = true };

            _queryBuilder.BuildSearchQuery(_searchQuery).ShouldNotContain("rank=-" + orderByField);
            _queryBuilder.BuildSearchQuery(_searchQuery).ShouldContain("rank=" + orderByField);
        }

        [Test]
        public void ShouldOrderByDescIfOrderByAscFalse()
        {
            string orderByField = "year";
            _searchQuery = new SearchQuery<Movie> { OrderByField = orderByField, OrderByAsc = false };

            _queryBuilder.BuildSearchQuery(_searchQuery).ShouldContain("rank=-" + orderByField);
            _queryBuilder.BuildSearchQuery(_searchQuery).ShouldNotContain("rank=" + orderByField);
        }

    }


    //[TestFixture]
    //public class WhenAddingOrderByCallTests : InteractionContext<QueryBuilder<Movie>>
    //{
    //    [Test]
    //    public void ShouldCallOrderByIfOrderByNotNullOrEmpty()
    //    {
    //        string orderByField = "year";
    //        SearchQuery<Movie> _searchQuery = new SearchQuery<Movie> {OrderByField = orderByField, OrderByAsc = true};

    //        ClassUnderTest.BuildSearchQuery(_searchQuery);
    //        ClassUnderTest.AssertWasCalled(x => x.OrderBy(null, true, null), x => x.IgnoreArguments());
    //    }
    //}

}