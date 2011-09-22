using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc.Razor;
using System.Web.Razor.Generator;

namespace Lervik.Minifier.Mvc
{
    public sealed class MinifyHtmlMvcWebPageRazorHost : MvcWebPageRazorHost
    {
        public MinifyHtmlMvcWebPageRazorHost(string virtualPath, string physicalPath)
            : base(virtualPath, physicalPath) { }

        public override RazorCodeGenerator DecorateCodeGenerator(RazorCodeGenerator incomingCodeGenerator)
        {
            if (incomingCodeGenerator is CSharpRazorCodeGenerator && ConfigHelper.HtmlMinificationEnabled())
            {
                return new MinifyHtmlCodeGenerator(incomingCodeGenerator.ClassName, incomingCodeGenerator.RootNamespaceName, incomingCodeGenerator.SourceFileName, incomingCodeGenerator.Host);
            }

            return base.DecorateCodeGenerator(incomingCodeGenerator);
        }
    }
}
