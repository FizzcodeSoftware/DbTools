﻿using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.DataDefinition.Generic1;
using FizzCode.DbTools.TestBase;

namespace FizzCode.DbTools.DataDefinition.Tests;
public class TestDatabaseSelfFK : TestDatabaseDeclaration
{
    public SqlTable Company { get; } = AddTable(table =>
      {
          table.AddInt32("Id").SetPK().SetIdentity();
          table.AddForeignKey(nameof(Company), "Parent");
          table.AddNVarChar("Name", 100);
      });
}