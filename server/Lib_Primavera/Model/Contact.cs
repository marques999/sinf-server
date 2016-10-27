using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FirstREST.Lib_Primavera.Model
{
    public class Contact
    {
        [JsonProperty(PropertyName = "id")]
        public string Identifier
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "name")]
        public string Name
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "location")]
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

        [JsonProperty(PropertyName = "mobile")]
        public string MobilePhone
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "lastContact")]
        public string LastContact
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "createdAt")]
        [JsonConverter(typeof(JavaScriptDateTimeConverter))]
        public DateTime DateCreated
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