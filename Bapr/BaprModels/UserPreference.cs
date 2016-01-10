using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaprModels
{
    public class Interest
    {
        public string Name { get; set; }
        public bool Checked { get; set; }
    }

    public class UserPreference
    {
        public List<Interest> Interests { get; set; }
        public List<Interest> Cuisine { get; set; }
        public long MaxBudget { get; set; }
        public int NoOfDays { get; set; }
        public bool NeedMedicalSupport { get; set; }
    }

}
