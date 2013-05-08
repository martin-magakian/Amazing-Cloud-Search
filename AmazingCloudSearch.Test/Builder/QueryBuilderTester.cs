using System.Text;
using AmazingCloudSearch.Builder;
using NUnit.Framework;

namespace AmazingCloudSearch.Test.Builder
{
    [TestFixture]
    public class WhenFeedingAKeyword : InteractionContext<QueryBuilder<Movie>>
    {
        QueryBuilder<Movie> _queryBuilder;

        protected override void beforeEach()
        {
            _queryBuilder = new QueryBuilder<Movie>();
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
    }
}