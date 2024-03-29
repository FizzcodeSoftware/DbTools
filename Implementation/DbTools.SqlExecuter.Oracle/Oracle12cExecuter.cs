﻿using System.Data.Common;
using FizzCode.LightWeight;
using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.Common;
using FizzCode.DbTools.Interfaces;
using System.Collections.Generic;
using System;
using System.Linq;

namespace FizzCode.DbTools.SqlExecuter.Oracle;
public class Oracle12cExecuter(ContextWithLogger context, NamedConnectionString connectionString, ISqlGenerator sqlGenerator)
    : SqlStatementExecuter(context, connectionString, sqlGenerator)
{
    public override DbConnection OpenConnectionMaster()
    {
        return OpenConnection();
    }

    public override string GetDatabase()
    {
        var oracleDatabaseName = Context.Settings.SqlVersionSpecificSettings.GetAs<string>("OracleDatabaseName");
        Throw.InvalidOperationExceptionIfNull(oracleDatabaseName);
        return oracleDatabaseName;
    }

    public override void InitializeDatabase(bool dropIfExists, params IDatabaseDefinition[] dds)
    {
        var defaultSchema = Context.Settings.SqlVersionSpecificSettings.GetAs<string>("DefaultSchema");

        if (dropIfExists)
        {
            var allSchemas = dds.SelectMany(dd => dd.GetSchemaNames().ToList()).ToList();

            Throw.InvalidOperationExceptionIfNull(defaultSchema);
            allSchemas.Add(defaultSchema);

            foreach (var schema in allSchemas)
            {
                if (CheckIfUserExists(schema))
                {
                    var schemasList = new List<string>
                    {
                        schema
                    };

                    var sql = Generator.DropSchemas(schemasList, true);
                    ExecuteNonQuery(sql);
                }
            }
        }

        // TODO password
        ExecuteNonQuery($"CREATE USER \"{defaultSchema}\" IDENTIFIED BY sa123");
        ExecuteNonQuery($"GRANT CONNECT, DBA TO \"{defaultSchema}\"");
        ExecuteNonQuery($"GRANT CREATE SESSION TO \"{defaultSchema}\"");
        ExecuteNonQuery($"GRANT UNLIMITED TABLESPACE TO \"{defaultSchema}\"");

        /*var builder = GetConnectionStringBuilder();
        builder.ConnectionString = ConnectionString;
        var connect_identifier = "//" + builder.ValueOfKey("DATA SOURCE");
        ExecuteQuery($"CONNECT {defaultSchema}/sa123@{connect_identifier}");*/

        ExecuteNonQuery($"ALTER SESSION SET current_schema = \"{defaultSchema}\"");
    }

    public static SqlStatementWithParameters IfExists(string table, string column, object value)
    {
        return new SqlStatementWithParameters($@"
SELECT CASE WHEN MAX({column}) IS NULL THEN 1 ELSE 0 END
FROM {table}
WHERE {column} = @value", value);
    }

    public bool CheckIfUserExists(string userName)
    {
        var result = ExecuteScalar(IfExists("dba_users", "username", userName));
        return (decimal)result == 0;
    }

    public override void CleanupDatabase(bool hard, params IDatabaseDefinition[] dds)
    {
        var allSchemas = dds.SelectMany(dd => dd.GetSchemaNames().ToList()).ToList();
        if (allSchemas.Count > 0)
        {
            var sql = Generator.DropSchemas(allSchemas, hard);
            ExecuteNonQuery(sql);
        }

        var defaultSchema = Context.Settings.SqlVersionSpecificSettings.GetAs<string>("DefaultSchema");
        Throw.InvalidOperationExceptionIfNull(defaultSchema);
        var currentUser = ExecuteQuery("select user from dual")[0].GetAs<string>("USER");

        ExecuteNonQuery($"ALTER SESSION SET current_schema = {currentUser}");

        if (CheckIfUserExists(defaultSchema))
            ExecuteNonQuery($"DROP USER \"{defaultSchema}\" CASCADE");
    }

    public override DbCommand PrepareSqlCommand(SqlStatementWithParameters sqlStatementWithParameters)
    {
        var dbCommand = base.PrepareSqlCommand(sqlStatementWithParameters);
        return OracleDbCommandPreparer.PrepareSqlCommand(dbCommand);
    }

    public override void ExecuteNonQuery(SqlStatementWithParameters sqlStatementWithParameters)
    {
        var sqlStatements = BreakIfMultipleCommands(sqlStatementWithParameters);
        foreach (var sqlStatement in sqlStatements)
            base.ExecuteNonQuery(sqlStatement);
    }

    protected override void ExecuteNonQueryMaster(SqlStatementWithParameters sqlStatementWithParameters)
    {
        ExecuteQuery(sqlStatementWithParameters);
    }

    private static List<SqlStatementWithParameters> BreakIfMultipleCommands(SqlStatementWithParameters sqlStatementWithParameters)
    {
        var sqlStatements = new List<SqlStatementWithParameters>();

        if (!(sqlStatementWithParameters.Statement.Trim().StartsWith("BEGIN", StringComparison.InvariantCultureIgnoreCase)
            && sqlStatementWithParameters.Statement.Trim().EndsWith("END;", StringComparison.InvariantCultureIgnoreCase)))
        {
            var count = 0;
            foreach (var c in sqlStatementWithParameters.Statement)
            {
                if (c == ';')
                    count++;

                if (count > 1)
                    break;
            }

            var sqlStatementTrimEnd = sqlStatementWithParameters.Statement.TrimEnd();

            if (count == 1 && sqlStatementTrimEnd[^1] == ';')
            {
                sqlStatementWithParameters.Statement = sqlStatementTrimEnd.Remove(sqlStatementTrimEnd.Length - 1);
                sqlStatements.Add(sqlStatementWithParameters);
            }
            else if (count > 1)
            {
                foreach (var subStatement in sqlStatementWithParameters.Statement.Split(';'))
                {
                    if (subStatement.Trim().Length > 0)
                        sqlStatements.Add(new SqlStatementWithParameters(subStatement, sqlStatementWithParameters.Parameters));
                }
            }
            else
            {
                sqlStatements.Add(sqlStatementWithParameters);
            }
        }
        else
        {
            sqlStatements.Add(sqlStatementWithParameters);
        }

        return sqlStatements;
    }
}