﻿namespace FizzCode.DbTools.DataDefinition
{
    using FizzCode.DbTools.DataDefinition.Generic1;
    using FizzCode.DbTools.TestBase;

    public class DbUniqueConstratintAsFk : TestDatabaseDeclaration
    {
        public SqlTable Primary { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddInt32("UniqueId");
            table.AddNVarChar("Name", 100);
            table.AddUniqueConstraint("UniqueId");
        });

        public SqlTable Foreign { get; } = AddTable(table =>
        {
            table.AddInt32("Id");
            table.AddInt32("PrimaryId").SetForeignKeyToColumn(nameof(Primary), "UniqueId");
        });
    }

    public class DbUniqueConstratintAsFkInvalidSet : TestDatabaseDeclaration
    {
        public SqlTable Primary { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddInt32("UniqueId");
            table.AddNVarChar("Name", 100);
            table.AddUniqueConstraint("UniqueId");
        });

        public SqlTable Foreign { get; } = AddTable(table =>
        {
            table.AddInt32("Id");
            table.AddInt32("PrimaryId").SetForeignKeyToTable(nameof(Primary));
        });
    }

    public class DbUniqueConstratintAsFkInvalidAdd : TestDatabaseDeclaration
    {
        public SqlTable Primary { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddInt32("UniqueId");
            table.AddNVarChar("Name", 100);
            table.AddUniqueConstraint("UniqueId");
        });

        public SqlTable Foreign { get; } = AddTable(table =>
        {
            table.AddInt32("Id");
            table.AddForeignKey(nameof(Primary));
        });
    }
}
