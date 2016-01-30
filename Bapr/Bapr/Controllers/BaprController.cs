using System.Web.Mvc;
using Bapr.Models;
using Bapr.Utils;

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

        public ActionResult  GetFavouritedPlaces()
        {
            return  Json(new {}, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetVisitedPlaces()
        {
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPlacesByText(string text)
        {
            return Json(text, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPlacesByCategory(PlaceCategory category)
        {
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

    }
}