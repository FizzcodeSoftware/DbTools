﻿namespace FizzCode.DbTools.DataDefinitionDocumenter.Tests
{
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Tests;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DocumenterTests
    {
        [TestMethod]
        public void DocumentTest()
        {
            var db = new TestDatabaseFks();
            var documenter = new Documenter(TestHelper.GetDefaultTestSettings(SqlDialect.MsSql),  "TestDatabaseFks");
            documenter.Document(db);
        }

        [TestMethod]
        public void TableCustomizerTest()
        {
            var db = new TestDatabaseFks();
            var documenter = new Documenter(TestHelper.GetDefaultTestSettings(SqlDialect.MsSql), "TestDatabaseFks", new TableCustomizer());
            documenter.Document(db);
        }

        [TestMethod]
        public void DocumentTestForeignKeyComposite()
        {
            var db = new TestDataBaseForeignKeyComposite();
            var documenter = new Documenter(TestHelper.GetDefaultTestSettings(SqlDialect.MsSql), "ForeignKeyCompositeTestsDb");
            documenter.Document(db);
        }

        public class TableCustomizer : ITableCustomizer
        {
            public string BackGroundColor(SchemaAndTableName tableName)
            {
                if (tableName.SchemaAndName == "Child")
                    return "#00FFFF";

                return null;
            }

            public string Category(SchemaAndTableName tableName)
            {
                if (tableName.SchemaAndName == "Child")
                    return "CategoryTest";

                return null;
            }

            public bool ShouldSkip(SchemaAndTableName tableName)
            {
                return tableName.SchemaAndName == "ChildChild";
            }
        }

        [TestMethod]
        public void GeneratorTestDatabaseFks()
        {
            var db = new TestDatabaseFks();
            var generator = new CsGenerator(TestHelper.GetDefaultTestSettings(SqlDialect.MsSql), "TestDatabaseFks", "FizzCode.DbTools.DataDefinitionDocumenter.Tests", new TableCustomizer());
            generator.Generate(db);
        }

        [TestMethod]
        public void GeneratorForeignKeyCompositeTestsDb()
        {
            var db = new TestDataBaseForeignKeyComposite();
            var generator = new CsGenerator(TestHelper.GetDefaultTestSettings(SqlDialect.MsSql), "TestDatabaseFks", "FizzCode.DbTools.DataDefinitionDocumenter.Tests", new TableCustomizer());
            generator.Generate(db);
        }
    }
}