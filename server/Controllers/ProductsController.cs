using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Collections.Generic;

using FirstREST.Lib_Primavera;
using FirstREST.Lib_Primavera.Model;

namespace FirstREST.Controllers
{
    public class ProductsController : ApiController
    {
        public IEnumerable<Product> Get()
        {
            return PriIntegration.listProducts();
        }

        public Product Get(string id)
        {
            var product = PriIntegration.getProduct(id);

            if (product != null)
            {
                return product;
            }

            throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
        }
    }
}