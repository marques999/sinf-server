using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FirstREST.LibPrimavera.Model
{
    public class OpportunityInfo : Opportunity
    {
        [JsonProperty(PropertyName = "dateCreated")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime DataCriacao
        {
            get;
            set;
        }
    }
}