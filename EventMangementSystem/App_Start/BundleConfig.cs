using BundleTransformer.Core.Bundles;
using BundleTransformer.Core.Orderers;
using BundleTransformer.Core.Transformers;
using System.Web;
using System.Web.Optimization;

namespace EventManagementSystem
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = false;

            bundles.UseCdn = true;
            var cssTransformer = new StyleTransformer();
            var jsTransformer = new ScriptTransformer();
            var nullOrderer = new NullOrderer();

            var cssBundle = new CustomStyleBundle("~/bundles/css");
            cssBundle.Include("~/Content/bootstrap/ems.less");
            cssBundle.Transforms.Add(cssTransformer);
            cssBundle.Orderer = nullOrderer;
            bundles.Add(cssBundle);

            var jqueryBundle = new CustomScriptBundle("~/bundles/jquery");
            jqueryBundle.Include("~/Scripts/jquery-{version}.js");
            jqueryBundle.Transforms.Add(jsTransformer);
            jqueryBundle.Orderer = nullOrderer;
            bundles.Add(jqueryBundle);

            var emsscriptBundle = new CustomScriptBundle("~/bundles/emsscript");
            emsscriptBundle.Include("~/Scripts/ems.js");
            emsscriptBundle.Transforms.Add(jsTransformer);
            emsscriptBundle.Orderer = nullOrderer;
            bundles.Add(emsscriptBundle);


            var jqueryvalBundle = new CustomScriptBundle("~/bundles/jqueryval");
            jqueryvalBundle.Include("~/Scripts/jquery.validate*");
            jqueryvalBundle.Transforms.Add(jsTransformer);
            jqueryvalBundle.Orderer = nullOrderer;
            bundles.Add(jqueryvalBundle);

            var bootstrapDatePickerBundle = new CustomScriptBundle("~/bundles/bootstrapdatepicker");
            bootstrapDatePickerBundle.Include("~/Scripts/bootstrap-datepicker.js");
            bootstrapDatePickerBundle.Transforms.Add(jsTransformer);
            bootstrapDatePickerBundle.Orderer = nullOrderer;
            bundles.Add(bootstrapDatePickerBundle);

            var bootstrapDateTimePickerBundle = new CustomScriptBundle("~/bundles/bootstrapdatetimepicker");
            bootstrapDateTimePickerBundle.Include("~/Scripts/moment.js", "~/Scripts/bootstrap-datetimepicker.js");
            bootstrapDateTimePickerBundle.Transforms.Add(jsTransformer);
            bootstrapDateTimePickerBundle.Orderer = nullOrderer;
            bundles.Add(bootstrapDateTimePickerBundle);

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.

            var modernizrBundle = new CustomScriptBundle("~/bundles/modernizr");
            modernizrBundle.Include("~/Scripts/modernizr-*");
            modernizrBundle.Transforms.Add(jsTransformer);
            modernizrBundle.Orderer = nullOrderer;
            bundles.Add(modernizrBundle);


            var bootstrapBundle = new CustomScriptBundle("~/bundles/bootstrap");
            bootstrapBundle.Include("~/Scripts/bootstrap.js", "~/Scripts/respond.js");
            bootstrapBundle.Transforms.Add(jsTransformer);
            bootstrapBundle.Orderer = nullOrderer;
            bundles.Add(bootstrapBundle);


        }
    }
}
