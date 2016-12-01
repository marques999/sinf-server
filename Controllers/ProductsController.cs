using System;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Text;

using FirstREST.LibPrimavera;
using FirstREST.LibPrimavera.Model;
using FirstREST.LibPrimavera.Integration;

namespace FirstREST.Controllers
{
    public class ProductsController : ApiController
    {
        // GET api/products?token={$token}/
        // FEATURE: Listar produto
        public HttpResponseMessage Get([FromUri] string token)
        {
            if (Authentication.VerifyToken(token))
            {
                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK, ProductIntegration.List());
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

        // GET api/products/{$id}?token={$token}/
        // FEATURE: Visualizar produto
        public HttpResponseMessage Get(string id, [FromUri] string token)
        {
            if (Authentication.VerifyToken(token))
            {
                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK, ProductIntegration.View(Encoding.UTF8.GetString(Convert.FromBase64String(id))));
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