using System.Web.Mvc;
using Bapr.Models;
using Bapr.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;

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
        public ActionResult GetPlacesByText(string text, string latitude, string longitude)
        {
            ICollection<Location> locations = new Collection<Location>();
            locations.Add(GetMockLocation());
            locations.Add(GetMockLocation2());

            return Json(locations, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPlacesByCategory(PlaceCategory category, string latitude, string longitude)
        {
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddToFavorites(Location location)
        {
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddToVisited(Location location)
        {
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        #region mock locations

        private Location GetMockLocation()
        {
            Location location = new Location();
            location.latitude = 48.8567;
            location.longitude = 2.3508;
            location.name = "Paris Cafe";
            location.attributes = new Collection<LocationAttribute>();
            location.attributes.Add(new LocationAttribute("Opening Hours", "09:00 - 22:00", "string"));
            location.attributes.Add(new LocationAttribute("About", "bla bla", "string"));
            location.attributes.Add(new LocationAttribute("Smokers", (true).ToString(), "bool"));
            location.attributes.Add(new LocationAttribute("Stars", (3).ToString(), "string"));
            location.attributes.Add(new LocationAttribute("About", "bla bla", "string"));
            location.attributes.Add(new LocationAttribute("Happy Hour", new DateTime(2016, 5, 1, 4, 0, 0).ToString(), "string"));
            location.attributes.Add(new LocationAttribute("About", "bla bla", "string"));

            return location;
        }

        private Location GetMockLocation2()
        {
            Location location = new Location();
            location.latitude = 51.5072;
            location.longitude = 0.1275;
            location.name = "London Bridge";
            location.attributes = new Collection<LocationAttribute>();
            location.attributes.Add(new LocationAttribute("Opening Hours", "09:00 - 22:00", "string"));
            location.attributes.Add(new LocationAttribute("About", "bla bla", "string"));
            location.attributes.Add(new LocationAttribute("Smokers", (false).ToString(), "bool"));
            location.attributes.Add(new LocationAttribute("Stars", (3).ToString(), "string"));
            location.attributes.Add(new LocationAttribute("Happy Hour", new DateTime(2014, 5, 1, 4, 0, 0).ToString(), "string"));

            return location;
        }

        #endregion

    }
}