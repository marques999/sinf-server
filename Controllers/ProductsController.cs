using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using FirstREST.LibPrimavera;
using FirstREST.LibPrimavera.Model;
using FirstREST.LibPrimavera.Integration;

namespace FirstREST.Controllers
{
    public class ProductsController : ApiController
    {
        // GET api/products/
        // FEATURE: Listar produto
        public HttpResponseMessage Get()
        {
            if (PrimaveraEngine.IsAuthenticated())
            {
                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK, ProductIntegration.List());
                }
                catch
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden);
            }
        }

        // GET api/products/{$productId}/
        // FEATURE: Visualizar produto
        public HttpResponseMessage Get(string id)
        {
            if (PrimaveraEngine.IsAuthenticated())
            {
                try
                {
                    var queryResult = ProductIntegration.View(id);

                    if (queryResult == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, queryResult);
                    }
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