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
    public class AgendaController : ApiController
    {
        // GET api/agenda/?token={$token}&status=active
        // Feature: Visualizar agenda
        public HttpResponseMessage Get([FromUri] string token, [FromUri] bool status, [FromUri] string empty = "")
        {
            if (Authentication.VerifyToken(token))
            {
                try
                {
                    if (status)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, AgendaIntegration.ListActive(Authentication.GetRepresentative(token)));
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, AgendaIntegration.ListInactive(Authentication.GetRepresentative(token)));
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

        // GET api/agenda/{$id}?token={$token}/
        // FEATURE: Visualizar actividade
        public HttpResponseMessage Get(string id, [FromUri] string token)
        {
            if (Authentication.VerifyToken(token))
            {
                try
                {
                    var operationResult = AgendaIntegration.View(Authentication.GetRepresentative(token), HttpUtility.UrlDecode(id));

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

        // POST api/agenda?token={$token}/
        // Feature: Agendar actividade
        public HttpResponseMessage Post([FromBody] Activity jsonObject, [FromUri] string token)
        {
            if (Authentication.VerifyToken(token))
            {
                try
                {
                    var operationResult = AgendaIntegration.Insert(Authentication.GetRepresentative(token), jsonObject);

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

        // PUT api/agenda/{$id}?token={$token}/
        // Feature: Modificar actividade existente
        public HttpResponseMessage Put(string id, [FromBody] Activity jsonObject, [FromUri] string token)
        {
            if (Authentication.VerifyToken(token))
            {
                try
                {
                    var operationResult = AgendaIntegration.Update(Authentication.GetRepresentative(token), HttpUtility.UrlDecode(id), jsonObject);

                    if (operationResult == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK);
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

        // DELETE api/agenda/{$id}?token={$token}/
        // FEATURE: Remover actividade existente
        public HttpResponseMessage Delete(string id, [FromUri] string token)
        {
            if (Authentication.VerifyToken(token))
            {
                try
                {
                    var operationResult = AgendaIntegration.Delete(Authentication.GetRepresentative(token), HttpUtility.UrlDecode(id));

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