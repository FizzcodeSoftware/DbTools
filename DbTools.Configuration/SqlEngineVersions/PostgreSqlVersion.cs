﻿using FizzCode.LightWeight.AdoNet;

namespace FizzCode.DbTools;
public class PostgreSqlVersion : SqlEngineVersion
{
    internal PostgreSqlVersion(string uniqueName, string versionString, string versionNumber)
        : base(AdoNetEngine.PostgreSql, uniqueName, versionString, versionNumber, "Npgsql")
    {
    }

    public static PostgreSqlVersion Generic { get; } = new PostgreSqlVersion(nameof(Generic), "0", "0");
}