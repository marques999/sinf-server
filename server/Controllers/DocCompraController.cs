using System;
using System.Collections.Generic;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using FirstREST.Lib_Primavera;
using FirstREST.Lib_Primavera.Model;

namespace FirstREST.Controllers
{
    public class DocCompraController : ApiController
    {
        public IEnumerable<DocCompra> Get()
        {
            return PriIntegration.VGR_List();
        }

        /*
        public Lib_Primavera.Model.DocCompra Get(string id)
        {
            var doccompra = Lib_Primavera.Comercial.GR_List(id);
            
            if (docvenda == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }
            else
            {
                return docvenda;
            }
        }*/

        public HttpResponseMessage Post(DocCompra dc)
        {
            var erro = PriIntegration.VGR_New(dc);

            if (erro.Code != 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var response = Request.CreateResponse(HttpStatusCode.Created, dc.id);
            var uri = Url.Link("DefaultApi", new { DocId = dc.id });

            response.Headers.Location = new Uri(uri);

            return response;
        }
    }
}