using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Mvc;
using BaprModels;
using Bapr.Models;

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

        public ActionResult Register()
        {
            //var result = new UserAccount();
            //string baseUri = "http://localhost:1256/api/authapi/5";

            //using (var client = new HttpClient())
            //{

            //    var response = client.GetAsync(baseUri).Result;
            //    if (response.IsSuccessStatusCode)
            //    {
            //        result = JsonConvert.DeserializeObject<UserAccount>(response.Content.ReadAsStringAsync().Result);

            //    }
            //}
            return View();
        }
        [HttpPost]
        public ActionResult Register(Register model)
        {
            //string baseUri = "http://localhost:1256/api/authapi";

            //using (var client = new HttpClient())
            //{

            //    var response = client.PostAsync(baseUri, user, new JsonMediaTypeFormatter()).Result;
            //    if (response.IsSuccessStatusCode)
            //    {
            //        var result = response.Content.ReadAsAsync<int>().Result;
            //        Console.WriteLine("Performance instance successfully sent to the API");
            //    }
            //}
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index", "Bapr");
            }
            return View(model);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Login userModel)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index", "Bapr");
            }
            return View(userModel);
        }

        [HttpPost]
        public ActionResult LogOut()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}