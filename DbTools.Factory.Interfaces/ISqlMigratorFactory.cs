﻿namespace FizzCode.DbTools.Factory.Interfaces
{
    using FizzCode.DbTools.Interfaces;
    using FizzCode.LightWeight.AdoNet;

    public interface ISqlMigratorFactory
    {
        IDatabaseMigrator FromConnectionStringSettings(NamedConnectionString connectionString);
    }
}