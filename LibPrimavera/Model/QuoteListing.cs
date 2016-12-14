using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FirstREST.LibPrimavera.Model
{
    public class QuoteListing
    {
        [JsonProperty(PropertyName = "quoteNum")]
        public int NumEncomenda
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "clientRef")]
        public string Cliente
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "clientName")]
        public string NomeCliente
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "TotalDocument")]
        public double TotalDocumento
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "date")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime Data
        {
            get;
            set;
        }
    }
}