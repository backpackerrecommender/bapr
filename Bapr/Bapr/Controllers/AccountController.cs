using Bapr.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;

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
            result.Interests = new Collection<Interest>();
            result.Interests.Add(new Interest() { Checked = false, Name = "Literature" });
            result.Interests.Add(new Interest() { Checked = false, Name = "History" });
            result.Interests.Add(new Interest() { Checked = false, Name = "Shopping" });
            result.Interests.Add(new Interest() { Checked = false, Name = "Dancing" });
            result.Interests.Add(new Interest() { Checked = false, Name = "Geography" });
            result.Interests.Add(new Interest() { Checked = false, Name = "Cuisine" });
            result.Interests.Add(new Interest() { Checked = false, Name = "Archeology" });

            result.Cuisine = new Collection<Interest>();
            result.Cuisine.Add(new Interest() { Checked = false, Name = "French" });
            result.Cuisine.Add(new Interest() { Checked = false, Name = "Italian" });
            result.Cuisine.Add(new Interest() { Checked = false, Name = "Asian" });
            result.Cuisine.Add(new Interest() { Checked = false, Name = "Seafood" });
            result.Cuisine.Add(new Interest() { Checked = false, Name = "Indian" });

            return result;
        }

        private HttpResponseMessage SaveUserPreferences(UserPreference preferences, string user)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:18323/api/UserPreference/SaveUserPreference");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var json = new JavaScriptSerializer().Serialize(preferences);
            string urlParameters = "?user=" + user + "&preferences=" + json.ToString();
           
            HttpContent content = new StringContent("");
            HttpResponseMessage response = client.PostAsync(urlParameters, content).Result;
            
            return response;
        }

        //
        // GET: /Auth/
        public ActionResult UserPreference()
        {
            var result = GetUserProfile();

            return View(result);
        }


        [HttpPost]
        public ActionResult UserPreference(UserPreference result)
        {
            result.Interests = new Collection<Interest>();
            result.SelectedInterests = result.SelectedInterests != null ? result.SelectedInterests : new Collection<string>();
            result.Interests.Add(new Interest() { Checked = result.SelectedInterests.Contains("Literature"), Name = "Literature" });
            result.Interests.Add(new Interest() { Checked = result.SelectedInterests.Contains("History"), Name = "History" });
            result.Interests.Add(new Interest() { Checked = result.SelectedInterests.Contains("Shopping"), Name = "Shopping" });
            result.Interests.Add(new Interest() { Checked = result.SelectedInterests.Contains("Dancing"), Name = "Dancing" });
            result.Interests.Add(new Interest() { Checked = result.SelectedInterests.Contains("Geography"), Name = "Geography" });
            result.Interests.Add(new Interest() { Checked = result.SelectedInterests.Contains("Cuisine"), Name = "Cuisine" });
            result.Interests.Add(new Interest() { Checked = result.SelectedInterests.Contains("Archeology"), Name = "Archeology" });

            result.Cuisine = new Collection<Interest>();
            result.SelectedCuisine = result.SelectedCuisine != null ? result.SelectedCuisine : new Collection<string>();
            result.Cuisine.Add(new Interest() { Checked = result.SelectedCuisine.Contains("French"), Name = "French" });
            result.Cuisine.Add(new Interest() { Checked = result.SelectedCuisine.Contains("Italian"), Name = "Italian" });
            result.Cuisine.Add(new Interest() { Checked = result.SelectedCuisine.Contains("Asian"), Name = "Asian" });
            result.Cuisine.Add(new Interest() { Checked = result.SelectedCuisine.Contains("Seafood"), Name = "Seafood" });
            result.Cuisine.Add(new Interest() { Checked = result.SelectedCuisine.Contains("Indian"), Name = "Indian" });

            HttpResponseMessage response = SaveUserPreferences(result, Session["logged_username"].ToString());
            if (response.IsSuccessStatusCode)
            {
                ViewBag.result = "Data Saved Successfully!";
            }
            return View(result);
        }

        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(Register model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:18323/api/LogIn/Register");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            string urlParameters = "?email=" + model.Email + "&password=" + EncryptPassword(model.Password);

            HttpContent content = new StringContent("");
            HttpResponseMessage response = client.PostAsync(urlParameters, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login", "Account");
            }

            return RedirectToAction("Register", "Account");
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Login userModel)
        {
            if (!ModelState.IsValid)
            {
                return View(userModel);
            }
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:18323/api/LogIn/LogIn");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            string urlParameters = "?email=" + userModel.Email + "&password=" + EncryptPassword(userModel.Password);

            HttpContent content = new StringContent("");
            HttpResponseMessage response = client.PostAsync(urlParameters, content).Result;
            if (response.IsSuccessStatusCode)
            {
                Session["logged_username"] = userModel.Email;
                return RedirectToAction("Index", "Bapr");
            }

            return RedirectToAction("Login", "Account");
        }

        public ActionResult LogOut()
        {
            Session["logged_username"] = "";
            return RedirectToAction("Index", "Home");
        }

        private string EncryptPassword(string password)
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes(password);
            data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
            return System.Text.Encoding.ASCII.GetString(data);
        }
    }
}