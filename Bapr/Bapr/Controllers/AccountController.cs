using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Mvc;
using BaprModels;

namespace Bapr.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        private UserPreference GetUserProfile()
        {
            var result = new UserPreference();
            result.Interests = new List<Interest>();
            result.Interests.Add(new Interest() { Checked = false, Name = "Literature" });
            result.Interests.Add(new Interest() { Checked = false, Name = "History" });
            result.Interests.Add(new Interest() { Checked = false, Name = "Shopping" });
            result.Interests.Add(new Interest() { Checked = false, Name = "Dancing" });
            result.Interests.Add(new Interest() { Checked = false, Name = "Geography" });
            result.Interests.Add(new Interest() { Checked = false, Name = "Cuisine" });
            result.Interests.Add(new Interest() { Checked = false, Name = "Archeology" });

            result.Cuisine = new List<Interest>();
            result.Cuisine.Add(new Interest() { Checked = false, Name = "French" });
            result.Cuisine.Add(new Interest() { Checked = false, Name = "Italian" });
            result.Cuisine.Add(new Interest() { Checked = false, Name = "Asian" });
            result.Cuisine.Add(new Interest() { Checked = false, Name = "Seefood" });
            result.Cuisine.Add(new Interest() { Checked = false, Name = "Indian" });

            return result;
        }

        //
        // GET: /Auth/
        public ActionResult UserPreference()
        {
            var result = GetUserProfile();

            return View(result);
        }


        [HttpPost]
        public ActionResult UserPreference(UserPreference userPreferences)
        {
            ViewBag.result = "Data Saved Successfully!";
            return View(userPreferences);
        }
    }
}