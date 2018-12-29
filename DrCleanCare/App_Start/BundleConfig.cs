using System.Web;
using System.Web.Optimization;

namespace DrCleanCare
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.IgnoreList.Clear();
            BundleTable.EnableOptimizations = false;

            // JQuery
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            // Bootstrap
            bundles.Add(new StyleBundle("~/content/bootstrap").Include(
                        "~/Content/bootstrap.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/bootstrap.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            // DatePicker
            bundles.Add(new StyleBundle("~/Content/bsdatepicker").Include(
                        "~/Content/datepicker.css"));

            bundles.Add(new ScriptBundle("~/bundles/bsdatepicker").Include(
                         "~/Scripts/bootstrap-datepicker.js"));

            // JNumber
            bundles.Add(new ScriptBundle("~/bundles/jnumber").Include(
                         "~/Scripts/jquery.number.min.js"));

            // CanvasJS
            bundles.Add(new ScriptBundle("~/bundles/jnumber").Include(
                         "~/Scripts/canvasjs.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            // Site
            bundles.Add(new StyleBundle("~/content/css").Include(
                       "~/Content/site.css",
                       "~/Content/style.css",
                       "~/Content/font-awesome.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                        "~/Scripts/jquery.flexisel.js",
                        "~/Scripts/responsive-nav.js"));


            // Site admin
            bundles.Add(new StyleBundle("~/content/admin").Include(
                      "~/Content/admin.css",
                      "~/Content/font-awesome.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/admin").Include(
                        "~/Scripts/admin.js",
                        "~/Scripts/metisMenu.min.js"));

            // Lightbox
            bundles.Add(new StyleBundle("~/content/ekko-lightbox").Include(
                      "~/Content/ekko-lightbox.css"));

            bundles.Add(new ScriptBundle("~/bundles/ekko-lightbox").Include(
                        "~/Scripts/ekko-lightbox-min.js"));

            // Bootstrap dialog
            bundles.Add(new StyleBundle("~/content/bootstrap-dialog").Include(
                      "~/Content/bootstrap-dialog.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap-dialog").Include(
                        "~/Scripts/bootstrap-dialog.min.js"));


        }
    }
}