using BaprAPI.Models;
using BrightstarDB.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BaprAPI.Utils
{
    public static class Utils
    {
        public static IUserPreference GetUserPreference(string email)
        {
            var connectionString = "type=embedded;storesdirectory=" + "D:\\brightstar" + ";storename=Users";
            var client = BrightstarService.GetClient(connectionString);

            using (var ctx = new MyEntityContext(connectionString))
            {
                var currentUser = ctx.Users.FirstOrDefault(x => x.Email == email);
                if (currentUser != null)
                {
                    return currentUser.UserPreference;
                }
            }
            return new UserPreference();
        }
    }
}