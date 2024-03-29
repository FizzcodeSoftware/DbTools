﻿using FizzCode.DbTools.DataDefinition.Factory;
using FizzCode.DbTools.DataDefinition.Tests;
using FizzCode.DbTools.Factory.Interfaces;
using FizzCode.DbTools.TestBase;
using FizzCode.LightWeight.MsTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FizzCode.DbTools.QueryBuilder.Tests;
public abstract class QueryBuilderTestBase
{
    protected readonly IQueryBuilderFactory _queryBuilderFactory;

    public QueryBuilderTestBase()
    {
        var contextFactory = new TestContextFactory(s => s.Options.ShouldNotGuardKeywords = true);
        var sqlGeneratorBaseFactory = new SqlGeneratorBaseFactory(contextFactory);
        _queryBuilderFactory = new QueryBuilderFactory(contextFactory, sqlGeneratorBaseFactory);
    }
}

[TestClass]
public class QueryBuilderKeywordTests : QueryBuilderTestBase
{
    [TestMethod]
    public void SimpleTable()
    {
        var db = new TestDatabaseFksTyped();
        var qb = _queryBuilderFactory.CreateQueryBuilder(MsSqlVersion.MsSql2016);
        var q = new Query(db.Parent);

        var result = qb.Build(q);

        AssertCustom.AreEqual("SELECT p.Id, p.Name\r\nFROM Parent p", result);
    }
}

[TestClass]
public class QueryBuilderTests : QueryBuilderTestBase
{
    [TestMethod]
    public void SimpleTable()
    {
        var db = new TestDatabaseFksTyped();
        var qb = _queryBuilderFactory.CreateQueryBuilder(MsSqlVersion.MsSql2016);
        var q = new Query(db.Parent);

        var result = qb.Build(q);

        AssertCustom.AreEqual("SELECT p.Id, p.Name\r\nFROM Parent p", result);
    }

    [TestMethod]
    public void JoinWithAlias()
    {
        var db = new TestDatabaseFksTyped();
        var qb = _queryBuilderFactory.CreateQueryBuilder(MsSqlVersion.MsSql2016);
        var q = new Query(db.Child, QueryColumnAliasStrategy.PrefixTableAliasAlways)
            .LeftJoinAlias(db.Parent, "ppp");

        var result = qb.Build(q);

        AssertCustom.AreEqual("SELECT c.Id, c.Name, c.ParentId, ppp.Id AS 'ppp_Id', ppp.Name AS 'ppp_Name'\r\n"
            + "FROM Child c\r\n"
            + "LEFT JOIN Parent ppp ON ppp.Id = c.ParentId", result);
    }

    [TestMethod]
    public void JoinColumnsNone()
    {
        var db = new TestDatabaseFksTyped();
        var qb = _queryBuilderFactory.CreateQueryBuilder(MsSqlVersion.MsSql2016);
        var q = new Query(db.Child)
            .InnerJoin(db.Parent, new None());

        var result = qb.Build(q);

        AssertCustom.AreEqual("SELECT c.Id, c.Name, c.ParentId\r\n"
            + "FROM Child c\r\n"
            + "INNER JOIN Parent p ON p.Id = c.ParentId", result);
    }

    [TestMethod]
    public void JoinColumnsParentNone()
    {
        var db = new TestDatabaseFksTyped();
        var qb = _queryBuilderFactory.CreateQueryBuilder(MsSqlVersion.MsSql2016);
        var q = new Query(db.Child, new None())
            .InnerJoin(db.Parent);

        var result = qb.Build(q);

        AssertCustom.AreEqual("SELECT p.Id, p.Name\r\n"
            + "FROM Child c\r\n"
            + "INNER JOIN Parent p ON p.Id = c.ParentId", result);
    }

    [TestMethod]
    public void JoinColumnsParentNone_PrefixTableAliasAlways()
    {
        var db = new TestDatabaseFksTyped();
        var qb = _queryBuilderFactory.CreateQueryBuilder(MsSqlVersion.MsSql2016);
        var q = new Query(db.Child, QueryColumnAliasStrategy.PrefixTableAliasAlways, new None())
            .InnerJoin(db.Parent);

        var result = qb.Build(q);

        AssertCustom.AreEqual("SELECT p.Id AS 'p_Id', p.Name AS 'p_Name'\r\n"
            + "FROM Child c\r\n"
            + "INNER JOIN Parent p ON p.Id = c.ParentId", result);
    }

    [TestMethod]
    public void ParentFieldWithChild()
    {
        var db = new TestDatabaseFksTyped();
        var qb = _queryBuilderFactory.CreateQueryBuilder(MsSqlVersion.MsSql2016);
        var q = new Query(db.Child)
            .LeftJoin(db.Parent, db.Parent.Name);

        var result = qb.Build(q);

        AssertCustom.AreEqual("SELECT c.Id, c.Name, c.ParentId, p.Name AS 'ParentName'\r\nFROM Child c\r\n"
            + "LEFT JOIN Parent p ON p.Id = c.ParentId", result);
    }

    [TestMethod]
    public void SimpleTableColumns()
    {
        var db = new TestDatabaseFksTyped();
        var qb = _queryBuilderFactory.CreateQueryBuilder(MsSqlVersion.MsSql2016);
        var q = new Query(db.Child, db.Child.Name, db.Child.ParentId);

        var result = qb.Build(q);

        AssertCustom.AreEqual("SELECT c.Name, c.ParentId\r\nFROM Child c", result);
    }

