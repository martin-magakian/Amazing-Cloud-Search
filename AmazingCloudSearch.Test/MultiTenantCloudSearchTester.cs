using AmazingCloudSearch.Query;
using AmazingCloudSearch.Query.Boolean;
using NUnit.Framework;
using Rhino.Mocks;

namespace AmazingCloudSearch.Test
{
    [TestFixture]
    public class WhenSearching : InteractionContext<CloudSearch<Movie>>
    {
        private ICloudSearchSettings _cloudSearchSettings;

        protected override void beforeEach()
        {
            _cloudSearchSettings = new CloudSearchSettings() { ApiVersion = string.Empty, CloudSearchId = string.Empty };
            Services.Inject(_cloudSearchSettings);
            Services.PartialMockTheClassUnderTest();            
        }

        [Test]
        public void ShouldCallSearchWithException()
        {
            ClassUnderTest.Search(new SearchQuery<Movie>());
            ClassUnderTest.AssertWasCalled(x => x.SearchWithException(null), x => x.IgnoreArguments());
        }

    }

    [TestFixture]
    public class WhenAddingTenantBooleanConditionToQuery
    {
        private CloudSearch<Movie> _cloudSearch;
        private ICloudSearchSettings _cloudSearchSettings;

        [SetUp]
        public void SetUp()
        {
            _cloudSearchSettings = new CloudSearchSettings(){ApiVersion = string.Empty, CloudSearchId = string.Empty};
            _cloudSearch = new CloudSearch<Movie>(_cloudSearchSettings);
            
        }

        [Test]
        public void ShouldAddTheBooleanConditionToTheQuery()
        {
            var searchQuery = new SearchQuery<Movie>();
            var stringBooleanCondition = new StringBooleanCondition("fooTenant", "fooParameterName");
            _cloudSearch.AddPresistantCondition(stringBooleanCondition);
            var result = _cloudSearch.AddPresistantConditionsToQuery(searchQuery);
            var output = result.BooleanQuery.Conditions;
            output.Count.ShouldEqual(1);
            output[0].GetConditionParam().ShouldEqual(stringBooleanCondition.GetConditionParam());
        }        
    }

    [TestFixture]
    public class WhenCreatingATenantBooleanCondition
    {
        private CloudSearch<Movie> _cloudSearch;
        private ICloudSearchSettings _cloudSearchSettings;

        [SetUp]
        public void SetUp()
        {
            _cloudSearchSettings = new CloudSearchSettings() { ApiVersion = string.Empty, CloudSearchId = string.Empty };
            _cloudSearch = new CloudSearch<Movie>(_cloudSearchSettings);
            _cloudSearch.AddPresistantCondition(new StringBooleanCondition("fooTenant", "fooParameterName"));
        }

        [Test]
        public void ShouldCreateAStringBooleanCondition()
        {
            var result = _cloudSearch.PersistanteCondition;
            result[0].ShouldBeOfType<StringBooleanCondition>();
        }

        [Test]
        public void TestConditionValue()
        {
            var result = _cloudSearch.PersistanteCondition;
            var condition = ((StringBooleanCondition)result[0]);
            condition.Field.ShouldEqual("fooTenant");
            condition.Condition.ShouldEqual("fooParameterName");
        }
    }
}