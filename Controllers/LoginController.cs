using System;
using System.Net;
using System.Net.Http;
using System.Web;
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
                var operationRresult = LoginIntegration.Authenticate(jsonObject);

                if (operationRresult == null)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, operationRresult);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        // PUT api/login/{$id}?token={$token}/
        // FEATURE: Actualizar vendedor
        public HttpResponseMessage Put(string id, [FromBody]UserInfo jsonObject, [FromUri] string token)
        {
            if (Authentication.VerifyToken(token))
            {
                try
                {
                    var operationResult = LoginIntegration.Update(HttpUtility.UrlDecode(id), jsonObject);

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

        // DELETE api/login/{$id}?token={$token}/
        // FEATURE: Terminar sessão
        public HttpResponseMessage Delete(string id, [FromUri] string token)
        {
            if (Authentication.VerifyToken(token))
            {
                try
                {
                    if (LoginIntegration.Logout(HttpUtility.UrlDecode(id)))
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
    }
}