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
                if (JsonFormatter.ValidateJson(jsonString) == false)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                var myInstance = JsonConvert.DeserializeObject<Lead>(jsonString);

                if (myInstance == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                if (LeadIntegration.CreateLead("leadId", myInstance))
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
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        // POST api/leads/{$paramId}/
        // FEATURE: Modificar lead existente
        public HttpResponseMessage Post(string paramId, [FromBody] string jsonString)
        {
            try
            {
                if (JsonFormatter.ValidateJson(jsonString) == false)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                var myInstance = JsonConvert.DeserializeObject<Lead>(jsonString);

                if (myInstance == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                if (LeadIntegration.UpdateLead(paramId, myInstance))
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
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}