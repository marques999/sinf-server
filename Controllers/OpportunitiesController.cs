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
    public class OpportunitiesController : ApiController
    {
        // GET api/opportunities/
        // FEATURE: Listar oportunidades
        public HttpResponseMessage Get()
        {
            if (PrimaveraEngine.IsAuthenticated())
            {
                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK, OpportunityIntegration.List(Thread.CurrentPrincipal.Identity.Name));
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

        // GET api/opportunities/{$opportunityId}/
        // FEATURE: Visualizar oportunidade
        public HttpResponseMessage Get(string id)
        {
            if (PrimaveraEngine.IsAuthenticated())
            {
                try
                {
                    var queryResult = OpportunityIntegration.View(Thread.CurrentPrincipal.Identity.Name, id);

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

        // POST api/opportunities/
        // FEATURE: Adicionar oportunidade
        public HttpResponseMessage Post([FromBody] Opportunity jsonObject)
        {
            if (PrimaveraEngine.IsAuthenticated())
            {
                try
                {
                    jsonObject.Identifier = "opportunityId";
                    jsonObject.DateCreated = DateTime.Now;
                    jsonObject.DateModified = jsonObject.DateCreated;

                    if (OpportunityIntegration.Insert(Thread.CurrentPrincipal.Identity.Name, jsonObject))
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

        // POST api/opportunities/{$opportunityId}/
        // FEATURE: Modificar oportunidade existente
        public HttpResponseMessage Post(string id, [FromBody] Opportunity jsonObject)
        {
            if (PrimaveraEngine.IsAuthenticated())
            {
                try
                {
                    jsonObject.DateModified = DateTime.Now;

                    if (OpportunityIntegration.Update(Thread.CurrentPrincipal.Identity.Name, id, jsonObject))
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

        // DELETE api/opportunities/{$opportunityId}/
        // FEATURE: Remover oportunidade
        public HttpResponseMessage Delete(string id)
        {
            if (PrimaveraEngine.IsAuthenticated())
            {
                try
                {
                    if (OpportunityIntegration.Delete(Thread.CurrentPrincipal.Identity.Name, id))
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