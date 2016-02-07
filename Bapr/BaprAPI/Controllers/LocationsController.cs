using BaprAPI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Query.Inference;
using VDS.RDF.Writing;

namespace BaprAPI.Controllers
{
    public class LocationsController : ApiController
    {
        public HttpResponseMessage Get(string text, double latitude, double longitude, string userEmail)
        {
            var userPreference = BaprAPI.Utils.Utils.GetUserPreference(userEmail);
            var fromDbPedia = GetFromDbpedia(text, latitude, longitude, userPreference);
            var fromLinkedGeoData = GetFromLinkedGeoData(text, latitude, longitude, userPreference);
            var allCollections = fromDbPedia.Concat(fromLinkedGeoData);
            return Request.CreateResponse(HttpStatusCode.OK, allCollections);
        }

        public ICollection<BaprLocation> GetFromDbpedia(string text, double latitude, double longitude, IUserPreference userPreference)
        {
            var cuisineFromDb = BaprAPI.Utils.Utils.GetCuisines(userPreference, true);
            var interests = BaprAPI.Utils.Utils.GetInterests(userPreference);
            string searchQueryByText = "SELECT DISTINCT ?lat ?long ?name ?address ?cuisine ?website ?established ?comment WHERE{\n"
              + "?s a dbo:Location .\n"
              + "?s geo:lat ?lat .\n"
              + "?s geo:long ?long .\n"
              + "?s rdfs:label ?name.\n"
              + "?s a ?type. \n"
              + "OPTIONAL { ?s dbp:established ?established.}\n"
              + "OPTIONAL { ?s rdfs:comment ?comment. FILTER(langMatches(lang(?comment ), \"en\"))}\n"
              + "OPTIONAL { ?s dbo:address ?address .}\n "
              + "OPTIONAL { ?s dbo:cuisine ?cuisine .}\n"
              + "OPTIONAL { ?s foaf:homepage ?website .}\n"
              + "FILTER(langMatches(lang(?name ), \"en\") "
                + " && ?lat >" + latitude + " - 0.5 && ?lat < " + latitude + "+ 0.5 "
                + " && ?long > " + longitude + " - 0.5 && ?long < " + longitude + " + 0.5"
               + "  && contains(lcase(?name),lcase(\"" + text.ToLower() + "\"))"
               + "  && ((?type=dbo:Restaurant " + (!string.IsNullOrEmpty(cuisineFromDb) ? "&& lcase(?cuisine) in (" + cuisineFromDb + ")" : string.Empty) + ")\n"
                      + "|| (?type = dbo:Museum  " + (!string.IsNullOrEmpty(interests) ? " &&?type in (" + interests + ")" : string.Empty) + ")\n"

                      + "|| ?type=dbo:Town || ?type=dbo:Village "
                      + "|| ?type=dbo:Park || ?type=dbo:Garden || ?type=dbo:HistoricPlace || ?type=dbo:Monument)) "
              + "}LIMIT 10";

            SparqlParameterizedString sparqlQueryString = new SparqlParameterizedString();
            sparqlQueryString.Namespaces.AddNamespace("rdf", new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            sparqlQueryString.Namespaces.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));
            sparqlQueryString.Namespaces.AddNamespace("dbo", new Uri("http://dbpedia.org/ontology/"));
            sparqlQueryString.Namespaces.AddNamespace("geo", new Uri("http://www.w3.org/2003/01/geo/wgs84_pos#"));
            sparqlQueryString.Namespaces.AddNamespace("dbp", new Uri("http://dbpedia.org/property/"));
            sparqlQueryString.Namespaces.AddNamespace("foaf", new Uri("http://xmlns.com/foaf/0.1/"));
            sparqlQueryString.CommandText = searchQueryByText;

            return BaprAPI.Utils.Utils.ParseSparqlQuery(sparqlQueryString, @"http://dbpedia.org/sparql");
           
        }

