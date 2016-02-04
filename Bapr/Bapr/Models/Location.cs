using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bapr.Models
{
    public class Location
    {
        public double latitude { get; set; }

        public double longitude { get; set; }

        public String name { get; set; }

        public ICollection<LocationAttribute> attributes { get; set; }
    }

    public class LocationAttribute
    {
        public String Name { get; set; }
        public String Value { get; set; }
        public String Type { get; set; }

        public LocationAttribute(String name, String value, String type)
        {
            this.Name = name;
            this.Value = value;
            this.Type = type;
        }
    }
}