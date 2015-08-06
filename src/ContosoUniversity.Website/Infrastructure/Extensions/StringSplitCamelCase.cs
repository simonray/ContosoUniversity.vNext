using System.Text.RegularExpressions;

namespace System
{
    /// <exclude/>
    public static partial class ExtensionMethods
    {
        /// <exclude/>
        public static string SplitCamelCase(this string @this)
            => Regex.Replace(Regex.Replace(@this, @"(\P{Ll})(\P{Ll}\p{Ll})", $"$1 $2"), @"(\p{Ll})(\P{Ll})", $"$1 $2");
    }
}
