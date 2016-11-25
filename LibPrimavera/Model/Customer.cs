using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FirstREST.LibPrimavera.Model
{
    public class Customer : Entity
    {
        [JsonProperty(PropertyName = "website")]
        public string EnderecoWeb
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "status")]
        public string Situacao
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

        [JsonProperty(PropertyName = "taxNumber")]
        public string NumContribuinte
        {
            get;
            set;
        }
    }
}