using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FirstREST.Lib_Primavera.Model
{
    public class Lead : Contact
    {
        [JsonProperty(PropertyName = "lastContact")]
        [JsonConverter(typeof(JavaScriptDateTimeConverter))]
        public DateTime ModifiedAt
        {
            get;
            set;
        }
    }
}