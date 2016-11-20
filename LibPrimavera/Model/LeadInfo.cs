using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FirstREST.LibPrimavera.Model
{
    public class LeadInfo : Lead
    {
        [JsonProperty(PropertyName = "id")]
        public string Identficador
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "owner")]
        public string Responsavel
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
    }
}