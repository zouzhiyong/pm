using System.Web;
using System.Web.Optimization;

namespace PM
{
    public class BundleConfig
    {
        // 有关绑定的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/metisMenu").Include(
                        "~/Scripts/metisMenu.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/common").Include(
                    "~/Scripts/common.js")); 

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            // 生产准备时，请使用 http://modernizr.com 上的生成工具来仅选择所需的测试。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/echarts.min.js",
                      "~/Scripts/select2.min.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/font-awesome.min.css",
                      "~/Content/bootstrap.css",
                      "~/Content/select2.min.css",
                      "~/Content/metisMenu.min.css"));
            bundles.Add(new StyleBundle("~/Site/css").Include(
                      "~/Content/site.css"));




        }
    }
}
