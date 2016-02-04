using BrightstarDB.EntityFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BaprAPI.Models
{
    [Entity]
    public interface ILocation
    {
        double latitude { get; set; }

        double longitude { get; set; }

        String name { get; set; }
        ICollection<IAttribute> attributes { get; set; }
    }

    [Entity]
    public interface IAttribute
    {
        String Name { get; set; }

        string Value { get; set; }

        String Type { get; set; }
    }
}