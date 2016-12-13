using Newtonsoft.Json;
using System.Collections.Generic;

namespace FirstREST.LibPrimavera.Model
{
    public class Quote
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

        [JsonProperty(PropertyName = "totalDocument")]
        public double TotalDocumento
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "date")]
        public System.DateTime Data
        {
            get;
            set;
        }
        
        /*[JsonProperty(PropertyName = "description")]
        public string Descricao
        {
            get;
            set;
        }*/
    }
}