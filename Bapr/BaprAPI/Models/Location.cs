using BrightstarDB.EntityFramework;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

namespace BaprAPI.Models
{
    [Entity]
    [JsonConverter(typeof(LocationConverter))]
    public interface ILocation
    {
        double latitude { get; set; }

        double longitude { get; set; }

        String name { get; set; }
        bool IsVisited { get; set; }
        bool IsFavorite { get; set; }
        ICollection<ILocationAttribute> attributes { get; set; }
    }

    [Entity]
    public interface ILocationAttribute
    {
        String Name { get; set; }

        string Value { get; set; }

        String Type { get; set; }
    }

    public partial class Location
    {
        public override bool Equals(object other)
        {
            var item = other as Location;

            if (item == null)
                return false;

            if (this.name != item.name)
                return false;

            if (this.latitude != item.latitude || this.longitude != item.longitude)
                return false;

            return this.attributes.OrderBy(x => x.Name).SequenceEqual(item.attributes.OrderBy(x => x.Name));
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public partial class LocationAttribute 
    {
        public override bool Equals(object other)
        {
            var item = other as LocationAttribute;

            if (item == null)
                return false;

            return this.Name == item.Name && this.Value == this.Value && this.Type == this.Type;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    class LocationConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Location));
        }
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            Location location = new Location();
            location.name = (String)jo["name"];
            location.latitude = (double)jo["latitude"];
            location.longitude = (double)jo["longitude"];
            location.IsFavorite = (bool)jo["IsFavorite"];
            location.IsVisited = (bool)jo["IsVisited"];
            location.attributes = new Collection<ILocationAttribute>();
            foreach (JObject obj in jo["attributes"])
            {
                location.attributes.Add(obj.ToObject<LocationAttribute>(serializer));
            }
            return location;
        }
        public override bool CanWrite
        {
            get { return false; }
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public class BaprLocation
    {
        public double latitude { get; set; }

        public double longitude { get; set; }

        public String name { get; set; }
        public bool IsVisited { get; set; }
        public bool IsFavorite { get; set; }
        public Collection<BaprLocationAttribute> attributes { get; set; }
    }

    public class BaprLocationAttribute
    {
        public String Name { get; set; }
        public string Value { get; set; }
        public String Type { get; set; }
    }
}