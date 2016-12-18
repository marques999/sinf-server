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
        // GET api/customers?token={$token}/
        // FEATURE: Listar clientes
        public HttpResponseMessage Get([FromUri] string token)
        {
            if (Authentication.VerifyToken(token))
            {
                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK, CustomerIntegration.List(Authentication.GetRepresentative(token)));
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

        // GET api/customers/{$id}?token={$token}/
        // FEATURE: Visualizar cliente
        public HttpResponseMessage Get(string id, [FromUri] string token)
        {
            if (Authentication.VerifyToken(token))
            {
                try
                {
                    var operationResult = CustomerIntegration.View(Authentication.GetRepresentative(token), Encoding.UTF8.GetString(Convert.FromBase64String(id)));

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

        // POST api/customers?token={$token}/
        // FEATURE: Adicionar cliente
        public HttpResponseMessage Post([FromBody] Customer jsonObject, [FromUri] string token)
        {
            if (Authentication.VerifyToken(token))
            {
                try
                {
                    var operationResult = CustomerIntegration.Insert(Authentication.GetRepresentative(token), jsonObject);

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

        // PUT api/customers/{$id}?token={$token}/
        // FEATURE: Modificar cliente existente
        public HttpResponseMessage Put(string id, [FromBody] Customer jsonObject, [FromUri] string token)
        {
            if (Authentication.VerifyToken(token))
            {
                try
                {
                    var operationResult = CustomerIntegration.Update(Authentication.GetRepresentative(token), Encoding.UTF8.GetString(Convert.FromBase64String(id)), jsonObject);

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

        // DELETE api/customers/{$id}?token={$token}/
        // FEATURE: Remover cliente existente
        public HttpResponseMessage Delete(string id, [FromUri] string token)
        {
            if (Authentication.VerifyToken(token))
            {
                try
                {
                    var operationResult = CustomerIntegration.Delete(Authentication.GetRepresentative(token), Encoding.UTF8.GetString(Convert.FromBase64String(id)));

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
    }
}