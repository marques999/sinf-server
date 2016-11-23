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
        private static SqlColumn[] sqlProdutos =
        {
            new SqlColumn("ARTIGO.Artigo", null),
            new SqlColumn("ARTIGO.Descricao", null),
            new SqlColumn("ARTIGO.PCMedio", null),
            new SqlColumn("ARTIGO.Iva", null),
            new SqlColumn("FAMILIAS.Familia", "IdFamilia"),
            new SqlColumn("FAMILIAS.Descricao", "Familia"),
            new SqlColumn("ARTIGO.STKActual", "Stock")
        };

        private static int SortProduct(ProductListing lhs, ProductListing rhs)
        {
            if (lhs.Identificador == null || rhs.Identificador == null)
            {
                return -1;
            }

            return lhs.Identificador.CompareTo(rhs.Identificador);
        }

        public static List<ProductListing> List()
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var productList = new List<ProductListing>();
            var productInfo = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("ARTIGO")
                .Columns(sqlProdutos)
                .LeftJoin("FAMILIAS", "Familia", Comparison.Equals, "ARTIGO", "Familia"));

            while (!productInfo.NoFim())
            {
                productList.Add(new ProductListing()
                {
                    Identificador = TypeParser.String(productInfo.Valor("Artigo")),
                    Descricao = TypeParser.String(productInfo.Valor("Descricao")),
                    Preco = TypeParser.Double(productInfo.Valor("PCMedio")),
                    IVA = TypeParser.Double(productInfo.Valor("Iva")),
                    Stock = TypeParser.Double(productInfo.Valor("Stock")),
                    Categoria = new Reference(
                        TypeParser.String(productInfo.Valor("IdFamilia")),
                        TypeParser.String(productInfo.Valor("Familia"))
                    )
                });

                productInfo.Seguinte();
            }

            productList.Sort(SortProduct);

            return productList;
        }

        public static Product View(string productId)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var productsTable = PrimaveraEngine.Engine.Comercial.Artigos;

            if (productsTable.Existe(productId) == false)
            {
                return null;
            }

            var productInfo = productsTable.Edita(productId);

            return new Product()
            {
                Identificador = productInfo.get_Artigo(),
                Descricao = productInfo.get_Descricao(),
                CodigoBarras = productInfo.get_CodBarras(),
                Unidade = productInfo.get_UnidadeVenda(),
                PrecoMedio = productInfo.get_PCMedio(),
                Desconto = productInfo.get_Desconto(),
                IVA = productInfo.get_IVA(),
                Stock = productInfo.get_StkActual(),
                Categoria = CategoryIntegration.GenerateReference(productInfo.get_Familia()),
                Armazens = WarehouseIntegration.GetWarehouses(productInfo.get_Artigo())
            };
        }

        public static List<ProductListing> ByCategory(string categoryId)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var productList = new List<ProductListing>();
            var productInfo = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("ARTIGO")
                .Columns(sqlProdutos)
                .LeftJoin("FAMILIAS", "Familia", Comparison.Equals, "ARTIGO", "Familia")
                .Where("ARTIGO.Familia", Comparison.Equals, categoryId));

            while (!productInfo.NoFim())
            {
                productList.Add(new ProductListing()
                {
                    Identificador = TypeParser.String(productInfo.Valor("Artigo")),
                    Descricao = TypeParser.String(productInfo.Valor("Descricao")),
                    Preco = TypeParser.Double(productInfo.Valor("PCMedio")),
                    IVA = TypeParser.Double(productInfo.Valor("Iva")),
                    Stock = TypeParser.Double(productInfo.Valor("Stock")),
                    Categoria = new Reference(
                        TypeParser.String(productInfo.Valor("IdFamilia")),
                        TypeParser.String(productInfo.Valor("Familia"))
                    )
                });

                productInfo.Seguinte();
            }

            productList.Sort(SortProduct);

            return productList;
        }
    }
}