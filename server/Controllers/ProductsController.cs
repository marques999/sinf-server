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
                return new SuccessResponse(ProductIntegration.listProducts());
            }
            catch (Exception ex)
            {
                return new ErrorResponse(ex.Message);
            }
        }

        // GET api/products/?id={$productId}
        // FEATURE: Visualizar produto
        public ServerResponse Get([FromUri] string id)
        {
            try
            {
                return new SuccessResponse(ProductIntegration.getProduct(id));
            }
            catch (Exception ex)
            {
                return new ErrorResponse(ex.Message);
            }
        }
    }
}