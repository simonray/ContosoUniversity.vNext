
namespace System
{
    /// <exclude/>
    public static partial class ExtensionMethods
    {
        /// <exclude/>
        public static bool Contains(this string @this, string value, StringComparison comparisonType)
            => @this.IndexOf(value, comparisonType) != -1;
    }
}
