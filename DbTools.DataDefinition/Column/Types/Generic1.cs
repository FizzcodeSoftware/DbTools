﻿namespace FizzCode.DbTools.DataDefinition.Generic1
{
    public static class Generic1
    {
        public static SqlTypeInfo GetSqlTypeInfo(string name)
        {
            return GenericInfo.Get(new Configuration.Generic1())[name];
        }

        public static SqlColumn AddChar(this SqlTable table, string name, int length, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("CHAR"),
                Length = length,
                IsNullable = isNullable
            };

            return SqlColumnHelper.Add(table, name, sqlType);
        }

        public static SqlColumn AddNChar(this SqlTable table, string name, int length, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("NCHAR"),
                Length = length,
                IsNullable = isNullable
            };

            return SqlColumnHelper.Add(table, name, sqlType);
        }

        public static SqlColumn AddVarChar(this SqlTable table, string name, int length, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("VARCHAR"),
                Length = length,
                IsNullable = isNullable
            };

            return SqlColumnHelper.Add(table, name, sqlType);
        }

        public static SqlColumn AddNVarChar(this SqlTable table, string name, int length, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("NVARCHAR"),
                Length = length,
                IsNullable = isNullable
            };

            return SqlColumnHelper.Add(table, name, sqlType);
        }

        public static SqlColumn AddPK(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("DEFAULTPK"),
                IsNullable = isNullable
            };

            return SqlColumnHelper.Add(table, name, sqlType);
        }

        public static SqlColumn AddFloatSmall(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("FLOAT_SMALL"),
                IsNullable = isNullable
            };

            return SqlColumnHelper.Add(table, name, sqlType);
        }

        public static SqlColumn AddFloatLarge(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("FLOAT_LARGE"),
                IsNullable = isNullable
            };

            return SqlColumnHelper.Add(table, name, sqlType);
        }

        public static SqlColumn AddBit(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("BIT"),
                IsNullable = isNullable
            };

            return SqlColumnHelper.Add(table, name, sqlType);
        }

        public static SqlColumn AddByte(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("BYTE"),
                IsNullable = isNullable
            };

            return SqlColumnHelper.Add(table, name, sqlType);
        }

        public static SqlColumn AddInt16(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("INT16"),
                IsNullable = isNullable
            };

            return SqlColumnHelper.Add(table, name, sqlType);
        }

        public static SqlColumn AddInt32(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("INT32"),
                IsNullable = isNullable
            };

            return SqlColumnHelper.Add(table, name, sqlType);
        }

        public static SqlColumn AddInt64(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("INT64"),
                IsNullable = isNullable
            };

            return SqlColumnHelper.Add(table, name, sqlType);
        }

        public static SqlColumn AddNumber(this SqlTable table, string name, int? length, int? scale, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("INT64"),
                Length = length,
                Scale = scale,
                IsNullable = isNullable
            };

            return SqlColumnHelper.Add(table, name, sqlType);
        }

        public static SqlColumn AddDate(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("DATE"),
                IsNullable = isNullable
            };

            return SqlColumnHelper.Add(table, name, sqlType);
        }

        public static SqlColumn AddTime(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("TIME"),
                IsNullable = isNullable
            };

            return SqlColumnHelper.Add(table, name, sqlType);
        }

        public static SqlColumn AddDateTime(this SqlTable table, string name, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GetSqlTypeInfo("DATETIME"),
                IsNullable = isNullable
            };

            return SqlColumnHelper.Add(table, name, sqlType);
        }
    }
}