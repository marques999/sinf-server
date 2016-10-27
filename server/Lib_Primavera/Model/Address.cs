using System;
using Newtonsoft.Json;

namespace FirstREST.Lib_Primavera.Model
{
    [Serializable]
    public class Address
    {
        [JsonProperty(PropertyName = "address")]
        public string Street
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "postal")]
        public string PostalCode
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "parish")]
        public string Parish
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "state")]
        public string State
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "country")]
        public string Country
        {
            get;
            set;
        }
    }
}