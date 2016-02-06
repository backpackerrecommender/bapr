using BaprAPI.Models;
using BrightstarDB.Client;
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
            string connectionString = Constants.storeConnectionString;
            var client = BrightstarService.GetClient(connectionString);
            UserPreferenceModel userPreferencesModel =
                new JavaScriptSerializer().Deserialize<UserPreferenceModel>(preferences);

            using (var ctx = new MyEntityContext(connectionString))
            {
                var userPreference = ctx.UserPreferences.Create();
               
                List<IInterest> interestsList = new List<IInterest>();        
                foreach (var item in  userPreferencesModel.Interests)
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
    }
}
