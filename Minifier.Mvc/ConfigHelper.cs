using System.Configuration;

namespace Lervik.Minifier.Mvc
{
    internal static class ConfigHelper
    {
        public static bool HtmlMinificationEnabled()
        {
            var result = ConfigurationManager.AppSettings["HtmlMinificationEnabled"];
            return result != null && result.ToLower() == "true";
        }
    }
}
