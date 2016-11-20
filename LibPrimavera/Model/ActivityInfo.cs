using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FirstREST.LibPrimavera.Model
{
    public class ActivityInfo : Activity
    {
        [JsonProperty(PropertyName = "id")]
        public string Identificador
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

        [JsonProperty(PropertyName = "duration")]
        public int Duracao
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "entity")]
        public new EntityReference Entidade
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

        [JsonProperty(PropertyName = "dateModified")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime ModificadoEm
        {
            get;
            set;
        }
    }
}