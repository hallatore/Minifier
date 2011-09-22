using System;
using System.IO;
using System.Web;
using System.Web.Hosting;
using Microsoft.Web.Optimization;

namespace Lervik.Minifier.Core
{
    public class MinifyJsHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            var filename = context.Request.Path.Substring(0, context.Request.Path.Length - 6) + "js";
            string output = GetCachedOutput(context);
            var file = new FileInfo(HostingEnvironment.MapPath(filename));

            if (output == null)
            {
                if (file.Exists)
                {
                    var minifier = new JsMinify();
                    var bundle = new BundleResponse
                    {
                        Content = File.ReadAllText(file.FullName)
                    };
                    minifier.Process(bundle);
                    output = bundle.Content;
                    AddCachedOutput(context, output, file.FullName);
                }
                else
                {
                    throw new HttpException(404, "NotFound");
                }
            }

            context.Response.Write(output);
            context.Response.ContentType = "text/javascript";

            HttpContext.Current.Response.AddFileDependency(file.FullName);
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.Public);
            HttpContext.Current.Response.Cache.SetExpires(DateTime.Now.AddYears(1));
        }

        private void AddCachedOutput(HttpContext context, string output, string filename)
        {
            context.Cache.Insert(GetCacheKey(context.Request.Path), output, new System.Web.Caching.CacheDependency(filename));
        }

        private string GetCachedOutput(HttpContext context)
        {
            return context.Cache[GetCacheKey(context.Request.Path)] as string;
        }

        private string GetCacheKey(string path)
        {
            return "Lervik.Minifier.Core:" + path;
        }
    }
}
