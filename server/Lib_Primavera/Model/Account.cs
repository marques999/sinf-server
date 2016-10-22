using System;
using Newtonsoft.Json;

namespace FirstREST.Lib_Primavera.Model
{
    public class Account : Contact
    {
        [JsonProperty(PropertyName = "website")]
        public string Website
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "taxNumber")]
        public string TaxNumber
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "currency")]
        public string Currency
        {
            get;
            set;
        }
    }
}