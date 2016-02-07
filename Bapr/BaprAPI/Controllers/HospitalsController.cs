using BrightstarDB;
using BrightstarDB.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Parsing.Handlers;
using VDS.RDF.Query;
using VDS.RDF.Query.Datasets;
using VDS.RDF.Query.Inference;
using VDS.RDF.Writing;
using VDS.RDF.Writing.Formatting;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BaprAPI.Models;
using System.Collections.ObjectModel;

namespace BaprAPI.Controllers
{
    public class HospitalsController : ApiController
    {
        [HttpGet]
        [ActionName("GetAllHospitalsNearby")]
        public HttpResponseMessage GetAllHospitalsNearby(double lat, double lng, string userEmail)
        {
            var userPreference = BaprAPI.Utils.Utils.GetUserPreference(userEmail);
            var fromDbPedia = DbPedia_GetAllHospitalsNearby(lat, lng, userPreference);
            var fromLinkedGeoData = LinkedGeoData_GetAllHospitalsNearby(lat, lng, userPreference);
            var allLocations = fromDbPedia.Concat(fromLinkedGeoData);
            return Request.CreateResponse(HttpStatusCode.OK, allLocations);
        }

        private Collection<BaprLocation> DbPedia_GetAllHospitalsNearby(double lat, double lng, IUserPreference userPreference)
        {                                                                                    
            string myQuery = "SELECT DISTINCT ?name ?website ?address ?phone ?lat ?long \n" +
                            "WHERE {	\n" +
                            "?hospital a ?type. \n" +
                            "?hospital ?p ?name. \n" +
                            "Optional { ?hospital dbpproperty:address ?address. }\n" +
                            "Optional { ?hospital dbpproperty:website ?website. }\n" +
                            "Optional { ?hospital foaf:phone ?phone. }\n" +
                            "Optional { ?hospital schema:department ?department} \n" +
                            "?hospital geo:lat ?lat.\n" +
                            "?hospital geo:long ?long.\n" +
                            "FILTER (?p=<http://www.w3.org/2000/01/rdf-schema#label>).\n" +
                            "FILTER (?type IN (<http://dbpedia.org/ontology/Hospital>, <http://schema.org/Hospital>)).\n" +
                            "FILTER ( ?long > " + lng + " - 0.5 && ?long < " + lng + " + 0.5 && \n" +
                            "?lat > " + lat + " - 0.5 && ?lat < " + lat + " + 0.5)\n" +
                            "FILTER ( lang(?name) = 'en')}\n" +
                            "LIMIT 30";

            SparqlParameterizedString dbpediaQuery = new SparqlParameterizedString();

            dbpediaQuery.Namespaces.AddNamespace("geo", new Uri("http://www.w3.org/2003/01/geo/wgs84_pos#"));
            dbpediaQuery.Namespaces.AddNamespace("dbpproperty", new Uri("http://dbpedia.org/property/"));
            dbpediaQuery.Namespaces.AddNamespace("foaf", new Uri("http://xmlns.com/foaf/spec/"));
            dbpediaQuery.Namespaces.AddNamespace("schema", new Uri("http://schema.org/"));
            dbpediaQuery.CommandText = myQuery;

            return BaprAPI.Utils.Utils.ParseSparqlQuery(dbpediaQuery, @"http://dbpedia.org/sparql");
        }

        private Collection<BaprLocation> LinkedGeoData_GetAllHospitalsNearby(double lat, double lng, IUserPreference userPreference)
        {
            string myQuery = "SELECT DISTINCT ?name ?address ?homepage ?phone ?lat ?long \n" +
                            "WHERE {	\n" +
                            "?hospital a ?type. \n" +
                            "?hospital ?p ?name. \n" +
                            "Optional { ?hospital lgdo:address ?address. }\n" +
                            "Optional { ?hospital foaf:homepage ?homepage. }\n" +
                            "Optional { ?hospital foaf:phone ?phone. }\n" +
                            "?hospital geo:lat ?lat.\n" +
                            "?hospital geo:long ?long.\n" +
                            "FILTER (?p=<http://www.w3.org/2000/01/rdf-schema#label>).\n" +
                            "FILTER (?type IN (<http://linkedgeodata.org/ontology/Hospital>, <http://schema.org/Hospital>)).\n" +
                            "FILTER ( ?long > " + lng + " - 0.5 && ?long < " + lng + " + 0.5 && \n" +
                            "?lat > " + lat + " - 0.5 && ?lat < " + lat + " + 0.5)}\n" +
                            "LIMIT 30";

            SparqlParameterizedString lgdoQuery = new SparqlParameterizedString();

            lgdoQuery.Namespaces.AddNamespace("lgdo", new Uri("http://linkedgeodata.org/ontology/"));
            lgdoQuery.Namespaces.AddNamespace("geo", new Uri("http://www.w3.org/2003/01/geo/wgs84_pos#"));  
            lgdoQuery.Namespaces.AddNamespace("foaf", new Uri("http://xmlns.com/foaf/spec/"));
            lgdoQuery.CommandText = myQuery;

            return BaprAPI.Utils.Utils.ParseSparqlQuery(lgdoQuery, @"http://dbpedia.org/sparql");
        }
    }
}
