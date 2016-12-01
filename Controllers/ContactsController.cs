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
    public class ContactsController : ApiController
    {
        // GET api/contacts?token={$token}/
        // FEATURE: Listar contactos
        public HttpResponseMessage Get([FromUri] string token)
        {
            if (Authentication.VerifyToken(token))
            {
                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK, ContactIntegration.List(Authentication.GetRepresentative(token)));
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

        // GET api/contacts/{$id}?token={$token}/
        // FEATURE: Visualizar cliente
        public HttpResponseMessage Get(string id, [FromUri] string token)
        {
            if (Authentication.VerifyToken(token))
            {
                try
                {
                    return Request.CreateResponse(HttpStatusCode.OK, ContactIntegration.View(Authentication.GetRepresentative(token), HttpUtility.UrlDecode(id)));
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

        // POST api/contacts?token={$token}/
        // FEATURE: Adicionar contacto
        public HttpResponseMessage Post([FromBody] Contact jsonObject, [FromUri] string token)
        {
            if (Authentication.VerifyToken(token))
            {
                try
                {
                    var operationResult = ContactIntegration.Insert(Authentication.GetRepresentative(token), jsonObject);

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

        // PUT api/contacts/{$id}?token={$token}/
        // FEATURE: Modificar contacto existente
        public HttpResponseMessage Put(string id, [FromBody] Contact jsonObject, [FromUri] string token)
        {
            if (Authentication.VerifyToken(token))
            {
                var operationResult = ContactIntegration.Update(Authentication.GetRepresentative(token), HttpUtility.UrlDecode(id), jsonObject);

                try
                {
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

        // DELETE api/contacts/{$id}?token={$token}/
        // FEATURE: Remover contacto existente
        public HttpResponseMessage Delete(string id, [FromUri] string token)
        {
            if (Authentication.VerifyToken(token))
            {
                try
                {
                    if (ContactIntegration.Delete(Authentication.GetRepresentative(token), HttpUtility.UrlDecode(id)))
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