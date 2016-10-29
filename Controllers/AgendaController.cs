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
        public static Agenda ParseWhen(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return Agenda.Today;
            }

            Agenda parseResult;

            return Enum.TryParse(value, true, out parseResult) ? parseResult : Agenda.Today;
        }

        public static AgendaType ParseType(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return AgendaType.All;
            }

            AgendaType parseResult;

            if (Enum.TryParse(value, true, out parseResult))
            {
                return parseResult;
            }

            return AgendaType.All;
        }

        public static AgendaStatus ParseStatus(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return AgendaStatus.Any;
            }

            AgendaStatus parseResult;

            if (Enum.TryParse(value, true, out parseResult))
            {
                return parseResult;
            }

            return AgendaStatus.Any;
        }

        [Authorize]
        public HttpResponseMessage Get()
        {
            if (PrimaveraEngine.IsAuthenticated())
            {
                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK, AgendaIntegration.Get(AgendaType.All, AgendaStatus.Any, Agenda.Today));
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

        [Authorize]
        public HttpResponseMessage Get(string type)
        {
            if (PrimaveraEngine.IsAuthenticated())
            {
                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK, AgendaIntegration.Get(ParseType(type), AgendaStatus.Any, Agenda.Today));
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

        [Authorize]
        public HttpResponseMessage Get(string type, string when)
        {
            if (PrimaveraEngine.IsAuthenticated())
            {
                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK, AgendaIntegration.Get(ParseType(type), AgendaStatus.Any, ParseWhen(when)));
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

        // GET api/agenda/?type=calls&when=today&status=ongoing
        // Feature: Visualizar agenda
        [Authorize]
        public HttpResponseMessage Get(string type, string when, string status)
        {
            if (PrimaveraEngine.IsAuthenticated())
            {
                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK, AgendaIntegration.Get(ParseType(type), ParseStatus(status), ParseWhen(when)));
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

        // POST api/agenda/
        // Feature: Agendar actividade
        [Authorize]
        public HttpResponseMessage Post([FromBody] Activity jsonObject)
        {
            if (PrimaveraEngine.IsAuthenticated())
            {
                try
                {
                    jsonObject.Identifier = "activityId";
                    jsonObject.DateCreated = DateTime.Now;
                    jsonObject.Status = AgendaStatus.Ongoing;
                    jsonObject.DateModified = jsonObject.DateCreated;

                    if (AgendaIntegration.Insert(Thread.CurrentPrincipal.Identity.Name, jsonObject))
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

        // POST api/agenda/{$activityId}/
        // Feature: Modificar actividade existente
        [Authorize]
        public HttpResponseMessage Post(string id, [FromBody] Activity jsonObject)
        {
            if (PrimaveraEngine.IsAuthenticated())
            {
                try
                {
                    jsonObject.Identifier = id;
                    jsonObject.DateModified = DateTime.Now;

                    if (AgendaIntegration.Update(Thread.CurrentPrincipal.Identity.Name, jsonObject))
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

        // DELETE api/agenda/{$activityId}/
        // FEATURE: Remover actividade existente
        [Authorize]
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