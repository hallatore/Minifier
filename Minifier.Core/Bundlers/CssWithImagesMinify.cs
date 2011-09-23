using System.Web;
using Microsoft.Web.Optimization;

namespace Lervik.Minifier.Core.Bundlers
{
    public class CssWithImagesMinify : CssMinify
    {
        public override void Process(BundleResponse bundle)
        {
            HttpContext.Current.Response.Cache.SetLastModifiedFromFileDependencies();
            base.Process(bundle);
            EmbedImages.Process(bundle);
        }
    }
}