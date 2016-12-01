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
    public class CategoriesController : ApiController
    {
        // GET api/categories?token={$token}/
        // FEATURE: Listar categorias
        public HttpResponseMessage Get([FromUri] string token)
        {
            if (Authentication.VerifyToken(token))
            {
                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CategoryIntegration.List());
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

        // GET api/categories/{$id}?token={$token}/
        // FEATURE: Visualizar categoria
        public HttpResponseMessage Get(string id, [FromUri] string token)
        {
            if (Authentication.VerifyToken(token))
            {
                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CategoryIntegration.Get(HttpUtility.UrlDecode(id)));
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