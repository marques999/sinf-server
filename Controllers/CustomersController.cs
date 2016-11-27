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
    public class CustomersController : ApiController
    {
        // GET api/customers/
        // FEATURE: Listar clientes
        public HttpResponseMessage Get()
        {
            if (Authentication.VerifyToken("?"))
            {
                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CustomerIntegration.List(Authentication.GetRepresentative(null)));
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
            if (Authentication.VerifyToken("?"))
            {
                try
                {
                    var operationResult = CustomerIntegration.View(Authentication.GetRepresentative(null), Encoding.UTF8.GetString(Convert.FromBase64String(id)));

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
            else
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden);
            }
        }

        // POST api/customers/
        // FEATURE: Adicionar cliente
        public HttpResponseMessage Post([FromBody] Customer jsonObject)
        {
            if (Authentication.VerifyToken("?"))
            {
                try
                {
                    var operationResult = CustomerIntegration.Insert(Authentication.GetRepresentative(null), jsonObject);

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
            else
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden);
            }
        }

        // PUT api/customers/{$customerId}/
        // FEATURE: Modificar cliente existente
        public HttpResponseMessage Put(string id, [FromBody] Customer jsonObject)
        {
            if (Authentication.VerifyToken("?"))
            {
                try
                {
                    var operationResult = CustomerIntegration.Update(Authentication.GetRepresentative(null), Encoding.UTF8.GetString(Convert.FromBase64String(id)), jsonObject);

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
            else
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden);
            }
        }

        // DELETE api/customers/{$customerId}/
        // FEATURE: Remover cliente existente
        public HttpResponseMessage Delete(string id)
        {
            if (Authentication.VerifyToken("?"))
            {
                try
                {
                    if (CustomerIntegration.Delete(Authentication.GetRepresentative(null), Encoding.UTF8.GetString(Convert.FromBase64String(id))))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound);
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