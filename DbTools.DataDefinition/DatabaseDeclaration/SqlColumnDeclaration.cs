﻿namespace FizzCode.DbTools.DataDefinition
{
    public class SqlColumnDeclaration : SqlColumn
    {
        public new SqlTableDeclaration Table
        {
            get
            {
                return (SqlTableDeclaration)base.Table;
            }
            set
            {
                base.Table = value;
            }
        }

        public SqlColumnDeclaration SetPKIdentity()
        {
            SetPK();
            Properties.Add(new Identity(this));
            return this;
        }

        public SqlColumnDeclaration SetPK()
        {
            Table.SetPK(this);
            return this;
        }
    }
}