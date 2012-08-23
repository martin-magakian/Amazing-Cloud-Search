Amazing Cloud Search - C# API For Amazon Cloud Search
============

Allow you to search, faceted search, add, update, remove objects from your Amazon Cloud Search Index in C#.

Improuve Amazing Cloud Search !
=========

Feel free to do fork this code and improuve it. (CF: Licene MIT)

How to use
---------

First you need a Amazon cloud search instance and it URL (We will call it a key)
It should look like : yourDomainName-xxxxxxxxxxxxx.us-east-1.cloudsearch.amazonaws.com

This exemple can be use with IMDB default index that you can select when creating your cloud search.
It index movies who implement SearchDocument (just an ID):

	public class Movie : SearchDocument
	{
		public List<string> actor { get; set; }
		public string director { get; set; }
		public DateTime mydate { get; set; }
		public string title { get; set; }
		public int year { get; set; }
	}

> The raison why my object field start in lowercase is because YOU NEED to match the field name of your database.
>For the moment you need to match the field name.

Add a movie
------

	var cloudSearch = new CloudSearch<Movie>("YOU_AMAZON_CLOUD_SEARCH_KEY", "2011-02-01");
	var movie = new Movie { id = "fjuhewdijsdjoi", title = "simple title", year = 2012, mydate = DateTime.Now, actor = new List<string> { "good actor1", "good actor2" }, director = "martin magakian" };
	cloudSearch.Add(movie);


Remove a movie
------
	var movie = new Movie { id = "fjuhewdijsdjoi" }
	cloudSearch.Delete(movie);
	
	
Update a movie
------
	movie.title = "In the skin of Amazon cloud search";
	cloudSearch.Update(movie);
	
	
Search for movies
------
	var searchQuery = new SearchQuery<Movie> {Keyword = "star wars"};
	var found = cloudSearch.Search(searchQuery);
	
	
Search for a maximum of 25 movies only in the categorie from Sci-Fi
------
	var bQuery = new BooleanQuery();
	var gCondition = new StringBooleanCondition("genre", "Sci-Fi");
	bQuery.Conditions.Add(gCondition);
	
	var searchQuery = new SearchQuery<Movie> {Keyword = "star wars", Size = 25, BooleanQuery = bQuery};
	var found = cloudSearch.Search(searchQuery);
	
	
Search for movies only from 2000 to 2004 in category Sci-Fi
------
	var bQuery = new BooleanQuery();
	var gCondition = new StringBooleanCondition("genre", "Sci-Fi");
	var yCondition = new IntBooleanCondition("year");
	yCondition.SetInterval(2000,2004);
	bQuery.Conditions.Add(gCondition);
	bQuery.Conditions.Add(yCondition);

	var searchQuery = new SearchQuery<Movie> {Keyword = "star wars", Facets = liFacet, Size = 20, BooleanQuery = bQuery};
	var found = cloudSearch.Search(searchQuery);
	
	
Search for movies + number of result per category (faceted search)
------
	var genreFacet = new Facet { Name = "genre" };
	var liFacet = new List<Facet> { genreFacet };
	
	var searchQuery = new SearchQuery<Movie> {Keyword = "star wars"};
	var found = cloudSearch.Search(searchQuery);
	
	
Search for movies + number of result in 'Sci-Fi' and 'Fantasy' category (faceted search)
------
	var genreFacetContraint = new StringFacetConstraints();
	genreFacetContraint.AddContraint("Sci-Fi");
	genreFacetContraint.AddContraint("Fantasy");
	var genreFacet = new Facet { Name = "genre", FacetContraint = genreFacetContraint };
	var liFacet = new List<Facet> { genreFacet };
	
	var searchQuery = new SearchQuery<Movie> {Keyword = "star wars", Facets = liFacet};
	var found = cloudSearch.Search(searchQuery);
	
	
Search for movies + number of result in 'Sci-Fi' and 'Fantasy' category + number of result in the year 1950 and between 1980 and 2012 (faceted search)
------
	var genreFacetContraint = new StringFacetConstraints();
	genreFacetContraint.AddContraint("Sci-Fi");
	genreFacetContraint.AddContraint("Fantasy");
	var genreFacet = new Facet { Name = "genre", FacetContraint = genreFacetContraint };

	var yearFacetContraint = new IntFacetContraints();
	yearFacetContraint.AddFrom(1950);
	yearFacetContraint.AddInterval(1980, 2012);
	var yearFacet = new Facet { Name = "year", FacetContraint = yearFacetContraint };

	var liFacet = new List<Facet> { genreFacet, yearFacet };
	
    var searchQuery = new SearchQuery<Movie> {Keyword = "star wars", Facets = liFacet};
	var found = cloudSearch.Search(searchQuery);
	
	
All together now: Facet for Sci-Fi,Fantasy, in 1950, between 1980 to 2012 + search only for movie in Sci-Fi from 2000 to 2004
------
	var cloudSearch = new CloudSearch<Movie>("YOUR_CLOUD_SEARCH_API", "2011-02-01");

	//build facet
	var genreFacetContraint = new StringFacetConstraints();
	genreFacetContraint.AddContraint("Sci-Fi");
	genreFacetContraint.AddContraint("Fantasy");
	var genreFacet = new Facet { Name = "genre", FacetContraint = genreFacetContraint };

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
	var searchQuery = new SearchQuery<Movie> {Keyword = "star wars", Facets = liFacet, Size = 20, BooleanQuery = bQuery};

	//search
	var found = cloudSearch.Search(searchQuery);
	
	
More option
=========

Pagination
------
SearchQuery accept parameter Size for the number of result.
But also accept a parameter Start who can by use to paginate the result.
The total number of result is also display in the search result:
found.hits.found 

Limitate top facet
------
You can only request the top facet as part of the result.
Facet object accept a TopResult parameter.
For exemple we went the top 3 movie genre of our search
var genreFacet = new Facet { Name = "genre", TopResult = 2};


Dependency
=========
- Json.NET


Contact
=========
Developed by Martin Magakian
dev.martin.magakian@gmail.com


Licene
=========
MIT License (MIT)
