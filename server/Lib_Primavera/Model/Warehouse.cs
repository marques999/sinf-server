using System;
using Newtonsoft.Json;

namespace FirstREST.Lib_Primavera.Model
{
    [Serializable]
    public class Warehouse
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

        [JsonProperty(PropertyName = "location")]
        public Address Location
        {
            get
            {
                return Location;
            }
            set
            {
                if (value != null)
                {
                    Location = value;
                }
            }
        }
    }
}