using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FirstREST.Lib_Primavera.Model
{
    public class Contact
    {
        [JsonProperty(PropertyName = "name")]
        public string Name
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "lccation")]
        public Address Location
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "email")]
        public string Email
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "phone")]
        public string Phone
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "lastUpdated")]
        [JsonConverter(typeof(JavaScriptDateTimeConverter))]
        public DateTime LastUpdated
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "createdAt")]
        [JsonConverter(typeof(JavaScriptDateTimeConverter))]
        public DateTime CreatedAt
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "owner")]
        public UserReference Owner
        {
            get;
            set;
        }
    }
}