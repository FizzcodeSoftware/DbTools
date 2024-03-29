﻿using System.Linq;
using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.DataDefinition.Generic;
using FizzCode.DbTools.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FizzCode.DbTools.DataDefinition.Tests;
public class TestDatabaseCircularFKSetPK : TestDatabaseDeclaration
{
    public SqlTable FK1 { get; } = AddTable(table =>
      {
          table.AddInt32("FK1Id").SetPK().SetIdentity();
          table.AddForeignKey(nameof(FK2));
      });

    public SqlTable FK2 { get; } = AddTable(table =>
      {
          table.AddInt32("FK2Id").SetPK().SetIdentity();
          table.AddForeignKey(nameof(FK1));
      });
}

[TestClass]
public class DatabaseDefinitionCircularFKWithSetFK
{
    [TestMethod]
    public void TestDatabaseDefinitionCircularFKWithSetFK()
    {
        var db = new TestDatabaseCircularFKSetPK();

        var fk1Cfks = db.GetTable("FK1").Properties.OfType<CircularFK>().ToList();
        var fk2Cfks = db.GetTable("FK2").Properties.OfType<CircularFK>().ToList();

        Assert.AreEqual(1, fk1Cfks.Count);
        Assert.AreEqual(1, fk2Cfks.Count);
    }
}
