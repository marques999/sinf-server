using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using System.Threading;

using FirstREST.LibPrimavera;
using FirstREST.LibPrimavera.Model;
using FirstREST.LibPrimavera.Integration;

namespace FirstREST.Controllers
{
    public class UsersController : ApiController
    {
        // GET api/users/
        // FEATURE: Listar vendedores
        public HttpResponseMessage Get()
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, UserIntegration.List());
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        // GET api/users/{$userId}/
        // FEATURE: Visualizar perfil
        public HttpResponseMessage Get(string id)
        {
            if (Authentication.VerifyToken("?"))
            {
                try
                {
                    var queryResult = UserIntegration.View(Thread.CurrentPrincipal.Identity.Name);

                    if (queryResult == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, queryResult);
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

        // POST api/users/
        // FEATURE: Registar vendedor
        public HttpResponseMessage Post([FromBody]UserInfo jsonObject)
        {
            try
            {
                if (UserIntegration.Insert(jsonObject))
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
    }
}