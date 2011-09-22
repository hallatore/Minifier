using System.Web.Mvc.Razor;
using System.Web.Razor;
using System.Web.Razor.Parser.SyntaxTree;

namespace Lervik.Minifier.Mvc
{
    public sealed class MinifyHtmlCodeGenerator : MvcCSharpRazorCodeGenerator
    {
        public MinifyHtmlCodeGenerator(string className, string rootNamespaceName, string sourceFileName, RazorEngineHost host)
            : base(className, rootNamespaceName, sourceFileName, host) { }

        public override void VisitSpan(Span span)
        {
            var markupSpan = span as MarkupSpan;

            if (markupSpan == null)
            {
                base.VisitSpan(span);
                return;
            }

            if (ConfigHelper.HtmlMinificationEnabled())
                span.Content = Core.Minify.Complete(markupSpan.Content);

            base.VisitSpan(span);
        }
    }
}
