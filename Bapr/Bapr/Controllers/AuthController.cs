using Bapr.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Bapr.Controllers
{
    public class AuthController : Controller
    {
        //
        // GET: /Auth/
        public ActionResult Register()
        {
            var result = new UserAccount();
            string baseUri = "http://localhost:1256/api/authapi/5";

            using (var client = new HttpClient())
            {

                var response = client.GetAsync(baseUri).Result;
                if (response.IsSuccessStatusCode)
                {
                     result = JsonConvert.DeserializeObject<UserAccount>(response.Content.ReadAsStringAsync().Result);
                   
                }
            }
            return View(result);
        }
        [HttpPost]
        public ActionResult Register(UserAccount user)
        {
            string baseUri = "http://localhost:1256/api/authapi";

            using (var client = new HttpClient())
            {

                var response = client.PostAsync(baseUri, user, new JsonMediaTypeFormatter()).Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsAsync<int>().Result;
                    Console.WriteLine("Performance instance successfully sent to the API");
                } 
            }
            return View();
        }

    }
}
