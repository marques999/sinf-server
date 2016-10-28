using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using FirstREST.Lib_Primavera.Integration;
using FirstREST.Lib_Primavera.Model;

namespace FirstREST.Controllers
{
    public class CategoriesController : ApiController
    {
        // GET api/categories/
        // FEATURE: Listar categorias
        public ServerResponse Get()
        {
            try
            {
                return new SuccessResponse(CategoryIntegration.Get());
            }
            catch (Exception ex)
            {
                return new ErrorResponse(ex.Message);
            }
        }

        // GET api/categories/{$categoryId}/
        // FEATURE: Visualizar categoria
        public ServerResponse Get(string id)
        {
            try
            {
                return new SuccessResponse(ProductIntegration.GetByCategory(id));
            }
            catch (Exception ex)
            {
                return new ErrorResponse(ex.Message);
            }
        }
    }
}