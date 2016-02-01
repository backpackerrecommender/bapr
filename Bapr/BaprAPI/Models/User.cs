using BrightstarDB.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BaprAPI.Models
{
    [Entity]
    public interface IUser
    {
        string Email { get; set; }

        string Password { get; set; }

        IUserPreference UserPreference {get; set;}
    }
}