using System.ComponentModel;

using Newtonsoft.Json;

namespace FirstREST.LibPrimavera.Model
{
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

        [DefaultValue("PT")]
        [JsonProperty(PropertyName = "country")]
        public string Country
        {
            get;
            set;
        }
    }
}