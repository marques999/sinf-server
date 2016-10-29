using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FirstREST.LibPrimavera.Model
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

        [JsonProperty(PropertyName = "title")]
        public string Title
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

        [JsonProperty(PropertyName = "dateModified")]
        [JsonConverter(typeof(JavaScriptDateTimeConverter))]
        public DateTime DateModified
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "owner")]
        public Reference Owner
        {
            get;
            set;
        }
    }

    public class ContactListing
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

        [JsonProperty(PropertyName = "title")]
        public string Title
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "address")]
        public string Address
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

        [JsonProperty(PropertyName = "email")]
        public string Email
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

        [JsonProperty(PropertyName = "dateModified")]
        [JsonConverter(typeof(JavaScriptDateTimeConverter))]
        public DateTime DateModified
        {
            get;
            set;
        }
    }
}