using BrightstarDB.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
        public bool NeedMedicalSupport { get; set; }
        public ICollection<Interest> Interests { get; set; }
        public ICollection<Interest> Cuisine { get; set; }
        public ICollection<string> SelectedInterests { get; set; }
        public ICollection<string> SelectedCuisine { get; set; }

    }




}