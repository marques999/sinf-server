using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using FirstREST.LibPrimavera;
using FirstREST.LibPrimavera.Model;
using FirstREST.LibPrimavera.Integration;

namespace FirstREST.Controllers
{
    public class LoginController : ApiController
    {
        // POST api/login/
        // FEATURE: Autenticar utilizador
        public HttpResponseMessage Post([FromBody]UserLogin jsonObject)
        {
            try
            {
                var representativeInfo = LoginIntegration.Authenticate(jsonObject);

                if (representativeInfo == null)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, representativeInfo);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        // DELETE api/login/{$sessionToken}/
        // FEATURE: Terminar sessão
        public HttpResponseMessage Post(string id)
        {
            if (Authentication.VerifyToken("?"))
            {
                try
                {
                    if (LoginIntegration.Logout(id))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.Forbidden);
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

        // PUT api/login/{$sessionToken}/
        // FEATURE: Alterar password
        public HttpResponseMessage Put(string id, [FromBody]UserPassword jsonObject)
        {
            if (Authentication.VerifyToken("?"))
            {
                try
                {
                    if (LoginIntegration.ChangePassword(id, jsonObject))
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