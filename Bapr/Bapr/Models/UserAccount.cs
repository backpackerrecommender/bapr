using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bapr.Models
{
    public class UserAccount
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public Genre Genre { get; set; }
        public string ContactPersonEmail { get; set; }
        public bool IsVegetarian { get; set; }
        public Cuisine Cuisine { get; set; }
        public bool PreferHistory { get; set; }
        public decimal MaxBudget { get; set; }
        public int NoOfDays { get; set; }
        public bool PreferDance { get; set; }
        public bool NeedMedicalSupport { get; set; }
        public string OtherComments { get; set; }
        
    }
    public enum Genre { 
        Female , 
        Male
    }
    public enum Cuisine
    {
        NoPreference,
        Italian,
        Mediteranian,
        American,
        French
        
    }
}