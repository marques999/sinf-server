using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FirstREST.LibPrimavera.Model
{
    public class Product : ProductListing
    {
        /// <summary>
        /// Private access fields
        /// </summary>

        private double _discount;

        /// <summary>
        /// Public access properties
        /// </summary>

        [JsonProperty(PropertyName = "barcode")]
        public string CodigoBarras
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "dateCreated")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime UltimaEntrada
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "dateModified")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime UltimaSaida
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "discount")]
        public double Desconto
        {
            get
            {
                return _discount;
            }
            set
            {
                if (value < 0.0)
                {
                    _discount = 0.0;
                }
                else if (value > 100)
                {
                    _discount = 1.0;
                }
                else
                {
                    _discount = value * 0.01;
                }
            }
        }

        [JsonProperty(PropertyName = "category")]
        public Reference Categoria
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "prices")]
        public List<ProductPrice> Precos
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "warehouses")]
        public List<WarehouseProduct> Armazens
        {
            get;
            set;
        }
    }
}