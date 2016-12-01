using System;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

using FirstREST.LibPrimavera;
using FirstREST.LibPrimavera.Model;
using FirstREST.LibPrimavera.Integration;

namespace FirstREST.Controllers
{
    public class LocationController : ApiController
    {
        // GET api/location?token={$token}/
        // FEATURE: Listar distritos
        public HttpResponseMessage Get([FromUri] string token)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, LocationIntegration.List());
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        // GET api/location/{$id}?token={$token}/
        // FEATURE: Listar concelhos
        public HttpResponseMessage Get(string id, [FromUri] string token)
        {
            try
            {
                var operationResult = LocationIntegration.View(HttpUtility.UrlDecode(id));

                if (operationResult == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, operationResult);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
    }
}