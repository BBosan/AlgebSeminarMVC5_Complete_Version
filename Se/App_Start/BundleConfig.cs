using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace Se//.App_Start //ovo uncommented da bi bundles radio
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            BundleTable.EnableOptimizations = false; //mozda isto za IIS

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            "~/Scripts/jquery-{version}.js"));


            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                        //"~/Scripts/jquery-{version}.js",
                        "~/Scripts/bootstrap.js",
                        //"~/Scripts/jquery.validate*",
                        "~/Scripts/jquery.validate.js",
                        "~/Scripts/jquery.validate.unobtrusive.js",
                        "~/Scripts/jquery.unobtrusive-ajax.js"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*")); // new CssRewriteUrlTransform()) //iis test inace ne treba

            bundles.Add(new StyleBundle("~/Content").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"
                      ));

            //admin
            //bundles.Add(new StyleBundle("~/Content/CSS/Admin/Predb").Include(
            //          "~/Content/CSS/Admin/Predb/Index.css"
            //          ));
            //bundles.Add(new StyleBundle("~/Content/CSS/Admin/Sem").Include(
            //          "~/Content/CSS/Admin/Sem/Index.css"
            //          ));
        }
    }
}