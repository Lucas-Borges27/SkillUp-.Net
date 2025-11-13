Oracle Entity Framework Core 8.21.121 README
============================================
Release Notes: Oracle Entity Framework Core 8 Provider

December 2023 

This README supplements the main ODP.NET 21c documentation.
https://docs.oracle.com/en/database/oracle/oracle-database/21/odpnt/

Oracle Entity Framework Core 8.21.121 supports .NET 8 runtime.


Bugs Fixed since Oracle.EntityFrameworkCore NuGet Package 7.21.12
=================================================================
Bug 35719179 - USING .HASCONVERSION<INT> RESULTS IN NULLREFERENCEEXCEPTION
Bug 35579522 - TRACING ISSUES WITH ORACLERELATIONALCOMMAND.EXECUTE* CALLS
Bug 36003953 - USING GUIDS AS KEYS SKIP THE INDEX AND INSTEAD CONDUCT A FULL TABLE SCAN
Bug 35533638 - INCORRECT SQL LITERAL GENERATED WHEN USING GUIDS
Bug 34989781 - USING .NET STRING COMPARISON FOR CLOB COLUMN RESULTS IN ORA-00932 OR ORA-22848
Bug 31965621 - USING ISROWVERSION ON DATETIME/DATETIMEOFFSET PROPERTY RESULTS IN ORA-01400
Bug 35958423 - DATA CORRUPTION WHEN INSERTING DATA INTO NCHAR COLUMN

Tips, Limitations, and Known Issues
===================================
Code First
----------
* The HasIndex() Fluent API cannot be invoked on an entity property that will result in a primary key in the Oracle database. 
Oracle Database does not support index creation for primary keys since an index is implicitly created for all primary keys.

* Oracle Database 11.2 does not support default expression to reference any PL/SQL functions nor any pseudocolumns such as 
'<sequence>.NEXTVAL'. As such, HasDefaultValue() and HasDefaultValueSql() Fluent APIs cannot be used in conjunction with 
'sequence.NEXTVAL' as the default value, for example, with the Oracle Database 11.2. However, the application can use the 
UseOracleIdentityColumn() extension method to have the column be populated with server generated values even for Oracle 
Database 11.2. Please read about UseOracleIdentityColumn() for more details.

