using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Newtonsoft.Json;

using FirstREST.Lib_Primavera.Model;
using FirstREST.Lib_Primavera.Integration;

namespace FirstREST.Controllers
{
    public class OpportunitiesController : ApiController
    {
        // GET api/opportunities/
        // FEATURE: Listar oportunidades
        public ServerResponse Get()
        {
            try
            {
                return new SuccessResponse(OpportunityIntegration.GetOpportunities());
            }
            catch (Exception ex)
            {
                return new ErrorResponse(ex.Message);
            }
        }

        // GET api/opportunities/{$opportuniyId}/
        // FEATURE: Visualizar oportunidade
        public ServerResponse Get(string id)
        {
            try
            {
                return new SuccessResponse(OpportunityIntegration.GetOpportunity(id));
            }
            catch (Exception ex)
            {
                return new ErrorResponse(ex.Message);
            }
        }

        // POST api/opportunities/
        // FEATURE: Adicionar oportunidade
        public HttpResponseMessage Post([FromBody]string jsonString)
        {
            try
            {
                if (jsonString == null || jsonString.Length == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                var myInstance = JsonConvert.DeserializeObject<Opportunity>(jsonString);

                if (myInstance == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                OpportunityIntegration.CreateOpportunity("opportunityId", myInstance);
            }
            catch (Exception ex)
            {
                return new ErrorResponse(ex.Message).sendResponse(Request);
            }

            return new SuccessResponse(true).sendResponse(Request);
        }

        // POST api/opportunities/{$opportunityId}/
        // FEATURE: Modificar oportunidade existente
        public HttpResponseMessage Post(string opportunityId, [FromBody]string jsonString)
        {
            try
            {
                if (jsonString == null || jsonString.Length == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                var myInstance = JsonConvert.DeserializeObject<Opportunity>(jsonString);

                if (myInstance == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                OpportunityIntegration.UpdateOpportunity(opportunityId, myInstance);
            }
            catch (Exception ex)
            {
                return new ErrorResponse(ex.Message).sendResponse(Request);
            }

            return new SuccessResponse(true).sendResponse(Request);
        }
    }
}