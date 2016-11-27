using Newtonsoft.Json;
using System.Collections.Generic;

namespace FirstREST.LibPrimavera.Model
{
    public class Quote
    {
        [JsonProperty(PropertyName = "description")]
        public string Descricao
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "opportunity")]
        public string OpportunityId
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "products")]
        public List<Order> Produtos
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "notes")]
        public string Notas
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "billingAddress")]
        public Address BillingAddress
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "shippingAddress")]
        public Address ShippingAddress
        {
            get;
            set;
        }
    }
}