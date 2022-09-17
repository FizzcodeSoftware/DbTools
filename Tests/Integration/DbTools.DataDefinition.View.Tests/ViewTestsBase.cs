﻿// Ensure no in-assembly parallel execution of tests (“IAP”) is happening
[assembly: Microsoft.VisualStudio.TestTools.UnitTesting.Parallelize(Workers = 1, Scope = Microsoft.VisualStudio.TestTools.UnitTesting.ExecutionScope.ClassLevel)]

namespace FizzCode.DbTools.DataDefinition.View.Tests
{
    using FizzCode.DbTools.SqlExecuter;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ViewTestsBase
    {
        protected static SqlExecuterTestAdapter SqlExecuterTestAdapter { get; } = new();

        [AssemblyCleanup]
        public static void Cleanup()
        {
            // SqlExecuterTestAdapter.Cleanup();
        }

        protected static void Init(SqlEngineVersion version, DatabaseDefinition dd)
        {
            SqlExecuterTestAdapter.Check(version);
            SqlExecuterTestAdapter.Initialize(version.UniqueName, dd);

            SqlExecuterTestAdapter.GetContext(version).Settings.Options.ShouldUseDefaultSchema = true;

            var databaseCreator = new DatabaseCreator(dd, SqlExecuterTestAdapter.GetExecuter(version.UniqueName));

            databaseCreator.ReCreateDatabase(true);
        }
    }
}
