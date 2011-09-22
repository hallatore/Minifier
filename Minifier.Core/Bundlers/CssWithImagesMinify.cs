using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using Microsoft.Web.Optimization;

namespace Lervik.Minifier.Core.Bundlers
{
    public class CssWithImagesMinify : CssMinify
    {
        private static readonly Regex url = new Regex(@"url\((([^\)]*)\?embed)\)", RegexOptions.Singleline);
        private const string format = "url(data:image/{0};base64,{1})";

        public override void Process(BundleResponse bundle)
        {
            HttpContext.Current.Response.Cache.SetLastModifiedFromFileDependencies();
            base.Process(bundle);
            string reference = HttpContext.Current.Request.Path.Replace("/css", "/");

            // When publishing the bundler can be called from "/" causing image reference to be invalid.
            if (reference != "/Content")
                reference = "/Content/";

            foreach (Match match in url.Matches(bundle.Content))
            {
                var file = new FileInfo(HostingEnvironment.MapPath("/Content/" + match.Groups[2].Value));
                if (file.Exists)
                {
                    string dataUri = GetDataUri(file);
                    bundle.Content = bundle.Content.Replace(match.Value, dataUri);
                    HttpContext.Current.Response.AddFileDependency(file.FullName);
                }
            }
        }

        private string GetDataUri(FileInfo file)
        {
            byte[] buffer = File.ReadAllBytes(file.FullName);
            string ext = file.Extension.Substring(1);
            return string.Format(format, ext, Convert.ToBase64String(buffer));
        }
    }
}