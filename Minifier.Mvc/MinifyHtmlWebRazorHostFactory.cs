using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.WebPages.Razor;

namespace Lervik.Minifier.Mvc
{
    public sealed class MinifyHtmlWebRazorHostFactory : WebRazorHostFactory
    {
        public override WebPageRazorHost CreateHost(string virtualPath, string physicalPath)
        {
            WebPageRazorHost host = base.CreateHost(virtualPath, physicalPath);

            if (host.IsSpecialPage)
                return host;

            return new MinifyHtmlMvcWebPageRazorHost(virtualPath, physicalPath);
        }
    }
}
