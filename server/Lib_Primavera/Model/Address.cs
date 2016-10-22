using System;
using Newtonsoft.Json;

namespace FirstREST.Lib_Primavera.Model
{
    [Serializable]
    public class Address
    {
        [JsonProperty(PropertyName = "street")]
        public string Street
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "postalCode")]
        public string PostalCode
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "city")]
        public string City
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

        [JsonProperty(PropertyName = "coordinates")]
        public string Coordinates
        {
            get;
            set;
        }
    }
}