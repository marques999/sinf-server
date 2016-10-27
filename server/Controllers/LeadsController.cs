using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Newtonsoft.Json;

using FirstREST.Lib_Primavera.Model;
using FirstREST.Lib_Primavera.Integration;

namespace FirstREST.Controllers
{
    public class LeadsController : ApiController
    {
        // GET api/leads/
        // FEATURE: Listar leads
        public ServerResponse Get()
        {
            try
            {
                return new SuccessResponse(LeadIntegration.GetLeads());
            }
            catch (Exception ex)
            {
                return new ErrorResponse(ex.Message);
            }
        }

        // GET api/leads/{$leadId}/
        // FEATURE: Visualizar lead
        public ServerResponse Get(string id)
        {
            try
            {
                return new SuccessResponse(LeadIntegration.GetLead(id));
            }
            catch (Exception ex)
            {
                return new ErrorResponse(ex.Message);
            }
        }

        // POST api/leads/
        // FEATURE: Adicionar lead
        public HttpResponseMessage Post([FromBody] string jsonString)
        {
            try
            {
                if (jsonString == null || jsonString.Length == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                var myInstance = JsonConvert.DeserializeObject<Lead>(jsonString);

                if (myInstance == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                LeadIntegration.CreateLead("leadId", myInstance);
            }
            catch (Exception ex)
            {
                return new ErrorResponse(ex.Message).sendResponse(Request);
            }

            return new SuccessResponse(true).sendResponse(Request);
        }

        // POST api/leads/{$paramId}/
        // FEATURE: Modificar lead existente
        public HttpResponseMessage Post(string leadId, [FromBody] string jsonString)
        {
            try
            {
                if (jsonString == null || jsonString.Length == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                var myInstance = JsonConvert.DeserializeObject<Lead>(jsonString);

                if (myInstance == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                LeadIntegration.UpdateLead(leadId, myInstance);
            }
            catch (Exception ex)
            {
                return new ErrorResponse(ex.Message).sendResponse(Request);
            }

            return new SuccessResponse(true).sendResponse(Request);
        }
    }
}