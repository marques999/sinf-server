using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FirstREST.LibPrimavera.Model
{
    public class Activity
    {
        /// <summary>
        /// Private access fields
        /// </summary>

        private int _priority;


        /// <summary>
        /// Public access properties
        /// </summary>

        [JsonProperty(PropertyName = "id")]
        public string Identificador
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "name")]
        public string Name
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
        public ActivityStatus Status
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "type")]
        public Reference Type
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "entity")]
        public Reference Entity
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "priority")]
        public string Prioridade
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

        [JsonProperty(PropertyName = "duration")]
        public string Responsavel
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

        [JsonProperty(PropertyName = "entityType")]
        public string TipoEntidade
        {
            get;
            set;
        }
    }
}