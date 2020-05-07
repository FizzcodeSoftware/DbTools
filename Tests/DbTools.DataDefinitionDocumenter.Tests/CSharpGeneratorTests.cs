﻿namespace FizzCode.DbTools.DataDefinitionDocumenter.Tests
{
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Tests;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CSharpGeneratorTests
    {
        [TestMethod]
        public void GeneratorTestDatabaseFks()
        {
            GeneratorTestDatabaseFks(MsSqlVersion.MsSql2016);
            GeneratorTestDatabaseFks(GenericVersion.Generic1);
            GeneratorTestDatabaseFks(OracleVersion.Oracle12c);
            GeneratorTestDatabaseFks(SqLiteVersion.SqLite3);
        }

        public static void GeneratorTestDatabaseFks(SqlEngineVersion version)
        {
            var db = new TestDatabaseFks();

            var documenterContext = DataDefinitionDocumenterTestsHelper.CreateTestGeneratorContext(version, new DocumenterTests.TableCustomizer());
            var writer = CSharpWriterFactory.GetCSharpWriter(version, documenterContext);
            var generator = new CSharpGenerator(writer, version, "TestDatabaseFks", "FizzCode.DbTools.DataDefinitionDocumenter.Tests");
            generator.GenerateMultiFile(db);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void GeneratorForeignKeyComposite(SqlEngineVersion version)
        {
            var db = new ForeignKeyComposite();

            var documenterContext = DataDefinitionDocumenterTestsHelper.CreateTestGeneratorContext(version);
            var writer = CSharpWriterFactory.GetCSharpWriter(version, documenterContext);
            var generator = new CSharpGenerator(writer, version, "TestDatabaseFks", "FizzCode.DbTools.DataDefinitionDocumenter.Tests");
            generator.GenerateMultiFile(db);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void GeneratorForeignKeyComposite1(SqlEngineVersion version)
        {
            var db = new ForeignKeyComposite();

            var documenterContext = DataDefinitionDocumenterTestsHelper.CreateTestGeneratorContext(version);
            var writer = CSharpWriterFactory.GetCSharpWriter(version, documenterContext);
            var generator = new CSharpGenerator(writer, version, "ForeignKeyComposite", "FizzCode.DbTools.DataDefinitionDocumenter.Tests");
            generator.GenerateMultiFile(db);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void GeneratorForeignKeyComposite2(SqlEngineVersion version)
        {
            var db = new ForeignKeyCompositeSetForeignKeyTo();
            var documenterContext = DataDefinitionDocumenterTestsHelper.CreateTestGeneratorContext(version, new DocumenterTests.TableCustomizer());
            var writer = CSharpWriterFactory.GetCSharpWriter(version, documenterContext);
            var generator = new CSharpGenerator(writer, version, "ForeignKeyCompositeSetForeignKeyTo", "FizzCode.DbTools.DataDefinitionDocumenter.Tests");
            generator.GenerateMultiFile(db);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void GeneratorIndex(SqlEngineVersion version)
        {
            var db = new TestDatabaseIndex();
            var documenterContext = DataDefinitionDocumenterTestsHelper.CreateTestGeneratorContext(version, new DocumenterTests.TableCustomizer());
            var writer = CSharpWriterFactory.GetCSharpWriter(version, documenterContext);
            var generator = new CSharpGenerator(writer, version, "TestDatabaseIndex", "FizzCode.DbTools.DataDefinitionDocumenter.Tests");
            generator.GenerateMultiFile(db);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void GeneratorUniqueIndex(SqlEngineVersion version)
        {
            var db = new TestDatabaseUniqueIndex();
            var documenterContext = DataDefinitionDocumenterTestsHelper.CreateTestGeneratorContext(version, new DocumenterTests.TableCustomizer());
            var writer = CSharpWriterFactory.GetCSharpWriter(version, documenterContext);
            var generator = new CSharpGenerator(writer, version, "TestDatabaseUniqueIndex", "FizzCode.DbTools.DataDefinitionDocumenter.Tests");
            generator.GenerateMultiFile(db);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void GeneratorUniqueConstraint(SqlEngineVersion version)
        {
            var db = new TestDatabaseUniqueConstraint();
            var documenterContext = DataDefinitionDocumenterTestsHelper.CreateTestGeneratorContext(version, new DocumenterTests.TableCustomizer());
            var writer = CSharpWriterFactory.GetCSharpWriter(version, documenterContext);
            var generator = new CSharpGenerator(writer, version, "TestDatabaseUniqueConstraint", "FizzCode.DbTools.DataDefinitionDocumenter.Tests");
            generator.GenerateMultiFile(db);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void GeneratorUniqueConstratintAsFk(SqlEngineVersion version)
        {
            var db = new DbUniqueConstratintAsFk();
            var documenterContext = DataDefinitionDocumenterTestsHelper.CreateTestGeneratorContext(version, new DocumenterTests.TableCustomizer());
            var writer = CSharpWriterFactory.GetCSharpWriter(version, documenterContext);
            var generator = new CSharpGenerator(writer, version, "DbUniqueConstratintAsFk", "FizzCode.DbTools.DataDefinitionDocumenter.Tests");
            generator.GenerateMultiFile(db);
        }

        [TestMethod]
        public void FkNoCheckTest()
        {
            var version = MsSqlVersion.MsSql2016;

            var dd = new TestDatabaseFkNoCheckTest();
            var documenterContext = DataDefinitionDocumenterTestsHelper.CreateTestGeneratorContext(version, new DocumenterTests.TableCustomizer());
            var writer = CSharpWriterFactory.GetCSharpWriter(version, documenterContext);
            var generator = new CSharpGenerator(writer, version, "TestDatabaseFkNoCheckTest", "FizzCode.DbTools.DataDefinitionDocumenter.Tests");
            generator.GenerateSingleFile(dd, "TestDatabaseFkNoCheckTest.cs");
        }
    }
}