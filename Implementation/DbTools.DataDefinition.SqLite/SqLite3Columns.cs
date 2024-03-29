﻿using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.DataDefinition.SqLite3;
public static class SqLite3Columns
{
    private static SqlColumn Add(SqlTable table, string name, SqlType sqlType)
    {
        return SqlColumnHelper.Add(SqLiteVersion.SqLite3, table, name, sqlType);
    }

    public static SqlColumn AddInteger(this SqlTable table, string name, bool isNullable = false)
    {
        var sqlType = new SqlType
        {
            SqlTypeInfo = SqLiteType3.Integer,
            IsNullable = isNullable
        };

        return Add(table, name, sqlType);
    }

    public static SqlColumn AddReal(this SqlTable table, string name, bool isNullable = false)
    {
        var sqlType = new SqlType
        {
            SqlTypeInfo = SqLiteType3.Real,
            IsNullable = isNullable
        };

        return Add(table, name, sqlType);
    }

    public static SqlColumn AddText(this SqlTable table, string name, bool isNullable = false)
    {
        var sqlType = new SqlType
        {
            SqlTypeInfo = SqLiteType3.Text,
            IsNullable = isNullable
        };

        return Add(table, name, sqlType);
    }

    public static SqlColumn AddBlob(this SqlTable table, string name, bool isNullable = false)
    {
        var sqlType = new SqlType
        {
            SqlTypeInfo = SqLiteType3.Blob,
            IsNullable = isNullable
        };

        return Add(table, name, sqlType);
    }
}