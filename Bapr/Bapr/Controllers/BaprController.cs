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
            string user = GetUserEmail();

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
            string user = GetUserEmail();

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
            string url = string.IsNullOrEmpty(text) ?
                "http://localhost:18323/api/Locations/InferredGet" :
                "http://localhost:18323/api/Locations/Get";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var urlParam = "?text=" + text + "&latitude=" + latitude
                                + "&longitude=" + longitude + "&userEmail=" + GetUserEmail();
                HttpResponseMessage response = client.GetAsync(urlParam).Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }

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
                    client.BaseAddress = new Uri("http://localhost:18323/api/Hospitals/GetAllHospitalsNearby");
                    break;
                case PlaceCategory.Hotels:
                    client.BaseAddress = new Uri("http://localhost:18323/api/Hotels/GetByCoordinates");
                    break;
                case PlaceCategory.Restaurants:
                    client.BaseAddress = new Uri("http://localhost:18323/api/Restaurants/GetRestaurantsNearby");
                    break;
                case PlaceCategory.Shops:
                    client.BaseAddress = new Uri("http://localhost:18323/api/Shops/GetShopsNearby");
                    break;
                default:
                    client.BaseAddress = new Uri("http://localhost:18323/api/Museums/GetAllHospitalsNearby");
                    break;
            }

            var userEmail = GetUserEmail();
            var urlParam = "?lat=" + latitude + "&lng=" + longitude + "&userEmail=" + userEmail;
            HttpResponseMessage response = client.GetAsync(urlParam).Result;
            
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MarkLocation(Location location, string type)
        {
            string user = GetUserEmail();
            if (!String.IsNullOrEmpty(user))
            {
                string uri = "http://localhost:18323/api/MarkedLocations/MarkLocation";
                HttpClient client = new HttpClient();

                var formVars = new Dictionary<string, string>();
                formVars.Add("user", user);
                formVars.Add("type", type);
                formVars.Add("location", new JavaScriptSerializer().Serialize(location));
                var content = new FormUrlEncodedContent(formVars);

                HttpResponseMessage response = client.PostAsync(uri, content).Result;
            }
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

        private string GetUserEmail()
        {
            var session = Session["logged_username"];
            var userEmail = string.Empty;
            if (session != null)
            {
                userEmail = session.ToString();
            }
            return userEmail;
        }
    }
}