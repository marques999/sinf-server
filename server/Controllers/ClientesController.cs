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
    public class ClientesController : ApiController
    {
        public IEnumerable<Account> Get()
        {
            return PriIntegration.listAccounts();
        }

        public Account Get(string paramId)
        {
            try
            {
                var myAccount = PriIntegration.getAccount(paramId);

                if (myAccount != null)
                {
                    return myAccount;
                }

                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }
        }

        public HttpResponseMessage Post(Account myAccount)
        {
            var error = PriIntegration.createAccount(myAccount);

            if (error.Code != 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, error.Message);
            }

            var response = Request.CreateResponse(HttpStatusCode.Created, myAccount);
            var uri = Url.Link("DefaultApi", new { ID = myAccount.ID });

            response.Headers.Location = new Uri(uri);

            return response;
        }

        public HttpResponseMessage Put(Account myAccount)
        {
            try
            {
                var error = PriIntegration.updateAccount(myAccount);

                if (error.Code == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, error.Message);
                }

                return Request.CreateResponse(HttpStatusCode.NotFound, error.Message);
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        public HttpResponseMessage Delete(string paramId)
        {
            try
            {
                var error = PriIntegration.deleteAccount(paramId);

                if (error.Code == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, error.Message);
                }

                return Request.CreateResponse(HttpStatusCode.NotFound, error.Message);
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }
    }
}