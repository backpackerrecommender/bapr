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

namespace BaprAPI.Controllers
{
    public class RestaurantsController : ApiController
    {
        [HttpGet]
        [ActionName("GetRestaurantsNearby")]
        public void GetRestaurantsNearby(double lat, double lng, string userEmail)
        {
            DBpedia_GetRestaurantsNearby(lat, lng, userEmail);
            Linkedgeodata_GetRestaurantsNearby(lat, lng, userEmail);
        }
       
        public void DBpedia_GetRestaurantsNearby(double lat, double lng, string userEmail)
        {
            string queryRestaurantsDBpedia = "SELECT DISTINCT * \n" + 
                               " WHERE { \n" +
                                "?f rdf:type dbo:Restaurant .\n" +
                                "?f rdfs:label ?name .\n" +
                                "?f geo:lat ?lat .\n" +
                                "?f geo:long ?long .\n" +
                                "Optional { ?f dbo:cuisine ?cuisine .}\n " +
                                "Optional { ?f foaf:homepage ?website .}\n " +
                                "Optional {?f dbo:address ?address .}\n" +
                                "Optional { ?f dbp:dressCode  ?dressCode  .} \n" +
                                "Optional { ?f dbp:headChef  ?headChef .} \n" +
                                "Optional { ?f dbp:reservations  ?reservations .} \n" +
                                " FILTER ( langMatches(lang(?name), \"EN\")\n"
                                 + " && ?lat > " + lat + " - 1 && ?lat < " + lat + " + 1\n"
                                 + " && ?long > " + lng + " - 1 && ?long < " + lng + " + 1 )}\n" +
                                "LIMIT 30";

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
        }

        public void Linkedgeodata_GetRestaurantsNearby(double lat, double lng, string userEmail)
        {
            string queryRestaurantsLinkedgeodata = "SELECT DISTINCT * \n" +
                               " WHERE { \n" +
                                "?f rdf:type lgdo:Restaurant .\n" +
                                "?f rdfs:label ?name .\n" +
                                "?f geo:lat ?lat .\n" +
                                "?f geo:long ?long .\n" +
                                "Optional { ?f lgdo:cuisine ?cuisine .}\n " +
                                "Optional { ?f foaf:homepage ?website .}\n " +
                                "Optional { ?f lgdo:address ?address .}\n " +
                                "Optional { ?f lgdo:opening_hours  ?opening_hours .}\n " +
                                "Optional { ?f lgdo:smoking  ?smoking .} \n" +
                                "Optional { ?f lgdo:internet_access  ?internet_access .}\n" +
                                "Optional { ?f lgdo:wifi  ?wifi .}\n " +
                                "Optional { ?f lgdo:fast_food  ?fast_food .} \n" +
                                "Optional { ?f lgdo:wheelchair  ?wheelchair .}\n" +
                                " FILTER ( langMatches(lang(?name), \"EN\")\n"
                                 + " && ?lat > " + lat + " - 1 && ?lat < " + lat + " + 1\n"
                                 + " && ?long > " + lng + " - 1 && ?long < " + lng + " + 1 )}\n" +
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
        }
    }
}
