﻿namespace FizzCode.DbTools.DataDefinition.Base
{
    using System.Collections.Generic;
    using System.Linq;


    public abstract class IndexBase<T> : SqlTableOrViewPropertyBase<T> where T : SqlTableOrView
    {
        public string Name { get; set; }

        public List<ColumnAndOrder> SqlColumns { get; set; } = new List<ColumnAndOrder>();

        public bool Unique { get; set; }
        public bool? Clustered { get; set; }

        protected IndexBase(T sqlTable, string name, bool unique = false)
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