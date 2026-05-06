# Understanding the N+1 Problem

*A practical explanation with examples in EF Core and Dapper*

---

## Introduction

The **N+1 problem** is a common performance issue in applications that fetch related data from a database.

At a high level, it happens when:
- you make **1 query** to fetch the main records
- then make **N additional queries** to fetch related records for each row

So if you load 50 parent records and then run one more query per parent, your application ends up making **51 database queries**.

This is why it is called the **N+1 problem**:
- `1` query for the initial data
- `N` more queries for related data

---

## A Simple Mental Model

Think of it like this.

You go to the grocery store for one item.  
Then come back home.  
Then realize you need another item and go again.  
Then again.  
Then again.

The problem is not that each trip is impossible.  
The problem is that the repeated trips create unnecessary overhead.

Databases behave in a similar way.  
Each query may be small, but too many small queries become expensive.

---

## A Basic Example

Suppose we have:
- `Author`
- `Book`

Each author can have many books.

Now imagine that we want to display:
- the author name
- the books written by that author

A first implementation may look like this:

```csharp
var authors = await context.Authors.ToListAsync();

foreach (var author in authors)
{
    var books = await context.Books
        .Where(book => book.AuthorId == author.Id)
        .ToListAsync();

    Console.WriteLine($"{author.Name} has {books.Count} books.");
}
```

This looks straightforward, but the query behavior is not great:

1. one query loads all authors
2. then one query runs per author to load books

If there are 100 authors, this becomes:
- 1 query for authors
- 100 queries for books

That is **101 total queries**.

---

## Why This Is Bad

The N+1 problem matters because databases are not free to call repeatedly.

Every extra query adds:
- network cost
- database execution cost
- connection and transaction overhead
- extra latency

This means:
- pages become slower
- APIs become slower
- the database does unnecessary work
- performance gets worse as data grows

The dangerous part is that this may not show up during early development.  
With only a few records in a local database, the code may appear perfectly fine.

---

## Where Lazy Loading Enters the Picture

To understand why the N+1 problem is so common in ORMs, we need to talk about **lazy loading**.

**Lazy loading** means related data is not fetched immediately.  
Instead, it is fetched only when you access it.

That sounds convenient, but it can easily hide extra database calls.

For example:

```csharp
var authors = await context.Authors.ToListAsync();

foreach (var author in authors)
{
    Console.WriteLine($"{author.Name}: {author.Books.Count}");
}
```

If `Books` is lazily loaded, then `author.Books` can trigger another query every time the loop touches that property.

So the code looks small and clean, but the runtime behavior may still be N+1.

This is one of the biggest reasons developers get surprised by N+1 issues in ORM-based applications.

---

## Eager Loading Versus Lazy Loading

This is the key distinction:

### Lazy Loading

- related data is loaded only when accessed
- convenient, but easy to misuse
- may silently trigger N+1 queries

### Eager Loading

- related data is loaded up front
- the access pattern is explicit
- it is usually the safer choice when you already know you need the related data

So in simple terms:
- **lazy loading** delays loading
- **eager loading** loads intentionally

If you know you are going to need the child records, eager loading is usually the better choice.

---

## Solving It in EF Core

In EF Core, one common fix is **eager loading** with `Include`.

```csharp
var authors = await context.Authors
    .Include(author => author.Books)
    .ToListAsync();

foreach (var author in authors)
{
    Console.WriteLine($"{author.Name} has {author.Books.Count} books.");
}
```

Here the intent is much clearer:
- load authors
- load their related books as part of the same data-fetching operation

This avoids the classic pattern of querying related data inside the loop.

It is also easier to reason about performance, because the query shape is visible in one place.

---

## Another Example: Orders and Order Items

Let us take another familiar example:
- `Order`
- `OrderItem`

This version has the same problem:

```csharp
var orders = await context.Orders.ToListAsync();

foreach (var order in orders)
{
    var items = await context.OrderItems
        .Where(item => item.OrderId == order.Id)
        .ToListAsync();

    order.Items = items;
}
```

Again the issue is:
- 1 query for orders
- N queries for order items

A more intentional version is:

```csharp
var orders = await context.Orders
    .Include(order => order.Items)
    .ToListAsync();
```

This is simpler and avoids the repeated round-trip pattern.

---

## How To Spot the N+1 Problem

The most common signs are:
- related data fetched inside a loop
- repeated access to navigation properties with lazy loading enabled
- many nearly identical SQL queries in logs
- performance degrading sharply as row count increases

A good rule of thumb is this:

> If related data is being fetched inside a loop, inspect it carefully.

