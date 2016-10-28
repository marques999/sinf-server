using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Newtonsoft.Json;

using FirstREST.Lib_Primavera.Model;
using FirstREST.Lib_Primavera.Integration;

namespace FirstREST.Controllers
{
    public class AccountsController : ApiController
    {
        // GET api/accounts/
        // FEATURE: Listar clientes
        public ServerResponse Get()
        {
            try
            {
                return new SuccessResponse(AccountIntegration.GetAccounts());
            }
            catch (Exception ex)
            {
                return new ErrorResponse(ex.Message);
            }
        }

        // GET api/accounts/{$accountId}/
        // FEATURE: Visualizar cliente
        public ServerResponse Get(string id)
        {
            try
            {
                return new SuccessResponse(AccountIntegration.GetAccount(id));
            }
            catch (Exception ex)
            {
                return new ErrorResponse(ex.Message);
            }
        }

        // POST api/accounts/
        // FEATURE: Adicionar cliente
        public HttpResponseMessage Post([FromBody] string jsonString)
        {
            try
            {
                if (JsonFormatter.ValidateJson(jsonString) == false)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                var myInstance = JsonConvert.DeserializeObject<Account>(jsonString);

                if (myInstance == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                if (AccountIntegration.CreateAccount("accountId", myInstance))
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

        // POST api/accounts/{$accountId}/
        // FEATURE: Modificar cliente existente
        public HttpResponseMessage Post(string paramId, [FromBody] string jsonString)
        {
            try
            {
                if (JsonFormatter.ValidateJson(jsonString) == false)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                var myInstance = JsonConvert.DeserializeObject<Account>(jsonString);

                if (myInstance == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                if (AccountIntegration.UpdateAccount(paramId, myInstance))
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