﻿using System.Linq;
using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.DataDefinition.Tests;
using FizzCode.DbTools.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FizzCode.DbTools.DataDefinitionReader.Tests;
[TestClass]
public class DataDefinitionReaderForeignKeyCompositeTests : DataDefinitionReaderTests
{
    [DataTestMethod]
    [LatestSqlVersions]
    public void ReadTables(SqlEngineVersion version)
    {
        Init(version, new ForeignKeyComposite());

        TestHelper.CheckFeature(version, "ReadDdl");

        var dd = ReadDd(version, new ForeignKeyComposite().GetSchemaNames());

        var company = dd.GetTable("Company");

        var pkCompany = company.Properties.OfType<PrimaryKey>().FirstOrDefault();
        Assert.IsNotNull(pkCompany);
        Assert.AreEqual(1, pkCompany.SqlColumns.Count);
        Assert.AreEqual("Id", pkCompany.SqlColumns[0].SqlColumn.Name);

        var topOrdersPerCompany = dd.GetTable("TopOrdersPerCompany");

        var fks = topOrdersPerCompany.Properties.OfType<ForeignKey>().ToList();
        Assert.AreEqual(2, fks.Count);

        var fk1 = fks[0];
        var fk2 = fks[1];

        Assert.AreEqual(2, fk1.ForeignKeyColumns.Count);
        Assert.AreEqual(2, fk2.ForeignKeyColumns.Count);

        var order = dd.GetTable("Order");

        // TODO for Oracle, order of FK columns can be different
        Assert.AreEqual(topOrdersPerCompany.Columns["Top1A"], fk1.ForeignKeyColumns[0].ForeignKeyColumn);
        Assert.AreEqual(topOrdersPerCompany.Columns["Top1B"], fk1.ForeignKeyColumns[1].ForeignKeyColumn);

        Assert.AreEqual(order.Columns["OrderHeaderId"], fk1.ForeignKeyColumns[0].ReferredColumn);
        Assert.AreEqual(order.Columns["LineNumber"], fk1.ForeignKeyColumns[1].ReferredColumn);

        Assert.AreEqual(topOrdersPerCompany.Columns["Top2A"], fk2.ForeignKeyColumns[0].ForeignKeyColumn);
        Assert.AreEqual(topOrdersPerCompany.Columns["Top2B"], fk2.ForeignKeyColumns[1].ForeignKeyColumn);

        Assert.AreEqual(order.Columns["OrderHeaderId"], fk2.ForeignKeyColumns[0].ReferredColumn);
        Assert.AreEqual(order.Columns["LineNumber"], fk2.ForeignKeyColumns[1].ReferredColumn);
    }
}