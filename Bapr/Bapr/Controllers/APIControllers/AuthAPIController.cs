using Bapr.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Bapr.Controllers
{
    public class AuthAPIController : ApiController
    {
        // GET api/authapi
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/authapi/5
        public UserAccount Get(int id)
        {
            return new UserAccount
            {
                FirstName="test"
            };
        }

        // POST api/authapi
        public int Post(UserAccount userAccount)
        {
            return 100;
        }

        // PUT api/authapi/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/authapi/5
        public void Delete(int id)
        {
        }
    }
}
