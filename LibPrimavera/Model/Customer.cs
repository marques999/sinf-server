using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FirstREST.LibPrimavera.Model
{
    public class Customer : Contact
    {
        [JsonProperty(PropertyName = "website")]
        public string EnderecoWeb
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "status")]
        public string Estado
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "dateCreated")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime CriadoEm
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "particular")]
        public bool Particular
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "debt")]
        public double Debito
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "pending")]
        public double Pendentes
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "taxNumber")]
        public string NumContribuinte
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "currency")]
        public string Moeda
        {
            get;
            set;
        }
    }
}