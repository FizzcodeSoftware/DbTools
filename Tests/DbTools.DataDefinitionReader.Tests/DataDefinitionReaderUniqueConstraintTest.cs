﻿#pragma warning disable CA1034 // Nested types should not be visible

namespace FizzCode.DbTools.DataDefinitionReader.Tests
{
    using System.Linq;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Generic1;
    using FizzCode.DbTools.DataDefinition.SqlExecuter;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DataDefinitionReaderUniqueConstraintTest : DataDefinitionReaderTests
    {
        [DataTestMethod]
        // [SqlVersions(typeof(MsSql2016))]
        //public void CreateTables(SqlVersion version)
        public void CreateTables()
        {
            var version = MsSqlVersion.MsSql2016;
            var dd = new TestDatabaseUniqueConstraint();
            Init(version, dd);
            var creator = new DatabaseCreator(dd, _sqlExecuterTestAdapter.GetExecuter(version.UniqueName));
            creator.ReCreateDatabase(true);
        }

        [DataTestMethod]
        //[SqlVersions(typeof(MsSql2016))]
        //public void ReadTables(SqlVersion version)
        public void ReadTables()
        {
            var version = MsSqlVersion.MsSql2016;

            Init(version, null);

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(
                _sqlExecuterTestAdapter.ConnectionStrings[version.UniqueName],
                _sqlExecuterTestAdapter.GetContext(version), null);
            var dd = ddlReader.GetDatabaseDefinition();

            var _ = dd.GetTable("Company").Properties.OfType<UniqueConstraint>().First();
        }

        public class TestDatabaseUniqueConstraint : TestDatabaseDeclaration
        {
            public SqlTable Company { get; } = AddTable(table =>
            {
                table.AddInt32("Id").SetPK().SetIdentity();
                table.AddNVarChar("Name", 100);
                table.AddUniqueConstraint("Name");
            });
        }
    }
}