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
        private static SqlColumn[] sqlColumns =
        {
            new SqlColumn("ARTIGO.Artigo", null),
            new SqlColumn("ARTIGO.Descricao", null),
            new SqlColumn("ARTIGO.PCMedio", null),
            new SqlColumn("ARTIGO.Desconto", null),
            new SqlColumn("ARTIGO.Iva", null),
            new SqlColumn("FAMILIAS.Familia", "IdFamilia"),
            new SqlColumn("FAMILIAS.Descricao", "Familia"),
            new SqlColumn("ARTIGO.STKActual", "Stock")
        };

        private static Product Generate(StdBELista queryResult)
        {
            string productId = queryResult.Valor("Artigo");

            return new Product()
            {
                Identifier = productId,
                Name = TypeParser.String(queryResult.Valor("Descricao")),
                Price = TypeParser.Double(queryResult.Valor("PCMedio")),
                DiscountValue = TypeParser.Double(queryResult.Valor("Desconto")),
                Tax = TypeParser.Double(queryResult.Valor("Iva")),
                Stock = TypeParser.Double(queryResult.Valor("Stock")),
                Category = CategoryIntegration.getReference(queryResult),
                Warehouses = WarehouseIntegration.GetWarehouses(productId)
            };
        }

        public static List<Product> Get()
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var queryResult = new List<Product>();
            var queryObject = PrimaveraEngine.Consulta(new SqlBuilder()
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

        public static Product GetByIdentifier(string productId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            if (PrimaveraEngine.Engine.Comercial.Artigos.Existe(productId) == false)
            {
                return null;
            }

            return Generate(PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("ARTIGO")
                .Columns(sqlColumns)
                .Where("ARTIGO.Artigo", Comparison.Equals, productId)
                .LeftJoin("FAMILIAS", "Familia", Comparison.Equals, "ARTIGO", "Familia")));
        }

        public static List<Product> GetByCategory(string categoryId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            if (PrimaveraEngine.Engine.Comercial.Familias.Existe(categoryId) == false)
            {
                return null;
            }

            var queryResult = new List<Product>();
            var queryObject = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("ARTIGO")
                .Columns(sqlColumns)
                .LeftJoin("FAMILIAS", "Familia", Comparison.Equals, "ARTIGO", "Familia")
                .Where("ARTIGO.Familia", Comparison.Equals, categoryId));

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