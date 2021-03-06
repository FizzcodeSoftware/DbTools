﻿namespace FizzCode.DbTools.DataDefinition.Migration
{
    public abstract class ColumnMigration : IMigration
    {
        public SqlColumn SqlColumn { get; set; }

        public override string ToString()
        {
            return SqlColumn.ToString();
        }
    }
}
