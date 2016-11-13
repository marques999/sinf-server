using System;
using System.Collections.Generic;

using Interop.StdBE900;

using FirstREST.QueryBuilder;
using FirstREST.QueryBuilder.Enums;
using FirstREST.LibPrimavera.Model;

namespace FirstREST.LibPrimavera.Integration
{
    public class ProductIntegration
    {
        private static SqlColumn[] sqlColumnsFull =
        {
            new SqlColumn("ARTIGO.Artigo", null),
            new SqlColumn("ARTIGO.Descricao", null),
            new SqlColumn("ARTIGO.CodBarras", null),
            new SqlColumn("ARTIGO.UnidadeVenda", null),
            new SqlColumn("ARTIGO.PCMedio", null),
            new SqlColumn("ARTIGO.Desconto", null),
            new SqlColumn("ARTIGO.Iva", null),
            new SqlColumn("FAMILIAS.Familia", "IdFamilia"),
            new SqlColumn("FAMILIAS.Descricao", "Familia"),
            new SqlColumn("ARTIGO.STKActual", "Stock")
        };

        private static SqlColumn[] sqlColumnsListing =
        {
            new SqlColumn("ARTIGO.Artigo", null),
            new SqlColumn("ARTIGO.Descricao", null),
            new SqlColumn("ARTIGO.PCMedio", null),
            new SqlColumn("ARTIGO.Iva", null),
            new SqlColumn("FAMILIAS.Familia", "IdFamilia"),
            new SqlColumn("FAMILIAS.Descricao", "Familia"),
            new SqlColumn("ARTIGO.STKActual", "Stock")
        };

        private static Product GenerateFull(StdBELista queryResult)
        {
            string productId = TypeParser.String(queryResult.Valor("Artigo"));

            return new Product()
            {
                Identifier = productId,
                Name = TypeParser.String(queryResult.Valor("Descricao")),
                Barcode = TypeParser.String(queryResult.Valor("CodBarras")),
                Unit = TypeParser.String(queryResult.Valor("UnidadeVenda")),
                Price = TypeParser.Double(queryResult.Valor("PCMedio")),
                Discount = TypeParser.Double(queryResult.Valor("Desconto")),
                Tax = TypeParser.Double(queryResult.Valor("Iva")),
                Stock = TypeParser.Double(queryResult.Valor("Stock")),
                Category = CategoryIntegration.GenerateReference(queryResult),
                Warehouses = WarehouseIntegration.GetWarehouses(productId)
            };
        }

        private static ProductListing GenerateListing(StdBELista queryResult)
        {
            return new ProductListing()
            {
                Identifier = TypeParser.String(queryResult.Valor("Artigo")),
                Name = TypeParser.String(queryResult.Valor("Descricao")),
                Price = TypeParser.Double(queryResult.Valor("PCMedio")),
                Tax = TypeParser.Double(queryResult.Valor("Iva")),
                Stock = TypeParser.Double(queryResult.Valor("Stock")),
                Category = CategoryIntegration.GenerateReference(queryResult)
            };
        }

        private static int SortProduct(ProductListing lhs, ProductListing rhs)
        {
            if (lhs.Identifier == null || rhs.Identifier == null)
            {
                return -1;
            }

            return lhs.Identifier.CompareTo(rhs.Identifier);
        }

        public static List<ProductListing> List(string sessionId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var queryResult = new List<ProductListing>();
            var queryObject = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("ARTIGO")
                .Columns(sqlColumnsListing)
                .LeftJoin("FAMILIAS", "Familia", Comparison.Equals, "ARTIGO", "Familia"));
            
            while (!queryObject.NoFim())
            {
                queryResult.Add(GenerateListing(queryObject));
                queryObject.Seguinte();
            }

            queryResult.Sort(SortProduct);

            return queryResult;
        }

        public static Product View(string sessionId, string productId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            if (PrimaveraEngine.Engine.Comercial.Artigos.Existe(productId) == false)
            {
                return null;
            }

            return GenerateFull(PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("ARTIGO")
                .Columns(sqlColumnsFull)
                .Where("ARTIGO.Artigo", Comparison.Equals, productId)
                .LeftJoin("FAMILIAS", "Familia", Comparison.Equals, "ARTIGO", "Familia")));
        }

        public static List<ProductListing> ByCategory(string categoryId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var queryResult = new List<ProductListing>();
            var queryObject = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("ARTIGO")
                .Columns(sqlColumnsListing)
                .LeftJoin("FAMILIAS", "Familia", Comparison.Equals, "ARTIGO", "Familia")
                .Where("ARTIGO.Familia", Comparison.Equals, categoryId));

            while (!queryObject.NoFim())
            {
                queryResult.Add(GenerateListing(queryObject));
                queryObject.Seguinte();
            }

            queryResult.Sort(SortProduct);

            return queryResult;
        }
    }
}