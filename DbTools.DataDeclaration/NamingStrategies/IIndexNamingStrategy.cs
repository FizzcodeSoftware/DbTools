﻿namespace FizzCode.DbTools.DataDeclaration
{
    using FizzCode.DbTools.DataDefinition.Base;

    public interface IIndexNamingStrategy : INamingStrategy
    {
        void SetIndexName(Index index);
    }

    public interface IUniqueConstraintNamingStrategy : INamingStrategy
    {
        void SetUniqueConstraintName(UniqueConstraint uniqueConstraint);
    }
}