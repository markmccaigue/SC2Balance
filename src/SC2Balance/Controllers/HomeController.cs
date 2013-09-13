using Elmah;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SC2Balance.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            ViewBag.Hours = new DataController().GetHoursSinceUpdate();
            return View();
        }

        public void LogJavaScriptError(string message, string errorUrl, int lineNumber)
        {
            //TODO: Throttling
            //var e = new JavaScriptException(message, errorUrl, lineNumber);
            //ErrorSignal.FromCurrentContext().Raise(e);
        }
    }
}
