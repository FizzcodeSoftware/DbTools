﻿namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections.Generic;
    using System.Linq;

    public class StoredProcedure
    {
        public StoredProcedure(string sqlStatementBody)
        {
            SqlStatementBody = sqlStatementBody;
        }

        public StoredProcedure(string sqlStatementBody, params SpParameter[] spParameters)
        {
            SqlStatementBody = sqlStatementBody;
            SpParameters = spParameters.ToList();
        }

        public DatabaseDefinition DatabaseDefinition { get; set; }
        public SchemaAndTableName SchemaAndSpName { get; set; }

        public List<SpParameter> SpParameters { get; } = new List<SpParameter>();

        public string  SqlStatementBody { get; set; }
    }
}
