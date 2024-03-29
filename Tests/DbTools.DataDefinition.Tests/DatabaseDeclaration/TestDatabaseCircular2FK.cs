﻿using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.DataDefinition.Generic;
using FizzCode.DbTools.TestBase;

namespace FizzCode.DbTools.DataDefinition.Tests;
public class TestDatabaseCircular2FK : TestDatabaseDeclaration
{
    public SqlTable A { get; } = AddTable(table =>
      {
          table.AddInt32("Id").SetPK().SetIdentity();
          table.AddForeignKey(nameof(B));
          table.AddNVarChar("Name", 100);
      });

    public SqlTable B { get; } = AddTable(table =>
      {
          table.AddInt32("Id").SetPK().SetIdentity();
          table.AddForeignKey(nameof(A));
          table.AddNVarChar("Name", 100);
      });
}