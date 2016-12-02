using Newtonsoft.Json;

namespace FirstREST.LibPrimavera.Model
{
    public class RepresentativeListing
    {
        /// <summary>
        /// Private access fields
        /// </summary>

        private double _comissao;

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
        public string Nome
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "comission")]
        public double Comissao
        {
            get
            {
                return _comissao;
            }
            set
            {
                if (value < 0.0)
                {
                    _comissao = 0.0;
                }
                else if (value > 100.0)
                {
                    _comissao = 1.0;
                }
                else
                {
                    _comissao = value * 0.01;
                }
            }
        }
    }
}