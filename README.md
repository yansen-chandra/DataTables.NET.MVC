# DataTables.NET.MVC
An ASP.NET MVC Razor Wrapper for DataTables

After using DataTables in one of my project build on ASP.NET MVC, Its simple yet rich feature captivate my interest.
Compared to some expensive license component, DataTables is friendlier to developer and highly supported by the community.

This is my attempt to make development on ASP.NET MVC even more easier by creating a custom html wrapper.
The idea is you will be able to use the Razor Html Extension to define a DataTables.

A simple illustration of how the piece of code will work

@model IEnumerable<Person>
@(
    Html.DTC().HtmlTable(Model)
        .Columns(column =>
        {
            column.Expression(p => p.Id).CssClass("col-sm-2 break-word");
            column.Expression(p => p.FirstName).CssClass("col-sm-2");
            column.Expression(p => p.LastName);
            column.Expression(p => p.DateOfBirth);
        })
)
