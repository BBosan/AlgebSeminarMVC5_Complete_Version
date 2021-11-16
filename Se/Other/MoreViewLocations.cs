using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Se.Other
{
    public class MoreViewLocations : RazorViewEngine
    {
        #region NeRadiUCore
        //https://exceptionnotfound.net/a-simple-custom-viewengine-for-asp-net-mvc/

        private static string[] Normal = new[] {
                "~/Views/Admin/{1}/{0}.cshtml",
                "~/Views/Admin/{1}/{0}.vbhtml",
                "~/Views/Public/{1}/{0}.cshtml",
                "~/Views/Public/{1}/{0}.vbhtml",
            };

        private static string[] Partial = new[] {
                "~/Views/{1}/Partial/{0}.cshtml",
                "~/Views/Admin/{1}/Partial/{0}.cshtml",
                "~/Views/Public/{1}/Partial/{0}.cshtml",
            };

        public MoreViewLocations()
        {
            #region Staro
            //PartialViewLocationFormats = new[] {
            //    "~/Views/Admin/{1}/{0}.cshtml",
            //    "~/Views/Admin/{1}/{0}.vbhtml",
            //}; 
            //https://stackoverflow.com/a/33317204
            #endregion
            base.ViewLocationFormats = base.ViewLocationFormats.Union(Normal).ToArray();
            base.PartialViewLocationFormats = base.PartialViewLocationFormats.Union(Partial).ToArray();
        }
        #endregion
    }
}
