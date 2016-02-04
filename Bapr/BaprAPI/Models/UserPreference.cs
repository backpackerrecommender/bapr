using BrightstarDB.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BaprAPI.Models
{
    [Entity]
    public interface IInterest
    {
        string Name { get; set; }
        bool Checked { get; set; }
    }

    [Entity]
    public interface IUserPreference
    {
        ICollection<IInterest> Interests { get; set; }
        ICollection<IInterest> Cuisine { get; set; }
        long MaxBudget { get; set; }
        int NoOfDays { get; set; }
        bool NeedMedicalSupport { get; set; }
    }

    public class UserPreferenceModel
    {
        public long MaxBudget { get; set; }
        public int NoOfDays { get; set; }
        public bool NeedMedicalSupport { get; set; }
        public ICollection<Interest> Interests { get; set; }
        public ICollection<Interest> Cuisine { get; set; }
    }
}