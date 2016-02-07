using BaprAPI.Models;
using BrightstarDB.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using VDS.RDF.Query;

namespace BaprAPI.Utils
{
    public static class Utils
    {
        public static IUserPreference GetUserPreference(string email)
        {
            var connectionString = "type=embedded;storesdirectory=" + "D:\\brightstar" + ";storename=Users";
            var client = BrightstarService.GetClient(connectionString);

            using (var ctx = new MyEntityContext(connectionString))
            {
                var currentUser = ctx.Users.FirstOrDefault(x => x.Email == email);
                if (currentUser != null)
                {
                    return currentUser.UserPreference;
                }
            }
            return new UserPreference();
        }

        public static string GetCuisines(IUserPreference userPreference, bool addXsdSTring)
        {
            var result = string.Empty;
            var cuisines = userPreference.Cuisine.Where(c => c.Checked == true).Select(c => c.Name.ToLower());
            foreach (var cuisine in cuisines)
            {
                result = result + "\"" + cuisine + "\"" + (addXsdSTring ? Constants.xsdString.ToLower()  : string.Empty) +",";
            }
            result = !string.IsNullOrEmpty(result) ? result.Remove(result.Length - 1) : result;
            return result;
        }

        public static Collection<BaprLocation> ConvertFromSparqlSetToBaprLocations(SparqlResultSet entities)
        {
            var finalResult = new Collection<BaprLocation>();
            foreach (VDS.RDF.Query.SparqlResult result in entities)
            {
                var baprLocation = new BaprLocation
                {
                    attributes = new Collection<BaprLocationAttribute>()
                };
                var properties = result.ToString().Split(',');
                foreach (var property in properties)
                {
                    var list = property.Split('=');
                    var attr = list[0].Replace("?", "");
                    var @value = list.Length > 1 ?  list[1] : string.Empty;
                    var index = @value.IndexOf('^');
                    if (index > -1)
                    {
                        @value = @value.Remove(index);
                    }

                   if (attr.Trim() == "s") { }
                   else
                    if (attr.Trim() == "lat")
                    {
                        baprLocation.latitude = Convert.ToDouble(@value);
                    }
                    else if (attr.Trim() == "long")
                    {
                        baprLocation.longitude = Convert.ToDouble(@value);
                    }
                    else if (attr.Trim() == "name")
                    {
                        var indexForName = @value.IndexOf('@');
                        if (indexForName > -1)
                        {
                            @value = @value.Remove(indexForName);
                        }
                        baprLocation.name = @value;
                    }
                   
                    else if (!string.IsNullOrWhiteSpace(@value))
                    {
                        baprLocation.attributes.Add(new BaprLocationAttribute
                        {
                            Name = attr,
                            Value = @value,
                            Type ="string"
                        });

                    }
                }
                finalResult.Add(baprLocation);
            }
            return finalResult;
        }
    }
}