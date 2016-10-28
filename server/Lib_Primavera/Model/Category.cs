using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace FirstREST.Lib_Primavera.Model
{
    [Serializable]
    public class Category
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

        [JsonProperty(PropertyName = "length")]
        public int NumberProducts
        {
            get;
            set;
        }
    }
}