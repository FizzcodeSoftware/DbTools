﻿namespace FizzCode.DbTools.TestBase
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class ConnectionsAttribute : Attribute, ITestDataSource
    {
        public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        {
            StaticConfiguration.Initialize("testconfig");

            foreach (var c in StaticConfiguration.ConnectionStrings.All)
            {
                var sqlEngineVersion = c.GetSqlEngineVersion();

                if (TestHelper.ShouldRunIntegrationTest(sqlEngineVersion))
                    yield return new[] { c.Name };
            }
        }

        public string GetDisplayName(MethodInfo methodInfo, object[] data)
        {
            var connectionName = (string)data[0];
            return $"{methodInfo.Name} {connectionName}";
        }
    }
}