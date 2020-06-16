﻿namespace FizzCode.DbTools.QueryBuilder
{
    /// <summary>
    /// Helper class for shorthand methods.
    /// </summary>
    public static class Q
    {
        /// <summary>
        /// Shorthand to create new Expression "&lt;<paramref name="obj1"/>&gt; = &lt;<paramref name="obj2"/>&gt;".
        /// </summary>
        public static new Expression Equals(object obj1, object obj2)
        {
            return new Expression(obj1, "=", obj2);
        }

        /// <summary>
        /// Shorthand for new Expression().
        /// </summary>
        public static Expression Ex(params object[] expressionParts)
        {
            return new Expression(expressionParts);
        }
    }
}
