using FirstREST.Lib_Primavera.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FirstREST.Controllers
{
    public class AgendaController : ApiController
    {
        // GET api/agenda/calls/
        public IEnumerable<string> Get(string when, string type)
        {
            if (when != null) Console.WriteLine(when);

            switch (type)
            {
                case "tasks":
                    return GetTasks(when);
                case " meetings":
                    return GetMeetings(when);
                case "calls":
                    return GetCalls(when);
            }

            return GetActivities(when);
        }

        // GET api/agenda/
        public IEnumerable<string> Get(string when)
        {
            return GetActivities(when);
        }

        // GET api/agenda/calls/
        // FEATURE: Histórico de chamadas
        private IEnumerable<string> GetCalls(string activityId)
        {
            return new string[] { "value1", activityId };
        }

        // GET api/agenda/all/
        // FEATURE: Histórico de actividades
        private IEnumerable<string> GetActivities(string activityId)
        {
            return new string[] { "value1", activityId };
        }

        // GET api/agenda/tasks/
        // FEATURE: Histórico de tarefas
        private IEnumerable<string> GetTasks(string activityId)
        {
            return new string[] { "value1", activityId };
        }

        // GET api/agenda/meetings
        // FEATURE: Histórico de reuniões
        private IEnumerable<string> GetMeetings(string activityId)
        {
            return new string[] { "value1", activityId };
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