Amazing Cloud Search - C# API For Amazon Cloud Search
============

Allow you to search, faceted search, add, update, remove objects from your Amazon Cloud Search Index in C#.


How to use
---------
First you need a Amazon cloud search instance and it's URL (We will call it a key)
It should look like : yourDomainName-xxxxxxxxxxxxx.us-east-1.cloudsearch.amazonaws.com

> This example can be used with the IMDB default index that you can select when creating your Cloud Search.

It indexes movies which implement CloudSearchDocument (just an ID):

	public class Movie : CloudSearchDocument
	{
		public List<string> actor { get; set; }
		public string director { get; set; }
		public DateTime mydate { get; set; }
		public string title { get; set; }
		public int year { get; set; }
	}

> Object field starts in lowercase because THEY NEED to match the field name in your index. 
> This needs to be improved (feel free)

### Add a movie ###
	var cloudSearch = new CloudSearch<Movie>("YOU_AMAZON_CLOUD_SEARCH_KEY", "2011-02-01");
	var movie = new Movie { id = "fjuhewdijsdjoi", title = "simple title", year = 2012, mydate = DateTime.Now, actor = new List<string> { "good actor1", "good actor2" }, director = "martin magakian" };
	cloudSearch.Add(movie);


### Remove a movie ###
	var movie = new Movie { id = "fjuhewdijsdjoi" }
	cloudSearch.Delete(movie);
	
	
### Update a movie ###
	movie.title = "In the skin of Amazon cloud search";
	cloudSearch.Update(movie);
	

Search
======
### Search for movies ###
	var searchQuery = new SearchQuery<Movie> {Keyword = "star wars"};
	var found = cloudSearch.Search(searchQuery);
	
	
### maximum of 25 movies only in the category from Sci-Fi ###
	var bQuery = new BooleanQuery();
	var gCondition = new StringBooleanCondition("genre", "Sci-Fi");
	bQuery.Conditions.Add(gCondition);
	
	var searchQuery = new SearchQuery<Movie> {Keyword = "star wars", Size = 25, BooleanQuery = bQuery};
	var found = cloudSearch.Search(searchQuery);
	
	
### from 2000 to 2004 in category Sci-Fi ###
	var bQuery = new BooleanQuery();
	var gCondition = new StringBooleanCondition("genre", "Sci-Fi");
	var yCondition = new IntBooleanCondition("year");
	yCondition.SetInterval(2000,2004);
	bQuery.Conditions.Add(gCondition);
	bQuery.Conditions.Add(yCondition);

	var searchQuery = new SearchQuery<Movie> {Keyword = "star wars", Size = 20, BooleanQuery = bQuery};
	var found = cloudSearch.Search(searchQuery);
	
	
Faceted Search
======

Faceted search are used to display how many result can be find in each categorie. The user is usually able to drill-down each facet.

