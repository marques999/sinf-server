using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FirstREST.LibPrimavera.Model
{
    public class LeadInfo : Lead
    {
        [JsonProperty(PropertyName = "id")]
        public string Identificador
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "active")]
        public bool Activo
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "owner")]
        public Reference Responsavel
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "dateModified")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime DataModificacao
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

        /// <summary>
        /// Tipo Terceiro que indica que foi covertido em cliente
        /// Util para o dashboard e o historico
        /// </summary>
        public const string CONVERT_TO_CLIENT_ID = "010";
    }
}