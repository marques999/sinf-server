using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using FirstREST.Lib_Primavera;
using FirstREST.Lib_Primavera.Model;

namespace FirstREST.Controllers
{
    public class DocVendaController : ApiController
    {
        public IEnumerable<DocVenda> Get()
        {
            return PriIntegration.Encomendas_List();
        }

        public DocVenda Get(string id)
        {
            var docvenda = PriIntegration.Encomenda_Get(id);

            if (docvenda == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return docvenda;
        }

        public HttpResponseMessage Post(DocVenda dv)
        {
            var erro = PriIntegration.Encomendas_New(dv);

            if (erro.Code != 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var response = Request.CreateResponse(HttpStatusCode.Created, dv.id);
            var uri = Url.Link("DefaultApi", new { DocId = dv.id });

            response.Headers.Location = new Uri(uri);

            return response;
        }

        public HttpResponseMessage Put(int id, Account cliente)
        {
            try
            {
                var erro = PriIntegration.updateAccount(cliente);

                if (erro.Code == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, erro.Message);
                }

                return Request.CreateResponse(HttpStatusCode.NotFound, erro.Message);
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        public HttpResponseMessage Delete(string id)
        {
            try
            {
                var erro = Lib_Primavera.PriIntegration.deleteAccount(id);

                if (erro.Code == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, erro.Message);
                }

                return Request.CreateResponse(HttpStatusCode.NotFound, erro.Message);
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }
    }
}