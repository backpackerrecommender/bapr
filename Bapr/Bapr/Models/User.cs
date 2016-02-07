using BrightstarDB.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace Bapr.Models
{
    public class User
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public UserPreference UserPreference { get; set; }
    }

    public class Interest
    {
        public string Name { get; set; }
        public bool Checked { get; set; }
    }

    public class UserPreference
    {
        public long MaxBudget { get; set; }
        public int NoOfDays { get; set; }
        public bool NeedWheelchair { get; set; }
        public ICollection<Interest> Interests { get; set; }
        public ICollection<Interest> Cuisine { get; set; }
        [ScriptIgnore()]
        public ICollection<string> SelectedInterests { get; set; }
        [ScriptIgnore()]
        public ICollection<string> SelectedCuisine { get; set; }

    }




}