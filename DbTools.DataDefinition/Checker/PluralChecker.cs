﻿using System;
using System.Collections.Generic;

namespace FizzCode.DbTools.DataDefinition.Checker;
public class PluralChecker
{
    private readonly string _singularsInput = @"
os
us
bus
gas
yes
abs
cos
ems
his
ops
sos
boss
basis
abyss
arcus
lens
mass";
    private readonly List<string> _singulars = [];

    public PluralChecker()
    {
        _singulars.AddRange(_singularsInput.Split("\r\n"));
    }

    public bool CheckValidity(string tableName)
    {
        return !tableName.EndsWith('s')
            || _singularsInput.IndexOf(tableName, StringComparison.InvariantCulture) != -1;
    }
}
