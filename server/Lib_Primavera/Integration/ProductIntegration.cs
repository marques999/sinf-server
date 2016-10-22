using System;
using System.Collections.Generic;

using FirstREST.Lib_Primavera.Model;

namespace FirstREST.Lib_Primavera.Integration
{
    public class ProductIntegration
    {
        public static Product getProduct(string productId)
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            if (PriEngine.Produtos.Existe(productId) == false)
            {
                throw new NotFoundException();
            }

            var queryResult = PriEngine.Produtos.Edita(productId);

            return new Product
            {
                Identifier = queryResult.get_Artigo(),
                Name = queryResult.get_Descricao()
            };
        }

        public static List<Product> listProducts()
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var productList = new List<Product>();
            var queryResult = PriEngine.Produtos.LstArtigos();

            while (!queryResult.NoFim())
            {
                productList.Add(new Product
                {
                    Identifier = queryResult.Valor("artigo"),
                    Name = queryResult.Valor("descricao")
                });

                queryResult.Seguinte();
            }

            return productList;
        }
    }
}