* The HasFilter() Fluent API is not supported. For example, 
modelBuilder.Entity<Blog>().HasIndex(b => b.Url.HasFilter("Url is not null");

* Data seeding using the UseIdentityColumn is not supported.

* The UseCollation() Fluent API is not supported.

* The new DateOnly and TimeOnly types from .NET 6 are not supported.

* DatabaseTable.Indexes() is not supported for descending indexes. 

* The following usage is not supported because of a limitation in the provider: 
 -> HasColumnType("float").HasPrecision(38).
As a workaround, set the precision value in the HasColumnType() fluent API instead of explicitly using the HasPrecision() fluent API,
e.g., HasColumnType("float (38)").
NOTE: Except for 38 as a precision value, other precision values between 1 and 126 can be set using the HasPrecision() Fluent API. This limitation and workaround also apply when annotations are used instead of the above mentioned fluent API's.

Computed Columns
----------------
* Literal values used for computed columns must be encapsulated by two single-quotes. In the example below, the literal string 
is the comma. It needs to be surrounded by two single-quotes as shown below.

     // C# - computed columns code sample
    modelBuilder.Entity<Blog>()
    .Property(b => b.BlogOwner)
    .HasComputedColumnSql("\"LastName\" || '','' || \"FirstName\"");

Database Scalar Function Mapping
--------------------------------
* Database scalar function mapping does not provide a native way to use functions residing within PL/SQL packages. To work around 
this limitation, map the package and function to an Oracle synonym, then map the synonym to the EF Core function.

LINQ
----
* Oracle Database 12.1 has the following limitation: if the select list contains columns with identical names and you specify the 
row limiting clause, then an ORA-00918 error occurs. This error occurs whether the identically named columns are in the same table 
or in different tables.

Let us suppose that database contains following two table definitions:
SQL> desc X;
 Name    Null?    Type
 ------- -------- ----------------------------
 COL1    NOT NULL NUMBER(10)
 COL2             NVARCHAR2(2000)

SQL> desc Y;
 Name    Null?    Type
 ------- -------- ----------------------------
 COL0    NOT NULL NUMBER(10)
 COL1             NUMBER(10)
 COL3             NVARCHAR2(2000)

Executing the following LINQ, for example, would generate a select query which would contain "COL1" column from both the tables. 
Hence, it would result in error ORA-00918:
dbContext.Y.Include(a => a.X).Skip(2).Take(3).ToList();
This error does not occur when using Oracle Database 11.2, 12.2, and higher versions.

* Certain LINQs cannot be executed against Oracle Database 11.2.
Let us first imagine an Entity Model with the following entities:

public class Gear
{
    public string FullName { get; set; }
    public virtual ICollection<Weapon> Weapons { get; set; }
}

public class Weapon
{
    public int Id { get; set; }
    public bool IsAutomatic { get; set; }
    public string OwnerFullName { get; set; }
    public Gear Owner { get; set; }
}

The following LINQ will not work against Oracle Database 11.2:

dbContext.Gear.Include(i => i.Weapons).OrderBy(o => o.Weapons.OrderBy(w => w.Id).FirstOrDefault().IsAutomatic).ToList();

This is due to LINQ creating the following SQL query:

SELECT "i"."FullName"
FROM "Gear" "i"
ORDER BY (
    Select
     K0 "IsAutomatic" from(
    SELECT "w"."IsAutomatic" K0
    FROM "Weapon" "w"
    WHERE ("i"."FullName" = "w"."OwnerFullName")
    ORDER BY "w"."Id" NULLS FIRST
    ) "m1"
    where rownum <= 1
) NULLS FIRST, "i"."FullName" NULLS FIRST

Within the SELECT statement, there are two nested SELECTs. The generated SQL will encounter a ORA-00904 : 
"invalid identifier" error with Oracle Database 11.2 since it has a restriction where it does not recognize outer 
select table alias "i" in the inner nested select query.

* LINQ query's that are used to query or restore historical (Temporal) data are not supported.

* LINQ query's that are used to query the new DateOnly and TimeOnly types from .NET 6 are not supported.

* HasRowsAffectedReturnValue is not supported because Oracle does not support having a return value from a stored procedure. For example,
modelBuilder.Entity<Person>()
    .UpdateUsingStoredProcedure(
        "People_Update",
        storedProcedureBuilder =>
        {
            storedProcedureBuilder.HasRowsAffectedReturnValue(true)
        });

* Certain LINQs cannot be executed against Oracle Database 21c or lower.
Let us first imagine an Entity Model with the following entity:

public class MyTable
    {
        public int Id { get; set; }
        public int? Value { get; set; }
    }

The following LINQ will not work against Oracle Database 21c or lower:

var query = from t in context.Table
                    group t.Id by t.Value
                    into tg
                    select new
                    {
                        A = tg.Key,
                        B = context.Table.Where(t => t.Value == tg.Max() * 6).Max(t => (int?)t.Id),
                    };

This is due to LINQ creating the following SQL query:

SELECT "t"."Value" "A", "t"."Id", (
    SELECT MAX("t0"."Id")
    FROM "MyTable" "t0"
    WHERE (("t0"."Value" = "t"."Id") OR ("t0"."Value" IS NULL AND
MAX("t"."Id") IS NULL))) "B"
FROM "MyTable" "t"
GROUP BY "t"."Value"

The issue is because the inner select query uses a MAX function which refers to a column from the outer select query.
Also the way in which the MAX function is used within the WHERE clause is not supported in Oracle Database.
The same issue is also applicable when the MIN function is used.

* Oracle DB doesn't support UPDATE queries with FROM clause in 21c DB or lower
So certain LINQs cannot be executed  against Oracle Database which generate UPDATE query with FROM clause. For example,
Imagine an Entity Model with the following entities:

public class Blog
{
	public int Id { get; private set; }
	public string Name { get; set; }
	public List<Post> Posts { get; } = new();
}

public class Post
{
  public int Id { get; private set; }
  public string Title { get; set; }
  public string Content { get; set; }
  public DateTime PublishedOn { get; set; }
}

Trying to update the Blog.Name using below LINQ would throw 'ORA-00933: SQL command not properly ended'

var query = from blog in context.Set<Blog>().Where(c => c.Name == "MyBlog")
                      join post in context.Set<Post>().Where(p => p.Title == "Oracle")
                      on blog.Name equals post.Title
                      select new { blog, post };
var updateQuery = query.ExecuteUpdate(s => s.SetProperty(c => c.blog.Name, "Updated"));

This is due to LINQ creating the following SQL query, which is not supported by Oracle database.
	
UPDATE "Blogs" "b"
SET "b"."Name" = N'Updated'
FROM (
    SELECT "p"."Id", "p"."BlogId", "p"."Content", "p"."PublishedOn", "p"."Title"
    FROM "Posts" "p"
    WHERE "p"."Title" = N'Oracle'
) "t"
WHERE (("b"."Name" = "t"."Title") AND ("b"."Name" = N'MyBlog'))

* The PL/SQL returned by ToQueryString() does not execute successfully if the input to the LINQ query contains a TimeSpan. This is because in PL/SQL, interval value with precision is not accepted. Consider this example,

Imagine an Entity Model with the following entity:

public class Author
{
  public int Id { get; set; }
  public string Name { get; set; }
  public DateTimeOffset Timeline { get; set; }
}

The following LINQ will not work:

var timeSpan = new TimeSpan(1000);
var authorsInChigley1 = context.Authors.Where(e => e.Timeline > DateTimeOffset.Now - timeSpan).ToQueryString();

Following is the PL/SQL that gets generated.

DECLARE
l_sql     varchar2(32767);
l_cur     pls_integer;
l_execute pls_integer;
BEGIN
l_cur := dbms_sql.open_cursor;
l_sql := 'SELECT "a"."Id", "a"."Name", "a"."Timeline"
FROM "Authors" "a"
WHERE "a"."Timeline" > (SYSDATE - :timeSpan_0)';
dbms_sql.parse(l_cur, l_sql, dbms_sql.native);
dbms_sql.bind_variable(l_cur, ':timeSpan_0', INTERVAL '0 0:0:0.0001000' DAY(8) TO SECOND(7));
l_execute:= dbms_sql.execute(l_cur);
dbms_sql.return_result(l_cur);
END;


Migrations
----------
* If more than one column is associated with any sequence/trigger, then ValueGeneratedOnAdd() Fluent API will be generated 
for each of these columns when performing a scaffolding operation. If we then use this scaffolded model to perform a migration, 
then an issue occurs. Each column associated with the ValueGeneratedOnAdd() Fluent API is made an identity column by default. 
To avoid this issue, use UseOracleSQLCompatibility("11") which will force Entity Framework Core to generate triggers/sequences 
instead.

Scaffolding
-----------
* Scaffolding a table that uses Function Based Indexes is supported. However, the index will NOT be scaffolded.
* Scaffolding a table that uses Conditional Indexes is not supported.

Sequences
---------
* A sequence cannot be restarted.
* Extension methods related to SequenceHiLo is not supported, except for columns with Char, UInt, ULong, and UByte data types.

Error Messages
--------------
Oracle Entity Framework Core 7.x only supports English messages

 Copyright (c) 2021, 2023, Oracle and/or its affiliates. 
