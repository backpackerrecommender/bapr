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
            var allCollections = fromDbPedia.Concat(fromLinkedGeoData);
            return Request.CreateResponse(HttpStatusCode.OK, allCollections);
        }

        private Collection<BaprLocation> GetFromDbpedia(double latitude, double longitude, IUserPreference userPreference)
        {
            string searchHotelsQuery = "SELECT DISTINCT ?lat ?long ?name ?website ?address ?number_of_restaurants "
               + "?star_rating ?number_of_rooms ?number_of_suites ?comment ?arhitect ?chain "
                                + " WHERE {\n ?s a dbo:Hotel ."
                                + "?s rdfs:label ?name .\n"
                                + "?s geo:lat ?lat .\n"
                                + "?s geo:long ?long .\n"
                                + "OPTIONAL { ?s rdfs:comment ?comment. FILTER(langMatches(lang(?comment ), \"en\"))}\n"
                                + "OPTIONAL { ?s dbo:arhitect ?arhitect. }\n"
                                + "OPTIONAL { ?s dbp:chain ?chain. }\n"
                                + "OPTIONAL { ?s foaf:homepage ?website .}\n"
                                + "OPTIONAL { ?s dbo:address ?address .}\n"
                                + "OPTIONAL { ?s dbp:numberOfRestaurants ?number_of_restaurants.} \n"
                                + "OPTIONAL { ?s dbp:starRating  ?star_rating. } \n"
                                + "OPTIONAL { ?s dbp:numberOfRooms ?number_of_rooms. } \n"
                                + "OPTIONAL { ?s  dbp:numberOfSuites ?number_of_suites. } \n"
                                + "FILTER ( langMatches(lang(?name), \"EN\")"
                                + " && ?lat > " + latitude + " - 0.5  && ?lat < " + latitude + " + 0.5"
                                + " && ?long > " + longitude + " - 0.5 && ?long < " + longitude + " + 0.5 )\n"
                                + " } LIMIT 10";


            SparqlParameterizedString sparqlQueryString = new SparqlParameterizedString();
            sparqlQueryString.Namespaces.AddNamespace("rdf", new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            sparqlQueryString.Namespaces.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));
            sparqlQueryString.Namespaces.AddNamespace("dbo", new Uri("http://dbpedia.org/ontology/"));
            sparqlQueryString.Namespaces.AddNamespace("dbp", new Uri("http://dbpedia.org/property/"));
            sparqlQueryString.Namespaces.AddNamespace("geo", new Uri("http://www.w3.org/2003/01/geo/wgs84_pos#"));
            sparqlQueryString.Namespaces.AddNamespace("foaf", new Uri("http://xmlns.com/foaf/0.1/"));
            sparqlQueryString.CommandText = searchHotelsQuery;

            return BaprAPI.Utils.Utils.ParseSparqlQuery(sparqlQueryString, @"http://dbpedia.org/sparql");

        }
        private Collection<BaprLocation> GetFromLinkedGeoData(double latitude, double longitude, IUserPreference userPreference)
        {

            string searchHotelsQuery = "SELECT DISTINCT ?lat ?long ?name ?website ?phone ?address ?rooms ?internet_acces"
                + " ?opening_hours ?wheelchair ?stars ?operator"
                + " WHERE { ?s a lgd:Hotel .\n"
                + " ?s rdfs:label ?name . \n"
                + " ?s geo:lat ?lat .\n"
                + " ?s geo:long ?long . \n"
                + "OPTIONAL {?s foaf:homepage ?website .}\n"
                + "OPTIONAL {?s foaf:phone ?phone. }\n"
                + " OPTIONAL {?s lgd:address ?address .}\n "
                + " OPTIONAL {?s lgd:rooms ?rooms.} \n"
                + " OPTIONAL {?s lgd:internet_access ?internet_access.} \n"
                + " OPTIONAL {?s lgd:opening_hours ?opening_hours.} \n"
                + (userPreference.NeedWheelchair ? "?s lgd:wheelchair ?wheelchair. FILTER (?wheelchair =" + BaprAPI.Models.Constants.xsdBooleanIsTrue + ")\n"
                                                      : " OPTIONAL {?s lgd:wheelchair ?wheelchair.} \n")
                + " OPTIONAL {?s lgd:stars ?stars. } \n"
                + " OPTIONAL {?s lgd:operator ?operator. } \n"
                + " FILTER ( ?lat > " + latitude + " - 0.5  && ?lat < " + latitude + " + 0.5"
                + " && ?long > " + longitude + " - 0.5 && ?long < " + longitude + " + 0.5 )\n"
                + "} LIMIT 10";

            SparqlParameterizedString sparqlQueryString = new SparqlParameterizedString();
            sparqlQueryString.Namespaces.AddNamespace("rdf", new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            sparqlQueryString.Namespaces.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));
            sparqlQueryString.Namespaces.AddNamespace("lgd", new Uri("http://linkedgeodata.org/ontology/"));
            sparqlQueryString.Namespaces.AddNamespace("geo", new Uri("http://www.w3.org/2003/01/geo/wgs84_pos#"));
            sparqlQueryString.Namespaces.AddNamespace("foaf", new Uri("http://xmlns.com/foaf/0.1/"));
            sparqlQueryString.CommandText = searchHotelsQuery;

            return BaprAPI.Utils.Utils.ParseSparqlQuery(sparqlQueryString, @"http://linkedgeodata.org/sparql");
        }
    }
}
