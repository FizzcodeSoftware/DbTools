﻿namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections.Generic;
    using System.Linq;

    public class StoredProcedure
    {
        public StoredProcedure(string sqlStatementBody, params SqlParameter[] sqlParameters)
        {
            SqlStatementBody = sqlStatementBody;
            SpParameters = sqlParameters.ToList();
        }

        public string SqlStatementBody { get; set; }

        public DatabaseDefinition DatabaseDefinition { get; set; }
        public SchemaAndTableName SchemaAndSpName { get; set; }

        public List<SqlParameter> SpParameters { get; } = new List<SqlParameter>();
    }
}
