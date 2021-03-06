﻿namespace FizzCode.DbTools.DataDefinitionReader
{
    using System.Collections.Generic;

    public class SchemaNamesToRead
    {
        public SchemaNamesToRead(List<string> schemaNames)
        {
            SchemaNames = schemaNames;
        }

        public SchemaNamesToRead(bool allDefaultNotSystem = true, bool allNotSystem = false, bool all = false)
        {
            AllDefault = allDefaultNotSystem;
            All = all;
            AllNotSystem = allNotSystem;
        }

        public bool All { get; set; }
        public bool AllNotSystem { get; set; }
        public bool AllDefault { get; set; }

        public List<string> SchemaNames { get; set; }

        public static implicit operator SchemaNamesToRead(List<string> schemaNames)
        {
            if (schemaNames.Count == 0)
                return new SchemaNamesToRead(true);

            return new SchemaNamesToRead(schemaNames);
        }

        public static SchemaNamesToRead AllSchemas => new SchemaNamesToRead(false, false, true);
        public static SchemaNamesToRead AllNotSystemSchemas => new SchemaNamesToRead(false, true);
        public static SchemaNamesToRead AllDefaultSchemas => new SchemaNamesToRead(true);
    }
}
