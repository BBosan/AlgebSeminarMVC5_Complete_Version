using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Se.Controllers
{
    public class ErrorController : Controller
    {
        public static string emot = string.Concat(Enumerable.Repeat("\uD83E\uDD14", 10));

        // GET: Error
        public ActionResult Index()
        {
            return View("_Error");
        }

        public ActionResult NotFound()
        {
            HttpRequest req = System.Web.HttpContext.Current.Request;
            string browserName = req.Browser.Browser;
            string emotOrnot;
            emotOrnot = browserName == "Firefox" ? emot : "NotFound_404";
            Response.StatusCode = 404;
            return View("_Error", new { id = emotOrnot } );
        }

    }
}