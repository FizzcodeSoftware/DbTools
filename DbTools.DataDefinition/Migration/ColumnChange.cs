﻿namespace FizzCode.DbTools.DataDefinition.Migration
{
    using System.Collections.Generic;
    using System.Text;

    public class ColumnChange : ColumnMigration
    {
        public SqlColumn NewNameAndType { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("CC: New column: ");
            sb.AppendLine(NewNameAndType.ToString());
            sb.AppendLine(", Original column: ");
            sb.AppendLine(base.ToString());

            return sb.ToString();
        }

        private List<SqlColumnPropertyMigration> _sqlColumnPropertyMigrations;

        public List<SqlColumnPropertyMigration> SqlColumnPropertyMigrations => _sqlColumnPropertyMigrations ??= new List<SqlColumnPropertyMigration>();
    }
}
