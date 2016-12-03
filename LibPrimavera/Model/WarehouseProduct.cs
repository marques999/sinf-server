using Newtonsoft.Json;

namespace FirstREST.LibPrimavera.Model
{
    public class WarehouseProduct : WarehouseListing
    {
        /// <summary>
        /// Private access fields
        /// </summary>

        private double _stock;

        /// <summary>
        /// Public access properties
        /// </summary>

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