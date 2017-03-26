using System.Web.Optimization;

namespace ImgJar
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/js/vendor/bootstrap.min.js", 
            //          "~/js/vendor/dropzonejs/dropzone.js"));


            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                      "~/js/vendor/dropzonejs/dropzone.js",
                      "~/js/imgjar.js"
                      ));

            bundles.Add(new StyleBundle("~/bundles/css").Include(
                      "~/css/vendor/bootstrap.min.css",
                      "~/css/imgjar.css"));
        }
    }
}
