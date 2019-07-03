using System.Web;
using System.Web.Optimization;

namespace Engnest
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/assets/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/assets/Scripts/jquery.validate*"));
			  bundles.Add(new ScriptBundle("~/bundles/js").Include(
                        "~/assets/Scripts/admin.js", 
						"~/assets/Scripts/JavaScript.js",
						"~/assets/Scripts/croppie.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/assets/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/css/css").Include(
                      "~/assets/css/bootstrap.css",
                      "~/assets/css/site.css",
					  "~/assets/css/font-awesome.min.css",
					  "~/assets/css/croppie.css"
					  ));
        }
    }
}
