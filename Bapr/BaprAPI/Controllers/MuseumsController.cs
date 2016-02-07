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
using VDS.RDF.Query.Inference;

namespace BaprAPI.Controllers
{
    public class MuseumsController : ApiController
    {
        [HttpGet]
        [ActionName("GetMuseumsNearby")]
        public HttpResponseMessage GetMuseumsNearby(double lat, double lng, string userEmail)
        {
            var userPreference = BaprAPI.Utils.Utils.GetUserPreference(userEmail);
            var fromDbPedia = DbPedia_GetMuseumsNearby(lat, lng, userPreference);
            var fromLinkedGeoData = LinkedGeoData_GetMuseumsNearby(lat, lng, userPreference);
            var allLocations = fromDbPedia.Concat(fromLinkedGeoData);
            return Request.CreateResponse(HttpStatusCode.OK, allLocations);
            
        }
        private Collection<BaprLocation> DbPedia_GetMuseumsNearby(double lat, double lng, IUserPreference userPreference)
        {
            string myQuery = "SELECT DISTINCT ?name ?website ?address ?phone ?lat ?long  ?comment \n" +
                           "WHERE {	\n" +
                           "?museum a ?type. \n" +
                           "?museum ?p ?name. \n" +
                           "Optional { ?museum dbpproperty:address ?address. }" +
                           "Optional { ?museum dbpproperty:website ?website. }\n" +
                           "Optional { ?museum foaf:phone ?phone. }" +
                           "Optional { ?s rdfs:comment ?comment. FILTER(langMatches(lang(?comment ), \"en\"))}\n" +
                           "?museum geo:lat ?lat.\n" +
                           "?museum geo:long ?long.\n" +
                           "FILTER (?p=<http://www.w3.org/2000/01/rdf-schema#label>).\n" +
                           "FILTER (?type IN (<http://dbpedia.org/ontology/Museum>, <http://schema.org/Museum>, <http://dbpedia.org/class/yago/Museum103800563>)).\n" +
                           "FILTER ( ?long > " + lng + " - 0.5 && ?long < " + lng + " + 0.5 && \n" +
                           "?lat > " + lat + " - 0.5 && ?lat < " + lat + " + 0.5)\n" +
                           "FILTER ( lang(?name) = 'en')}\n" +
                           "LIMIT 30";

            SparqlParameterizedString dbpediaQuery = new SparqlParameterizedString();

            dbpediaQuery.Namespaces.AddNamespace("geo", new Uri("http://www.w3.org/2003/01/geo/wgs84_pos#"));
            dbpediaQuery.Namespaces.AddNamespace("dbpedia-owl", new Uri("http://dbpedia.org/ontology/"));
            dbpediaQuery.Namespaces.AddNamespace("dbpproperty", new Uri("http://dbpedia.org/property/"));
            dbpediaQuery.Namespaces.AddNamespace("foaf", new Uri("http://xmlns.com/foaf/spec/"));
            dbpediaQuery.Namespaces.AddNamespace("rdf", new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            dbpediaQuery.Namespaces.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));
            dbpediaQuery.CommandText = myQuery;

            return BaprAPI.Utils.Utils.ParseSparqlQuery(dbpediaQuery, @"http://dbpedia.org/sparql");
        }

        private Collection<BaprLocation> LinkedGeoData_GetMuseumsNearby(double lat, double lng, IUserPreference userPreference) 
        {                                                                  
            string myQuery = "SELECT DISTINCT ?name ?address ?homepage ?lat ?long \n" +
                            "WHERE {	\n" +
                            "?museum a ?type. \n" +
                            "?museum ?p ?name. \n" +
                            "Optional { ?museum lgdo:address ?address. }\n" +
                            "Optional { ?museum foaf:homepage ?homepage. }\n" +
                            "Optional { ?museum foaf:phone ?phone. }\n" +
                            "?museum geo:lat ?lat.\n" +
                            "?museum geo:long ?long.\n" +
                            "FILTER (?p=<http://www.w3.org/2000/01/rdf-schema#label>).\n" +
                            "FILTER (?type IN (<http://linkedgeodata.org/ontology/Museum>, <http://schema.org/Museum>)).\n" +
                            "FILTER ( ?long > " + lng + " - 0.5 && ?long < " + lng + " + 0.5 && \n" +
                            "?lat > " + lat + " - 0.5 && ?lat < " + lat + " + 0.5)}\n" +
                            "LIMIT 30";

            SparqlParameterizedString dbpediaQuery = new SparqlParameterizedString();

            dbpediaQuery.Namespaces.AddNamespace("geo", new Uri("http://www.w3.org/2003/01/geo/wgs84_pos#"));
            dbpediaQuery.Namespaces.AddNamespace("lgdo", new Uri("http://linkedgeodata.org/ontology/"));
            dbpediaQuery.Namespaces.AddNamespace("foaf", new Uri("http://xmlns.com/foaf/spec/"));
            dbpediaQuery.CommandText = myQuery;

            return BaprAPI.Utils.Utils.ParseSparqlQuery(dbpediaQuery, @"http://linkedgeodata.org/sparql");
        }
    }
}
