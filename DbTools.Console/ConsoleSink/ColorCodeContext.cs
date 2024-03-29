﻿using System;
using System.Collections.Generic;
using System.IO;
using Serilog.Events;

namespace FizzCode.DbTools.Console.ConsoleSink;
internal struct ColorCodeContext(TextWriter builder)
    : IDisposable
{
#pragma warning disable IDE0069 // Disposable fields should be disposed
    private readonly TextWriter _builder = builder;
#pragma warning restore IDE0069 // Disposable fields should be disposed

    private static readonly IDictionary<ColorCode, string> _colorCodeValues = new Dictionary<ColorCode, string>
    {
        [ColorCode.Message_Exception] = "\x1b[38;5;0015m",
        [ColorCode.TimeStamp_Property_Exception] = "\x1b[38;5;0007m",
        [ColorCode.Value] = "\x1b[38;5;0008m",
        [ColorCode.NullValue] = "\x1b[38;5;0027m",
        [ColorCode.StructureName] = "\x1b[38;5;0007m",
        [ColorCode.StringValue] = "\x1b[38;5;0045m",
        [ColorCode.NumberValue] = "\x1b[38;5;0204m",
        [ColorCode.BooleanValue] = "\x1b[38;5;0033m",
        [ColorCode.ScalarValue] = "\x1b[38;5;0085m",
        [ColorCode.TimeSpanValue] = "\x1b[38;5;0220m",
        [ColorCode.LvlTokenVrb] = "\x1b[38;5;0007m",
        [ColorCode.LvlTokenDbg] = "\x1b[38;5;0007m",
        [ColorCode.LvlTokenInf] = "\x1b[38;5;0015m",
        [ColorCode.LvlTokenWrn] = "\x1b[38;5;000m\x1b[48;5;0214m",
        [ColorCode.LvlTokenErr] = "\x1b[38;5;0015m\x1b[48;5;0196m",
        [ColorCode.LvlTokenFtl] = "\x1b[38;5;0015m\x1b[48;5;0196m",
        [ColorCode.Module] = "\x1b[38;5;0007m",
        [ColorCode.Query] = "\x1b[38;5;0007m",
        // [ColorCode.Process] = "\x1b[38;5;0228m",
        // [ColorCode.Operation] = "\x1b[38;5;0085m",
        // [ColorCode.Job] = "\x1b[38;5;0085m",
        [ColorCode.ConnectionStringKey] = "\x1b[38;5;0135m",
        [ColorCode.SourceOrTarget] = "\x1b[38;5;0035m",
        [ColorCode.Transaction] = "\x1b[38;5;0245m",
    };

    private const string ResetColorCodeValue = "\x1b[0m";

    public readonly void Dispose()
    {
        _builder.Write(ResetColorCodeValue);
    }

    internal static ColorCodeContext StartOverridden(TextWriter builder, LogEvent logEvent, ColorCode colorCode)
    {
        colorCode = GetOverridenColorCode(logEvent.Level, colorCode);
        if (_colorCodeValues.TryGetValue(colorCode, out var colorCodeValue))
        {
            builder.Write(colorCodeValue);
        }

        return new ColorCodeContext(builder);
    }

    internal static void Write(TextWriter builder, ColorCode colorCode, string? text)
    {
        if (_colorCodeValues.TryGetValue(colorCode, out var value))
        {
            builder.Write(value);
        }

        builder.Write(text);
        builder.Write(ResetColorCodeValue);
    }

    internal static void WriteOverridden(TextWriter builder, LogEvent logEvent, ColorCode colorCode, string text)
    {
        colorCode = GetOverridenColorCode(logEvent.Level, colorCode);
        if (_colorCodeValues.TryGetValue(colorCode, out var colorCodeValue))
        {
            builder.Write(colorCodeValue);
        }

        builder.Write(text);
        builder.Write(ResetColorCodeValue);
    }

    internal static ColorCode GetOverridenColorCode(LogEventLevel level, ColorCode colorCode)
    {
        return level switch
        {
            LogEventLevel.Warning => ColorCode.LvlTokenWrn,
            LogEventLevel.Fatal => ColorCode.LvlTokenFtl,
            LogEventLevel.Error => ColorCode.LvlTokenErr,
            _ => colorCode,
        };
    }
}