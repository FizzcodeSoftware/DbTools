﻿namespace FizzCode.DbTools.DataDefinitionReader.Tests
{
    using System.Linq;
    using FizzCode.DbTools.DataDefinition.Tests;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FizzCode.DbTools.DataDefinition.Base;

    [TestClass]
    public class DataDefinitionReaderIndexTest : DataDefinitionReaderTests
    {
        [DataTestMethod]
        [LatestSqlVersions]
        public void ReadTables(SqlEngineVersion version)
        {
            TestHelper.CheckFeature(version, "ReadDdl");

            Init(version, new TestDatabaseIndex());

            var dd = ReadDd(version, null);

            var _ = dd.GetTable("Company").Properties.OfType<Index>().First();
        }
    }
}