using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading;

using FirstREST.LibPrimavera;
using FirstREST.LibPrimavera.Model;
using FirstREST.LibPrimavera.Integration;

namespace FirstREST.Controllers
{
    public class ContactsController : ApiController
    {
        // GET api/contacts/
        // FEATURE: Listar contactos
        public HttpResponseMessage Get()
        {
            if (Authentication.VerifyToken("?"))
            {
                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK, ContactIntegration.List(Thread.CurrentPrincipal.Identity.Name));
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

        // GET api/contacts/{$contactId}
        // FEATURE: Visualizar cliente
        public HttpResponseMessage Get(string id)
        {
            if (Authentication.VerifyToken("?"))
            {
                try
                {
                    var queryResult = ContactIntegration.View(Thread.CurrentPrincipal.Identity.Name, id);

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

        // POST api/contacts/
        // FEATURE: Adicionar contacto
        public HttpResponseMessage Post([FromBody] Contact jsonObject)
        {
            if (Authentication.VerifyToken("?"))
            {
                try
                {
                    if (ContactIntegration.Insert(Thread.CurrentPrincipal.Identity.Name, jsonObject))
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

        // POST api/contacts/{$contactId}/
        // FEATURE: Modificar contacto existente
        public HttpResponseMessage Post(string id, [FromBody] Contact jsonObject)
        {
            if (Authentication.VerifyToken("?"))
            {
                try
                {
                    if (ContactIntegration.Update(Thread.CurrentPrincipal.Identity.Name, id, jsonObject))
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

        // DELETE api/contacts/{$contactId}/
        // FEATURE: Remover contacto existente
        public HttpResponseMessage Delete(string id)
        {
            if (Authentication.VerifyToken("?"))
            {
                try
                {
                    if (ContactIntegration.Delete(Thread.CurrentPrincipal.Identity.Name, id))
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