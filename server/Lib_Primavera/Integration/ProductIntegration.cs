using FirstREST.Lib_Primavera.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FirstREST.Lib_Primavera.Integration
{
    public class ProductIntegration
    {
        public static Product getProduct(string productId)
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                return null;
            }

            if (PriEngine.Engine.Comercial.Artigos.Existe(productId) == false)
            {
                return null;
            }

            var queryResult = PriEngine.Engine.Comercial.Artigos.Edita(productId);

            return new Product
            {
                ID = queryResult.get_Artigo(),
                Nome = queryResult.get_Descricao()
            };
        }

        public static List<Product> listProducts()
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                return null;
            }

            var productList = new List<Product>();
            var queryResult = PriEngine.Engine.Comercial.Artigos.LstArtigos();

            while (!queryResult.NoFim())
            {
                productList.Add(new Product
                {
                    ID = queryResult.Valor("artigo"),
                    Nome = queryResult.Valor("descricao")
                });
                
                queryResult.Seguinte();
            }

            return productList;
        }
    }
}