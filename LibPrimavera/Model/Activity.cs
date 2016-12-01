using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FirstREST.LibPrimavera.Model
{
    public class Activity
    {
        [JsonProperty(PropertyName = "name")]
        public string Resumo
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "description")]
        public string Descricao
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "status")]
        public int Estado
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "type")]
        public Reference Tipo
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "priority")]
        public int Prioridade
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "entity")]
        public string Entidade
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "entityType")]
        public string TipoEntidade
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "start")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime DataInicio
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "end")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime DataFim
        {
            get;
            set;
        }
    }
}