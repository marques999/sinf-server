using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FirstREST.LibPrimavera.Model
{
    public class Contact
    {
        [JsonProperty(PropertyName = "id")]
        public string Identficador
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "name")]
        public string NomeFiscal
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "title")]
        public string Titulo
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "location")]
        public Address Localizacao
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
        public string Telefone
        {
            get;
            set;
        }


        [JsonProperty(PropertyName = "phone2")]
        public string Telefone2
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "mobile")]
        public string Telemovel
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "dateModified")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime ModificadoEm
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
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime DateModified
        {
            get;
            set;
        }
    }
}