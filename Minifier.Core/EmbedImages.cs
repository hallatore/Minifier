using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Web.Optimization;

namespace Lervik.Minifier.Core
{
    internal class EmbedImages
    {
        private const string format = "url(data:image/{0};base64,{1})";
        private static readonly Regex url = new Regex(@"url\((([^\)]*)\?embed)\)", RegexOptions.Singleline);

        public static void Process(BundleResponse bundle)
        {
            foreach (Match match in url.Matches(bundle.Content))
            {
                var file = GetFileInfo(match.Groups[2].Value, bundle.Files);
                if (file != null)
                {
                    string dataUri = GetDataUri(file);
                    bundle.Content = bundle.Content.Replace(match.Value, dataUri);
                    HttpContext.Current.Response.AddFileDependency(file.FullName);
                }
            }
        }

        private static FileInfo GetFileInfo(string filename, IEnumerable<FileInfo> files)
        {
            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file.Directory + "/" + filename);
                if (fileInfo.Exists)
                    return fileInfo;
            }

            return null;
        }

        private static string GetDataUri(FileInfo file)
        {
            byte[] buffer = File.ReadAllBytes(file.FullName);
            string ext = file.Extension.Substring(1);
            return string.Format(format, ext, Convert.ToBase64String(buffer));
        }
    }
}
