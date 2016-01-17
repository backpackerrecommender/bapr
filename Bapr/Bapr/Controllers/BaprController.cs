using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bapr.Models;

namespace Bapr.Controllers
{
    public class BaprController : Controller
    {
        //
        // GET: /Bapr/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Menu(double latitude = double.MinValue, double longitude = double.MinValue)
        {
            if (latitude == double.MinValue || longitude == double.MinValue)
                return RedirectToAction("Index");

            return View(new Location()
            {
                latitude = latitude,
                longitude = longitude
            });
        }
    }
}