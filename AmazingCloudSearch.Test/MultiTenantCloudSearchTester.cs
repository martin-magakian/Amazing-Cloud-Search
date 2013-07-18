using AmazingCloudSearch.Query;
using AmazingCloudSearch.Query.Boolean;
using NUnit.Framework;
using Rhino.Mocks;

namespace AmazingCloudSearch.Test
{
    [TestFixture]
    public class WhenSearching : InteractionContext<MultiTenantCloudSearch<Movie>>
    {
        private IMultiTenantCloudSearchSettings _multiTenantCloudSearchSettings;

        protected override void beforeEach()
        {
            _multiTenantCloudSearchSettings = new MultiTenantCloudSearchSettings() { Tenant = "fooTenant", TenantParameterName = "fooParameterName", ApiVersion = string.Empty, CloudSearchId = string.Empty };
            Services.Inject(_multiTenantCloudSearchSettings);
            Services.PartialMockTheClassUnderTest();            
        }

        [Test]
        public void ShouldCallSearchWithException()
        {
            ClassUnderTest.Search(new SearchQuery<Movie>());
            ClassUnderTest.AssertWasCalled(x=> x.SearchWithException(null), x=> x.IgnoreArguments());
        }

        [Test]
        public void ShouldAddATenantConstraintToTheQuery()
        {
            ClassUnderTest.Search(new SearchQuery<Movie>());
            ClassUnderTest.AssertWasCalled(x => x.SearchWithException(Arg<SearchQuery<Movie>>.Matches(y=> ArgMatches(y))));
        }

        private bool ArgMatches(SearchQuery<Movie> searchQuery)
        {
            var hasACondition = searchQuery.BooleanQuery.Conditions.Count == 1;
            var hasATenantCondition =
                   searchQuery.BooleanQuery.Conditions[0].GetConditionParam()
                                            .Contains(_multiTenantCloudSearchSettings.TenantParameterName);
            var tenantConditionHasCorrectValue =
                searchQuery.BooleanQuery.Conditions[0].GetConditionParam()
                                            .Contains(_multiTenantCloudSearchSettings.Tenant);
            return hasACondition && hasATenantCondition && tenantConditionHasCorrectValue;
//            return hasACondition && hasATenantCondition;
        }
    }

    [TestFixture]
    public class WhenAddingTenantBooleanConditionToQuery
    {
        private MultiTenantCloudSearch<Movie> _multiTenantCloudSearch;
        private IMultiTenantCloudSearchSettings _multiTenantCloudSearchSettings;

        [SetUp]
        public void SetUp()
        {
            _multiTenantCloudSearchSettings = new MultiTenantCloudSearchSettings(){Tenant = "fooTenant", TenantParameterName = "fooParameterName", ApiVersion = string.Empty, CloudSearchId = string.Empty};
            _multiTenantCloudSearch = new MultiTenantCloudSearch<Movie>(_multiTenantCloudSearchSettings);
        }

        [Test]
        public void ShouldAddTheBooleanConditionToTheQuery()
        {
            var stringBooleanCondition = new StringBooleanCondition(
                _multiTenantCloudSearchSettings.TenantParameterName, _multiTenantCloudSearchSettings.Tenant);
            var searchQuery = new SearchQuery<Movie>();
            var result = _multiTenantCloudSearch.AddTenantBooleanConditionToQuery(stringBooleanCondition, searchQuery);
            var output = result.BooleanQuery.Conditions;
            output.Count.ShouldEqual(1);
            output[0].GetConditionParam().ShouldEqual(stringBooleanCondition.GetConditionParam());
        }        
    }

    [TestFixture]
    public class WhenCreatingATenantBooleanCondition
    {
        private MultiTenantCloudSearch<Movie> _multiTenantCloudSearch;
        private IMultiTenantCloudSearchSettings _multiTenantCloudSearchSettings;

        [SetUp]
        public void SetUp()
        {
            _multiTenantCloudSearchSettings = new MultiTenantCloudSearchSettings() { Tenant = "fooTenant", TenantParameterName = "fooParameterName", ApiVersion = string.Empty, CloudSearchId = string.Empty };
            _multiTenantCloudSearch = new MultiTenantCloudSearch<Movie>(_multiTenantCloudSearchSettings);
        }

        [Test]
        public void ShouldCreateAStringBooleanCondition()
        {
            var result = _multiTenantCloudSearch.CreateTenantBooleanCondition();
            result.ShouldBeOfType<StringBooleanCondition>();
        }

        [Test]
        public void TheFieldValueShouldBeTheSettingsTenantParameterName()
        {
            var result = _multiTenantCloudSearch.CreateTenantBooleanCondition();
            result.Field.ShouldEqual(_multiTenantCloudSearchSettings.TenantParameterName);
        }

        [Test]
        public void TheConditionValueShouldBeTheSettingsTenantParam()
        {
            var result = _multiTenantCloudSearch.CreateTenantBooleanCondition();
            result.Condition.ShouldEqual(_multiTenantCloudSearchSettings.Tenant);
        }
    }    
}