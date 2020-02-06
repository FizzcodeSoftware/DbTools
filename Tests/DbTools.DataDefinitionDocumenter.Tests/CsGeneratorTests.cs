﻿namespace FizzCode.DbTools.DataDefinitionDocumenter.Tests
{
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition.Tests;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CsGeneratorTests
    {
        [TestMethod]
        public void GeneratorTestDatabaseFks()
        {
            GeneratorTestDatabaseFks(SqlVersions.MsSql2016);
            GeneratorTestDatabaseFks(SqlVersions.Generic1);
            GeneratorTestDatabaseFks(SqlVersions.Oracle12c);
            GeneratorTestDatabaseFks(SqlVersions.SqLite3);
        }

        public void GeneratorTestDatabaseFks(SqlVersion version)
        {
            var db = new TestDatabaseFks();

            var generator = new CsGenerator(DataDefinitionDocumenterTestsHelper.CreateTestContext(new DocumenterTests.TableCustomizer()), version, "TestDatabaseFks", "FizzCode.DbTools.DataDefinitionDocumenter.Tests");
            generator.GenerateMultiFile(db);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void GeneratorForeignKeyComposite(SqlVersion version)
        {
            var db = new ForeignKeyComposite();

            var generator = new CsGenerator(DataDefinitionDocumenterTestsHelper.CreateTestContext(), version, "TestDatabaseFks", "FizzCode.DbTools.DataDefinitionDocumenter.Tests");
            generator.GenerateMultiFile(db);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void GeneratorForeignKeyComposite1(SqlVersion version)
        {
            var db = new ForeignKeyComposite();

            var generator = new CsGenerator(DataDefinitionDocumenterTestsHelper.CreateTestContext(), version, "ForeignKeyComposite", "FizzCode.DbTools.DataDefinitionDocumenter.Tests");
            generator.GenerateMultiFile(db);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void GeneratorForeignKeyComposite2(SqlVersion version)
        {
            var db = new ForeignKeyCompositeSetForeignKeyTo();
            var generator = new CsGenerator(DataDefinitionDocumenterTestsHelper.CreateTestContext(new DocumenterTests.TableCustomizer()), version, "ForeignKeyCompositeSetForeignKeyTo", "FizzCode.DbTools.DataDefinitionDocumenter.Tests");
            generator.GenerateMultiFile(db);
        }
    }
}