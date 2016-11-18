using System;
using System.Collections.Generic;

using Interop.StdBE900;

using FirstREST.QueryBuilder;
using FirstREST.QueryBuilder.Enums;
using FirstREST.LibPrimavera.Model;
using Interop.GcpBE900;

namespace FirstREST.LibPrimavera.Integration
{
    public class ProductIntegration
    {
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

        private static Product GenerateFull(GcpBEArtigo productInfo)
        {
            return new Product()
            {
                Identificador = productInfo.get_Artigo(),
                Nome = productInfo.get_Descricao(),
                CodigoBarras = productInfo.get_CodBarras(),
                Unidade = productInfo.get_UnidadeVenda(),
                PrecoMedio = productInfo.get_PCMedio(),
                Desconto = productInfo.get_Desconto(),
                IVA = productInfo.get_IVA(),
                Stock = productInfo.get_StkActual(),
                Categoria = CategoryIntegration.GenerateReference(productInfo.get_Familia()),
                Warehouses = WarehouseIntegration.GetWarehouses(productInfo.get_Artigo())
            };
        }

        private static ProductListing GenerateListing(StdBELista productInfo)
        {
            return new ProductListing()
            {
                Identifier = TypeParser.String(productInfo.Valor("Artigo")),
                Name = TypeParser.String(productInfo.Valor("Descricao")),
                Price = TypeParser.Double(productInfo.Valor("PCMedio")),
                Tax = TypeParser.Double(productInfo.Valor("Iva")),
                Stock = TypeParser.Double(productInfo.Valor("Stock")),
                Category = new Reference(
                    TypeParser.String(productInfo.Valor("IdFamilia")),
                    TypeParser.String(productInfo.Valor("Familia"))
                )
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

        public static List<ProductListing> List()
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

        public static Product View(string productId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var productsTable = PrimaveraEngine.Engine.Comercial.Artigos;

            if (productsTable.Existe(productId) == false)
            {
                return null;
            }

            return GenerateFull(productsTable.Edita(productId));
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