using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Collections.Generic;

using FirstREST.LibPrimavera;
using FirstREST.LibPrimavera.Model;
using FirstREST.LibPrimavera.Integration;

namespace FirstREST.Controllers
{
    public class DefinitionsController : ApiController
    {
        private static Dictionary<string, DefinitionType> definitionMapping = new Dictionary<string, DefinitionType>
        {
            { "paises", DefinitionType.Country },
            { "titulos", DefinitionType.Title },
            { "idiomas", DefinitionType.Language },
            { "zonas", DefinitionType.Zone }
        };

        public HttpResponseMessage Get([FromUri]string type2get)
        {
            try
            {
                if (type2get.Equals("tipoterceirosLead"))
                {
                    return Request.CreateResponse(HttpStatusCode.OK, DefinitionsIntegration.ListThirdPartyTypes());
                }
                else if (type2get.Equals("eventos"))
                {
                    return Request.CreateResponse(HttpStatusCode.OK, DefinitionsIntegration.ListActivityTypes());
                }
                else if (type2get.Equals("campanhas"))
                {
                    return Request.CreateResponse(HttpStatusCode.OK, DefinitionsIntegration.ListCampaigns());
                }
                else if (definitionMapping.ContainsKey(type2get))
                {
                    return Request.CreateResponse(HttpStatusCode.OK, DefinitionsIntegration.ListTypes(definitionMapping[type2get]));
                }
                else
                {
                    throw new NotFoundException("tipo", false);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
    }
}