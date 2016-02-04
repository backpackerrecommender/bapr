using BaprAPI.Models;
using BrightstarDB.Client;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace BaprAPI.Controllers
{
    public class UserPreferenceController : ApiController
    {

        [HttpPost]
        [ActionName("SaveUserPreference")]
        public HttpResponseMessage SaveUserPreference(string user, string preferences)
        {
            string connectionString = "type=embedded;storesdirectory=" + "D:\\brightstar" + ";storename=Users";
            var client = BrightstarService.GetClient(connectionString);
            UserPreferenceModel userPreferencesModel =
                new JavaScriptSerializer().Deserialize<UserPreferenceModel>(preferences);

            using (var ctx = new MyEntityContext(connectionString))
            {
                var userPreference = ctx.UserPreferences.Create();

                List<IInterest> interestsList = new List<IInterest>();
                foreach (var item in userPreferencesModel.Interests)
                {
                    var interests = ctx.Interests.Create();
                    interests.Name = item.Name;
                    interests.Checked = item.Checked;
                    interestsList.Add(interests);
                }
                //add Interests collection
                ICollection<IInterest> interestsCollection = interestsList;
                userPreference.Interests = interestsCollection;

                List<IInterest> cuisineList = new List<IInterest>();
                foreach (var item in userPreferencesModel.Cuisine)
                {
                    var interests = ctx.Interests.Create();
                    interests.Name = item.Name;
                    interests.Checked = item.Checked;
                    cuisineList.Add(interests);
                }
                //add Cuisine collection
                ICollection<IInterest> cuisineCollection = cuisineList;
                userPreference.Cuisine = cuisineCollection;

                userPreference.MaxBudget = userPreferencesModel.MaxBudget;
                userPreference.NeedMedicalSupport = userPreferencesModel.NeedMedicalSupport;
                userPreference.NoOfDays = userPreferencesModel.NoOfDays;

                var currentUser = ctx.Users.FirstOrDefault(x => x.Email == user);
                IUserPreference usrPreferences = userPreference;
                currentUser.UserPreference = usrPreferences;

                ctx.SaveChanges();

                return new HttpResponseMessage(HttpStatusCode.OK);//200
            }
        }


        [HttpGet]
        [ActionName("GetUserPreference")]
        public string GetUserPreference(string user)
        {
            UserPreferenceModel usrPreference = null;
            string connectionString = "type=embedded;storesdirectory=" + "D:\\brightstar" + ";storename=Users";
            var client = BrightstarService.GetClient(connectionString);


            using (var ctx = new MyEntityContext(connectionString))
            {
                var currentUser = ctx.Users.FirstOrDefault(x => x.Email == user);
                if (currentUser != null && currentUser.UserPreference != null)
                {
                    List<IInterest> listIInterests = currentUser.UserPreference.Interests.ToList<IInterest>();
                    List<Interest> listInterest = listIInterests.ConvertAll(obj => (Interest)obj);
                    listInterest.Sort(delegate(Interest interest1, Interest interest2) { return interest1.Name.CompareTo(interest2.Name); });
                    ICollection<Interest> interestsCollection = listInterest;

                    listIInterests = currentUser.UserPreference.Cuisine.ToList<IInterest>();
                    listInterest = listIInterests.ConvertAll(obj => (Interest)obj);
                    listInterest.Sort(delegate(Interest interest1, Interest interest2) { return interest1.Name.CompareTo(interest2.Name); });
                    ICollection<Interest> cuisineCollection = listInterest;

                    usrPreference = new UserPreferenceModel()
                    {
                        NeedMedicalSupport = currentUser.UserPreference.NeedMedicalSupport,
                        Cuisine = cuisineCollection,
                        Interests = interestsCollection,
                        NoOfDays = currentUser.UserPreference.NoOfDays,
                        MaxBudget = currentUser.UserPreference.MaxBudget
                    };
                }
                var json = JsonConvert.SerializeObject(usrPreference,
                                    Formatting.None,
                                        new JsonSerializerSettings()
                                        {
                                            ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                                        });

                return json;
            }
        }
    }
}
