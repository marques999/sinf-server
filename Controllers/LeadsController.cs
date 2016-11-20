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
    public class LeadsController : ApiController
    {
        // GET api/leads/
        // FEATURE: Listar leads
        public HttpResponseMessage Get([FromUri]string token)
        {
            if (Authentication.VerifyToken("?"))
            {
                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK, LeadIntegration.List(token));
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

        // GET api/leads/{$prospectId}/
        // FEATURE: Visualizar lead
        public HttpResponseMessage Get(string id, [FromUri]string token)
        {
            if (Authentication.VerifyToken("?"))
            {
                try
                {
                    var queryResult = LeadIntegration.View(token, id);

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

        // POST api/leads/
        // FEATURE: Adicionar lead
        public HttpResponseMessage Post([FromBody] Lead jsonObject)
        {
            if (Authentication.VerifyToken("?"))
            {
                try
                {
                    if (LeadIntegration.Insert(Thread.CurrentPrincipal.Identity.Name, jsonObject))
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

        // POST api/leads/{$prospectId}/
        // FEATURE: Modificar lead existente
        public HttpResponseMessage Post(string id, [FromBody] Lead jsonObject)
        {
            if (Authentication.VerifyToken("?"))
            {
                try
                {
                    if (LeadIntegration.Update(Thread.CurrentPrincipal.Identity.Name, id, jsonObject))
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

        // DELETE api/leads/{$prospectId}/
        // FEATURE: Remover lead existente
        public HttpResponseMessage Delete(string id)
        {
            if (Authentication.VerifyToken("?"))
            {
                try
                {
                    if (LeadIntegration.Delete(Thread.CurrentPrincipal.Identity.Name, id))
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