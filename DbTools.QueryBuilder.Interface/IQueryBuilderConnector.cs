﻿namespace FizzCode.DbTools.QueryBuilder.Interface
{
    public interface IQueryBuilderConnector
    {
        void ProcessStoredProcedureFromQuery(IStoredProcedureFromQuery storedProcedureFromQuery);
        void ProcessViewFromQuery(IViewFromQuery viewFromQuery);
    }
}