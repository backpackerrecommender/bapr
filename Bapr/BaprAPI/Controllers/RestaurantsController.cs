using BaprAPI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Parsing.Handlers;
using VDS.RDF.Query;

namespace BaprAPI.Controllers
{
    public class RestaurantsController : ApiController
    {
        [HttpGet]
        [ActionName("GetRestaurantsNearby")]
        public HttpResponseMessage GetRestaurantsNearby(double lat, double lng, string userEmail)
        {
            var userPreference = BaprAPI.Utils.Utils.GetUserPreference(userEmail);
            var fromDbPedia = DBpedia_GetRestaurantsNearby(lat, lng, userPreference);
            var fromLinkedGeoData = Linkedgeodata_GetRestaurantsNearby(lat, lng, userPreference);
            var allCollections = fromDbPedia.Concat(fromLinkedGeoData);
            return Request.CreateResponse(HttpStatusCode.OK, allCollections);//200
           
        }

        public Collection<BaprLocation> DBpedia_GetRestaurantsNearby(double lat, double lng, IUserPreference userPreference)
        {

            var cuisine = BaprAPI.Utils.Utils.GetCuisines(userPreference, true);
            string queryRestaurantsDBpedia = "SELECT DISTINCT ?name ?lat ?long ?cuisine ?comment ?website ?address ?dress_code ?head_chef ?reservations \n" + 
                               " WHERE { \n" +
                                "?f rdf:type dbo:Restaurant .\n" +
                                "?f rdfs:label ?name .\n" +
                                "?f geo:lat ?lat .\n" +
                                "?f geo:long ?long .\n" +
                                "Optional { ?f dbo:cuisine ?cuisine .}\n " +
                                "Optional { ?f rdfs:comment ?comment . FILTER(langMatches(lang(?comment ), \"en\"))}\n " +
                                "Optional { ?f foaf:homepage ?website .}\n " +
                                "Optional { ?f dbo:address ?address .}\n" +
                                "Optional { ?f dbp:dressCode ?dress_code  .} \n" +
                                "Optional { ?f dbp:headChef ?head_chef .} \n" +
                                "Optional { ?f dbp:reservations ?reservations .} \n" +
                                " FILTER ( langMatches(lang(?name), \"EN\")\n" +
                                " && ?lat > " + lat + " - 1 && ?lat < " + lat + " + 1\n" +
                                " && ?long > " + lng + " - 1 && ?long < " + lng + " + 1\n" +
                                (!string.IsNullOrEmpty(cuisine) ? " && lcase(?cuisine) in (" + cuisine + ") " : string.Empty) +
                                ")}\n" +"LIMIT 30";

            //Create a Parameterized String
            SparqlParameterizedString queryString = new SparqlParameterizedString();
            //Add a namespace declaration
            queryString.Namespaces.AddNamespace("geo", new Uri("http://www.w3.org/2003/01/geo/wgs84_pos#"));
            queryString.Namespaces.AddNamespace("rdf", new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            queryString.Namespaces.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));
            queryString.Namespaces.AddNamespace("dbo", new Uri("http://dbpedia.org/ontology/"));
            queryString.Namespaces.AddNamespace("dbp", new Uri("http://dbpedia.org/property/"));
            queryString.Namespaces.AddNamespace("foaf", new Uri("http://xmlns.com/foaf/0.1/"));
            queryString.CommandText = queryRestaurantsDBpedia;

            SparqlQueryParser parser = new SparqlQueryParser();
            SparqlQuery query = parser.ParseFromString(queryString);

            Uri uri = new Uri(@"http://dbpedia.org/sparql");

            TripleStore tripleStore = new TripleStore();
            IRdfHandler rdfHandler = new StoreHandler(tripleStore);

            SparqlResultSet resultSet = new SparqlResultSet();
            ISparqlResultsHandler resultsHandler = new ResultSetHandler(resultSet);
            //Get the Query processor
            SparqlRemoteEndpoint endPoint = new SparqlRemoteEndpoint(uri);
            ISparqlQueryProcessor processor = new RemoteQueryProcessor(endPoint);
            object result = processor.ProcessQuery(query);
           
