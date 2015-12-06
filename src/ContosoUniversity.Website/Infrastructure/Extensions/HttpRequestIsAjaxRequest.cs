using Microsoft.AspNet.Http;

namespace System
{
    /// <exclude/>
    public static partial class ExtensionMethods
    {
        /// <exclude/>
        public static bool IsAjaxRequest(this HttpRequest @this)
        {
            if (@this == null)
                throw new ArgumentNullException("request");
            if (@this.Headers["X-Requested-With"] == "XMLHttpRequest")
                return true;
            if (@this.Headers != null)
                return @this.Headers["X-Requested-With"] == "XMLHttpRequest";
            return false;
        }
    }
}
