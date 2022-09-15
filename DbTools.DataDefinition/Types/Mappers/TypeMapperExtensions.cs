﻿namespace FizzCode.DbTools.DataDefinition
{
    using FizzCode.DbTools.DataDefinition.Base;

    public static class TypeMapperExtensions
    {
        public static AbstractTypeMapper GetTypeMapper(this GenericVersion version)
        {
            if (version == GenericVersion.Generic1)
                return new Generic1TypeMapper();

            return null;
        }
    }
}