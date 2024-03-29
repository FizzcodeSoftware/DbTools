﻿using FizzCode.LightWeight.AdoNet;

namespace FizzCode.DbTools;
public class OracleVersion : SqlEngineVersion
{
    public OracleVersion(string uniqueName, string versionString, string versionNumber)
        : base(AdoNetEngine.OracleSql, uniqueName, versionString, versionNumber, "Oracle.ManagedDataAccess.Client")
    {
    }

    public static OracleVersion Oracle12c { get; } = new OracleVersion(nameof(Oracle12c), "12c", "12.1.0.1");
}