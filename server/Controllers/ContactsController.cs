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
                if (JsonFormatter.ValidateJson(jsonString) == false)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                var myInstance = JsonConvert.DeserializeObject<Account>(jsonString);

                if (myInstance == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                if (ContactIntegration.CreateContact("contactId", myInstance))
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

        // POST api/contacts/{$contactId}/
        // FEATURE: Modificar contacto existente
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

                if (ContactIntegration.UpdateContact(paramId, myInstance))
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