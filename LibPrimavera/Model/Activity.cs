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

        [JsonProperty(PropertyName = "location")]
        public string Local
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "type")]
        public string Tipo
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

        [JsonProperty(PropertyName = "source")]
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