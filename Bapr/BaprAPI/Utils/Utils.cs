using BaprAPI.Models;
using BrightstarDB.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Parsing.Handlers;
using VDS.RDF.Query;

namespace BaprAPI.Utils
{
    public static class Utils
    {
        public static IUserPreference GetUserPreference(string email)
        {
            var connectionString = "type=embedded;storesdirectory=" + Constants.StoreLocation + ";storename=Users";
            var client = BrightstarService.GetClient(connectionString);

            using (var ctx = new MyEntityContext(connectionString))
            {
                var currentUser = ctx.Users.FirstOrDefault(x => x.Email == email);
                if (currentUser != null)
                {
                    return currentUser.UserPreference ?? new UserPreference();
                }
            }
            return new UserPreference();
        }

        public static string GetCuisines(IUserPreference userPreference, bool addXsdSTring)
        {
            var result = string.Empty;
            if (userPreference != null && userPreference.Cuisine != null)
            {
                var cuisines = userPreference.Cuisine.Where(c => c.Checked == true).Select(c => c.Name.ToLower());
                foreach (var cuisine in cuisines)
                {
                    result = result + "\"" + cuisine + "\"" + (addXsdSTring ? Constants.xsdString : string.Empty) + ",";
                }
                result = !string.IsNullOrEmpty(result) ? result.Remove(result.Length - 1) : result;
            }
            return result;
        }

        public static string GetInterests(IUserPreference userPreference)
        {
            var result = string.Empty;
            if (userPreference != null && userPreference.Interests != null)
            {
                var interests = userPreference.Interests.Where(c => c.Checked == true).Select(c => c.Name.ToLower());
                foreach (var interest in interests)
                {
                    result = result + string.Format("<http://dbpedia.org/resource/{0}>", interest) + ",";
                }
                result = !string.IsNullOrEmpty(result) ? result.Remove(result.Length - 1) : result;

            }

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

                    if (attr.Trim() == "head_chef" || attr.Trim().Contains("type") || attr.Trim() == "health_care")
                    {
                        string decodedUrlValue = HttpUtility.UrlDecode(@value);
                        var headChef = decodedUrlValue.Substring(decodedUrlValue.LastIndexOf('/') + 1);
                        @value = headChef;
                    }
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
                        @value = @value.Trim();
                        var indexForName = @value.IndexOf('@');
                        if (indexForName == @value.Length - 3 && indexForName > -1)
                        {
                            @value = @value.Remove(indexForName);
                        }
                        baprLocation.attributes.Add(new BaprLocationAttribute
                        {
                            Name = attr,
                            Value = attr.Trim() == "wheelchair" ? "True" : @value,
                            Type = attr.Trim() == "wheelchair" ? "bool" : "string"
                        });

                    }
                }
                finalResult.Add(baprLocation);
            }
            return finalResult;
        }

        private static Collection<BaprLocation> ExecuteQuery(ISparqlQueryProcessor processor, SparqlQuery query)
        {
            try
            {
                object queryResult = processor.ProcessQuery(query);
                if (queryResult is SparqlResultSet)
                {
                    SparqlResultSet entities = (SparqlResultSet)queryResult;
                    return BaprAPI.Utils.Utils.ConvertFromSparqlSetToBaprLocations(entities);
                }
            }
            catch (VDS.RDF.Query.RdfQueryException ex)
            {

            }
            return new Collection<BaprLocation>();
        }

        public static Collection<BaprLocation> ParseSparqlQuery(SparqlParameterizedString queryString, string endpointUrl)
        {
            try
            {
                SparqlQueryParser parser = new SparqlQueryParser();
                SparqlQuery query = parser.ParseFromString(queryString);

                Uri uri = new Uri(endpointUrl);
                SparqlRemoteEndpoint endPoint = new SparqlRemoteEndpoint(uri);
                ISparqlQueryProcessor processor = new RemoteQueryProcessor(endPoint);
                return BaprAPI.Utils.Utils.ExecuteQuery(processor, query);
            }
            catch (VDS.RDF.RdfException ex)
            {

            }
            return new Collection<BaprLocation>();
        }
    }
}