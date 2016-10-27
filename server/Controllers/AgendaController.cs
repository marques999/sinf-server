using FirstREST.Lib_Primavera.Integration;
using FirstREST.Lib_Primavera.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

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

        // GET api/agenda/?type=calls&when=today&status=ongoing
        // Feature: Visualizar agenda
        public ServerResponse Get([FromUri] string type = "all", [FromUri] string when = "today", [FromUri] string status = "any")
        {
            try
            {
                return new SuccessResponse(AgendaIntegration.Get(ParseType(type), ParseStatus(status), ParseWhen(when)));
            }
            catch (Exception ex)
            {
                return new ErrorResponse(ex.Message);
            }
        }

        // POST api/agenda/
        // Feature: Agendar actividade
        public HttpResponseMessage Post([FromBody] string jsonString)
        {
            try
            {
                var myActivity = JsonConvert.DeserializeObject<Activity>(jsonString);

                if (myActivity == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST /api/agenda/3
        // Feature: Modificar actividade
        public HttpResponseMessage Post(int id, [FromBody] string jsonString)
        {
            try
            {
                var myActivity = JsonConvert.DeserializeObject<Activity>(jsonString);

                if (myActivity == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}