![faceted search Amazon](https://raw.github.com/martin-magakian/Amazing-Cloud-Search/master/README_src/faceted-search-amazon.png)

Amazon.com use facet when searching for a book. In the left panel the user can see all search result matching is search ordered by categorie "language".

###  Search for star wars movies + number of result per categories (faceted search) ###
	var genreFacet = new Facet { Name = "genre" };
	var liFacet = new List<Facet> { genreFacet };
	
	var searchQuery = new SearchQuery<Movie> {Keyword = "star wars",  Facets = liFacet };
	var found = cloudSearch.Search(searchQuery);
	
	
### Search for movies + number of result in 'Sci-Fi' and 'Fantasy' category (faceted search) ###
	var genreFacetContraint = new StringFacetConstraints();
	genreFacetContraint.AddContraint("Sci-Fi");
	genreFacetContraint.AddContraint("Fantasy");
	var genreFacet = new Facet { Name = "genre", FacetContraint = genreFacetContraint };
	var liFacet = new List<Facet> { genreFacet };
	
	var searchQuery = new SearchQuery<Movie> {Keyword = "star wars", Facets = liFacet};
	var found = cloudSearch.Search(searchQuery);
	
	
### Search for movies + number of results in the 'Sci-Fi' and 'Fantasy' categories + the number of result in the year 1950 and between 1980 and 2012 (faceted search) ###
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
	
	
Complex example:
=====
Let put everything together now: Facet for Sci-Fi,Fantasy, in 1950, between 1980 to 2012 + search only for movies in Sci-Fi from 2000 to 2004

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
	

List Condition
========
### OR condition ###
Search for movies where genre is one of: "Sci-Fi" **or** "Fantasy" **or** "other" 

	var list = new List<string> { "Sci-Fi", "Fantasy", "other" };
	var stringList = new StringListBooleanCondition("genre", list, ConditionType.OR);
	var bQuery = new BooleanQuery();
	bQuery.Conditions.Add(stringList);

	_searchQuery = new SearchQuery<Movie> { BooleanQuery = bQuery };
	
### AND condition ###
Search for movies where genre categories is in: "Sci-Fi" **and** "Fantasy" **and** "other" in the same time.

	var list = new List<string> { "Sci-Fi", "Fantasy", "other" };
	var stringList = new StringListBooleanCondition("genre", list, ConditionType.OR);
	var bQuery = new BooleanQuery();
	bQuery.Conditions.Add(stringList);

	_searchQuery = new SearchQuery<Movie> { BooleanQuery = bQuery };

Note:<br />
IntListBooleanCondition achieve the same as StringListBooleanCondition with Integer.
	
Grouped Condition 
========
More complex search can be done using Grouped Condition.
It's possible to search for "ConditionA" **and / or** "ConditionB".

**Note:**<br />
maybe "StringListBooleanCondition" and "IntListBooleanCondition" are more suitable depending on the case.

	
### "OR" example: ###
The query will return all the movies who match "ConditionA" **or** "ConditionB"
Return the movies who are in Sci-Fi **or** made from 2013

            var conditionA = new StringBooleanCondition("genre", "Sci-Fi");
            var conditionB = new IntBooleanCondition("year");
            conditionB.SetFrom(2013);
            var groupCondition = new GroupedCondition(conditionA, ConditionType.AND, conditionB);
			
            var bQuery = new BooleanQuery();
            bQuery.Conditions.Add(groupCondition);
            _searchQuery = new SearchQuery<Movie> { BooleanQuery = bQuery };
	
	
### "AND" example: ###
The query will return all the movies who match "ConditionA" **and** "ConditionB"
Return the movies who are in Sci-Fi **and** made from 2013

            var conditionA = new StringBooleanCondition("genre", "Sci-Fi");
            var conditionB = new IntBooleanCondition("year");
            conditionB.SetFrom(2013);
            var groupCondition = new GroupedCondition(conditionA, ConditionType.OR, conditionB);
			
            var bQuery = new BooleanQuery();
            bQuery.Conditions.Add(groupCondition);
            _searchQuery = new SearchQuery<Movie> { BooleanQuery = bQuery };
			
Pagination
=========
SearchQuery accepts parameter Size for the number of results.
But also accepts a parameter Start which can by used for paginating the results.
The total number of results is also displayed in the search results:
found.hits.found 

Order By / Sorting Results
=========
SearchQuery also accepts parameters for ordering the results.

All movies from the oldest to the youngest (descending):

	var searchQuery = new SearchQuery<Movie> { OrderByField = "year", Order = Order.DESCENDING };

All movies from the youngest to the oldest (ascending):

	var searchQuery = new SearchQuery<Movie> { OrderByField = "year", Order = Order.ASCENDING };

Without any OrderByField set, CloudSearch default ordering will be used.


Limit top facet result
=========
You can also request the top facet as part of the results.
Facet object accepts a TopResult parameter.
For example we want the top 3 movie genres of our search

	var genreFacet = new Facet { Name = "genre", TopResult = 2};


Dependency
---------
- Json.NET


Improve Amazing Cloud Search !
---------
Feel free to fork and improuve Amazing Cloud Search API.

Contact
=========
Developed by Martin Magakian
dev.martin.magakian@gmail.com


License
=========
MIT License (MIT)

![githalytics.com alpha](https://cruel-carlota.gopagoda.com/cf04469d46e9c36bb429da1323bebabb "githalytics.com")

