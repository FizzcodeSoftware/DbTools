﻿namespace FizzCode.DbTools.TestBase
{
    using System.Collections.Generic;
    using FizzCode.DbTools.DataDefinition.Base.Interfaces;
    using FizzCode.DbTools.DataDefinition.Factory.Interfaces;
    using FizzCode.DbTools.Factory;

    public static class TypeMapperGetter
    {
        public static ITypeMapper GetTypeMapper(SqlEngineVersion version)
        {
            var root = new Root();
            var typeMapperFactory = root.Get<ITypeMapperFactory>();

            return typeMapperFactory.GetTypeMapper(version);
        }

        public static ITypeMapper[] GetTypeMappers(params SqlEngineVersion[] versions)
        {
            var typeMappers = new List<ITypeMapper>();
            var root = new Root();
            var typeMapperFactory = root.Get<ITypeMapperFactory>();

            foreach (var version in versions)
                typeMappers.Add(typeMapperFactory.GetTypeMapper(version));

            return typeMappers.ToArray();
        }
    }
}