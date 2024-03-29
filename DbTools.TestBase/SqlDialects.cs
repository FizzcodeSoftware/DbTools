﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using FizzCode.DbTools.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FizzCode.DbTools.TestBase;
[AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
public abstract class SqlVersionsBasAttribute : Attribute, ITestDataSource
{
    protected List<SqlEngineVersion> Versions { get; set; }

    protected SqlVersionsBasAttribute(params Type[] versionTypes)
    {
        Versions = [];
        foreach (var versionType in versionTypes)
        {
            var version = Activator.CreateInstance(versionType) as SqlEngineVersion;
            Throw.InvalidOperationExceptionIfNull(version, message: $"Cannot create instance of {versionType.GetFriendlyTypeName()}.");
            if (!Versions.Contains(version))
                Versions.Add(version);
        }
    }

    public virtual IEnumerable<object[]> GetData(MethodInfo methodInfo)
    {
        foreach (var item in Versions)
        {
            if (TestHelper.ShouldRunIntegrationTest(item))
                yield return new[] { (object)item };
        }
    }

    public string GetDisplayName(MethodInfo methodInfo, object?[]? data)
    {
        var versionKey = (SqlEngineVersion?)data?[0];
        return $"{methodInfo.Name} {versionKey}";
    }
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
public sealed class LatestSqlVersionsAttribute : SqlVersionsBasAttribute
{
    public LatestSqlVersionsAttribute(bool forceIntegrationTests = false)
    {
        _forceIntegrationTests = forceIntegrationTests;
        Versions = SqlEngineVersions.GetLatestExecutableVersions()
            .Where(x => x is MsSqlVersion || x is OracleVersion || x is SqLiteVersion)
            .ToList();
    }

    private readonly bool _forceIntegrationTests;

    public override IEnumerable<object[]> GetData(MethodInfo methodInfo)
    {
        foreach (var item in Versions)
        {
            if (_forceIntegrationTests || TestHelper.ShouldRunIntegrationTest(item))
                yield return new[] { (object)item };
        }
    }
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
public sealed class SqlVersionsAttribute : SqlVersionsBasAttribute
{
    public SqlVersionsAttribute(params string[] versionTypeNames)
    {
        Versions = [];
        var allversions = SqlEngineVersions.AllVersions;
        foreach (var versionTypeName in versionTypeNames)
        {
            var version = allversions.Find(v => v.UniqueName == versionTypeName);
            if (version != null && TestHelper.ShouldRunIntegrationTest(version))
            {
                Versions.Add(version);
            }
        }

        if (Versions.Count == 0)
        {
            Debug.WriteLine("No SqlEngineVersion was found, falling back to SqLiteVersion.SqLite3. Probable reason is missing test configuration with a connectionstring with a given sql type.");
            Versions.Add(SqLiteVersion.SqLite3);
        }
    }
}