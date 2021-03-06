﻿namespace FizzCode.DbTools.DataDefinitionReader.Tests
{
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public abstract class DataDefinitionReaderTests
    {
        protected static readonly SqlExecuterTestAdapter _sqlExecuterTestAdapter = new SqlExecuterTestAdapter();

        protected static void Init(SqlEngineVersion version, DatabaseDefinition dd)
        {
            _sqlExecuterTestAdapter.Check(version);
            if (dd == null)
                _sqlExecuterTestAdapter.Initialize(version.UniqueName);
            else
                _sqlExecuterTestAdapter.Initialize(version.UniqueName, dd);

            TestHelper.CheckFeature(version, "ReadDdl");

            _sqlExecuterTestAdapter.GetContext(version).Settings.Options.ShouldUseDefaultSchema = true;
        }

        [AssemblyCleanup]
        public static void Cleanup()
        {
            _sqlExecuterTestAdapter.Cleanup();
        }
    }
}