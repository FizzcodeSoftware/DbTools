﻿namespace FizzCode.DbTools.DataDefinitionDocumenter.Tests
{
    using System.IO;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Tests;
    using FizzCode.DbTools.DataDefinitionDocumenter;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PatternMatchingTableCustomizerTests
    {
        [TestMethod]
        public void GetPatternMatching_StartsWith_Test()
        {
            var pm = new PatternMatchingTableCustomizer();
            pm.AddPattern(null, "A*", null, null, true, null, null);

            Assert.IsTrue(pm.ShouldSkip("A"));
            Assert.IsTrue(pm.ShouldSkip("Aaa"));
            Assert.IsTrue(pm.ShouldSkip("Abb"));
            Assert.IsFalse(pm.ShouldSkip("B"));
            Assert.IsFalse(pm.ShouldSkip("Baa"));
            Assert.IsFalse(pm.ShouldSkip("Bbb"));
        }

        [TestMethod]
        public void GetPatternMatching_Contains_Test()
        {
            var pm = new PatternMatchingTableCustomizer();
            pm.AddPattern(null, "*apple*", null, null, true, null, null);

            Assert.IsTrue(pm.ShouldSkip("xapplex"));
            Assert.IsTrue(pm.ShouldSkip("apple"));
            Assert.IsTrue(pm.ShouldSkip("applex"));

            Assert.IsTrue(pm.ShouldSkip("The quick brown fox jumps over the lazy apple dog"));

            Assert.IsFalse(pm.ShouldSkip("appl"));
            Assert.IsFalse(pm.ShouldSkip("pple"));
            Assert.IsFalse(pm.ShouldSkip("xxx"));
        }

        [TestMethod]
        public void GetPatternMatching_Dot_Test()
        {
            var pm = new PatternMatchingTableCustomizer();
            pm.AddPattern(null, "ap?le*", null, null, true, null, null);

            Assert.IsTrue(pm.ShouldSkip("apple"));
            Assert.IsTrue(pm.ShouldSkip("apxle"));

            Assert.IsFalse(pm.ShouldSkip("The quick brown fox jumps over the lazy apXle dog"));

            Assert.IsTrue(pm.ShouldSkip("apXle The quick brown fox jumps over the lazy dog"));

            Assert.IsFalse(pm.ShouldSkip("appl"));
            Assert.IsFalse(pm.ShouldSkip("pple"));
            Assert.IsFalse(pm.ShouldSkip("xxx"));
        }

        [TestMethod]
        public void PatternMatching_Schema()
        {
            var pm = new PatternMatchingTableCustomizer();
            pm.AddPattern("apple", null, null, null, true, null, null);

            Assert.IsTrue(pm.ShouldSkip(new SchemaAndTableName("apple", "anything")));
            Assert.IsFalse(pm.ShouldSkip(new SchemaAndTableName("horus", "apple")));
        }

        [TestMethod]
        public void PatternMatching_Schema_And_Table()
        {
            var pm = new PatternMatchingTableCustomizer();
            pm.AddPattern("horus", "apple", null, null, true, null, null);

            Assert.IsFalse(pm.ShouldSkip(new SchemaAndTableName("apple", "anything")));
            Assert.IsTrue(pm.ShouldSkip(new SchemaAndTableName("horus", "apple")));
            Assert.IsFalse(pm.ShouldSkip(new SchemaAndTableName("horus", "horus")));
        }

        [TestMethod]
        public void PatternMatching_Schema_And_Table_Except_WildCard()
        {
            var pm = new PatternMatchingTableCustomizer();
            pm.AddPattern("horus*", "apple*", "horusExcept*", "appleExcept*", true, null, null);

            Assert.IsFalse(pm.ShouldSkip(new SchemaAndTableName("apple", "anything")));
            Assert.IsTrue(pm.ShouldSkip(new SchemaAndTableName("horus", "apple")));
            Assert.IsFalse(pm.ShouldSkip(new SchemaAndTableName("horus", "horus")));

            Assert.IsFalse(pm.ShouldSkip(new SchemaAndTableName("horusExcept1", "nothing")));

            Assert.IsTrue(pm.ShouldSkip(new SchemaAndTableName("horusExcept", "appleExcept")));
            Assert.IsTrue(pm.ShouldSkip(new SchemaAndTableName("horusExcept1", "appleExcept1")));
            Assert.IsTrue(pm.ShouldSkip(new SchemaAndTableName("horusExcept2", "appleExcept2")));
        }

        [TestMethod]
        public void PatternMatching_Schema_And_Table_Except()
        {
            var pm = new PatternMatchingTableCustomizer();
            pm.AddPattern("horus*", "apple*", "horusExcept", "appleExcept", true, null, null);

            Assert.IsFalse(pm.ShouldSkip(new SchemaAndTableName("apple", "anything")));
            Assert.IsTrue(pm.ShouldSkip(new SchemaAndTableName("horus", "apple")));
            Assert.IsFalse(pm.ShouldSkip(new SchemaAndTableName("horus", "horus")));

            Assert.IsFalse(pm.ShouldSkip(new SchemaAndTableName("horusExcept1", "nothing")));

            Assert.IsTrue(pm.ShouldSkip(new SchemaAndTableName("horusExcept", "appleExcept")));
            Assert.IsTrue(pm.ShouldSkip(new SchemaAndTableName("horusExcept1", "appleExcept1")));
            Assert.IsTrue(pm.ShouldSkip(new SchemaAndTableName("horusExcept2", "appleExcept2")));
        }

        [TestMethod]
        public void PatternMatchingTableCustomizerFromCsvTest()
        {
            using (var file =
            new StreamWriter("TestDatabaseFks.DbTools.Patterns.csv"))
            {
                file.WriteLine("PatternSchema;PatternTableName;PatternExceptSchema;PatternExceptTableName;ShouldSkipIfMatch;CategoryIfMatch;BackGroundColorIfMatch");
                file.WriteLine(";Parent;;;0;Parent;606060");
                file.WriteLine(";Child;;;1");
                file.WriteLine(";*ildC*;;;0;TestCategory");
            }

            var db = new TestDatabaseFks();
            db.SetVersions(GenericVersion.Generic1.GetTypeMapper());
            var patternMatching = PatternMatchingTableCustomizerFromPatterns.FromCsv("TestDatabaseFks", null);
            var documenter = new Documenter(DataDefinitionDocumenterTestsHelper.CreateTestDocumenterContext(GenericVersion.Generic1, patternMatching), GenericVersion.Generic1, "TestDatabaseFks");
            documenter.Document(db);
        }

        [TestMethod()]
        public void ShouldSkipTest()
        {
            // Pattern; PatternExcept; ShouldSkipIfMatch; CategoryIfMatch; BackGroundColorIfMatch

            const string patternContent = ";skip*;;;1;";

            var customizer = PatternMatchingTableCustomizerFromPatterns.FromString(patternContent);

            var schemaAndTableName1 = new SchemaAndTableName("dbo", "skipMe");
            var schemaAndTableName2 = new SchemaAndTableName("dbo", "dontSkipMe");

            var shouldSkip1 = customizer.ShouldSkip(schemaAndTableName1);
            var shouldSkip2 = customizer.ShouldSkip(schemaAndTableName2);

            Assert.IsTrue(shouldSkip1);
            Assert.IsFalse(shouldSkip2);
        }

        [TestMethod()]
        public void ShouldSkipTest2()
        {
            const string patternContent = "staging;;;;1";

            var customizer = PatternMatchingTableCustomizerFromPatterns.FromString(patternContent);

            var schemaAndTableName1 = new SchemaAndTableName("staging", "apple");
            var schemaAndTableName2 = new SchemaAndTableName("staging", "");
            var schemaAndTableName3 = new SchemaAndTableName("staging", null);

            var shouldSkip1 = customizer.ShouldSkip(schemaAndTableName1);
            var shouldSkip2 = customizer.ShouldSkip(schemaAndTableName2);
            var shouldSkip3 = customizer.ShouldSkip(schemaAndTableName3);

            Assert.IsTrue(shouldSkip1);
            Assert.IsTrue(shouldSkip2);
            Assert.IsTrue(shouldSkip3);
        }
    }
}