        public ICollection<BaprLocation> GetFromLinkedGeoData(string text, double latitude, double longitude, IUserPreference userPreference)
        {
            var cuisine = BaprAPI.Utils.Utils.GetCuisines(userPreference, false);
            string searchQueryByText = "SELECT DISTINCT ?lat ?long ?name ?website ?cuisine ?address ?opening_hours ?wheelchair WHERE {\n"
                + "?s geo:lat ?lat .\n"
                + "?s geo:long ?long .\n"
                + "?s rdfs:label ?name. \n"
                + "?s a ?type. \n"
                + "OPTIONAL { ?s foaf:homepage ?website .}\n"
                + "OPTIONAL { ?s lgd:cuisine ?cuisine .}\n"
                + "OPTIONAL { ?s lgd:address ?address .}\n "
                + "OPTIONAL { ?s lgd:opening_hours  ?opening_hours .}\n "
                + (userPreference.NeedWheelchair ? "?s lgd:wheelchair ?wheelchair. FILTER (?wheelchair =" + BaprAPI.Models.Constants.xsdBooleanIsTrue + ")\n"
                                                      : " OPTIONAL {?s lgd:wheelchair ?wheelchair.} \n")
                + "FILTER (langMatches(lang(?name ), \"en\") \n"
                + " && ?lat > " + latitude + " - 0.5 && ?lat < " + latitude + " + 0.5 "
                + " && ?long > " + longitude + " - 0.5 && ?long < " + longitude + " + 0.5\n"
                + " && contains(lcase(?name),lcase(\"" + text.ToLower() + "\"))\n"
                + " && ((?type = lgd:Restaurant " + (!string.IsNullOrEmpty(cuisine) ? "&& lcase(?cuisine) in (" + cuisine + ")" : string.Empty) + ") \n"
                        + "|| ?type = lgd:Hospital || ?type = lgd:Bar || ?type = lgd:Bank \n"
                        + "|| ?type = lgd:Pharmacy || ?type = lgd:FastFood || ?type=lgd:Cafe || ?type=lgd:Pub || ?type= lgd:Theatre"
                        + "|| ?type = lgd:Museum))"
                + "} LIMIT 10";


            SparqlParameterizedString sparqlQueryString = new SparqlParameterizedString();
            sparqlQueryString.Namespaces.AddNamespace("rdf", new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            sparqlQueryString.Namespaces.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));
            sparqlQueryString.Namespaces.AddNamespace("lgd", new Uri("http://linkedgeodata.org/ontology/"));
            sparqlQueryString.Namespaces.AddNamespace("geo", new Uri("http://www.w3.org/2003/01/geo/wgs84_pos#"));
            sparqlQueryString.Namespaces.AddNamespace("foaf", new Uri("http://xmlns.com/foaf/0.1/"));
            sparqlQueryString.CommandText = searchQueryByText;

