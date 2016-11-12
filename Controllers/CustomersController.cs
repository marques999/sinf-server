using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading;

using FirstREST.LibPrimavera;
using FirstREST.LibPrimavera.Model;
using FirstREST.LibPrimavera.Integration;

namespace FirstREST.Controllers
{
    public class CustomersController : ApiController
    {
        // GET api/customers/
        // FEATURE: Listar clientes
        public HttpResponseMessage Get()
        {
            if (PrimaveraEngine.IsAuthenticated())
            {
                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CustomerIntegration.List(Thread.CurrentPrincipal.Identity.Name));
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

        // GET api/customers/{$customerId}/
        // FEATURE: Visualizar cliente
        public HttpResponseMessage Get(string id)
        {
            if (PrimaveraEngine.IsAuthenticated())
            {
                try
                {
                    var queryResult = CustomerIntegration.View(Thread.CurrentPrincipal.Identity.Name, id);

                    if (queryResult == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, queryResult);
                    }
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

        // POST api/customers/
        // FEATURE: Adicionar cliente
        public HttpResponseMessage Post([FromBody] Customer jsonObject)
        {
            if (PrimaveraEngine.IsAuthenticated())
            {
                try
                {
                    jsonObject.Status = "ACTIVO";
                    jsonObject.Identifier = "accountId";
                    jsonObject.DateCreated = DateTime.Now;
                    jsonObject.DateModified = jsonObject.DateCreated;

                    System.Diagnostics.Debug.Print(jsonObject.DateModified.ToString());

                    if (CustomerIntegration.Insert(Thread.CurrentPrincipal.Identity.Name, jsonObject))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound);
                    }
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

        // POST api/customers/{$customerId}/
        // FEATURE: Modificar cliente existente
        public HttpResponseMessage Post(string id, [FromBody] Customer jsonObject)
        {
            if (PrimaveraEngine.IsAuthenticated())
            {
                try
                {
                    jsonObject.DateModified = DateTime.Now;

                    if (CustomerIntegration.Update(Thread.CurrentPrincipal.Identity.Name, id, jsonObject))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound);
                    }
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

        // DELETE api/customers/{$customerId}/
        // FEATURE: Remover cliente existente
        public HttpResponseMessage Delete(string id)
        {
            if (PrimaveraEngine.IsAuthenticated())
            {
                try
                {
                    if (CustomerIntegration.Delete(Thread.CurrentPrincipal.Identity.Name, id))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound);
                    }
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
    }
}