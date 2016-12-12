using System.Collections.Generic;

using Newtonsoft.Json;

namespace FirstREST.LibPrimavera.Model
{
    public class QuoteInfo : Quote
    {
        [JsonProperty(PropertyName = "id")]
        public string NumEncomenda
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

        [JsonProperty(PropertyName = "billingAddress")]
        public Address EnderecoFacturacao
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "shippingAddress")]
        public Address EnderecoExpedicao
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "opportunity")]
        public new Reference Oportunidade
        {
            get;
            set;
        }
    }
}