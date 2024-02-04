﻿namespace FizzCode.DbTools.DataDefinition.Base;

public class UniqueConstraint : IndexBase<SqlTable>
{
    public SqlTable SqlTable { get => SqlTableOrView; }

    public UniqueConstraint(SqlTable sqlTable, string name)
        : base(sqlTable, name, true)
    {
    }

    public new bool Unique
    {
        get => true;

        set
        {
            if (!value)
                throw new ArgumentException("Unique Constraint is always Unique.");
        }
    }

    public override string ToString()
    {
        return $"{GetColumnsInString()} on {SqlTableOrView.SchemaAndTableName}";
    }
}
