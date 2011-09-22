using System.Web.Mvc;
using System.Web.Routing;

[assembly: WebActivator.PreApplicationStartMethod(typeof($rootnamespace$.App_Start.AppStart_Minifier_Core), "PreStart")]
namespace $rootnamespace$.App_Start 
{
    public static class AppStart_Minifier_Core
	{
        public static void PreStart() 
		{
            RouteTable.Routes.IgnoreRoute("{*allaspx}", new { allaspx = @".*\.min.js(/.*)?" });
        }
    }
}