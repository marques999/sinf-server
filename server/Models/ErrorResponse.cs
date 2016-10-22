using System;
using Newtonsoft.Json;

namespace FirstREST.Lib_Primavera.Model
{
    [Serializable]
    public class ErrorResponse : ServerResponse
    {
        public ErrorResponse(string paramMessage) : base()
        {
            Message = paramMessage;
        }

        [JsonProperty(PropertyName = "error")]
        private string Message;
    }
}