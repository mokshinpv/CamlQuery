CamlQuery v0.8
==============

Query builder library for SharePoint SPQuery.
CamlQuery library helps to write SPQuery at more high level without using xml syntax.

# Just look at example:

## Classical creating of SPQuery:

private static SPQuery GetAllItemsQuery()
{
  var today = DateTime.Today;
	var currentUser = SPContext.Current.Web.CurrentUser;

  var query = new SPQuery
  {
    Query = string.Concat(@"<Where>
      <And>
         <And>
            <And>
               <NotIncludes>
                  <FieldRef Name=\"", NewsFields.IS_RECYCLED, @"\" />
                  <Value Type=\"UserMulti\" LookupId=\"TRUE\">", currentUser.ID, @"</Value>
               </NotIncludes>
               <NotIncludes>
                  <FieldRef Name=\"", NewsFields.IS_DELETED, @"\" />
                  <Value Type=\"UserMulti\" LookupId=\"TRUE\">", currentUser.ID, @"</Value>
               </NotIncludes>
            </And>
            <Leq>
               <FieldRef Name=\"", NewsFields.PUBLISHED_DATE, @"\" />
               <Value Type=\"DateTime\">", SPUtility.CreateISO8601DateTimeFromSystemDateTime(today), @"</Value>
            </Leq>
         </And>
         <Or>
            <Eq>
               <FieldRef Name=\"", NewsFields.DURATION, @"\" />
               <Value Type=\"Number\">0</Value>
            </Eq>
            <Geq>
               <FieldRef Name=\"", NewsFields.PUBLISHED_DATE_END, @"\" />
               <Value Type=\"DateTime\">", SPUtility.CreateISO8601DateTimeFromSystemDateTime(today), @"</Value>
            </Geq>
         </Or>
      </And>
    </Where>
    <OrderBy>
      <FieldRef Name=\"", NewsFields.PUBLISHED_DATE, @"\" Ascending=\"FALSE\" />
    </OrderBy>"),
    ViewFields = "<FieldRef Name=\"ID\" />"
  };

  return query;
}

## Creating of SPQuery using CamlQuery

private static SPQuery GetAllItemsQuery()
{
  var today = DateTime.Today;
	var currentUser = SPContext.Current.Web.CurrentUser;

	var query = new CamlQuery(
		CamlQuery.And(
			CamlQuery.NotIncludes(NewsFields.IS_RECYCLED, CamlFieldType.UserMulti, currentUser.ID, true),
			CamlQuery.NotIncludes(NewsFields.IS_DELETED, CamlFieldType.UserMulti, currentUser.ID, true),
			CamlQuery.Leq(NewsFields.PUBLISHED_DATE, today),
			CamlQuery.Or(
				CamlQuery.Eq(NewsFields.DURATION, CamlFieldType.Number, 0),
				CamlQuery.Geq(NewsFields.PUBLISHED_DATE_END, today)
			)
		),
	  CamlQuery.ViewFields(NewsFields.ID)
	);

  return query.GetSPQuery();
}

# Features:
- You write C# code with IntelliSense that reduces typos and mistakes in SPQuery xml syntax
- Your code is shorter and cleaner
- Text values in generated SPQuery are automatically wrapped into CDATA sections that improves security
- You don't need specify types for most used field types: int is automatically corresponds to Integer SPFieldType, string - to Text, etc.
Of course, you can set field type explicitly
- There's no stupid restrictions for And/Or: in CamlQuery And/Or you can put as much conditions as you want
- High level operators for some operations: Between, NotIn, etc.
- Support of RowLimit, Scope and ViewFieldsOnly attributes
- Support of OrderBy and ViewFields
- Chaining for CamlQuery object to set query properties

TODO:
- Support of Folder property
- Named And/Or clauses. Using them you can set structure of query once and then fill query using indexer
