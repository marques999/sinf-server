using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FirstREST.LibPrimavera.Model
{
    public class Warehouse
    {
        /// <summary>
        /// Private access fields
        /// </summary>

        private double _stock;

        /// <summary>
        /// Public access properties
        /// </summary>

        [JsonProperty(PropertyName = "id")]
        public string Identificador
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "name")]
        public string Descricao
        {
            get;
            set;
        }


        [JsonProperty(PropertyName = "phone")]
        public string Telefone
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "dateModified")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime ModificadoEm
        {
            get;
            set;
        }


        [JsonProperty(PropertyName = "location")]
        public Address Location
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "stock")]
        public double Stock
        {
            get
            {
                return _stock;
            }
            set
            {
                if (value < 0.0)
                {
                    _stock = 0.0;
                }
                else
                {
                    _stock = value;
                }
            }
        }
    }
}