using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Writing;

namespace BaprAPI.Controllers
{
    public class LocationsController : ApiController
    {
        public HttpResponseMessage Get(string text, double latitude, double longitude)
        {
            string searchQueryByText = "SELECT * WHERE{\n"
                + "?s a dbo:Location .\n"
                + "?s geo:lat ?lat .\n"
                + "?s geo:long ?long .\n"
                + "?s rdfs:label ?name. filter contains(lcase(?name),lcase(\"" + text.ToLower() + "\"))\n"
                + "?s a ?type  .\n"
                + "FILTER(langMatches(lang(?name ), \"en\") "
                  + " && ?lat >" + (latitude - 1) + " && ?lat < " + (latitude + 1)
                  + " && ?long > " + (longitude - 1) + " && ?long < " + (longitude + 1)
                 + "  && contains(lcase(?name),lcase(\"" + text.ToLower() + "\"))"
                 + "  && (?type=dbo:City || ?type=dbo:Town || ?type=dbo:Village || ?type=dbo:Restaurant || ?type=dbo:Park || ?type=dbo:Garden || ?type=dbo:HistoricPlace || ?type=dbo:Monument)) "
                + "}LIMIT 10";

            SparqlParameterizedString sparqlQueryString = new SparqlParameterizedString();
            sparqlQueryString.Namespaces.AddNamespace("rdf", new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            sparqlQueryString.Namespaces.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));
            sparqlQueryString.Namespaces.AddNamespace("dbo", new Uri("http://dbpedia.org/ontology/"));
            sparqlQueryString.Namespaces.AddNamespace("geo", new Uri("http://www.w3.org/2003/01/geo/wgs84_pos#"));
            sparqlQueryString.CommandText = searchQueryByText;

            SparqlQueryParser parser = new SparqlQueryParser();
            SparqlQuery query = parser.ParseFromString(sparqlQueryString);

            Uri uri = new Uri(@"http://dbpedia.org/sparql");
            SparqlRemoteEndpoint endPoint = new SparqlRemoteEndpoint(uri);
            ISparqlQueryProcessor processor = new RemoteQueryProcessor(endPoint);
            object queryResult = processor.ProcessQuery(query);
        
            if (queryResult is SparqlResultSet)
            {
               
            }

            return Request.CreateResponse(HttpStatusCode.OK, "No results");

        }
    }
}
