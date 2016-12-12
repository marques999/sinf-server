using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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

        [JsonProperty(PropertyName = "notes")]
        public string Notas
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

        [JsonProperty(PropertyName = "opportunity")]
        public string Oportunidade
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "total")]
        public double Total
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

        [JsonProperty(PropertyName = "date")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime DataEncomenda
        {
            get;
            set;
        }
    }
}