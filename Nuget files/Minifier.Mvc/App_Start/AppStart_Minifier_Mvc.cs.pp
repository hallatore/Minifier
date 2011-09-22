using System.Web.Mvc;
using System.Web.Routing;

[assembly: WebActivator.PreApplicationStartMethod(typeof($rootnamespace$.App_Start.AppStart_Minifier_Mvc), "PreStart")]
namespace $rootnamespace$.App_Start 
{
    public static class AppStart_Minifier_Mvc 
	{
        public static void PreStart() 
		{
            GlobalFilters.Filters.Add(new Lervik.Minifier.Mvc.MinifyActionAttribute());
        }
    }
}