    [TestMethod]
    public void SimpleTableColumnExpression()
    {
        var db = new TestDatabaseFksTyped();
        var qb = _queryBuilderFactory.CreateQueryBuilder(MsSqlVersion.MsSql2016);
        var q = new Query(db.Child, db.Child.Name, db.Child.ParentId).AddColumn("now", "GetDate()");

        var result = qb.Build(q);

        AssertCustom.AreEqual("SELECT c.Name, c.ParentId, GetDate() AS 'now'\r\nFROM Child c", result);
    }

    [TestMethod]
    public void SimpleTableColumnCase()
    {
        var db = new TestDatabaseFksTyped();
        var qb = _queryBuilderFactory.CreateQueryBuilder(MsSqlVersion.MsSql2016);

        var q = new Query(db.Child, db.Child.Name, db.Child.ParentId).AddColumn("IsParentId1", "CASE WHEN", db.Child.ParentId, "= 1 THEN", db.Child.ParentId, "ELSE 0 END");

        var result = qb.Build(q);

        AssertCustom.AreEqual("SELECT c.Name, c.ParentId, CASE WHEN c.ParentId = 1 THEN c.ParentId ELSE 0 END AS 'IsParentId1'\r\nFROM Child c", result);
    }

    [TestMethod]
    public void JoinSameTable()
    {
        var db = new TestDatabaseFksTyped();
        var qb = _queryBuilderFactory.CreateQueryBuilder(MsSqlVersion.MsSql2016);
        var q = new Query(db.Child)
            .LeftJoinAlias(db.Parent, "p1")
            .LeftJoinAlias(db.Parent, "p2");

        var result = qb.Build(q);

        AssertCustom.AreEqual("SELECT c.Id, c.Name, c.ParentId, p1.Id AS 'ParentId', p1.Name AS 'ParentName', p2.Id AS 'ParentId', p2.Name AS 'ParentName'\r\n"
            + "FROM Child c\r\n"
            + "LEFT JOIN Parent p1 ON p1.Id = c.ParentId\r\n"
            + "LEFT JOIN Parent p2 ON p2.Id = c.ParentId", result);
    }

    [TestMethod]
    public void WhereSimple()
    {
        var db = new TestDatabaseFksTyped();
        var qb = _queryBuilderFactory.CreateQueryBuilder(MsSqlVersion.MsSql2016);
        var q = new Query(db.Parent)
            .Where("1 = 1");

        var result = qb.Build(q);

        AssertCustom.AreEqual("SELECT p.Id, p.Name\r\nFROM Parent p\r\nWHERE 1 = 1", result);
    }

    [TestMethod]
    public void WhereExpression()
    {
        var db = new TestDatabaseFksTyped();
        var qb = _queryBuilderFactory.CreateQueryBuilder(MsSqlVersion.MsSql2016);
        var q = new Query(db.Parent)
            .Where(db.Parent.Name, "LIKE 'a%'");

        var result = qb.Build(q);

        AssertCustom.AreEqual("SELECT p.Id, p.Name\r\nFROM Parent p\r\nWHERE p.Name LIKE 'a%'", result);
    }

    [TestMethod]
    public void JoinSameTableAndWhere()
    {
        var db = new TestDatabaseFksTyped();
        var qb = _queryBuilderFactory.CreateQueryBuilder(MsSqlVersion.MsSql2016);
        var q = new Query(db.Child, QueryColumnAliasStrategy.PrefixTableAliasIfNeeded)
            .LeftJoinAlias(db.Parent, "p1")
            .LeftJoinAlias(db.Parent, "p2")
            .Where("p1.", db.Parent.Name, "LIKE 'a%'",
            "AND", "p2.", db.Parent.Name, "LIKE 'a%'");

        var result = qb.Build(q);

        AssertCustom.AreEqual("SELECT c.Id, c.Name, c.ParentId, p1.Id AS 'p1_Id', p1.Name AS 'p1_Name', p2.Id AS 'p2_Id', p2.Name AS 'p2_Name'\r\n"
            + "FROM Child c\r\n"
            + "LEFT JOIN Parent p1 ON p1.Id = c.ParentId\r\n"
            + "LEFT JOIN Parent p2 ON p2.Id = c.ParentId\r\n"
            + "WHERE p1.Name LIKE 'a%' AND p2.Name LIKE 'a%'", result);
    }

    [TestMethod]
    public void Union()
    {
        var db = new TestDatabaseFksTyped();
        var qb = _queryBuilderFactory.CreateQueryBuilder(MsSqlVersion.MsSql2016);
        var q = new Query(db.Parent)
            .Union(new Query(db.Parent));

        var result = qb.Build(q);

        AssertCustom.AreEqual("SELECT p.Id, p.Name\r\nFROM Parent p\r\nUNION\r\nSELECT p.Id, p.Name\r\nFROM Parent p", result);
    }

    [TestMethod]
    public void TableAliases()
    {
        var db = new TestDatabaseFksTyped();
        var qb = _queryBuilderFactory.CreateQueryBuilder(MsSqlVersion.MsSql2016);

        var p1 = db.Parent.Alias("p1");
        var p2 = db.Parent.Alias("p2");

        var q = new Query(db.Child)
            .LeftJoinOn(p1, new Expression(p1.Id, "=", db.Child.ParentId), new None())
            .LeftJoinOn(p2, new Expression(p2.Id, "=", p1.Id), new None());

        var result = qb.Build(q);

        AssertCustom.AreEqual("SELECT c.Id, c.Name, c.ParentId\r\n"
+ "FROM Child c\r\n"
+ "LEFT JOIN Parent p1 ON p1.Id = c.ParentId\r\n"
+ "LEFT JOIN Parent p2 ON p2.Id = p1.Id", result);
    }
}
