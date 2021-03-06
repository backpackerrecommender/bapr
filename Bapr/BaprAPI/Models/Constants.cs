﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BaprAPI.Models
{
    public static class Constants
    {
        public static string StoreLocation
        {
            get { return System.AppDomain.CurrentDomain.BaseDirectory.ToString(); }
        }

        public static string storeConnectionString = "type=embedded;storesdirectory=" + Constants.StoreLocation + ";storename=Users";
        
        public static string xsdBooleanIsTrue= "\"true\"^^<http://www.w3.org/2001/XMLSchema#boolean>";

        public static string xsdString = "^^<http://www.w3.org/2001/XMLSchema#string>";

        public static string Ontology = "Utils\\ontology.owl";
    }
}