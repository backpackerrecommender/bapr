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
        public void GetMuseumsNearby(double lat, double lng)
        {
            DbPedia_GetMuseumsNearby(Convert.ToDouble(lat), Convert.ToDouble(lng));
            //Graph linkedGeoDataResults = LinkedGeoData_GetAllHospitalsNearby(lat, lng);
            //Graph ontologySchema = new Graph();
            //FileLoader.Load(ontologySchema, "C:\\Users\\Oana\\bapr\\test4_rdf.owl");

            //IUriNode rdfType = dbPediaResults.CreateUriNode(new Uri(RdfSpecsHelper.RdfType));
            //IUriNode hospital = dbPediaResults.CreateUriNode("dbo:Hospital");

            //StaticRdfsReasoner reasoner = new StaticRdfsReasoner();
            //reasoner.Initialise(ontologySchema);
            //reasoner.Apply(dbPediaResults);

            //foreach (Triple t in dbPediaResults.GetTriplesWithPredicateObject(rdfType, hospital))
            //{
            //    System.Diagnostics.Debug.WriteLine(t.ToString());
            //}
        }
        private void DbPedia_GetMuseumsNearby(double lat, double lng)   //Barcelona: lat = 41.390205 lng = 2.154007
        {                                                               //1 degree = 110 km lat
            string myQuery = "SELECT DISTINCT ?label ?website ?lat ?long \n" +
                            "WHERE {	\n" + 
                            "?museum a ?type. \n" +
                            "?museum ?p ?label. \n" +
                            "?museum dbpproperty:website ?website.\n" +
                            "?museum dbpedia-owl:thumbnail ?thumbnail.\n" +
                            "?museum geo:lat ?lat.\n" +
                            "?museum geo:long ?long.\n" +
                            "FILTER (?p=<http://www.w3.org/2000/01/rdf-schema#label>).\n" +
                            "FILTER (?type IN (<http://dbpedia.org/ontology/Museum>, <http://schema.org/Museum>, <http://dbpedia.org/class/yago/Museum103800563>)).\n" +
                            "FILTER ( ?long > " + lng + " - 1 && ?long < " + lng + " + 1 && \n" +
                            "?lat > " + lat + " - 1 && ?lat < " + lat + " + 1)\n" +
                            "FILTER ( lang(?label) = 'en')}\n" + 
                            "LIMIT 30".Replace(',', '.');

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
    }
}
