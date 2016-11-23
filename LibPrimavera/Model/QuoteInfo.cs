using System.Collections.Generic;

using Newtonsoft.Json;

namespace FirstREST.LibPrimavera.Model
{
    public class QuoteInfo : Quote
    {
        [JsonProperty(PropertyName = "id")]
        public string Identificador
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "products")]
        public new List<OrderInfo> Produtos
        {
            get;
            set;
        }
    }
}