            return BaprAPI.Utils.Utils.ParseSparqlQuery(sparqlQueryString, @"http://linkedgeodata.org/sparql");
           
        }

        [HttpGet]
        public HttpResponseMessage InferredGet(string text, double latitude, double longitude)
        {
            UnionGraph resultsGraph = GetUnionGraph();
            Graph outputGraph = new Graph();

            //resultsGraph.NamespaceMap.AddNamespace("dbo", new Uri("http://dbpedia.org/ontology/"));
            Graph ontology = new Graph();
            FileLoader.Load(ontology, System.AppDomain.CurrentDomain.BaseDirectory.ToString() + Constants.Ontology);

            StaticRdfsReasoner reasoner = new StaticRdfsReasoner();
            reasoner.Initialise(ontology);
            reasoner.Apply(resultsGraph, outputGraph);

            Collection<BaprLocation> locations = ExtractResults(resultsGraph, GetInferredPlaces(outputGraph));
            return Request.CreateResponse(HttpStatusCode.OK, locations);
        }

        private UnionGraph GetUnionGraph()
        {
            SparqlQuery restaurantsQuery = GetRestaurantsQuery();
            SparqlQuery hotelsQuery = GetHotelsQuery();
            SparqlQuery museumsQuery = GetGeneralQuery("Museum");
            SparqlQuery hospitalsQuery = GetGeneralQuery("Hospital");
            SparqlQuery shopsQuery = GetGeneralQuery("ShoppingMall");

            Uri uri = new Uri(@"http://dbpedia.org/sparql");
            SparqlRemoteEndpoint endPoint = new SparqlRemoteEndpoint(uri);
            ISparqlQueryProcessor processor = new RemoteQueryProcessor(endPoint);
            Graph restaurantsGraph = (Graph)processor.ProcessQuery(restaurantsQuery);
            Graph hotelsGraph = (Graph)processor.ProcessQuery(hotelsQuery);
            Graph museumsGraph = (Graph)processor.ProcessQuery(museumsQuery);
            Graph hospitalsGraph = (Graph)processor.ProcessQuery(hospitalsQuery);
            Graph shopsGraph = (Graph)processor.ProcessQuery(shopsQuery);

            List<Graph> all = new List<Graph>();
            all.Add(restaurantsGraph);
            all.Add(hotelsGraph);
            all.Add(museumsGraph);
            all.Add(hospitalsGraph);
            all.Add(shopsGraph);
            return new UnionGraph(new Graph(), all);
        }

        private List<string> GetInferredPlaces(Graph outputGraph)
        {
            List<string> inferredResults = new List<string>();
            foreach (DataRow row in outputGraph.ToDataTable().Rows)
            {
                inferredResults.Add(row.ItemArray[0].ToString());
            }

            return inferredResults;
        }

        private Collection<BaprLocation> ExtractResults(Graph graph, List<string> inferredResults)
        {
            Collection<BaprLocation> results = new Collection<BaprLocation>();
            List<object[]> itemsArray = new List<object[]>();
            foreach (DataRow row in graph.ToDataTable().Rows)
            {
                itemsArray.Add(row.ItemArray);
            }
            foreach (var group in itemsArray.GroupBy(x => x[0]))
            {
                if (!inferredResults.Contains(group.Key.ToString()))
                    continue;

                BaprLocation location = new BaprLocation();
                location.attributes = new Collection<BaprLocationAttribute>();
                foreach (object[] attribute in group)
                {
                    string attrValue = attribute[2].ToString();
                    if (attribute[1].ToString().Contains("label"))
                        location.name = GetAttributeName(attrValue);
                    else if (attribute[1].ToString().Contains("lat"))
                        location.latitude = GetCoordinate(attrValue);
                    else if (attribute[1].ToString().Contains("long"))
                        location.longitude = GetCoordinate(attrValue);
                    else
                    {
                        BaprLocationAttribute attr = new BaprLocationAttribute();
                        attr.Name = GetLastSubstring(attribute[1].ToString());
                        attr.Value = SplitValue(attribute[1].ToString(), attribute[2].ToString());
                        attr.Type = "string";
                        location.attributes.Add(attr);
                    }
                }
                results.Add(location);
            }

            return results;
        }

        private SparqlQuery GetRestaurantsQuery()
        {
            string query = @"CONSTRUCT WHERE {
                ?f rdf:type dbo:Restaurant . 
                ?f dbo:cuisine ?cuisine .
                ?f foaf:homepage ?website .
                ?f rdfs:label ?name .
                ?f geo:lat ?latitude .
                ?f geo:long ?longitude .
                ?f dbo:address ?address
                } LIMIT 30";

            SparqlParameterizedString queryString = new SparqlParameterizedString();
            //Add a namespace declaration
            queryString.Namespaces.AddNamespace("dbo", new Uri("http://dbpedia.org/ontology/"));
            queryString.Namespaces.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));
            queryString.Namespaces.AddNamespace("rdf", new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            queryString.Namespaces.AddNamespace("foaf", new Uri("http://xmlns.com/foaf/0.1/"));
            queryString.Namespaces.AddNamespace("geo", new Uri("http://www.w3.org/2003/01/geo/wgs84_pos#"));
            queryString.CommandText = query;

            SparqlQueryParser parser = new SparqlQueryParser();
            return parser.ParseFromString(queryString);
        }

        private SparqlQuery GetHotelsQuery()
        {
            string query = @"CONSTRUCT WHERE
                { 
                    ?f rdf:type dbo:Hotel . 
                    ?f rdfs:label ?name .
                    ?f geo:lat ?latitude .
                    ?f geo:long ?longitude .
                    ?f foaf:homepage ?website .
                    ?f dbp:numberOfRooms ?numberOfRooms. 
                    ?f dbp:numberOfSuites ?numberOfSuites.
                } LIMIT 30 ";

            SparqlParameterizedString queryString = new SparqlParameterizedString();
            //Add a namespace declaration
            queryString.Namespaces.AddNamespace("dbo", new Uri("http://dbpedia.org/ontology/"));
            queryString.Namespaces.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));
            queryString.Namespaces.AddNamespace("rdf", new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            queryString.Namespaces.AddNamespace("foaf", new Uri("http://xmlns.com/foaf/0.1/"));
            queryString.Namespaces.AddNamespace("dbp", new Uri("http://dbpedia.org/property/"));
            queryString.Namespaces.AddNamespace("geo", new Uri("http://www.w3.org/2003/01/geo/wgs84_pos#"));
            queryString.CommandText = query;

            SparqlQueryParser parser = new SparqlQueryParser();
            return parser.ParseFromString(queryString);
        }

        private SparqlQuery GetGeneralQuery(string type)
        {
            string query = @"CONSTRUCT WHERE {?f rdf:type dbo:" + type + " . ?f rdfs:label ?name . ?f foaf:homepage ?website . ?f geo:lat ?latitude . ?f geo:long ?longitude . } LIMIT 30";

            SparqlParameterizedString queryString = new SparqlParameterizedString();
            //Add a namespace declaration
            queryString.Namespaces.AddNamespace("rdf", new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            queryString.Namespaces.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));
            queryString.Namespaces.AddNamespace("foaf", new Uri("http://xmlns.com/foaf/0.1/"));
            queryString.Namespaces.AddNamespace("dbo", new Uri("http://dbpedia.org/ontology/"));
            queryString.Namespaces.AddNamespace("geo", new Uri("http://www.w3.org/2003/01/geo/wgs84_pos#"));
            queryString.CommandText = query;

            SparqlQueryParser parser = new SparqlQueryParser();
            return parser.ParseFromString(queryString);
        }

        #region data extractor
        private double GetCoordinate(string text)
        {
            string strNumber = text.Split(new char[] { '^' })[0];
            return Convert.ToDouble(strNumber);
        }

        private string GetAttributeName(string text)
        {
            string[] splitText = text.Split(new string[] { "@", "#", @"\", "/" }, StringSplitOptions.RemoveEmptyEntries);
            return splitText[0];
        }

        private string GetLastSubstring(string name)
        {
            string[] splitText = name.Split(new string[] { "@", "#", @"\", "/" }, StringSplitOptions.RemoveEmptyEntries);
            string result = splitText[splitText.Length - 1];
            return result;
        }

        private string SplitValue(string name, string value)
        {
            if (name == "address")
            {
                string[] splitText = value.Split(new string[] { "@", "#", @"\", "/" }, StringSplitOptions.RemoveEmptyEntries);
                string result = splitText[splitText.Length - 1];
                return GetLastSubstring(result);
            }
            else if (name == "homepage")
            {
                return value;
            }
            else if (value.Contains("^^"))
            {
                return value.Split(new char[] { '^' })[0];
            }
            else if (name.Contains("type"))
            {
                return GetLastSubstring(value);
            }
            return value;
        }
        #endregion
    }
}
