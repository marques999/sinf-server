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
    public class WarehousesController : ApiController
    {
        // GET api/warehouses?token={$token}/
        // FEATURE: Listar armazéns
        public HttpResponseMessage Get([FromUri] string token)
        {
            if (Authentication.VerifyToken(token))
            {
                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK, WarehouseIntegration.List());
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden);
            }
        }

        // GET api/warehouses/{$id}?token={$token}/
        // FEATURE: Visualizar armazém
        public HttpResponseMessage Get(string id, [FromUri] string token)
        {
            if (Authentication.VerifyToken(token))
            {
                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK, WarehouseIntegration.Get(HttpUtility.UrlDecode(id)));
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden);
            }
        }
    }
}