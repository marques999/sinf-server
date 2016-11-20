using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FirstREST.LibPrimavera.Model
{
    public class Lead : Entity
    {
        [JsonProperty(PropertyName = "active")]
        public bool Activo
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
    }
}