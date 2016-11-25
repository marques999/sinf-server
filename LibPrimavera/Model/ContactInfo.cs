using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FirstREST.LibPrimavera.Model
{
    public class ContactInfo : Contact
    {
        [JsonProperty(PropertyName = "id")]
        public string Identficador
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "owner")]
        public Reference Responsavel
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "dateCreated")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime DataCriacao
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "dateModified")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime DataModificacao
        {
            get;
            set;
        }
    }
}