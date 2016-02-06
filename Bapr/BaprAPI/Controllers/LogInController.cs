using BaprAPI.Models;
using BrightstarDB.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BaprAPI.Controllers
{
    public class LogInController : ApiController
    {
        [HttpPost]
        [ActionName("Register")]
        public HttpResponseMessage Register(string Email, string Password)
        {
            string connectionString = Constants.storeConnectionString;
            
            var client = BrightstarService.GetClient(connectionString);

            using (var ctx = new MyEntityContext(connectionString))
            {
                if (ctx.Users.FirstOrDefault(x => x.Email == Email) != null)
                {
                    return new HttpResponseMessage(HttpStatusCode.Conflict);//409
                }

                var user = ctx.Users.Create();
                user.Email = Email;
                user.Password = Password;
                ctx.SaveChanges();

                return new HttpResponseMessage(HttpStatusCode.OK);//200
            }
        }

        [HttpPost]
        [ActionName("LogIn")]
        public HttpResponseMessage LogIn(string Email, string Password)
        {
            string connectionString = Constants.storeConnectionString;
            
            var client = BrightstarService.GetClient(connectionString);
            
            using (var ctx = new MyEntityContext(connectionString))
            {
                if (ctx.Users.FirstOrDefault(x => x.Email == Email && x.Password == Password) != null)
                {
                    return new HttpResponseMessage(HttpStatusCode.OK);//200
                }
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);//401
            }
        }
    }
}
