﻿namespace FizzCode.DbTools.DataDefinition.Tests
{
    using FizzCode.DbTools.DataDefinition.Generic1;
    using FizzCode.DbTools.TestBase;

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
}