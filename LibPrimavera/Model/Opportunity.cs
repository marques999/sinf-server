using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FirstREST.LibPrimavera.Model
{
    public class Opportunity
    {
        [JsonProperty(PropertyName = "id")]
        public string Identifier
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

        [JsonProperty(PropertyName = "dateModified")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime DateModified
        {
            get;
            set;
        }
    }
}