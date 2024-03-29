﻿using System.Collections.Generic;
using FizzCode.DbTools.Common;
using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.DataDefinition.Base.Interfaces;

namespace FizzCode.DbTools.Interfaces;
public interface ISqlGenerator : ISqlGeneratorBase
{
    SqlStatementWithParameters CreateSchema(string schemaName);

    string CreateTable(SqlTable table);

    string? CreateForeignKeys(SqlTable table);

    string CreateForeignKey(ForeignKey fk);

    string CreateIndexes(SqlTable table);

    string CreateUniqueConstrainsts(SqlTable table);

    string CreateStoredProcedure(StoredProcedure sp);
    string CreateView(SqlView view);

    SqlStatementWithParameters? CreateDbTableDescription(SqlTable table);
    SqlStatementWithParameters? CreateDbColumnDescription(SqlColumn column);

    string DropTable(SqlTable table);

    string DropAllViews();
    string DropAllForeignKeys();
    string DropAllTables();
    SqlStatementWithParameters DropSchemas(List<string> schemaNames, bool hard = false);

    SqlStatementWithParameters TableExists(SqlTable table);
    string TableNotEmpty(SqlTable table);

    string GenerateCreateColumn(SqlColumn column);

    string GenerateType(ISqlType type);
}