﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace FizzCode.DbTools.Common;

public static class Throw
{
    public static T IfNull<T>(T? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        InvalidOperationExceptionIfNull(argument, paramName);
        return argument;
    }

    public static void InvalidOperationExceptionIfNull([NotNull] object? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (argument is null)
            throw new InvalidOperationException($"{paramName} cannot be null.");
    }

    public static void InvalidOperationExceptionIfNull([NotNull] object? argument, Type typeOfProperty, Type typeOfObject)
    {
        if (argument is null)
            throw new InvalidOperationException($"The property {typeOfProperty.GetFriendlyTypeName()} cannot be null on {typeOfObject.Name}.");
    }
}
