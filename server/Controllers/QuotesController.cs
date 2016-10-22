using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Newtonsoft.Json;

using FirstREST.Lib_Primavera.Model;
using FirstREST.Lib_Primavera.Integration;

namespace FirstREST.Controllers
{
    public class QuotesController : ApiController
    {
        // GET api/quotes/
        // FEATURE: Listar encomendas
        public ServerResponse Get()
        {
            try
            {
                return new SuccessResponse(QuoteIntegration.GetQuotes());
            }
            catch (Exception ex)
            {
                return new ErrorResponse(ex.Message);
            }
        }

        // GET api/quotes/?id={$quoteId}
        // FEATURE: Visualizar encomenda
        public ServerResponse Get([FromUri] string id)
        {
            try
            {
                return new SuccessResponse(QuoteIntegration.GetQuote(id));
            }
            catch (Exception ex)
            {
                return new ErrorResponse(ex.Message);
            }
        }

        // POST api/quotes/
        // FEATURE: Adicionar encomenda
        public HttpResponseMessage Post([FromBody] string jsonString)
        {
            try
            {
                if (jsonString == null || jsonString.Length == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                var myInstance = JsonConvert.DeserializeObject<Quote>(jsonString);

                if (myInstance == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                QuoteIntegration.CreateQuote("quoteId", myInstance);
            }
            catch (Exception ex)
            {
                return new ErrorResponse(ex.Message).sendResponse(Request);
            }

            return new SuccessResponse(true).sendResponse(Request);
        }

        // POST api/quotes/{$quoteId}/
        // FEATURE: Modificar encomenda existente
        public HttpResponseMessage Post(string accountId, [FromBody] string jsonString)
        {
            try
            {
                if (jsonString == null || jsonString.Length == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                var myInstance = JsonConvert.DeserializeObject<Quote>(jsonString);

                if (myInstance == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                QuoteIntegration.UpdateQuote(accountId, myInstance);
            }
            catch (Exception ex)
            {
                return new ErrorResponse(ex.Message).sendResponse(Request);
            }

            return new SuccessResponse(true).sendResponse(Request);
        }
    }
}