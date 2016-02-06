using BaprAPI.Models;
using BrightstarDB;
using BrightstarDB.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Writing.Formatting;

namespace BaprAPI.Controllers
{
    public class HotelsController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Get(int id)
        {
            return new HttpResponseMessage(HttpStatusCode.OK);//200
        }

        [HttpGet]
        public HttpResponseMessage GetByCoordinates(double lat, double lng, string userEmail)
        {
            var userPreference = BaprAPI.Utils.Utils.GetUserPreference(userEmail);
            var fromDbPedia = GetFromDbpedia(lat, lng, userPreference);
            var fromLinkedGeoData = GetFromLinkedGeoData(lat, lng, userPreference);
            return new HttpResponseMessage(HttpStatusCode.OK);//200
        }
        private ICollection<ILocation> GetFromDbpedia(double latitude, double longitude, IUserPreference userPreference)
        {
            string searchHotelsQuery = "SELECT DISTINCT * WHERE {\n ?f a dbo:Hotel ."
                                 + "?f rdfs:label ?name .\n"
                                 + "OPTIONAL { ?f foaf:homepage ?website .}\n"
                                 + "?f geo:lat ?lat .\n"
                                 + "?f geo:long ?long .\n"
                                 + "OPTIONAL { ?f dbo:address ?address .}\n"
                                 + "OPTIONAL { ?f dbp:numberOfRestaurants ?numberOfRestaurants.} \n"
                                 + "OPTIONAL { ?f dbp:starRating  ?starRating. } \n"
                                 + "OPTIONAL { ?f dbp:numberOfRooms ?numberOfRooms. } \n"
                                 + "OPTIONAL { ?f  dbp:numberOfSuites ?numberOfSuites. } \n"
                                 + "FILTER ( langMatches(lang(?name), \"EN\")"
                                 + " && ?lat > " + latitude + " - 1  && ?lat < " + latitude  + " + 1"
                                 + " && ?long > " + longitude + " - 1 && ?long < " + longitude  + " + 1 )\n"
                                 + " } LIMIT 10";


            SparqlParameterizedString sparqlQueryString = new SparqlParameterizedString();
            sparqlQueryString.Namespaces.AddNamespace("rdf", new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            sparqlQueryString.Namespaces.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));
            sparqlQueryString.Namespaces.AddNamespace("dbo", new Uri("http://dbpedia.org/ontology/"));
            sparqlQueryString.Namespaces.AddNamespace("dbp", new Uri("http://dbpedia.org/property/"));
            sparqlQueryString.Namespaces.AddNamespace("geo", new Uri("http://www.w3.org/2003/01/geo/wgs84_pos#"));
            sparqlQueryString.Namespaces.AddNamespace("foaf", new Uri("http://xmlns.com/foaf/0.1/"));
            sparqlQueryString.CommandText = searchHotelsQuery;

            SparqlQueryParser parser = new SparqlQueryParser();
            SparqlQuery query = parser.ParseFromString(sparqlQueryString);

            Uri uri = new Uri(@"http://dbpedia.org/sparql");
            SparqlRemoteEndpoint endPoint = new SparqlRemoteEndpoint(uri);
            ISparqlQueryProcessor processor = new RemoteQueryProcessor(endPoint);
            object result = processor.ProcessQuery(query);
            if (result is SparqlResultSet)
            {
                ILocation loc = new Location();
            }

            return new Collection<ILocation>();
        }

        private ICollection<ILocation> GetFromLinkedGeoData(double latitude, double longitude, IUserPreference userPreference)
        {

            string searchHotelsQuery = "SELECT DISTINCT * WHERE { ?f a lgd:Hotel .\n"
                + " ?f rdfs:label ?name . \n"
                + " ?f foaf:homepage ?website .\n"
                + " ?f foaf:phone ?phone. \n"
                + " ?f geo:lat ?lat .\n"
                + " ?f geo:long ?long . \n"
                + " OPTIONAL {?f lgd:rooms ?rooms.} \n"
                + " OPTIONAL {?f lgd:internet_access ?internetAccess.} \n"
                + " OPTIONAL {?f lgd:opening_hours ?opening_hours.} \n"
                + (userPreference.NeedMedicalSupport ? "?f lgd:wheelchair ?wheelchair. FILTER (?wheelchair =" + BaprAPI.Models.Constants.xsdBooleanIsTrue + ")\n"
                                                      : " OPTIONAL {?f lgd:wheelchair ?wheelchair.} \n")
                + " OPTIONAL {?f lgd:stars ?stars. } \n"
                + " OPTIONAL {?f lgd:operator ?operator. } \n"
                + " FILTER ( ?lat > " + latitude + " - 1  && ?lat < " + latitude + " + 1"
                + " && ?long > " + longitude + " - 1 && ?long < " + longitude + " + 1 )\n"
                + "} LIMIT 10";

            SparqlParameterizedString sparqlQueryString = new SparqlParameterizedString();
            sparqlQueryString.Namespaces.AddNamespace("rdf", new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            sparqlQueryString.Namespaces.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));
            sparqlQueryString.Namespaces.AddNamespace("lgd", new Uri("http://linkedgeodata.org/ontology/"));
            sparqlQueryString.Namespaces.AddNamespace("geo", new Uri("http://www.w3.org/2003/01/geo/wgs84_pos#"));
            sparqlQueryString.Namespaces.AddNamespace("foaf", new Uri("http://xmlns.com/foaf/0.1/"));
            sparqlQueryString.CommandText = searchHotelsQuery;

            SparqlQueryParser parser = new SparqlQueryParser();
            SparqlQuery query = parser.ParseFromString(sparqlQueryString);

            Uri uri = new Uri(@"http://linkedgeodata.org/sparql");
            SparqlRemoteEndpoint endPoint = new SparqlRemoteEndpoint(uri);
            ISparqlQueryProcessor processor = new RemoteQueryProcessor(endPoint);
            object result = processor.ProcessQuery(query);
            if (result is SparqlResultSet)
            {
                ILocation loc = new Location();
            }

            return new Collection<ILocation>();
        }
    }
}