            if (result is SparqlResultSet)
            {
                SparqlResultSet entities = (SparqlResultSet)result;
                return BaprAPI.Utils.Utils.ConvertFromSparqlSetToBaprLocations(entities);
            }

            return new Collection<BaprLocation>();
        }

        public Collection<BaprLocation> Linkedgeodata_GetRestaurantsNearby(double lat, double lng, IUserPreference userPreference)
        {
            var cuisine = BaprAPI.Utils.Utils.GetCuisines(userPreference, false);
            string queryRestaurantsLinkedgeodata = "SELECT DISTINCT ?name ?lat ?long ?cuisine ?comment ?website ?address ?smoking ?internet_access ?wifi ?fast_food \n" +
                               " WHERE { \n" +
                                "?f rdf:type lgdo:Restaurant .\n" +
                                "?f rdfs:label ?name .\n" +
                                "?f geo:lat ?lat .\n" +
                                "?f geo:long ?long .\n" +
                                "Optional { ?f lgdo:cuisine ?cuisine .}\n " +
                                "Optional { ?f rdfs:comment ?comment . FILTER(langMatches(lang(?comment ), \"en\"))}\n " +
                                "Optional { ?f foaf:homepage ?website .}\n " +
                                "Optional { ?f lgdo:address ?address .}\n " +
                                "Optional { ?f lgdo:opening_hours ?opening_hours .}\n " +
                                "Optional { ?f lgdo:smoking  ?smoking .} \n" +
                                "Optional { ?f lgdo:internet_access ?internet_access .}\n" +
                                "Optional { ?f lgdo:wifi  ?wifi .}\n " +
                                "Optional { ?f lgdo:fast_food  ?fast_food .} \n" +
                                (userPreference.NeedWheelchair ? "?f lgdo:wheelchair ?wheelchair. FILTER (?wheelchair =" + BaprAPI.Models.Constants.xsdBooleanIsTrue + ")\n"
                                                      : " OPTIONAL {?f lgdo:wheelchair ?wheelchair.} \n")+
                                " FILTER ( langMatches(lang(?name), \"EN\")\n" +
                                " && ?lat > " + lat + " - 1 && ?lat < " + lat + " + 1\n" +
                                " && ?long > " + lng + " - 1 && ?long < " + lng + " + 1" +
                                (!string.IsNullOrEmpty(cuisine) ? "&& lcase(?cuisine) in (" + cuisine + ") \n" : string.Empty) +
                                 ")}\n" +
                                "LIMIT 30";

            //Create a Parameterized String
            SparqlParameterizedString queryString = new SparqlParameterizedString();
            //Add a namespace declaration
            queryString.Namespaces.AddNamespace("geo", new Uri("http://www.w3.org/2003/01/geo/wgs84_pos#"));
            queryString.Namespaces.AddNamespace("rdf", new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            queryString.Namespaces.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));
            queryString.Namespaces.AddNamespace("lgdo", new Uri("http://linkedgeodata.org/ontology/"));
            queryString.Namespaces.AddNamespace("foaf", new Uri("http://xmlns.com/foaf/0.1/"));
            queryString.CommandText = queryRestaurantsLinkedgeodata;

            SparqlQueryParser parser = new SparqlQueryParser();
            SparqlQuery query = parser.ParseFromString(queryString);

            Uri uri = new Uri(@"http://linkedgeodata.org/sparql");

            TripleStore tripleStore = new TripleStore();
            IRdfHandler rdfHandler = new StoreHandler(tripleStore);

            SparqlResultSet resultSet = new SparqlResultSet();
            ISparqlResultsHandler resultsHandler = new ResultSetHandler(resultSet);
            //Get the Query processor
            SparqlRemoteEndpoint endPoint = new SparqlRemoteEndpoint(uri);
            ISparqlQueryProcessor processor = new RemoteQueryProcessor(endPoint);
            object result = processor.ProcessQuery(query);

            if (result is SparqlResultSet)
            {
                SparqlResultSet entities = (SparqlResultSet)result;
                return BaprAPI.Utils.Utils.ConvertFromSparqlSetToBaprLocations(entities);
            }

            return new Collection<BaprLocation>();
        }
    }
}
