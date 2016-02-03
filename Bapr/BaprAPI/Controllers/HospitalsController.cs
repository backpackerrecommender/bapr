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

namespace BaprAPI.Controllers
{
    public class HospitalsController : ApiController
    {
        public void GetAllHospitalsNearby(string lat, string lng)
        {
            Graph dbPediaResults = DbPedia_GetAllHospitalsNearby(lat, lng);
            Graph linkedGeoDataResults = LinkedGeoData_GetAllHospitalsNearby(lat, lng);
            Graph ontologySchema = new Graph();
            FileLoader.Load(ontologySchema, "C:\\Users\\Oana\\bapr\\test4_rdf.owl");

            IUriNode rdfType = dbPediaResults.CreateUriNode(new Uri(RdfSpecsHelper.RdfType));
            IUriNode hospital = dbPediaResults.CreateUriNode("dbo:Hospital");

            StaticRdfsReasoner reasoner = new StaticRdfsReasoner();
            reasoner.Initialise(ontologySchema);
            reasoner.Apply(dbPediaResults);

            //foreach (Triple t in dbPediaResults.GetTriplesWithPredicateObject(rdfType, hospital))
            //{
            //    System.Diagnostics.Debug.WriteLine(t.ToString());
            //}
        }
        private Graph DbPedia_GetAllHospitalsNearby(string lat, string lng)  //Barcelona: lat = 41.390205 lng = 2.154007
        {                                                                    //1 degree = 110 km lat
            string myQuery = String.Format(@"construct where { 
                               ?s a dbo:Hospital; 
                               geo:lat ?lat;
                               geo:long ?long.
                               FILTER (?lat > {0} - 1 && ?lat < {0} + 1 && 
                                       ?long > {1} - 1 && ?long < {1} + 1 && )
                               }
                               LIMIT 20", lat, lng);

            SparqlParameterizedString dbpediaQuery = new SparqlParameterizedString();

            dbpediaQuery.Namespaces.AddNamespace("geo", new Uri("http://www.w3.org/2003/01/geo/wgs84_pos#"));
            dbpediaQuery.Namespaces.AddNamespace("dbo", new Uri("http://dbpedia.org/ontology/"));
            dbpediaQuery.CommandText = myQuery;

            SparqlQueryParser parser = new SparqlQueryParser();
            SparqlQuery query = parser.ParseFromString(dbpediaQuery);
            Uri uri = new Uri(@"http://dbpedia.org/sparql");
            SparqlResultSet resultSet = new SparqlResultSet();
            ISparqlResultsHandler resultsHandler = new ResultSetHandler(resultSet);
            SparqlRemoteEndpoint endPoint = new SparqlRemoteEndpoint(uri);
            ISparqlQueryProcessor processor = new RemoteQueryProcessor(endPoint);

            return (Graph)processor.ProcessQuery(query);

            //SparqlJsonWriter jsonWriter = new SparqlJsonWriter();
            //TextWriter writer = System.IO.File.CreateText("C:\\Users\\Oana\\Desktop\\results.txt");
            //jsonWriter.Save(processor.ProcessQuery(query), writer);
        }

        private Graph LinkedGeoData_GetAllHospitalsNearby(string lat, string lng)
        {
            string myQuery = String.Format(@"construct where { 
                               ?s a lgdo:Hospital; 
                               geo:lat ?lat;
                               geo:long ?long.
                               FILTER (?lat > {0} - 1 && ?lat < {0} + 1 && 
                                       ?long > {1} - 1 && ?long < {1} + 1 && )
                               }
                               LIMIT 20", lat, lng);

            SparqlParameterizedString lgdoQuery = new SparqlParameterizedString();

            lgdoQuery.Namespaces.AddNamespace("geo", new Uri("http://www.w3.org/2003/01/geo/wgs84_pos#"));
            lgdoQuery.Namespaces.AddNamespace("lgdo", new Uri("http://linkedgeodata.org/ontology/"));
            lgdoQuery.CommandText = myQuery;

            SparqlQueryParser parser = new SparqlQueryParser();
            SparqlQuery query = parser.ParseFromString(lgdoQuery);
            Uri uri = new Uri(@"http://linkedgeodata.org/sparql");
            SparqlResultSet resultSet = new SparqlResultSet();
            ISparqlResultsHandler resultsHandler = new ResultSetHandler(resultSet);
            SparqlRemoteEndpoint endPoint = new SparqlRemoteEndpoint(uri);
            ISparqlQueryProcessor processor = new RemoteQueryProcessor(endPoint);

            return (Graph)processor.ProcessQuery(query);
        }
    }
}
