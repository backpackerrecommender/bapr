using BrightstarDB.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bapr.Models
{
    public interface IUser
    {
        string Email { get; set; }

        string Password { get; set; }

        IUserPreference UserPreference { get; set; }
    }

    [Entity]
    public interface IInterest
    {
        string Name { get; set; }
        bool Checked { get; set; }
    }

    [Entity]
    public interface IUserPreference
    {
        long MaxBudget { get; set; }
        int NoOfDays { get; set; }
        bool NeedMedicalSupport { get; set; }
        ICollection<IInterest> Interests { get; set; }
        ICollection<IInterest> Cuisine { get; set; }
        ICollection<string> SelectedInterests { get; set; }
        ICollection<string> SelectedCuisine { get; set; }

    }




}