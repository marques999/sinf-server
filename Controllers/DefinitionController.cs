using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using FirstREST.LibPrimavera;
using FirstREST.LibPrimavera.Model;
using FirstREST.LibPrimavera.Integration;

namespace FirstREST.Controllers
{
    public class DefinitionsController : ApiController
    {
        public HttpResponseMessage Get([FromUri]string type2get)
        {
            if (Authentication.VerifyToken("?"))
            {
                try
                {
                    if (type2get.Equals("tipoterceirosLead"))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, DefinitionsIntegration.ListThirdPartyTypes());
                    }

                    if (type2get.Equals("paises"))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, DefinitionsIntegration.ListTypes(DefinitionType.Country));
                    }

                    if (type2get.Equals("titulos"))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, DefinitionsIntegration.ListTypes(DefinitionType.Title));
                    }

                    if (type2get.Equals("eventos"))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, DefinitionsIntegration.ListActivityTypes());
                    }

                    if (type2get.Equals("idiomas"))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, DefinitionsIntegration.ListTypes(DefinitionType.Language));
                    }

                    if (type2get.Equals("zonas"))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, DefinitionsIntegration.ListTypes(DefinitionType.Zone));
                    }

                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden);
            }
        }
    }
}