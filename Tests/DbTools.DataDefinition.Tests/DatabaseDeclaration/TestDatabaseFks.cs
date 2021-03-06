﻿namespace FizzCode.DbTools.DataDefinition.Tests
{
    using FizzCode.DbTools.DataDefinition.Generic1;
    using FizzCode.DbTools.TestBase;

    public class TestDatabaseFks : TestDatabaseDeclaration
    {
        public SqlTable Child { get; } = AddTable(table =>
          {
              table.AddInt32("Id").SetPK().SetIdentity();
              table.AddNVarChar("Name", 100);
              table.AddForeignKey(nameof(Parent));
          });

        public SqlTable ChildChild { get; } = AddTable(table =>
          {
              table.AddInt32("Id").SetPK().SetIdentity();
              table.AddNVarChar("Name", 100);
              table.AddForeignKey(nameof(Child));
          });

        public SqlTable Parent { get; } = AddTable(table =>
          {
              table.AddInt32("Id").SetPK().SetIdentity();
              table.AddNVarChar("Name", 100);
          });
    }
}