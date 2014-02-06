Amazing Cloud Search - C# API For Amazon Cloud Search
============

Allow you to search, faceted search, add, update, remove objects from your Amazon Cloud Search Index in C#.

Contribute
---------
Amazon CloudSearch is a AWS library created and maintain by [doduck prototype](http://doduck.com)<br />
Feel free to contribute to the project using push request.

Le site [doduck prototype](http://doduck.fr) est aussi disponible en Français.

How to use
---------
First you need a Amazon Cloud Search instance and its URL (we will call it a key).
It should look like:

> yourDomainName-xxxxxxxxxxxxx.us-east-1.cloudsearch.amazonaws.com

Next define a class which maps C# data to the Cloud Search index. This example can be used with the
IMDB default index you can select when setting up Cloud Search.

```C#
public class Movie : CloudSearchDocument
{
	[JsonProperty("title")]
	public string Title { get; set; }
	
	[JsonProperty("director")]
	public string Director { get; set; }
	
	[JsonProperty("actor")]
	public List<string> Actor { get; set; }
	
	[JsonProperty("mydate")]
	public DateTime MyDate { get; set; }
	
	// Optionally leave off JsonProperty and match the index name exactly
	public int year { get; set; }
}
```

Cloud Search has strict rules about the names of index fields which do not follow typical C#
conventions. We can either make our property names match, or use `JsonPropertyAttribute` to define
exactly how they map.

> Field names must begin with a letter and can contain the following characters: a-z (lowercase),
> 0-9, and _ (underscore). Uppercase letters and hyphens are not allowed. The names `body`,
> `docid`, and `text_relevance` are reserved and cannot be specified as field or rank expression
> names.  
> [http://docs.aws.amazon.com/cloudsearch/latest/developerguide/API_IndexField.html](http://docs.aws.amazon.com/cloudsearch/latest/developerguide/API_IndexField.html)

You can also use other [JSON.NET](http://james.newtonking.com/json) attributes such as
`JsonConverter` to control the format of the data being saved. This is especially useful for
converting dates into suitable formats for ranges and sorting.

### Add a movie ###
```C#
var cloudSearch = new CloudSearch<Movie>("YOUR_AMAZON_CLOUD_SEARCH_KEY", "2011-02-01");
var movie = new Movie
{
	Id = "fjuhewdijsdjoi",
	Title = "simple title",
	Director = "martin magakian"
	Actor = new List<string>
	{
		"good actor1",
		"good actor2"
	},
	MyDate = DateTime.Now,
	year = 2012,
};
cloudSearch.Add(movie);
```

### Remove a movie ###
```C#
var movie = new Movie
{
	Id = "fjuhewdijsdjoi"
}
cloudSearch.Delete(movie);
```

### Update a movie ###
```C#
movie.Title = "In the skin of Amazon cloud search";
cloudSearch.Update(movie);
```

Search
======
### Search for movies ###
```C#
var searchQuery = new SearchQuery<Movie> { Keyword = "star wars" };
var found = cloudSearch.Search(searchQuery);
```

### maximum of 25 movies only in the category from Sci-Fi ###
```C#
var bQuery = new BooleanQuery();
var gCondition = new StringBooleanCondition("genre", "Sci-Fi");
bQuery.Conditions.Add(gCondition);

var searchQuery = new SearchQuery<Movie> { Keyword = "star wars", Size = 25, BooleanQuery = bQuery };
var found = cloudSearch.Search(searchQuery);
```

### from 2000 to 2004 in category Sci-Fi ###
```C#
var bQuery = new BooleanQuery();
var gCondition = new StringBooleanCondition("genre", "Sci-Fi");
var yCondition = new IntBooleanCondition("year");
yCondition.SetInterval(2000,2004);
bQuery.Conditions.Add(gCondition);
bQuery.Conditions.Add(yCondition);

var searchQuery = new SearchQuery<Movie> { Keyword = "star wars", Size = 20, BooleanQuery = bQuery };
var found = cloudSearch.Search(searchQuery);
```

Faceted Search
======

Faceted search are used to display how many result can be find in each categorie. The user is usually able to drill-down each facet.

![faceted search Amazon](https://raw.github.com/martin-magakian/Amazing-Cloud-Search/master/README_src/faceted-search-amazon.png)

Amazon.com use facet when searching for a book. In the left panel the user can see all search result matching is search ordered by categorie "language".

### Search for star wars movies + number of result per categories (faceted search) ###
```C#
var genreFacet = new Facet { Name = "genre" };
var liFacet = new List<Facet> { genreFacet };

var searchQuery = new SearchQuery<Movie> { Keyword = "star wars",  Facets = liFacet };
var found = cloudSearch.Search(searchQuery);
```

### Search for movies + number of result in 'Sci-Fi' and 'Fantasy' category (faceted search) ###
```C#
var genreFacetContraint = new StringFacetConstraints();
genreFacetContraint.AddContraint("Sci-Fi");
genreFacetContraint.AddContraint("Fantasy");
var genreFacet = new Facet { Name = "genre", FacetContraint = genreFacetContraint };
var liFacet = new List<Facet> { genreFacet };

var searchQuery = new SearchQuery<Movie> { Keyword = "star wars", Facets = liFacet };
var found = cloudSearch.Search(searchQuery);
```

### Search for movies + number of results in the 'Sci-Fi' and 'Fantasy' categories + the number of result in the year 1950 and between 1980 and 2012 (faceted search) ###
```C#
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
```

Complex example:
=====
Let put everything together now: Facet for Sci-Fi,Fantasy, in 1950, between 1980 to 2012 + search only for movies in Sci-Fi from 2000 to 2004

```C#
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
var searchQuery = new SearchQuery<Movie> { Keyword = "star wars", Facets = liFacet, Size = 20, BooleanQuery = bQuery };

//search
var found = cloudSearch.Search(searchQuery);
```

List Condition
========
### OR condition ###
Search for movies where genre is one of: "Sci-Fi" **or** "Fantasy" **or** "other" 

```C#
var list = new List<string> { "Sci-Fi", "Fantasy", "other" };
var stringList = new StringListBooleanCondition("genre", list, ConditionType.OR);
var bQuery = new BooleanQuery();
bQuery.Conditions.Add(stringList);

_searchQuery = new SearchQuery<Movie> { BooleanQuery = bQuery };
```

### AND condition ###
Search for movies where genre categories is in: "Sci-Fi" **and** "Fantasy" **and** "other" in the same time.

```C#
var list = new List<string> { "Sci-Fi", "Fantasy", "other" };
var stringList = new StringListBooleanCondition("genre", list, ConditionType.OR);
var bQuery = new BooleanQuery();
bQuery.Conditions.Add(stringList);

_searchQuery = new SearchQuery<Movie> { BooleanQuery = bQuery };
```

Note:<br />
IntListBooleanCondition achieve the same as StringListBooleanCondition with Integer.

Grouped Condition 
========
More complex search can be done using Grouped Condition.
It's possible to search for "ConditionA" **and / or** "ConditionB".

**Note:**<br />
maybe "StringListBooleanCondition" and "IntListBooleanCondition" are more suitable depending on the case.

### OR grouping: ###
The query will return all the movies who match "ConditionA" **or** "ConditionB"
Return the movies who are in Sci-Fi **or** made from 2013

```C#
var conditionA = new StringBooleanCondition("genre", "Sci-Fi");
var conditionB = new IntBooleanCondition("year");
conditionB.SetFrom(2013);
var groupCondition = new GroupedCondition(conditionA, ConditionType.AND, conditionB);

var bQuery = new BooleanQuery();
bQuery.Conditions.Add(groupCondition);
_searchQuery = new SearchQuery<Movie> { BooleanQuery = bQuery };
```

### AND grouping: ###
The query will return all the movies who match "ConditionA" **and** "ConditionB"
Return the movies who are in Sci-Fi **and** made from 2013

```C#
var conditionA = new StringBooleanCondition("genre", "Sci-Fi");
var conditionB = new IntBooleanCondition("year");
conditionB.SetFrom(2013);
var groupCondition = new GroupedCondition(conditionA, ConditionType.OR, conditionB);

var bQuery = new BooleanQuery();
bQuery.Conditions.Add(groupCondition);
_searchQuery = new SearchQuery<Movie> { BooleanQuery = bQuery };
```


### AND / OR grouping example: ###
The query will return all movies who match "(movies in 1990 **AND** genre "Sci-Fi")" **OR** (movies in 2013 **AND** genre "Fantasy")

```C#
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
```

Batch
=========
Documents can be Add, Update and Delete by "pack" using batch.
It use only one HTTP request to add, remove, update multiple documents.

Note:<br />
Under the hood Amazing-Cloud-Search split the batch into 5 Mb maximum request in order to meet Amazon API requirement.
A 12Mb bash request will send 3 HTTP requests. 5Mb + 5Mb + 2Mb = 12Mb

### Adding multiple movies at once###
```C#
var movie1 = new Movie { id = "fjuhewdijsdjoi", title = "movie1" };
var movie2 = new Movie { id = "sdhuslzajshdus", title = "movie2" };
var movies = new List<Movie> { movie1, movie2 };
```

### Updating multiple movies at once###
```C#
movie1.Title = "movie1_bis";
movie2.Title = "movie2_bis";
var moviesBis = List<Movie>{ movie1, movie2 };
cloudSearch.Update(moviesBis);
```

### Deleting multiple movies at once###
```C#
var moviesToRemove = List<Movie>{ movie1, movie2 };
cloudSearch.Delete(moviesToRemove);
```

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

```C#
var searchQuery = new SearchQuery<Movie> { OrderByField = "year", Order = Order.DESCENDING };
```

All movies from the youngest to the oldest (ascending):

```C#
var searchQuery = new SearchQuery<Movie> { OrderByField = "year", Order = Order.ASCENDING };
```

Without any OrderByField set, CloudSearch default ordering will be used.

Limit top facet result
=========
You can also request the top facet as part of the results.
Facet object accepts a TopResult parameter.
For example we want the top 3 movie genres of our search

```C#
var genreFacet = new Facet { Name = "genre", TopResult = 2};
```

Dependencies
---------
- [Json.NET](http://james.newtonking.com/json)


Improve Amazing Cloud Search !
---------
Feel free to fork and improve Amazing Cloud Search API.

Contact
=========
Developed by Martin Magakian dev.martin.magakian@gmail.com<br />
For [doduck prototype](http://doduck.com) (English)

Pour [doduck prototype](http://doduck.fr) (Français)




License
=========
MIT License (MIT)

![githalytics.com alpha](https://cruel-carlota.gopagoda.com/cf04469d46e9c36bb429da1323bebabb "githalytics.com")

