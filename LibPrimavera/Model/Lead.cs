using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FirstREST.LibPrimavera.Model
{
    public class Lead : Contact
    {
        [JsonProperty(PropertyName = "active")]
        public bool Active
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "dateCreated")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime DateCreated
        {
            get;
            set;
        }
    }
}