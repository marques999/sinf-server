using System;
using System.Net;
using System.Net.Http;

using Newtonsoft.Json;

namespace FirstREST.Lib_Primavera.Model
{
    [Serializable]
    public abstract class ServerResponse
    {
        public ServerResponse()
        {
        }

        public HttpResponseMessage sendResponse(HttpRequestMessage paramRequest)
        {
            return paramRequest.CreateResponse(HttpStatusCode.BadRequest, this);
        }
    }
}