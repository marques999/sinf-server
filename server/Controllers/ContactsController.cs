using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Newtonsoft.Json;

using FirstREST.Lib_Primavera.Model;
using FirstREST.Lib_Primavera.Integration;

namespace FirstREST.Controllers
{
    public class ContactsController : ApiController
    {
        // GET api/contacts/
        // FEATURE: Listar contactos
        public ServerResponse Get()
        {
            try
            {
                return new SuccessResponse(ContactIntegration.GetContacts());
            }
            catch (Exception ex)
            {
                return new ErrorResponse(ex.Message);
            }
        }

        // GET api/contacts/{$contactId}/
        // FEATURE: Visualizar cliente
        public ServerResponse Get(string id)
        {
            try
            {
                return new SuccessResponse(ContactIntegration.GetContact(id));
            }
            catch (Exception ex)
            {
                return new ErrorResponse(ex.Message);
            }
        }

        // POST api/contacts/
        // FEATURE: Adicionar contacto
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

                ContactIntegration.CreateContact("contactId", myInstance);
            }
            catch (Exception ex)
            {
                return new ErrorResponse(ex.Message).sendResponse(Request);
            }

            return new SuccessResponse(true).sendResponse(Request);
        }

        // POST api/contacts/{$contactId}/
        // FEATURE: Modificar contacto existente
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

                ContactIntegration.UpdateContact(accountId, myInstance);
            }
            catch (Exception ex)
            {
                return new ErrorResponse(ex.Message).sendResponse(Request);
            }

            return new SuccessResponse(true).sendResponse(Request);
        }
    }
}