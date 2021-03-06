﻿namespace FizzCode.DbTools.QueryBuilder
{
    using System.Linq;
    using FizzCode.DbTools.DataDefinition;

    public class QueryColumn
    {
        public QueryColumn()
        {
        }

        public QueryColumn(QueryColumn column, string @as)
        {
            Value = column.Value;
            Alias = column.Alias;
            As = @as;
            IsDbColumn = column.IsDbColumn;
        }

        public QueryColumn(string value, string alias)
        {
            Value = value;
            As = alias;
        }

        public string Value { get; set; }

        /// <summary>
        /// The name of the column, if different from Value (ex. ou.OrgUnitId AS 'MyId').
        /// </summary>
        public string As { get; set; }

        /// <summary>
        /// The alias for the table.
        /// </summary>
        public string Alias { get; set; }

        public bool IsDbColumn { get; set; }

        public static implicit operator QueryColumn(SqlColumn column)
        {
            var queryColumn = new QueryColumn
            {
                Value = column.Name,
                IsDbColumn = true,
            };

            var aliasTableProperty = column.Table.Properties.OfType<AliasTableProperty>().FirstOrDefault();
            if (aliasTableProperty != null)
                queryColumn.Alias = aliasTableProperty.Alias;

            return queryColumn;
        }

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(Value);
            if (As != null)
            {
                sb.Append(" AS ");
                sb.Append(As);
            }
            if (IsDbColumn)
            {
                sb.Append(" (DbColumn)");
            }

            return sb.ToString();
        }
    }
}
