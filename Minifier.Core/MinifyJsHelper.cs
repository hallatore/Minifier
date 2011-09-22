using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Hosting;

namespace Lervik.Minifier.Core
{
    public class MinifyJsHelper
    {
        public static string ResolveMinifyUrl(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return path;

            var filename = path.Substring(0, path.Length - 6) + "js";
            var file = new FileInfo(HostingEnvironment.MapPath(filename));

            if (file.Exists)
            {
                var content = File.ReadAllText(file.FullName);
                var hash = GetHash(content);
                var url = string.Format("{0}?v={1}", path, hash);

                if (url.StartsWith("~/", StringComparison.OrdinalIgnoreCase))
                    url = url.Substring(2);

                var appPath = VirtualPathUtility.AppendTrailingSlash(HttpContext.Current.Request.ApplicationPath ?? string.Empty);
                return appPath + url;
            }

            throw new FileNotFoundException(path);
            return string.Empty;
        }

        private static long GetHash(string content)
        {
            if (string.IsNullOrEmpty(content))
                return 0L;

            using (SHA256 shA256 = (SHA256)new SHA256CryptoServiceProvider())
                return BitConverter.ToInt64(shA256.ComputeHash(Encoding.Unicode.GetBytes(content)), 0);
        }
    }
}
