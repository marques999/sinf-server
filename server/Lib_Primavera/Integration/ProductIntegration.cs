using System;
using System.Collections.Generic;

using Interop.StdBE900;

using FirstREST.Lib_Primavera;
using FirstREST.Lib_Primavera.Model;
using FirstREST.Lib_Primavera.Enums;

namespace FirstREST.Lib_Primavera.Integration
{
    public class ProductIntegration
    {
        private static SqlColumn[] sqlColumns =
        {
            new SqlColumn("ARTIGO.Artigo", null),
            new SqlColumn("ARTIGO.Descricao", null),
            new SqlColumn("ARTIGO.PCMedio", null),
            new SqlColumn("ARTIGO.Desconto", null),
            new SqlColumn("ARTIGO.Iva", null),
            new SqlColumn("FAMILIAS.Descricao", "Familia"),
            new SqlColumn("ARTIGO.STKActual", "Stock")
        };

        private static Product Generate(StdBELista queryResult)
        {
            string productId = queryResult.Valor("Artigo");

            return new Product()
            {
                Identifier = productId,
                Name = queryResult.Valor("Descricao"),
                Price = queryResult.Valor("PCMedio"),
                DiscountValue = queryResult.Valor("Desconto"),
                Tax = Convert.ToDouble(queryResult.Valor("Iva")),
                Category = queryResult.Valor("Familia"),
                Stock = queryResult.Valor("Stock"),
                Warehouses = WarehouseIntegration.GetWarehouses(productId)
            };
        }

        public static Product Get(string productId)
        {
            if (PriEngine.Produtos.Existe(productId) == false)
            {
                throw new NotFoundException();
            }

            return Generate(PriEngine.Consulta(new QueryBuilder()
                .FromTable("ARTIGO")
                .Columns(sqlColumns)
                .Where("ARTIGO.Artigo", Comparison.Equals, productId)
                .LeftJoin("FAMILIAS", "Familia", Comparison.Equals, "ARTIGO", "Familia")));
        }

        public static List<Product> Get()
        {
            var queryResult = new List<Product>();
            var queryObject = PriEngine.Consulta(new QueryBuilder()
                .FromTable("ARTIGO")
                .Columns(sqlColumns)
                .LeftJoin("FAMILIAS", "Familia", Comparison.Equals, "ARTIGO", "Familia"));

            while (!queryObject.NoFim())
            {
                queryResult.Add(Generate(queryObject));
                queryObject.Seguinte();
            }

            queryResult.Sort(delegate(Product lhs, Product rhs)
            {
                if (lhs.Identifier == null || rhs.Identifier == null)
                {
                    return -1;
                }

                return lhs.Identifier.CompareTo(rhs.Identifier);
            });

            return queryResult;
        }
    }
}