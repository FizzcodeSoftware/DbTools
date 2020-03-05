﻿namespace FizzCode.DbTools.Console
{
    using System.Collections.Generic;
    using CommandDotNet;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Common.Logger;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.SqlExecuter;
    using FizzCode.DbTools.DataDefinitionDocumenter;
    using Microsoft.Extensions.Configuration;

    [Command(Name = ">")]
#pragma warning disable CA1812
    internal class AppCommands
    {
        [Command(Name = "exit", Description = "Exit from the command-line utility.")]
        public void Exit()
        {
            Program.Terminated = true;
        }

        [Command(Name = "document", Description = "Generate excel documentation of an existing database")]
        public void Document(
            [Option(LongName = "connectionString", ShortName = "c")]
            string connectionString,
            [Option(LongName = "sqlType", ShortName = "t")]
            string sqlType,
            [Option(LongName = "patternFileName", ShortName = "p")]
            string patternFileName,
            [Option(LongName = "flags", ShortName = "f")]
            List<DocumenterFlag> flags)
        {
            var version = SqlEngineVersions.GetVersion(sqlType);

            var connectionStringWithProvider = new ConnectionStringWithProvider(
                version.GetType().Name,
                version.ProviderName,
                connectionString,
                version.VersionString);

            var context = CreateContext(version);

            var sqlExecuter = SqlExecuterFactory.CreateSqlExecuter(connectionStringWithProvider, context);
            var databaseName = sqlExecuter.GetDatabase();

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(connectionStringWithProvider, context, null);

            var dd = ddlReader.GetDatabaseDefinition();

            var flagsSet = flags == null ? new HashSet<DocumenterFlag>() : new HashSet<DocumenterFlag>(flags);

            var documenterContext = CreateDocumenterContext(context, patternFileName);

            var documenter = new Documenter(documenterContext, version, databaseName, null, flagsSet);

            documenter.Document(dd);
        }

        [Command(Name = "generate", Description = "Generate database definition into cs files.")]
        public void Generate(
            [Option(LongName = "connectionString", ShortName = "c", Description = "Provide a valid connection string to the database")]
            string connectionString,
            [Option(LongName = "singleOrMulti", ShortName = "m", Description = "multi for multi file, single for single file generation")]
            string singleOrMulti,
            [Option(LongName = "sqlType", ShortName = "t")]
            string sqlType,
            [Option(LongName = "namespace", ShortName = "n")]
            string @namespace,
            [Option(LongName = "newDatabaseName", ShortName = "b")]
            string newDatabaseName,
            [Option(LongName = "patternFileName", ShortName = "p")]
            string patternFileName)
        {
            var version = SqlEngineVersions.GetVersion(sqlType);

            var connectionStringWithProvider = new ConnectionStringWithProvider(
                version.GetType().Name,
                version.ProviderName,
                connectionString,
                version.VersionString);

            var context = CreateContext(version);

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(connectionStringWithProvider, context, null);

            var dd = ddlReader.GetDatabaseDefinition();

            var documenterContext = CreateDocumenterContext(context, patternFileName);

            var writer = CSharpWriterFactory.GetCSharpWriter(version, documenterContext);
            var generator = new CSharpGenerator(documenterContext, writer, version, newDatabaseName, @namespace);

            if (singleOrMulti == "s" || singleOrMulti == "single")
                generator.GenerateSingleFile(dd, newDatabaseName + ".cs");
            else
                generator.GenerateMultiFile(dd);
        }

        [Command(Name = "bim", Description = "Generate database definition into bim (analysis services Model.bim xml) file.")]
        public void Bim(
            [Option(LongName = "connectionString", ShortName = "c")]
            string connectionString,
            [Option(LongName = "sqlType", ShortName = "t")]
            string sqlType,
            [Option(LongName = "databaseName", ShortName = "b")]
            string databaseName,
            [Option(LongName = "patternFileName", ShortName = "p")]
            string patternFileName)
        {
            var version = SqlEngineVersions.GetVersion(sqlType);

            var connectionStringWithProvider = new ConnectionStringWithProvider(
                version.GetType().Name,
                version.ProviderName,
                connectionString,
                version.VersionString);

            var context = CreateContext(version);

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(connectionStringWithProvider, context, null);

            var dd = ddlReader.GetDatabaseDefinition();

            var documenterContext = CreateDocumenterContext(context, patternFileName);

            var generator = new BimGenerator(documenterContext, version, databaseName);

            generator.Generate(dd);
        }

        private static Logger CreateLogger()
        {
            var logger = new Logger();

            var logConfiguration = Program.Configuration.GetSection("Log").Get<LogConfiguration>();

            var iLogger = SerilogConfigurator.CreateLogger(logConfiguration);
            var iOpsLogger = SerilogConfigurator.CreateOpsLogger(logConfiguration);

            var consoleLogger = new ConsoleLogger();
            consoleLogger.Init(iLogger, iOpsLogger);

            logger.LogEvent += consoleLogger.OnLog;

            return logger;
        }

        private static Context CreateContext(SqlEngineVersion version)
        {
            var context = new Context
            {
                Logger = CreateLogger(),
                Settings = Helper.GetDefaultSettings(version, Program.Configuration)
            };

            return context;
        }

        private static DocumenterContext CreateDocumenterContext(Context context, string patternFileName)
        {
            var documenterSettings = Program.Configuration.GetSection("Documenter").Get<DocumenterSettings>();

            ITableCustomizer customizer = null;

            if (patternFileName != null)
                customizer = PatternMatchingTableCustomizerFromPatterns.FromCsv(patternFileName, documenterSettings);

            customizer ??= new EmptyTableCustomizer();

            var documenterContext = new DocumenterContext
            {
                DocumenterSettings = documenterSettings,
                Settings = context.Settings,
                Logger = context.Logger,
                Customizer = customizer
            };
            return documenterContext;
        }

        [Command(Name = "dropall", Description = "Drop every object from a database.")]
        public void DropAll(
            [Option(LongName = "connectionString", ShortName = "c")]
            string connectionString,
            [Option(LongName = "sqlType", ShortName = "t")]
            string sqlType
            )
        {
            var version = SqlEngineVersions.GetVersion(sqlType);

            var connectionStringWithProvider = new ConnectionStringWithProvider("", version.ProviderName, connectionString, version.VersionString);

            var context = CreateContext(version);

            var generator = SqlGeneratorFactory.CreateGenerator(version, context);

            var executer = SqlExecuterFactory.CreateSqlExecuter(connectionStringWithProvider, generator);
            var dc = new DatabaseCreator(null, executer);

            dc.DropAllViews();
            dc.DropAllForeignKeys();
            dc.DropAllTables();
            // TODO needs databasedefinition
            // dc.DropAllSchemas();
        }
    }
}
