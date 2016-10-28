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
                if (JsonFormatter.ValidateJson(jsonString) == false)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                var myInstance = JsonConvert.DeserializeObject<Opportunity>(jsonString);

                if (myInstance == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                if (OpportunityIntegration.CreateOpportunity("opportunityId", myInstance))
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

        // POST api/opportunities/{$opportunityId}/
        // FEATURE: Modificar oportunidade existente
        public HttpResponseMessage Post(string opportunityId, [FromBody]string jsonString)
        {
            try
            {
                if (JsonFormatter.ValidateJson(jsonString) == false)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                var myInstance = JsonConvert.DeserializeObject<Opportunity>(jsonString);

                if (myInstance == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                if (OpportunityIntegration.UpdateOpportunity(opportunityId, myInstance))
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