That pattern is not always wrong, but it is often a warning sign.

---

## Important Point: This Is Not Only an EF Core Problem

The N+1 problem is not caused by EF Core alone.  
It is a **data access pattern problem**.

Any tool can suffer from it:
- EF Core
- Dapper
- plain ADO.NET
- any ORM in any language

The difference is only in how visible the mistake is.

With EF Core, lazy loading or navigation access can hide the extra queries.  
With Dapper, you usually write the loop and SQL yourself, so the repeated calls are more explicit.

---

## EF Core Versus Dapper

Now let us compare how the N+1 problem often appears in both.

### EF Core: Problematic Version

```csharp
var authors = await context.Authors.ToListAsync();

foreach (var author in authors)
{
    var books = await context.Books
        .Where(book => book.AuthorId == author.Id)
        .ToListAsync();

    Console.WriteLine($"{author.Name}: {books.Count}");
}
```

Or, if lazy loading is enabled:

```csharp
var authors = await context.Authors.ToListAsync();

foreach (var author in authors)
{
    Console.WriteLine($"{author.Name}: {author.Books.Count}");
}
```

### EF Core: Better Version

```csharp
var authors = await context.Authors
    .Include(author => author.Books)
    .ToListAsync();
```

For larger relationship graphs, EF Core also gives you **split queries**:

```csharp
var authors = await context.Authors
    .Include(author => author.Books)
    .AsSplitQuery()
    .ToListAsync();
```

That is useful when a single joined query causes too much duplicated parent data.

---

### Dapper: Problematic Version

In Dapper, the N+1 version is usually more visible:

```csharp
var authors = (await connection.QueryAsync<Author>(
    "SELECT Id, Name FROM Authors"))
    .ToList();

foreach (var author in authors)
{
    author.Books = (await connection.QueryAsync<Book>(
        "SELECT Id, Title, AuthorId FROM Books WHERE AuthorId = @Id",
        new { author.Id }))
        .ToList();
}
```

This is still:
- 1 query for authors
- N queries for books

So Dapper does not protect you automatically.  
It just makes the behavior easier to see.

### Dapper: Better Version with Multi-Mapping

```csharp
var sql = @"
    SELECT a.*, b.*
    FROM Authors a
    LEFT JOIN Books b ON a.Id = b.AuthorId";

var authorDictionary = new Dictionary<int, Author>();

var authors = connection.Query<Author, Book, Author>(
    sql,
    (author, book) =>
    {
        if (!authorDictionary.TryGetValue(author.Id, out var currentAuthor))
        {
            currentAuthor = author;
            authorDictionary.Add(currentAuthor.Id, currentAuthor);
        }

        if (book != null)
        {
            currentAuthor.Books.Add(book);
        }

        return currentAuthor;
    },
    splitOn: "Id")
    .Distinct()
    .ToList();
```

This is the typical Dapper fix:
- write the join yourself
- map the flat rows back into objects
- use a dictionary to avoid duplicating the parent object in memory

### Dapper: Better Version with QueryMultiple

Another useful Dapper pattern is fetching multiple result sets in one round trip:

```csharp
var sql = "SELECT * FROM Authors; SELECT * FROM Books;";

using var multi = await connection.QueryMultipleAsync(sql);

var authors = (await multi.ReadAsync<Author>()).ToList();
var books = (await multi.ReadAsync<Book>()).ToList();

foreach (var author in authors)
{
    author.Books = books.Where(book => book.AuthorId == author.Id).ToList();
}
```

This is not N+1, because the data is still fetched in a single database round trip.

It can also be a better option when:
- the join would duplicate too much parent data
- the parent rows are large
- the object graph is more complex

---

## Comparative View

Here is the practical comparison:

- **EF Core eager loading with `Include`** is analogous to Dapper multi-mapping with a join.
- **EF Core `AsSplitQuery()`** is conceptually close to Dapper `QueryMultiple()`.
- **EF Core lazy loading** can accidentally create N+1.
- **Dapper** does not usually create hidden N+1 behavior, but it can still easily create explicit N+1 loops if the SQL is written that way.

So the difference is not that one tool has the problem and the other does not.

The difference is:
- EF Core can hide it more easily
- Dapper makes you manage the shape more manually

---

## Final Thought

The N+1 problem is simple in principle:
- one query for the main list
- one extra query per row for related data

The best defense is to think about **query shape** early.

Ask:
- do I already know I need the related data?
- am I loading data inside a loop?
- would eager loading, a join, split query, or multi-result fetch be better here?

Once you start thinking in terms of round trips instead of only code appearance, the N+1 problem becomes much easier to spot and prevent.
