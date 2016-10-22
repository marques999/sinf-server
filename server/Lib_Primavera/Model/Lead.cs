using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FirstREST.Lib_Primavera.Model
{
    public class Lead
    {
        [JsonProperty(PropertyName = "name")]
        public string Nome
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "address")]
        public Address Morada
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

        [JsonProperty(PropertyName = "website")]
        public string Website
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "phone")]
        public string Telefone
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "dateModified")]
        [JsonConverter(typeof(JavaScriptDateTimeConverter))]
        public DateTime LastUpdated
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "dateCreated")]
        [JsonConverter(typeof(JavaScriptDateTimeConverter))]
        public DateTime CreatedAt
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "lastContact")]
        [JsonConverter(typeof(JavaScriptDateTimeConverter))]
        public DateTime ModifiedAt
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