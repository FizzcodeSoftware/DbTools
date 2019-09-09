﻿namespace FizzCode.DbTools.DataDefinition.Tests
{
    using System.Collections.Generic;
    using FizzCode.DbTools.DataDefinition;

    public class ForeignKeyCompositeTestsDb : DatabaseDeclaration
    {
        public SqlTable OrderHeader { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddNVarChar("OrderHeaderDescription", 100);
        });

        public SqlTable Order { get; } = AddTable(table =>
        {
            table.AddInt32("OrderHeaderId").SetForeignKeyTo(nameof(OrderHeader)).SetPK();
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

    public class ForeignKeyCompositeSetForeignKeyVerboseTestsDb : DatabaseDeclaration
    {
        public SqlTable OrderHeader { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddNVarChar("OrderHeaderDescription", 100);
        });

        public SqlTable Order { get; } = AddTable(table =>
        {
            table.AddInt32("OrderHeaderId").SetForeignKeyTo(nameof(OrderHeader)).SetPK();
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
            table.AddInt32("Top1A");
            table.AddInt32("Top1B");
            table.AddInt32("Top2A");
            table.AddInt32("Top2B");

            var fk1 = new ForeignKey(table, nameof(Order), "FK_TopOrdersPerCompany__Top1A__Top1B");
            fk1.ForeignKeyColumns.Add(new ForeignKeyColumnMap(fk1, table.Columns["Top1A"], "OrderHeaderId"));
            fk1.ForeignKeyColumns.Add(new ForeignKeyColumnMap(fk1, table.Columns["Top1B"], "LineNumber"));
            table.Properties.Add(fk1);

            var fk2 = new ForeignKey(table, nameof(Order), "FK_TopOrdersPerCompany__Top2A__Top2B");
            fk2.ForeignKeyColumns.Add(new ForeignKeyColumnMap(fk1, table.Columns["Top2A"], "OrderHeaderId"));
            fk2.ForeignKeyColumns.Add(new ForeignKeyColumnMap(fk1, table.Columns["Top2B"], "LineNumber"));
            table.Properties.Add(fk2);
        });
    }

    public class ForeignKeyCompositeSetForeignKeyToTestDb : DatabaseDeclaration
    {
        public SqlTable OrderHeader { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddNVarChar("OrderHeaderDescription", 100);
        });

        public SqlTable Order { get; } = AddTable(table =>
        {
            table.AddInt32("OrderHeaderId").SetForeignKeyTo(nameof(OrderHeader)).SetPK();
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
            table.AddInt32("Top1A");
            table.AddInt32("Top1B");
            table.AddInt32("Top2A");
            table.AddInt32("Top2B");

            table.SetForeignKeyTo(nameof(Order), new List<ColumnReference>()
            {
                new ColumnReference("Top1A", "OrderHeaderId"),
                new ColumnReference("Top1B", "LineNumber"),
            });

            table.SetForeignKeyTo(nameof(Order), new List<ColumnReference>()
            {
                new ColumnReference("Top2A", "OrderHeaderId"),
                new ColumnReference("Top2B", "LineNumber"),
            });
        });
    }
}
