using System;
using System.Collections.Generic;
using System.Threading;
using AmazingCloudSearch.Query;
using AmazingCloudSearch.Query.Boolean;
using AmazingCloudSearch.Query.Facets;
using NUnit.Framework;
using AmazingCloudSearch.Contract;

namespace AmazingCloudSearch.Test
{
    [TestFixture]
    public class RoundTripSmokeTests
    {
        [Test]
        public void TestSearch()
        {
            var cloudSearch = new CloudSearch<Movie>("YOU_AMAZON_CLOUD_SEARCH_KEY", "2011-02-01");

            //build facet
            var genreFacetContraint = new StringFacetConstraints();
            genreFacetContraint.AddContraint("Sci-Fi");
            genreFacetContraint.AddContraint("Fantasy");
            genreFacetContraint.AddContraint("Action");
            var genreFacet = new Facet { Name = "genre", TopResult = 2};

            var yearFacetContraint = new IntFacetContraints();
            yearFacetContraint.AddFrom(1950);
            yearFacetContraint.AddInterval(1980, 2012);
            var yearFacet = new Facet { Name = "year", FacetContraint = yearFacetContraint };

            var liFacet = new List<Facet> { genreFacet, yearFacet };


            //build boolean query
            var bQuery = new BooleanQuery();
            var gCondition = new StringBooleanCondition("genre", "Sci-Fi");
            var yCondition = new IntBooleanCondition("year");
            yCondition.SetInterval(2000,2004);
            bQuery.Conditions.Add(gCondition);
            bQuery.Conditions.Add(yCondition);

            //build search
            var searchQuery = new SearchQuery<Movie> {Keyword = "star wars", Facets = liFacet, Size = 20, Start = 40, BooleanQuery = bQuery};

            //search
            var found = cloudSearch.Search(searchQuery);
            
            Assert.IsTrue(!found.IsError);

            Assert.IsTrue(found.hits.found > 0);

            Assert.IsTrue(found.facetsResults.Count > 0);

            Assert.AreEqual(found.facetsResults.Count, 2, "We request only the top 2 facet");
        }

        [Test]
        public void TestCRUD()
        {
            var cloudSearch = new CloudSearch<Movie>("YOU_AMAZON_CLOUD_SEARCH_KEY", "2011-02-01");

            var movie = new Movie { id = "fjuhewdijsdjoi", title = "simple title", year = 2012, mydate = DateTime.Now, actor = new List<string> { "good actor1", "good actor2" }, director = "martin magakian" };

            cloudSearch.Add(movie);

            Thread.Sleep(1000);

            movie.title = "In the skin of Amazon cloud search";
            cloudSearch.Update(movie);

            Thread.Sleep(1000);

            cloudSearch.Delete(movie);
        }
    }
}