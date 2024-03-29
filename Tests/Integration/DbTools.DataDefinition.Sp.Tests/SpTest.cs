﻿using FizzCode.DbTools.Common;
using FizzCode.DbTools.DataDeclaration;
using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.QueryBuilder;
using FizzCode.DbTools.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FizzCode.DbTools.DataDefinition.Sp.Tests;
[TestClass]
public class SpTest : SpTestsBase
{
    [TestMethod]
    [SqlVersions(nameof(MsSql2016))]
    public void SpSimple(SqlEngineVersion version)
    {
        var db = new DbWithSp();
        Init(version, db);

        SqlExecuterTestAdapter.GetExecuter(version.UniqueName).ExecuteQuery("GetCompanies");
    }

    public class DbWithSp : DatabaseDeclaration
    {
        public DbWithSp()
            : base(new TestFactoryContainer(), MsSqlVersion.MsSql2016, [OracleVersion.Oracle12c])
        {
        }

        public CompanyTable Company { get; } = new CompanyTable();

        public StoredProcedure GetCompanies { get; } = new StoredProcedure("SELECT Id, Name FROM Company");

        public class CompanyTable : SqlTable
        {
            public SqlColumn Id { get; } = MsSql2016.MsSql2016.AddInt().SetPK().SetIdentity();
            public SqlColumn Name { get; } = MsSql2016.MsSql2016.AddNVarChar(100);
        }
    }

    [TestMethod]
    [SqlVersions(nameof(MsSql2016))]
    public void SpSimpleQuryBuilder(SqlEngineVersion version)
    {
        var db = new DbWithSpQueryBuilder();
        Init(version, db);

        var sqlStatementWithParameters = new SqlStatementWithParameters("EXEC GetCompaniesWithParameter @Id=@Id");
        sqlStatementWithParameters.Parameters.Add("@Id", 1);

        SqlExecuterTestAdapter.GetExecuter(version.UniqueName).ExecuteQuery(sqlStatementWithParameters);
    }

    public class DbWithSpQueryBuilder : TestDatabaseDeclaration
    {
        public CompanyTable Company { get; } = new CompanyTable();

        public StoredProcedure GetCompanies => new StoredProcedureFromQuery(new Query(Company));

        public StoredProcedure GetCompaniesWithParameter => new StoredProcedureFromQuery(
                    new Query(Company).Where(Company.Id, "= @Id"),
                    Company.Id);

        public class CompanyTable : SqlTable
        {
            public SqlColumn Id { get; } = MsSql2016.MsSql2016.AddInt().SetPK().SetIdentity();
            public SqlColumn Name { get; } = MsSql2016.MsSql2016.AddNVarChar(100);
        }

        public Query GetCompaniesQuery => new(Company);
    }
}
