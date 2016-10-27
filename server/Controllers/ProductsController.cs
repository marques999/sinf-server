using System;
using System.Web.Http;

using FirstREST.Lib_Primavera.Model;
using FirstREST.Lib_Primavera.Integration;

namespace FirstREST.Controllers
{
    public class ProductsController : ApiController
    {
        // GET api/products/
        // FEATURE: Listar produtos
        public ServerResponse Get()
        {
            try
            {
                return new SuccessResponse(ProductIntegration.Get());
            }
            catch (Exception ex)
            {
                return new ErrorResponse(ex.Message);
            }
        }

        // GET api/products/{$productId}/
        // FEATURE: Visualizar produto
        public ServerResponse Get(string id)
        {
            try
            {
                return new SuccessResponse(ProductIntegration.Get(id));
            }
            catch (Exception ex)
            {
                return new ErrorResponse(ex.Message);
            }
        }
    }
}