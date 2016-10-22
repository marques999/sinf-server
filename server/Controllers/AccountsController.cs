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
                return new SuccessResponse(AccountIntegration.listAccounts());
            }
            catch (Exception ex)
            {
                return new ErrorResponse(ex.Message);
            }
        }

        // GET api/accounts/?id={$accountId}
        // FEATURE: Visualizar cliente
        public ServerResponse Get([FromUri] string id)
        {
            try
            {
                return new SuccessResponse(AccountIntegration.getAccount(id));
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
                if (jsonString == null || jsonString.Length == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                var myInstance = JsonConvert.DeserializeObject<Account>(jsonString);

                if (myInstance == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                AccountIntegration.CreateAccount("accountId", myInstance);
            }
            catch (Exception ex)
            {
                return new ErrorResponse(ex.Message).sendResponse(Request);
            }

            return new SuccessResponse(true).sendResponse(Request);
        }

        // POST api/accounts/{$accountId}/
        // FEATURE: Modificar cliente existente
        public HttpResponseMessage Post(string accountId, [FromBody] string jsonString)
        {
            try
            {
                if (jsonString == null || jsonString.Length == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                var myInstance = JsonConvert.DeserializeObject<Account>(jsonString);

                if (myInstance == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                AccountIntegration.UpdateAccount(accountId, myInstance);
            }
            catch (Exception ex)
            {
                return new ErrorResponse(ex.Message).sendResponse(Request);
            }

            return new SuccessResponse(true).sendResponse(Request);
        }
    }
}