using Microsoft.Web.Optimization;

namespace Lervik.Minifier.Core.Bundlers
{
public class JsBundler : IBundleTransform
{
    public virtual void Process(BundleResponse bundle)
    {
        bundle.ContentType = "text/javascript";
    }
}
}