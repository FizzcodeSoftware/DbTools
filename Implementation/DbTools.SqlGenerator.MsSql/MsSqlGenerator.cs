﻿namespace FizzCode.DbTools.SqlGenerator.MsSql
{
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.SqlGenerator.Base;

    public class MsSqlGenerator : AbstractSqlGeneratorBase
    {
        public MsSqlGenerator(Context context)
            : base(context)
        {
        }

        public override string GuardKeywords(string name)
        {
            return $"[{name}]";
        }
    }
}