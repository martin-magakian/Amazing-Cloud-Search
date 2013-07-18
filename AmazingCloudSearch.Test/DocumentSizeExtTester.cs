using AmazingCloudSearch.Contract;
using NUnit.Framework;

namespace AmazingCloudSearch.Test
{
    [TestFixture]
    public class DocumentSizeExtTester
    {
        [Test]
        public void ShouldMatchSizes()
        {
            var doc = new CloudSearchDocument { text_relevance = "Same Data"};
            var doc2 = new CloudSearchDocument { text_relevance = "Same Data" };

            doc.GetSize().ShouldEqual(doc2.GetSize());
        }

        [Test]
        public void ShouldNotMatchSize()
        {
            var doc = new CloudSearchDocument();
            var doc2 = new CloudSearchDocument { text_relevance = "More Data"};

            doc.GetSize().ShouldNotEqual(doc2.GetSize());
        }
    }
}
