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

        // GET api/leads/?id={$leadId}
        // FEATURE: Visualizar lead
        public ServerResponse Get([FromUri] string id)
        {
            try
            {
                var myInstance = LeadIntegration.GetLead(id);

                if (myInstance == null)
                {
                    return new ErrorResponse("notFound");
                }
                else
                {
                    return new SuccessResponse(myInstance);
                }
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

                var leadId = myInstance.Nome.Substring(0, 5).ToUpper();

                if (LeadIntegration.createLead(leadId, myInstance))
                {
                    return new SuccessResponse(true).sendResponse(Request);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                return new ErrorResponse(ex.Message).sendResponse(Request);
            }
        }

        // POST api/leads/{$paramId}
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

                if (LeadIntegration.updateLead(leadId, myInstance))
                {
                    return new SuccessResponse(true).sendResponse(Request);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
            }
            catch (Exception ex)
            {
                return new ErrorResponse(ex.Message).sendResponse(Request);
            }
        }
    }
}