using BaprAPI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public HttpResponseMessage Get(string text, double latitude, double longitude, string userEmail)
        {
            var userPreference = BaprAPI.Utils.Utils.GetUserPreference(userEmail);
            var fromDbPedia = GetFromDbpedia(text, latitude, longitude, userPreference);
            var fromLinkedGeoData = GetFromLinkedGeoData(text, latitude, longitude, userPreference);
            var allCollections = fromDbPedia.Concat(fromLinkedGeoData);
            return Request.CreateResponse(HttpStatusCode.OK, allCollections);
        }

        public ICollection<BaprLocation> GetFromDbpedia(string text, double latitude, double longitude, IUserPreference userPreference)
        {
            var cuisineFromDb = BaprAPI.Utils.Utils.GetCuisines(userPreference, true);
            string searchQueryByText = "SELECT DISTINCT ?lat ?long ?name ?address ?cuisine ?website ?established ?comment WHERE{\n"
              + "?s a dbo:Location .\n"
              + "?s geo:lat ?lat .\n"
              + "?s geo:long ?long .\n"
              + "?s rdfs:label ?name.\n"
              + "?s a ?type. \n"
              + "OPTIONAL { ?s dbp:established ?established.}\n"
              + "OPTIONAL { ?s rdfs:comment ?comment. FILTER(langMatches(lang(?comment ), \"en\"))}\n"
              + "OPTIONAL { ?s dbo:address ?address .}\n "
              + "OPTIONAL { ?s dbo:cuisine ?cuisine .}\n"
              + "OPTIONAL { ?s foaf:homepage ?website .}\n"
              + "FILTER(langMatches(lang(?name ), \"en\") "
                + " && ?lat >" + latitude + " - 1 && ?lat < " + latitude + "+ 1 "
                + " && ?long > " + longitude + " - 1 && ?long < " + longitude + " + 1"
               + "  && contains(lcase(?name),lcase(\"" + text.ToLower() + "\"))"
               + "  && ((?type=dbo:Restaurant &&" + (!string.IsNullOrEmpty(cuisineFromDb) ? " lcase(?cuisine) in (" + cuisineFromDb + ")) " : string.Empty)
                      +"|| (?type = dbo:Museum && contains(lcase(str(?type)), \"history\") )"
                      + "|| ?type=dbo:Town || ?type=dbo:Village "
                      + "|| ?type=dbo:Park || ?type=dbo:Garden || ?type=dbo:HistoricPlace || ?type=dbo:Monument)) "
              + "}LIMIT 20";

            SparqlParameterizedString sparqlQueryString = new SparqlParameterizedString();
            sparqlQueryString.Namespaces.AddNamespace("rdf", new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            sparqlQueryString.Namespaces.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));
            sparqlQueryString.Namespaces.AddNamespace("dbo", new Uri("http://dbpedia.org/ontology/"));
            sparqlQueryString.Namespaces.AddNamespace("geo", new Uri("http://www.w3.org/2003/01/geo/wgs84_pos#"));
            sparqlQueryString.Namespaces.AddNamespace("dbp", new Uri("http://dbpedia.org/property/"));
            sparqlQueryString.Namespaces.AddNamespace("foaf", new Uri("http://xmlns.com/foaf/0.1/"));
            sparqlQueryString.CommandText = searchQueryByText;

            SparqlQueryParser parser = new SparqlQueryParser();
            SparqlQuery query = parser.ParseFromString(sparqlQueryString);

            Uri uri = new Uri(@"http://dbpedia.org/sparql");
            SparqlRemoteEndpoint endPoint = new SparqlRemoteEndpoint(uri);
            ISparqlQueryProcessor processor = new RemoteQueryProcessor(endPoint);
            object queryResult = processor.ProcessQuery(query);
           
            if (queryResult is SparqlResultSet)
            {
                SparqlResultSet entities = (SparqlResultSet)queryResult;
                return BaprAPI.Utils.Utils.ConvertFromSparqlSetToBaprLocations(entities);
            }
            return new Collection<BaprLocation>();
        }

        public ICollection<BaprLocation> GetFromLinkedGeoData(string text, double latitude, double longitude, IUserPreference userPreference)
        {
            var cuisine = BaprAPI.Utils.Utils.GetCuisines(userPreference, false);
            string searchQueryByText = "SELECT DISTINCT ?lat ?long ?name ?website ?cuisine ?address ?opening_hours ?wheelchair WHERE {\n"
                + "?s geo:lat ?lat .\n"
                + "?s geo:long ?long .\n"
                + "?s rdfs:label ?name. \n"
                + "?s a ?type. \n"
                + "OPTIONAL { ?s foaf:homepage ?website .}\n"
                + "OPTIONAL { ?s lgd:cuisine ?cuisine .}\n"
                + "OPTIONAL { ?s lgd:address ?address .}\n "
                + "OPTIONAL { ?s lgd:opening_hours  ?opening_hours .}\n "
                + (userPreference.NeedMedicalSupport ? "?s lgd:wheelchair ?wheelchair. FILTER (?wheelchair =" + BaprAPI.Models.Constants.xsdBooleanIsTrue + ")\n"
                                                      : " OPTIONAL {?s lgd:wheelchair ?wheelchair.} \n")
                + "FILTER (langMatches(lang(?name ), \"en\") \n"
                + " && ?lat > " + latitude + " - 1 && ?lat < " + latitude + " + 1 "
                + " && ?long > " + longitude + " - 1 && ?long < " + longitude + " + 1 \n"
                + " && contains(lcase(?name),lcase(\"" + text.ToLower() + "\"))\n"
                + " && ((?type = lgd:Restaurant " + (!string.IsNullOrEmpty(cuisine) ? "&& lcase(?cuisine) in (" + cuisine + ")) \n" : string.Empty)
                        + "|| ?type = lgd:Hospital || ?type = lgd:Bar || ?type = lgd:Bank \n"
                        + "|| ?type = lgd:Pharmacy || ?type = lgd:FastFood || ?type=lgd:Cafe || ?type=lgd:Pub || ?type= lgd:Theatre"
                        + "|| ?type = lgd:Museum))"
                + "} LIMIT 10";


            SparqlParameterizedString sparqlQueryString = new SparqlParameterizedString();
            sparqlQueryString.Namespaces.AddNamespace("rdf", new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            sparqlQueryString.Namespaces.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));
            sparqlQueryString.Namespaces.AddNamespace("lgd", new Uri("http://linkedgeodata.org/ontology/"));
            sparqlQueryString.Namespaces.AddNamespace("geo", new Uri("http://www.w3.org/2003/01/geo/wgs84_pos#"));
            sparqlQueryString.Namespaces.AddNamespace("foaf", new Uri("http://xmlns.com/foaf/0.1/"));
            sparqlQueryString.CommandText = searchQueryByText;

            SparqlQueryParser parser = new SparqlQueryParser();
            SparqlQuery query = parser.ParseFromString(sparqlQueryString);

            Uri uri = new Uri(@"http://linkedgeodata.org/sparql");
            SparqlRemoteEndpoint endPoint = new SparqlRemoteEndpoint(uri);
            ISparqlQueryProcessor processor = new RemoteQueryProcessor(endPoint);
            object queryResult = processor.ProcessQuery(query);

            if (queryResult is SparqlResultSet)
            {
                SparqlResultSet entities = (SparqlResultSet)queryResult;
                return BaprAPI.Utils.Utils.ConvertFromSparqlSetToBaprLocations(entities);
            }
            return new Collection<BaprLocation>();
        }
    }
}
