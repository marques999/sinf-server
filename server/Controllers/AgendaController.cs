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
        // GET api/agenda/
        public IEnumerable<string> Get(string when = "today")
        {
            return GetActivities(when);
        }

        // GET api/agenda/calls/
        public IEnumerable<string> Get(string type, string when = "today")
        {
            if (type == "tasks") return GetTasks(when);
            if (type == " meetings") return GetMeetings(when);
            if (type == "calls") return GetCalls(when);

            return GetActivities(when);
        }

        // GET api/agenda/calls/
        private IEnumerable<string> GetCalls(string activityId)
        {
            return new string[] { "value1", "value2" };
        }
       
        // GET api/agenda/all/
        private IEnumerable<string> GetActivities(string activityId)
        {
            return new string[] { "value1", "value2" };
        }
        
        // GET api/agenda/tasks/
        private IEnumerable<string> GetTasks(string activityId)
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/agenda/meetings
        private IEnumerable<string> GetMeetings(string activityId)
        {
            return new string[] { "value1", "value2" };
        }

        // POST api/agenda
        public void Post(
            [FromBody]string ownerId,
            [FromBody]int type, 
            [FromBody]int priority,
            [FromBody]string contactId,
            [FromBody]string opportunityId,
            [FromBody]DateTime start,
            [FromBody]DateTime end)
        {
        }
    }
}