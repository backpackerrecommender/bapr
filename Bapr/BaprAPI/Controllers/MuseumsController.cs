using System;
using System.Collections.Generic;
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
        public void GetMuseumsNearby(double lat, double lng, string userEmail)
        {
            DbPedia_GetMuseumsNearby(lat, lng, userEmail);
            LinkedGeoData_GetMuseumsNearby(lat, lng, userEmail);
        }
        private void DbPedia_GetMuseumsNearby(double lat, double lng, string userEmail)   //Barcelona: lat = 41.390205 lng = 2.154007
        {                                                               //1 degree = 110 km lat
            string myQuery = "SELECT DISTINCT ?label ?website ?address ?phone ?lat ?long \n" +
                            "WHERE {	\n" + 
                            "?museum a ?type. \n" +
                            "?museum ?p ?label. \n" +
                            "Optional { ?museum dbpproperty:address ?address. }" +
                            "Optional { ?museum dbpproperty:website ?website. }\n" +
                            "Optional { ?museum foaf:phone ?phone. }" +
                            "?museum geo:lat ?lat.\n" +
                            "?museum geo:long ?long.\n" +
                            "FILTER (?p=<http://www.w3.org/2000/01/rdf-schema#label>).\n" +
                            "FILTER (?type IN (<http://dbpedia.org/ontology/Museum>, <http://schema.org/Museum>, <http://dbpedia.org/class/yago/Museum103800563>)).\n" +
                            "FILTER ( ?long > " + lng + " - 1 && ?long < " + lng + " + 1 && \n" +
                            "?lat > " + lat + " - 1 && ?lat < " + lat + " + 1)\n" +
                            "FILTER ( lang(?label) = 'en')}\n" + 
                          //"FILTER ( regex(str(?category),"Computer","i") ||
                                   //regex(str(?type),"Computer","i") ||
                                   //regex(str(?abstract),"Computer ","i"))." +
                            "LIMIT 30";

            SparqlParameterizedString dbpediaQuery = new SparqlParameterizedString();

            dbpediaQuery.Namespaces.AddNamespace("geo", new Uri("http://www.w3.org/2003/01/geo/wgs84_pos#"));
            dbpediaQuery.Namespaces.AddNamespace("dbpedia-owl", new Uri("http://dbpedia.org/ontology/"));
            dbpediaQuery.Namespaces.AddNamespace("dbpproperty", new Uri("http://dbpedia.org/property/"));
            dbpediaQuery.CommandText = myQuery;

            SparqlQueryParser parser = new SparqlQueryParser();
            SparqlQuery query = parser.ParseFromString(dbpediaQuery);
            Uri uri = new Uri(@"http://dbpedia.org/sparql");
            SparqlResultSet resultSet = new SparqlResultSet();
            ISparqlResultsHandler resultsHandler = new ResultSetHandler(resultSet);
            SparqlRemoteEndpoint endPoint = new SparqlRemoteEndpoint(uri);
            ISparqlQueryProcessor processor = new RemoteQueryProcessor(endPoint);

            var result = processor.ProcessQuery(query);
        }

        private void LinkedGeoData_GetMuseumsNearby(double lat, double lng, string userEmail)   //Barcelona: lat = 41.390205 lng = 2.154007
        {                                                                     //1 degree = 110 km lat
            string myQuery = "SELECT DISTINCT ?label ?address ?homepage ?lat ?long \n" +
                            "WHERE {	\n" +
                            "?museum a ?type. \n" +
                            "?museum ?p ?label. \n" +
                            "Optional { ?museum lgdo:address ?address. }\n" +
                            "Optional { ?museum foaf:homepage ?homepage. }\n" +
                            "Optional { ?museum foaf:phone ?phone. }\n" +
                            "?museum geo:lat ?lat.\n" +
                            "?museum geo:long ?long.\n" +
                            "FILTER (?p=<http://www.w3.org/2000/01/rdf-schema#label>).\n" +
                            "FILTER (?type IN (<http://linkedgeodata.org/ontology/Museum>, <http://schema.org/Museum>)).\n" +
                            "FILTER ( ?long > " + lng + " - 1 && ?long < " + lng + " + 1 && \n" +
                            "?lat > " + lat + " - 1 && ?lat < " + lat + " + 1)}\n" +
                            "LIMIT 30";

            SparqlParameterizedString dbpediaQuery = new SparqlParameterizedString();

            dbpediaQuery.Namespaces.AddNamespace("geo", new Uri("http://www.w3.org/2003/01/geo/wgs84_pos#"));
            dbpediaQuery.Namespaces.AddNamespace("lgdo", new Uri("http://linkedgeodata.org/ontology/"));
            dbpediaQuery.Namespaces.AddNamespace("foaf", new Uri("http://xmlns.com/foaf/spec/"));
            dbpediaQuery.CommandText = myQuery;

            SparqlQueryParser parser = new SparqlQueryParser();
            SparqlQuery query = parser.ParseFromString(dbpediaQuery);
            Uri uri = new Uri(@"http://linkedgeodata.org/sparql");
            SparqlResultSet resultSet = new SparqlResultSet();
            ISparqlResultsHandler resultsHandler = new ResultSetHandler(resultSet);
            SparqlRemoteEndpoint endPoint = new SparqlRemoteEndpoint(uri);
            ISparqlQueryProcessor processor = new RemoteQueryProcessor(endPoint);

            var result = processor.ProcessQuery(query);
        }
    }
}
