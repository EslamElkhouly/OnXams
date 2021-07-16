using System.Web.Optimization;

namespace OnlineExamPlatform
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

           
            bundles.Add(new ScriptBundle("~/Scripts/js").Include(
                "~/Scripts/js/jquery/jquery.min.js",
                "~/Scripts/ckeditor/ckeditor.js",
                "~/Scripts/js/jquery-ui/jquery-ui.min.js",
                "~/Scripts/js/popper.js/popper.min.js",
                "~/Scripts/js/bootstrap/js/bootstrap.min.js",
                "~/Scripts/pages/dashboard/custom-dashboard.js",


                "~/Scripts/DataTables/jquery.dataTables.js",
                "~/Scripts/DataTables/jquery.dataTables.min.js",
                "~/Scripts/DataTables/dataTables.bootstrap.js",
                "~/Scripts/DataTables/dataTables.bootstrap4.min.js",

                "~/Scripts/js/script.js",


             
            
                




                "~/Scripts/js/SmoothScroll.js",
                "~/Scripts/js/pcoded.min.js",
                "~/Scripts/js/demo-12.js",
                "~/Scripts/js/jquery.mCustomScrollbar.concat.min.js",
                "~/Scripts/bootstrap2-toggle.min.js"
                ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/css/googleFont.css",
                "~/icon/themify-icons/themify-icons.css",
                "~/icon/icofont/css/icofont.css",

                "~/Content/DataTables/css/dataTables.bootstrap.css",
                "~/Content/DataTables/css/dataTables.bootstrap.min.css",
                "~/Content/DataTables/css/dataTables.bootstrap4.css",
                "~/Content/DataTables/css/dataTables.bootstrap4.min.css",


                "~/Content/css/bootstrap/css/bootstrap.min.css",

                "~/Content/css/style.css",
                "~/Content/css/jquery.mCustomScrollbar.css",
                "~/Content/bootstrap2-toggle.min.css",
                "~/Content/css/CustomStyle.css"
                ,"~/Content/PagedList.css"
                
                ));
           
        }
    }
}
