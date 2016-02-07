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
    public class ShopsController : ApiController
    {
        [HttpGet]
        [ActionName("GetShopsNearby")]
        public HttpResponseMessage GetShopsNearby(double lat, double lng, string userEmail)
        {
            var userPreference = BaprAPI.Utils.Utils.GetUserPreference(userEmail);
            var fromDbPedia = DbPedia_GetShopsNearby(lat, lng, userPreference);
            var fromLinkedGeoData = LinkedGeoData_GetShopsNearby(lat, lng, userPreference);
          //  Collection<BaprLocation> result = fromDbPedia.Concat(fromLinkedGeoData).ToList();
            return new HttpResponseMessage(HttpStatusCode.OK);//200
        }
        public Collection<BaprLocation> DbPedia_GetShopsNearby(double lat, double lng, IUserPreference userPreference) 
        {
            string myQuery = "SELECT DISTINCT ?name ?address ?openingHours ?lat ?long  \n" +
                           "WHERE {	\n" +
                           "?shop a ?type. \n" +
                           "?shop ?p ?name. \n" +
                           "Optional { ?shop dbpproperty:address ?address. }" +
                           "Optional { ?shop schema:openingHours ?openingHours. }\n" +
                           "?shop geo:lat ?lat.\n" +
                           "?shop geo:long ?long.\n" +
                           "FILTER (?p=<http://www.w3.org/2000/01/rdf-schema#label>).\n" +
                           "FILTER (?type IN (<http://dbpedia.org/ontology/ShoppingMall>, <http://schema.org/ShoppingCenter>)).\n" +
                           //"FILTER ( ?long > " + lng + " - 1 && ?long < " + lng + " + 1 && \n" +
                           //"?lat > " + lat + " - 1 && ?lat < " + lat + " + 1)\n" +
                           "FILTER ( lang(?name) = 'en')}\n" +
                           "LIMIT 30";

            SparqlParameterizedString dbpediaQuery = new SparqlParameterizedString();

            dbpediaQuery.Namespaces.AddNamespace("geo", new Uri("http://www.w3.org/2003/01/geo/wgs84_pos#"));
            dbpediaQuery.Namespaces.AddNamespace("dbpproperty", new Uri("http://dbpedia.org/property/"));
            dbpediaQuery.Namespaces.AddNamespace("schema", new Uri("http://schema.org/"));
            dbpediaQuery.CommandText = myQuery;

            SparqlQueryParser parser = new SparqlQueryParser();
            SparqlQuery query = parser.ParseFromString(dbpediaQuery);
            Uri uri = new Uri(@"http://dbpedia.org/sparql");
            SparqlResultSet resultSet = new SparqlResultSet();
            ISparqlResultsHandler resultsHandler = new ResultSetHandler(resultSet);
            SparqlRemoteEndpoint endPoint = new SparqlRemoteEndpoint(uri);
            ISparqlQueryProcessor processor = new RemoteQueryProcessor(endPoint);

            SparqlResultSet result = (SparqlResultSet)processor.ProcessQuery(query);
            return BaprAPI.Utils.Utils.ConvertFromSparqlSetToBaprLocations(result);
        }

        private Collection<BaprLocation> LinkedGeoData_GetShopsNearby(double lat, double lng, IUserPreference userPreference)
        {
            string myQuery = "SELECT DISTINCT ?name ?address ?homepage ?phone ?lat ?long \n" +
                            "WHERE {	\n" +
                            "?shop a ?type. \n" +
                            "?shop ?p ?name. \n" +
                            "Optional { ?shop lgdo:address ?address. }\n" +
                            "Optional { ?shop foaf:homepage ?homepage. }\n" +
                            "Optional { ?shop foaf:phone ?phone. }\n" +
                            "?shop geo:lat ?lat.\n" +
                            "?shop geo:long ?long.\n" +
                            "FILTER (?p=<http://www.w3.org/2000/01/rdf-schema#label>).\n" +
                            "FILTER (?type IN (<http://linkedgeodata.org/ontology/ShoppingCenter>, <http://schema.org/ShoppingCenter>))\n" +
                            //"FILTER ( ?long > " + lng + " - 1 && ?long < " + lng + " + 1 && \n" +
                            //"?lat > " + lat + " - 1 && ?lat < " + lat + " + 1)\n" +
                            "}LIMIT 30";

            SparqlParameterizedString lgdoQuery = new SparqlParameterizedString();

            lgdoQuery.Namespaces.AddNamespace("lgdo", new Uri("http://linkedgeodata.org/ontology/"));
            lgdoQuery.Namespaces.AddNamespace("geo", new Uri("http://www.w3.org/2003/01/geo/wgs84_pos#"));
            lgdoQuery.Namespaces.AddNamespace("foaf", new Uri("http://xmlns.com/foaf/spec/"));

            lgdoQuery.CommandText = myQuery;

            SparqlQueryParser parser = new SparqlQueryParser();
            SparqlQuery query = parser.ParseFromString(lgdoQuery);
            Uri uri = new Uri(@"http://linkedgeodata.org/sparql");
            SparqlResultSet resultSet = new SparqlResultSet();
            ISparqlResultsHandler resultsHandler = new ResultSetHandler(resultSet);
            SparqlRemoteEndpoint endPoint = new SparqlRemoteEndpoint(uri);
            ISparqlQueryProcessor processor = new RemoteQueryProcessor(endPoint);

            SparqlResultSet result = (SparqlResultSet)processor.ProcessQuery(query);
            return BaprAPI.Utils.Utils.ConvertFromSparqlSetToBaprLocations(result);
        }
    }
}
