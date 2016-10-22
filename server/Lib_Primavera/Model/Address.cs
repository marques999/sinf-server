using System;
using Newtonsoft.Json;

namespace FirstREST.Lib_Primavera.Model
{
    [Serializable]
    public class Address
    {
        [JsonProperty(PropertyName = "street")]
        public string Rua
        {
            get;
            set;
        }
        
        [JsonProperty(PropertyName = "postalCode")]
        public string CodigoPostal
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "city")]
        public string Zona
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "country")]
        public string Pais
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "coordinates")]
        public string Coordinates
        {
            get;
            set;
        }
    }
}