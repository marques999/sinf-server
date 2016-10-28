using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FirstREST.Lib_Primavera.Model
{
    public class Activity
    {
        [JsonProperty(PropertyName = "id")]
        public string Identifier
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "description")]
        public string Description
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "status")]
        public AgendaStatus Status
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "type")]
        public AgendaType Type
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "start")]
        [JsonConverter(typeof(JavaScriptDateTimeConverter))]
        public DateTime Start
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "end")]
        [JsonConverter(typeof(JavaScriptDateTimeConverter))]
        public DateTime End
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "dateCreated")]
        [JsonConverter(typeof(JavaScriptDateTimeConverter))]
        public DateTime DateCreated
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "dateModified")]
        [JsonConverter(typeof(JavaScriptDateTimeConverter))]
        public DateTime DateModified
        {
            get;
            set;
        }
    }
}