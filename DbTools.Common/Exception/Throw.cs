﻿using System;
using System.Runtime.CompilerServices;

namespace FizzCode.DbTools.Common;

public static class Throw
{
    public static void InvalidOperationExceptionIfNull(object? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (argument is null)
            throw new InvalidOperationException($"{paramName} cannot be null.");
    }

    public static void InvalidOperationExceptionIfNull(object? argument, Type typeOfProperty, Type typeOfObject)
    {
        if (argument is null)
            throw new InvalidOperationException($"The property {typeOfProperty.GetFriendlyTypeName()} cannot be null on {typeOfObject.Name}.");
    }
}
