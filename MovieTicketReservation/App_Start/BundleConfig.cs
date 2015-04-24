using System.Web;
using System.Web.Optimization;

namespace MovieTicketReservation {
	public class BundleConfig {
		// For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
		public static void RegisterBundles(BundleCollection bundles) {
			bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
						"~/Scripts/jquery-2.1.1.min.js"));

			bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
						"~/Scripts/jquery.validate*"));

			// Use the development version of Modernizr to develop with and learn from. Then, when you're
			// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
			bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
						"~/Scripts/modernizr-*"));

			bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
					  "~/Scripts/bootstrap.min.js",
					  "~/Scripts/respond.min.js"));

			bundles.Add(new StyleBundle("~/bundles/website").Include(
					  "~/Content/bootstrap.min.css",
					  "~/Content/font-awesome.min.css",
                      "~/Content/layout.css"));

			bundles.Add(new StyleBundle("~/bundles/admin").Include(
					  "~/Content/bootstrap.min.css",
					  "~/Content/font-awesome.min.css",
					  "~/Content/sb-admin-2.css",
					  "~/Content/mentisMenu.css"));
        }
	}
}
