﻿using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.DataDefinition.Generic;
using FizzCode.DbTools.TestBase;

namespace FizzCode.DbTools.DataDefinition.Tests;
public class TestDatabaseUniqueConstraint2 : TestDatabaseDeclaration
{
    public SqlTable Company { get; } = AddTable(table =>
    {
        table.AddInt32("Id").SetPK().SetIdentity();
        table.AddNVarChar("Name", 100);
        table.AddNVarChar("Name2", 100);
        table.AddUniqueConstraint("Name", "Name2");
    });
}
