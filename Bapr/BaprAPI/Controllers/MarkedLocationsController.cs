using BaprAPI.Models;
using BrightstarDB.Client;
using BrightstarDB.EntityFramework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace BaprAPI.Controllers
{
    public class MarkedLocationsController : ApiController
    {
        [HttpPost]
        [ActionName("MarkLocation")]
        public HttpResponseMessage MarkLocation(string user, string location, string type)
        {
            string connectionString = Constants.storeConnectionString;
            var client = BrightstarService.GetClient(Constants.storeConnectionString);

            ILocation markedLocation = JsonConvert.DeserializeObject<Location>(location);
            using (var ctx = new MyEntityContext(connectionString))
            {
                IUser usr = ctx.Users.Single(x => x.Email == user);

                if (!usr.MarkedLocations.Any(x => x.Equals(markedLocation)))
                {
                    var marker = ctx.Locations.Create();
                    marker.name = markedLocation.name;
                    marker.latitude = markedLocation.latitude;
                    marker.longitude = markedLocation.longitude;
                    marker.attributes = markedLocation.attributes;
                    if(type == "fav")
                        marker.IsFavorite = markedLocation.IsFavorite;
                    else
                    marker.IsVisited = markedLocation.IsVisited;
                    usr.MarkedLocations.Add(marker);
                }
                else
                {
                    //update existing location
                    ILocation existingLocation = usr.MarkedLocations.Single(x => x.Equals(markedLocation));
                    if (type == "fav")
                        existingLocation.IsFavorite = markedLocation.IsFavorite;
                    else
                        existingLocation.IsVisited = markedLocation.IsVisited;
                }
                ctx.SaveChanges();

                return new HttpResponseMessage(HttpStatusCode.OK);//200
            }
        }

        [HttpGet]
        [ActionName("Favorites")]
        public Collection<BaprLocation> Favorites(string user)
        {
            string connectionString = Constants.storeConnectionString;
            var client = BrightstarService.GetClient(Constants.storeConnectionString);

            using (var ctx = new MyEntityContext(connectionString))
            {
                IUser usr = ctx.Users.Single(x => x.Email == user);
                ICollection<ILocation> result;
                if (!usr.MarkedLocations.Any())
                    result = new Collection<ILocation>();
                else
                    result = usr.MarkedLocations.Where(x => x.IsFavorite == true).ToList();

                Collection<BaprLocation> baprLocations = Convert(result);
                return baprLocations;
            }
        }

        [HttpGet]
        [ActionName("Visited")]
        public Collection<BaprLocation> Visited(string user)
        {
            string connectionString = Constants.storeConnectionString;
            var client = BrightstarService.GetClient(Constants.storeConnectionString);

            using (var ctx = new MyEntityContext(connectionString))
            {
                IUser usr = ctx.Users.Single(x => x.Email == user);
                ICollection<ILocation> result;
                if (!usr.MarkedLocations.Any())
                    result = new Collection<ILocation>();
                else
                    result = usr.MarkedLocations.Where(x => x.IsVisited == true).ToList();

                Collection<BaprLocation> baprLocations = Convert(result);
                return baprLocations;
            }
        }

        private Collection<BaprLocation> Convert(ICollection<ILocation> locations)
        {
            Collection<BaprLocation> baprLocations = new Collection<BaprLocation>();
            foreach (var location in locations)
            {
                BaprLocation baprLocation = new BaprLocation()
                {
                    name = location.name,
                    latitude = location.latitude,
                    longitude = location.longitude,
                    IsVisited = location.IsVisited,
                    IsFavorite = location.IsFavorite
                };
                baprLocation.attributes = new Collection<BaprLocationAttribute>();
                foreach (var attr in location.attributes)
                {
                    baprLocation.attributes.Add(new BaprLocationAttribute()
                    {
                        Name = attr.Name,
                        Value = attr.Value,
                        Type = attr.Type
                    });
                }
                baprLocations.Add(baprLocation);
            }
            return baprLocations;
        }
    }
}
