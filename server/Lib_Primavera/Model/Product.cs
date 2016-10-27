using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace FirstREST.Lib_Primavera.Model
{
    [Serializable]
    public class Product
    {
        [JsonProperty(PropertyName = "id")]
        public string Identifier
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

        [JsonProperty(PropertyName = "cost")]
        public DateTime LastUpdate
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "price")]
        public double Price
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "tax")]
        public double Tax
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "stock")]
        public double Stock
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "discountValue")]
        public double DiscountValue
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "discountEnabled")]
        public bool DiscountEnabled
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "warehouses")]
        public IEnumerable<Warehouse> Warehouses
        {
            get;
            set;
        }
        
        [JsonProperty(PropertyName = "category")]
        public string Category
        {
            get;
            set;
        }
    }
}