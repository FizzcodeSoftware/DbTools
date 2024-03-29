﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Display;
using Serilog.Parsing;

namespace FizzCode.DbTools.Console.ConsoleSink;
internal class ConsoleSink : ILogEventSink
{
    private readonly List<Action<LogEvent, TextWriter>> _writers;
    private readonly object _lock = new();

    public ConsoleSink(string outputTemplate)
    {
        EnableVirtualTerminalProcessingHack.ApplyHack();

        var template = new MessageTemplateParser().Parse(outputTemplate);

        _writers = [];
        foreach (var token in template.Tokens)
        {
            switch (token)
            {
                case TextToken textToken:
                    _writers.Add((e, b) => WriteText(e, b, textToken.Text));
                    break;
                case PropertyToken propertyToken:
                    switch (propertyToken.PropertyName)
                    {
                        case OutputProperties.LevelPropertyName:
                            _writers.Add(WriteLevel);
                            break;
                        case OutputProperties.NewLinePropertyName:
                            _writers.Add((_, b) => WriteNewLine(b));
                            break;
                        case OutputProperties.ExceptionPropertyName:
                            _writers.Add(WriteException);
                            break;
                        case OutputProperties.MessagePropertyName:
                            {
                                _writers.Add(WriteMessage);
                                break;
                            }
                        case OutputProperties.TimestampPropertyName:
                            _writers.Add((e, b) => WriteTimeStamp(e, b, propertyToken.Format));
                            break;
                        case "Properties":
                            {
                                _writers.Add((e, b) => WriteProperties(e, b, template));
                                break;
                            }
                        default:
                            _writers.Add((e, b) => WriteProperty(e, b, propertyToken.PropertyName, propertyToken.Format));
                            break;
                    }
                    break;
            }
        }
    }

    public void Emit(LogEvent logEvent)
    {
        if (logEvent is null)
            return;

        using (var builder = new StringWriter(new StringBuilder(1024)))
        {
            foreach (var writer in _writers)
            {
                writer.Invoke(logEvent, builder);
            }

            lock (_lock)
            {
                System.Console.Out.Write(builder.ToString());
                System.Console.Out.Flush();
            }
        }
    }

    private static void WriteText(LogEvent logEvent, TextWriter builder, string text)
    {
        ColorCodeContext.WriteOverridden(builder, logEvent, ColorCode.Value, text);
    }

    private static void WriteLevel(LogEvent logEvent, TextWriter builder)
    {
        var text = logEvent.Level switch
        {
            LogEventLevel.Verbose => "VRB",
            LogEventLevel.Debug => "DBG",
            LogEventLevel.Information => "INF",
            LogEventLevel.Warning => "WRN",
            LogEventLevel.Error => "ERR",
            LogEventLevel.Fatal => "FTL",
            _ => null,
        };

        var colorCode = logEvent.Level switch
        {
            LogEventLevel.Verbose => ColorCode.LvlTokenVrb,
            LogEventLevel.Debug => ColorCode.LvlTokenDbg,
            LogEventLevel.Information => ColorCode.LvlTokenInf,
            LogEventLevel.Warning => ColorCode.LvlTokenWrn,
            LogEventLevel.Error => ColorCode.LvlTokenErr,
            LogEventLevel.Fatal => ColorCode.LvlTokenFtl,
            _ => ColorCode.LvlTokenInf,
        };

        ColorCodeContext.Write(builder, colorCode, text);
    }

    private static void WriteNewLine(TextWriter builder)
    {
        builder.WriteLine();
    }

    private static void WriteException(LogEvent logEvent, TextWriter builder)
    {
        if (logEvent.Exception is null)
            return;

        var lines = logEvent.Exception.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            var colorCode = line.StartsWith("   ", StringComparison.InvariantCultureIgnoreCase)
                ? ColorCode.TimeStamp_Property_Exception
                : ColorCode.Message_Exception;

            ColorCodeContext.Write(builder, colorCode, line + Environment.NewLine);
        }
    }

    private static void WriteMessage(LogEvent logEvent, TextWriter builder)
    {
        foreach (var token in logEvent.MessageTemplate.Tokens)
        {
            switch (token)
            {
                case TextToken tt:
                    {
                        ColorCodeContext.WriteOverridden(builder, logEvent, ColorCode.Message_Exception, tt.Text);
                        break;
                    }
                case PropertyToken pt:
                    {
                        if (!logEvent.Properties.TryGetValue(pt.PropertyName, out var value))
                        {
                            ColorCodeContext.WriteOverridden(builder, logEvent, ColorCode.TimeStamp_Property_Exception, pt.ToString());
                        }
                        else
                        {
                            ValueFormatter.Format(logEvent, value, builder, pt.Format, pt.PropertyName);
                        }

                        break;
                    }
            }
        }
    }

    private static void WriteTimeStamp(LogEvent logEvent, TextWriter builder, string? format)
    {
        using (ColorCodeContext.StartOverridden(builder, logEvent, ColorCode.TimeStamp_Property_Exception))
        {
            new ScalarValue(logEvent.Timestamp).Render(builder, format);
        }
    }

    private static void WriteProperties(LogEvent logEvent, TextWriter builder, MessageTemplate outputTemplate)
    {
        var properties = new List<LogEventProperty>();
        foreach (var kvp in logEvent.Properties)
        {
            if (!logEvent.MessageTemplate.Tokens.Any(t => t is PropertyToken pt && pt.PropertyName == kvp.Key) && !outputTemplate.Tokens.Any(t => t is PropertyToken pt && pt.PropertyName == kvp.Key))
            {
                properties.Add(new LogEventProperty(kvp.Key, kvp.Value));
            }
        }

        if (properties.Count == 0)
            return;

        ValueFormatter.FormatStructureValue(logEvent, builder, new StructureValue(properties));
    }

    private static void WriteProperty(LogEvent logEvent, TextWriter builder, string propertyName, string? format)
    {
        if (!logEvent.Properties.TryGetValue(propertyName, out var propertyValue))
            return;

        using (ColorCodeContext.StartOverridden(builder, logEvent, ColorCode.TimeStamp_Property_Exception))
        {
            propertyValue.Render(builder, format);
        }
    }
}