using System;
using Newtonsoft.Json;

namespace FirstREST.Lib_Primavera.Model
{
    [Serializable]
    public class SuccessResponse : ServerResponse
    {
        public SuccessResponse(object paramObject): base()
        {
            Object = paramObject;
        }

        [JsonProperty(PropertyName = "success")]
        private object Object;
    }
}