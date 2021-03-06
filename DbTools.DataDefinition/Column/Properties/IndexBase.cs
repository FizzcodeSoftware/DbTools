﻿namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections.Generic;
    using System.Linq;

    public abstract class IndexBase : SqlTableProperty
    {
        public string Name { get; set; }

        public List<ColumnAndOrder> SqlColumns { get; set; } = new List<ColumnAndOrder>();

        public bool Unique { get; set; }
        public bool? Clustered { get; set; }

        protected IndexBase(SqlTable sqlTable, string name, bool unique = false)
            : base(sqlTable)
        {
            Name = name;
            Unique = unique;
        }

        protected string GetColumnsInString(bool withOrder = false)
        {
            if (withOrder)
                return string.Join(", ", SqlColumns);

            return string.Join(", ", SqlColumns.Select(cao => cao.SqlColumn.Name));
        }
    }
}
