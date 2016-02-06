using System.Web.Mvc;
using Bapr.Models;
using Bapr.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.Net.Http;
using System.Web.Script.Serialization;
using System.Net.Http.Headers;
using Newtonsoft.Json;

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
            string user = Session["logged_username"].ToString();

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:18323/api/MarkedLocations/Favorites");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string urlParameters = "?user=" + user;
            HttpResponseMessage response = client.GetAsync(urlParameters).Result;
            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                Collection<Location> markedLocations = JsonConvert.DeserializeObject<Collection<Location>>(result);
                return Json(markedLocations, JsonRequestBehavior.AllowGet);
            }
 
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetVisitedPlaces()
        {
            string user = Session["logged_username"].ToString();

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:18323/api/MarkedLocations/Visited");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string urlParameters = "?user=" + user;
            HttpResponseMessage response = client.GetAsync(urlParameters).Result;
            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                Collection<Location> markedLocations = JsonConvert.DeserializeObject<Collection<Location>>(result);
                return Json(markedLocations, JsonRequestBehavior.AllowGet);
            }

            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPlacesByText(string text, string latitude, string longitude)
        {
            ICollection<Location> locations = new Collection<Location>();
            locations.Add(GetMockLocation());
            locations.Add(GetMockLocation2());

            return Json(locations, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPlacesByCategory(PlaceCategory category, double latitude, double longitude)
        {
            HttpClient client = new HttpClient();

            switch (category)
            {
                case PlaceCategory.Museums:
                    client.BaseAddress = new Uri("http://localhost:18323/api/Museums/GetMuseumsNearby");
                    break;
                case PlaceCategory.Hospitals:
                    client.BaseAddress = new Uri("http://localhost:18323/api/Hospitals/GetHospitalsNearby");
                    break;
                //case PlaceCategory.Hotels:
                //    client.BaseAddress = new Uri("http://localhost:18323/api/Hotels/GetHotelsNearby");
                //    break;
                //case PlaceCategory.Restaurants:
                //    client.BaseAddress = new Uri("http://localhost:18323/api/Restaurants/GetRestaurantsNearby");
                //    break;
                //case PlaceCategory.Shops:
                //    client.BaseAddress = new Uri("http://localhost:18323/api/Shops/GetShopsNearby");
                //    break;
                default:
                    client.BaseAddress = new Uri("http://localhost:18323/api/Museums/GetHospitalsNearby");
                    break;
            }
            string urlParameters = "?lat=" + latitude + "&lng=" + longitude;
            HttpResponseMessage response = client.GetAsync(urlParameters).Result;
            var result = response.Content.ReadAsStringAsync().Result;
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MarkLocation(Location location, string type)
        {
            string user = Session["logged_username"].ToString();

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:18323/api/MarkedLocations/MarkLocation");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var json = new JavaScriptSerializer().Serialize(location);
            string urlParameters = "?user=" + user + "&location=" + json.ToString() + "&type=" + type;

            HttpContent content = new StringContent("");
            HttpResponseMessage response = client.PostAsync(urlParameters, content).Result;

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