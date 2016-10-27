using System;
using Newtonsoft.Json;

namespace FirstREST.Lib_Primavera.Model
{
    public class Lead : Contact
    {
        [JsonProperty(PropertyName = "title")]
        public dynamic Title
        {
            get;
            set;
        }
    }
}