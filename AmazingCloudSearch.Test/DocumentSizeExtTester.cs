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
            var doc = new CloudSearchDocument { id = "some id" };
            var doc2 = new CloudSearchDocument { id = "some id" };

            doc.GetSize().ShouldEqual(doc2.GetSize());
        }

        [Test]
        public void ShouldNotMatchSize()
        {
            var doc = new CloudSearchDocument();
            var doc2 = new CloudSearchDocument { id = "some different id"};

            doc.GetSize().ShouldNotEqual(doc2.GetSize());
        }
    }
}
