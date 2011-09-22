using System.Web.Mvc;

namespace Lervik.Minifier.Mvc
{
    public class MinifyActionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Minify(filterContext);
            base.OnActionExecuting(filterContext);
        }

        private void Minify(ActionExecutingContext filterContext)
        {
            if (ConfigHelper.HtmlMinificationEnabled())
            {
                var response = filterContext.HttpContext.Response;
                response.Filter = new StringFilterStream(response.Filter, Lervik.Minifier.Core.Minify.Quick);
            }
        }
    }
}
