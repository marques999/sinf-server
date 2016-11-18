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
    public class AgendaController : ApiController
    {
        // GET api/agenda/
        // Feature: Visualizar agenda
        public HttpResponseMessage Get()
        {
            if (PrimaveraEngine.IsAuthenticated())
            {
                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK, AgendaIntegration.List
                    (
                        Thread.CurrentPrincipal.Identity.Name,
                        ActivityType.ANY,
                        ActivityStatus.Any,
                        ActivityInterval.Today
                    ));
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

        // GET api/agenda/?type=calls
        // Feature: Visualizar agenda
        public HttpResponseMessage Get(string type)
        {
            if (PrimaveraEngine.IsAuthenticated())
            {
                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK, AgendaIntegration.List
                    (
                        Thread.CurrentPrincipal.Identity.Name,
                        TypeParser.Activity_Type(type),
                        ActivityStatus.Any,
                        ActivityInterval.Today
                    ));
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

        // GET api/agenda/?type=calls&when=today
        // Feature: Visualizar agenda
        public HttpResponseMessage Get(string type, string when)
        {
            if (PrimaveraEngine.IsAuthenticated())
            {
                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK, AgendaIntegration.List
                    (
                        Thread.CurrentPrincipal.Identity.Name,
                        TypeParser.Activity_Type(type),
                        ActivityStatus.Any,
                        TypeParser.Activity_Interval(when)
                    ));
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

        // GET api/agenda/?type=calls&when=today&status=ongoing
        // Feature: Visualizar agenda
        public HttpResponseMessage Get([FromUri] string type, [FromUri] string when, [FromUri] string status)
        {
            if (PrimaveraEngine.IsAuthenticated())
            {
                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK, AgendaIntegration.List
                    (
                        Thread.CurrentPrincipal.Identity.Name,
                        TypeParser.Activity_Type(type),
                        TypeParser.Activity_Status(status),
                        TypeParser.Activity_Interval(when)
                    ));
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

        // POST api/agenda/
        // Feature: Agendar actividade
        public HttpResponseMessage Post([FromBody] Activity jsonObject)
        {
            if (PrimaveraEngine.IsAuthenticated())
            {
                try
                {
                    jsonObject.Identificador = "activityId";
                    jsonObject.CriadoEm = DateTime.Now;
                    jsonObject.Status = ActivityStatus.Pendente;
                    jsonObject.ModificadoEm = jsonObject.CriadoEm;

                    if (AgendaIntegration.Insert(Thread.CurrentPrincipal.Identity.Name, jsonObject))
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

        // POST api/agenda/{$activityId}/
        // Feature: Modificar actividade existente
        public HttpResponseMessage Post(string id, [FromBody] Activity jsonObject)
        {
            if (PrimaveraEngine.IsAuthenticated())
            {
                try
                {
                    jsonObject.Identificador = id;
                    jsonObject.ModificadoEm = DateTime.Now;

                    if (AgendaIntegration.Update(Thread.CurrentPrincipal.Identity.Name, jsonObject))
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

        // DELETE api/agenda/{$activityId}/
        // FEATURE: Remover actividade existente
        public HttpResponseMessage Delete(string id)
        {
            if (PrimaveraEngine.IsAuthenticated())
            {
                try
                {
                    if (AgendaIntegration.Delete(Thread.CurrentPrincipal.Identity.Name, id))
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