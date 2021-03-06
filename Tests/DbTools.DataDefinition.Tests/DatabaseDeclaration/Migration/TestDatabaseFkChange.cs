﻿namespace FizzCode.DbTools.DataDefinition.Tests
{
    using FizzCode.DbTools.DataDefinition.Generic1;
    using FizzCode.DbTools.TestBase;

    public class TestDatabaseFkChange : TestDatabaseDeclaration
    {
        public SqlTable Primary1 { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK();
        });

        public SqlTable Primary2 { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK();
        });

        public SqlTable Foreign { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddInt32("PrimaryId").SetForeignKeyToTable(nameof(Primary1), "FkChange");
        });
    }
}
