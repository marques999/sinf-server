using FirstREST.LibPrimavera;
using FirstREST.LibPrimavera.Integration;
using FirstREST.LibPrimavera.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace FirstREST.Controllers
{
    public class ProposalLineController : ApiController
    {
        // GET: api/ProposalLine/{$opportunityID}/{$proposalNumber}/{$lineNumber}
        public HttpResponseMessage Get(string id, short sid, short tid)
        {
            if (Authentication.VerifyToken("?"))
            {
                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK, ProposalLinesIntegration.View(id, sid, tid));
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

        // GET: api/ProposalLine/{$opportunityID}/{$proposalNumber}
        public HttpResponseMessage Get(string id, short sid)
        {

            if (Authentication.VerifyToken("?"))
            {
                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK, ProposalLinesIntegration.List(id, sid));
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

        // POST: api/ProposalLine
        public HttpResponseMessage Post([FromBody] ProposalsLine jsonObject)
        {

            if (Authentication.VerifyToken("?"))
            {
                try
                {
                    var operationResult = ProposalLinesIntegration.Insert(Authentication.GetRepresentative(null), jsonObject);

                    if (operationResult == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, operationResult);
                    }
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

        // PUT: api/ProposalLine/{$opportunityID}
        public HttpResponseMessage Put(string id, [FromBody] ProposalsLine jsonObject)
        {
            if (Authentication.VerifyToken("?"))
            {
                try
                {
                    var operationResult = ProposalLinesIntegration.Update(Authentication.GetRepresentative(null), HttpUtility.UrlDecode(id), jsonObject);

                    if (operationResult == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, operationResult);
                    }
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

        // DELETE: api/ProposalLine/{$opportunityID}
        public HttpResponseMessage Delete(string id, [FromBody] ProposalsLine jsonObject)
        {
            if (Authentication.VerifyToken("?"))
            {
                try
                {
                    if (ProposalLinesIntegration.Delete(Authentication.GetRepresentative(null), HttpUtility.UrlDecode(id), jsonObject))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound);
                    }
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