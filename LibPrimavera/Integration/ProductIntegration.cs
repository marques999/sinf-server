using System;
using System.Collections.Generic;

using Interop.GcpBE900;
using Interop.StdBE900;

using FirstREST.QueryBuilder;
using FirstREST.QueryBuilder.Enums;
using FirstREST.LibPrimavera.Model;

namespace FirstREST.LibPrimavera.Integration
{
    public class ProductIntegration
    {
        private static List<ProductPrice> dummyPrices = new List<ProductPrice>
        {
            new ProductPrice(false, 0.0),
            new ProductPrice(false, 0.0),
            new ProductPrice(false, 0.0),
            new ProductPrice(false, 0.0),
            new ProductPrice(false, 0.0),
            new ProductPrice(false, 0.0)
        };

        private static List<ProductPrice> GetPrices(string productId, string unitId)
        {
            var priceInfo = PrimaveraEngine.Engine.Comercial.ArtigosPrecos.Edita(productId, "EUR", unitId);

            return priceInfo == null ? null : new List<ProductPrice>
            {
                new ProductPrice(priceInfo.get_PVP1IvaIncluido(), priceInfo.get_PVP1()),
                new ProductPrice(priceInfo.get_PVP2IvaIncluido(), priceInfo.get_PVP2()),
                new ProductPrice(priceInfo.get_PVP3IvaIncluido(), priceInfo.get_PVP3()),
                new ProductPrice(priceInfo.get_PVP4IvaIncluido(), priceInfo.get_PVP4()),
                new ProductPrice(priceInfo.get_PVP5IvaIncluido(), priceInfo.get_PVP5()),
                new ProductPrice(priceInfo.get_PVP6IvaIncluido(), priceInfo.get_PVP6())
            };
        }

        private static List<ProductPrice> GeneratePrices(string productId, string unitId)
        {
            var priceInfo = GetPrices(productId, unitId);
            return priceInfo == null ? dummyPrices : priceInfo;
        }

        private static double FindLowest(string productId, string unitId)
        {
            var priceList = GetPrices(productId, unitId);

            if (priceList == null)
            {
                return 0.0;
            }

            var lowestPrice = Double.MaxValue;

            foreach (var x in priceList)
            {
                double productPrice = x.Preco;

                if (productPrice > 0 && productPrice < lowestPrice)
                {
                    lowestPrice = productPrice;
                }
            }

            return lowestPrice == Double.MaxValue ? 0.0 : lowestPrice;
        }

        private static SqlColumn[] sqlColumnsListing =
        {
            new SqlColumn("ARTIGO.Artigo", null),
            new SqlColumn("ARTIGO.Descricao", null),
            new SqlColumn("ARTIGO.UnidadeVenda", null),	
            new SqlColumn("ARTIGO.Iva", null),
            new SqlColumn("ARTIGO.STKActual", "Stock")
        };

        private static ProductListing GenerateListing(StdBELista productInfo)
        {
            var productId = TypeParser.String(productInfo.Valor("Artigo"));
            var productUnit = TypeParser.String(productInfo.Valor("UnidadeVenda"));

            return new ProductListing()
            {
                Unidade = productUnit,
                Identificador = productId,
                PrecoMedio = FindLowest(productId, productUnit),
                Stock = TypeParser.Double(productInfo.Valor("Stock")),
                IVA = TypeParser.Double(productInfo.Valor("Iva")),
                Descricao = TypeParser.String(productInfo.Valor("Descricao"))
            };
        }

        public static List<ProductListing> List()
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var productList = new List<ProductListing>();
            var productInfo = PrimaveraEngine.Consulta(new SqlBuilder().FromTable("ARTIGO").Columns(sqlColumnsListing));

            if (productInfo == null || productInfo.Vazia())
            {
                return productList;
            }

            while (!productInfo.NoFim())
            {
                productList.Add(GenerateListing(productInfo));
                productInfo.Seguinte();
            }

            productList.Sort(delegate(ProductListing lhs, ProductListing rhs)
            {
                if (lhs.Identificador == null || rhs.Identificador == null)
                {
                    return -1;
                }

                return lhs.Identificador.CompareTo(rhs.Identificador);
            });

            return productList;
        }

        private static SqlColumn[] sqlColumnsFull =		
         {		
            new SqlColumn("ARTIGO.Artigo", null),		
            new SqlColumn("ARTIGO.Descricao", null),	
            new SqlColumn("ARTIGO.Desconto", null),		
            new SqlColumn("ARTIGO.CodBarras", null),		           
            new SqlColumn("ARTIGO.UnidadeVenda", null),			
            new SqlColumn("ARTIGO.Desconto", null),		
            new SqlColumn("ARTIGO.Iva", null),		
            new SqlColumn("ARTIGO.DataUltEntrada", null),		
            new SqlColumn("ARTIGO.DataUltSaida", null),		
            new SqlColumn("FAMILIAS.Familia", "IdFamilia"),		
            new SqlColumn("FAMILIAS.Descricao", "Familia"),		
            new SqlColumn("ARTIGO.STKActual", "Stock")		
        };

        public static Product View(string productId)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var productInfo = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("ARTIGO")
                .Columns(sqlColumnsFull)
                .Where("ARTIGO.Artigo", Comparison.Equals, productId)
                .LeftJoin("FAMILIAS", "Familia", Comparison.Equals, "ARTIGO", "Familia"));

            if (productInfo == null || productInfo.Vazia())
            {
                throw new NotFoundException("produto", false);
            }

            var productUnit = TypeParser.String(productInfo.Valor("UnidadeVenda"));
            var priceList = GeneratePrices(productId, productUnit);

            return new Product()
            {
                Unidade = productUnit,
                PrecoMedio = CalculateAverage(priceList),
                Precos = GeneratePrices(productId, productUnit),
                IVA = TypeParser.Double(productInfo.Valor("Iva")),
                Stock = TypeParser.Double(productInfo.Valor("Stock")),
                Armazens = WarehouseIntegration.GetWarehouses(productId),
                Desconto = TypeParser.Double(productInfo.Valor("Desconto")),
                Descricao = TypeParser.String(productInfo.Valor("Descricao")),
                Identificador = TypeParser.String(productInfo.Valor("Artigo")),
                CodigoBarras = TypeParser.String(productInfo.Valor("CodBarras")),
                Categoria = CategoryIntegration.GenerateReference(productInfo),
                UltimaEntrada = TypeParser.Date(productInfo.Valor("DataUltEntrada")),
                UltimaSaida = TypeParser.Date(productInfo.Valor("DataUltSaida"))
            };
        }

        private static double CalculateAverage(List<ProductPrice> priceList)
        {
            int priceCount = 0;
            double averagePrice = 0.0;

            foreach (var x in priceList)
            {
                if (x.Preco > 0)
                {
                    averagePrice += x.Preco;
                    priceCount++;
                }
            }

            return averagePrice / priceCount;
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
                .Columns(sqlColumnsListing)
                .Where("ARTIGO.Familia", Comparison.Equals, categoryId));

            if (productInfo == null || productInfo.Vazia())
            {
                throw new NotFoundException("categoria", true);
            }

            while (!productInfo.NoFim())
            {
                productList.Add(GenerateListing(productInfo));
                productInfo.Seguinte();
            }

            productList.Sort(delegate(ProductListing lhs, ProductListing rhs)
            {
                if (lhs.Identificador == null || rhs.Identificador == null)
                {
                    return -1;
                }

                return lhs.Identificador.CompareTo(rhs.Identificador);
            });

            return productList;
        }
    }
}