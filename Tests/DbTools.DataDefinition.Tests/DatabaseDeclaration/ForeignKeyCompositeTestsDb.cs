﻿using System.Collections.Generic;
using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.DataDefinition.Generic;
using FizzCode.DbTools.TestBase;

namespace FizzCode.DbTools.DataDefinition.Tests;
public class ForeignKeyComposite : TestDatabaseDeclaration
{
    public SqlTable OrderHeader { get; } = AddTable(table =>
    {
        table.AddInt32("Id").SetPK().SetIdentity();
        table.AddNVarChar("OrderHeaderDescription", 100);
    });

    public SqlTable Order { get; } = AddTable(table =>
    {
        table.AddInt32("OrderHeaderId").SetForeignKeyToTable(nameof(OrderHeader)).SetPK();
        table.AddInt32("LineNumber").SetPK();
        table.AddForeignKey(nameof(Company));
        table.AddNVarChar("OrderDescription", 100);
    });

    public SqlTable Company { get; } = AddTable(table =>
    {
        table.AddInt32("Id").SetPK().SetIdentity();
        table.AddNVarChar("Name", 100);
    });

    public SqlTable TopOrdersPerCompany { get; } = AddTable(table =>
    {
        table.AddForeignKey(nameof(Order), new List<ColumnReference>()
        {
            new ColumnReference("Top1A", "OrderHeaderId"),
            new ColumnReference("Top1B", "LineNumber"),
        });

        table.AddForeignKey(nameof(Order), new List<ColumnReference>()

        {
            new ColumnReference("Top2A", "OrderHeaderId"),
            new ColumnReference("Top2B", "LineNumber"),
        });
    });
}
