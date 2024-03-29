﻿using System;
using System.Collections.Generic;
using System.Linq;
using FizzCode.DbTools.Common;
using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.DataGenerator;
public class Generator(GeneratorContext context)
{
    private readonly GeneratorContext Context = context;

    // TODO
    private readonly SqlEngineVersion Version = GenericVersion.Generic1;

    // Get a SqlTable
    // * generate rows
    // - either object[]
    // - rows
    //   DataDefinitionExecuter.Row
    // * make use in Etl as generating rows
    // * create insert or update existing
    // * ensure FKs
    // * add some governing infos (type of data like name, adress, number ranges etc.)
    public IEnumerable<Row> Generate(SqlTable table, int numberOfRows)
    {
        for (var i = 0; i < numberOfRows; i++)
        {
            yield return Generate(table);
        }
    }

    public Row Generate(SqlTable table)
    {
        var row = new Row();

        foreach (var column in table.Columns)
        {
            GenerateValue(row, column);
        }

        return row;
    }

    private void GenerateValue(Row row, SqlColumn column)
    {
        var sqlColumnDataGenerator = column.Properties.OfType<SqlColumnDataGenerator>().FirstOrDefault();

        var generator = sqlColumnDataGenerator?.Generator;

        if (generator is null)
        {
            if (!column.Types[Version].IsNullable)
                generator = GetDefaultGenerator(column);
        }

        if (generator != null)
        {
            generator.SetContext(Context);
            row.Add(column.Name!, generator.Get());
        }
    }

    private GeneratorBase GetDefaultGenerator(SqlColumn column)
    {
        var type = column.Types[Version];
        return type.SqlTypeInfo switch
        {
            DataDefinition.Generic.SqlNVarChar _ => new GeneratorString(1, type.Length!.Value + 1),
            DataDefinition.MsSql2016.SqlNVarChar _ => new GeneratorString(1, type.Length!.Value + 1),
            DataDefinition.Oracle12c.SqlNVarChar2 _ => new GeneratorString(1, type.Length!.Value + 1),

            DataDefinition.Generic.SqlInt32 _ => new GeneratorInt32(),
            DataDefinition.MsSql2016.SqlInt _ => new GeneratorInt32(),

            DataDefinition.Generic.SqlDate _ => new GeneratorDateMinMax(new DateTime(1950, 1, 1), new DateTime(2050, 1, 1)),
            DataDefinition.MsSql2016.SqlDate _ => new GeneratorDateMinMax(new DateTime(1950, 1, 1), new DateTime(2050, 1, 1)),
            DataDefinition.Oracle12c.SqlDate _ => new GeneratorDateMinMax(new DateTime(1950, 1, 1), new DateTime(2050, 1, 1)),

            DataDefinition.Generic.SqlDateTime _ => new GeneratorDateTimeMinMax(new DateTime(1950, 1, 1), new DateTime(2050, 1, 1)),
            DataDefinition.MsSql2016.SqlDateTime _ => new GeneratorDateTimeMinMax(new DateTime(1950, 1, 1), new DateTime(2050, 1, 1)),

            _ => throw new NotImplementedException($"Unhandled type: {type.SqlTypeInfo}"),
        };
    }
}
