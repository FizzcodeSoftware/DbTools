﻿using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.DataDefinition.Generic;
public abstract class GenericSqlType : SqlTypeInfo
{
}

public class SqlChar : GenericSqlType1
{
    public override bool HasLength => true;
    public override bool HasScale => false;
}

public class SqlNChar : GenericSqlType1
{
    public override bool HasLength => true;
    public override bool HasScale => false;
}

public class SqlVarChar : GenericSqlType1
{
    public override bool HasLength => true;
    public override bool HasScale => false;
}

public class SqlNVarChar : GenericSqlType1
{
    public override bool HasLength => true;
    public override bool HasScale => false;
}

public class SqlText : GenericSqlType1
{
    public override bool HasLength => false;
    public override bool HasScale => false;
}

public class SqlFloatSmall : GenericSqlType1
{
    public override bool HasLength => false;
    public override bool HasScale => false;
}

public class SqlFloatLarge : GenericSqlType1
{
    public override bool HasLength => false;
    public override bool HasScale => false;
}

public class SqlBit : GenericSqlType1
{
    public override bool HasLength => false;
    public override bool HasScale => false;
}

public class SqlByte : GenericSqlType1
{
    public override bool HasLength => false;
    public override bool HasScale => false;
}

public class SqlInt16 : GenericSqlType1
{
    public override bool HasLength => false;
    public override bool HasScale => false;
}

public class SqlInt32 : GenericSqlType1
{
    public override bool HasLength => false;
    public override bool HasScale => false;
}

public class SqlInt64 : GenericSqlType1
{
    public override bool HasLength => false;
    public override bool HasScale => false;
}

public class SqlNumber : GenericSqlType1
{
    public override bool HasLength => true;
    public override bool HasScale => true;
}

public class SqlDate : GenericSqlType1
{
    public override bool HasLength => false;
    public override bool HasScale => false;
}

public class SqlTime : GenericSqlType1
{
    public override bool HasLength => false;
    public override bool HasScale => false;
}

public class SqlDateTime : GenericSqlType1
{
    public override bool HasLength => false;
    public override bool HasScale => false;
}