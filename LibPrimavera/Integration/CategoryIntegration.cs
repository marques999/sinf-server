using System.Collections.Generic;

using Interop.StdBE900;

using FirstREST.QueryBuilder;
using FirstREST.QueryBuilder.Enums;
using FirstREST.LibPrimavera.Model;

namespace FirstREST.LibPrimavera.Integration
{
    public class CategoryIntegration
    {
        private static SqlColumn[] sqlColumnsFull =
        {
            new SqlColumn("FAMILIAS.Familia", null),
            new SqlColumn("FAMILIAS.Descricao", null),
            new SqlColumn("COUNT(*)", "Count")
        };

        public static List<CategoryListing> List()
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var queryResult = new List<CategoryListing>();
            var queryObject = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("FAMILIAS")
                .Columns(sqlColumnsFull)
                .InnerJoin("ARTIGO", "Familia", Comparison.Equals, "FAMILIAS", "Familia")
                .GroupBy(new string[] { "FAMILIAS.Familia", "FAMILIAS.Descricao" }));

            while (!queryObject.NoFim())
            {
                queryResult.Add(new CategoryListing()
                {
                    Identificador = TypeParser.String(queryObject.Valor("Familia")),
                    Descricao = TypeParser.String(queryObject.Valor("Descricao")),
                    NumeroProdutos = TypeParser.Integer(queryObject.Valor("Count"))
                });

                queryObject.Seguinte();
            }

            queryResult.Sort(delegate(CategoryListing lhs, CategoryListing rhs)
            {
                if (lhs.Identificador == null || rhs.Identificador == null)
                {
                    return -1;
                }

                return lhs.Descricao.CompareTo(rhs.Descricao);
            });

            return queryResult;
        }

        public static Category Get(string categoryId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var categoriesTable = PrimaveraEngine.Engine.Comercial.Familias;

            if (categoriesTable.Existe(categoryId) == false)
            {
                return null;
            }

            return new Category
            {
                Identificador = categoryId,
                Descricao = categoriesTable.DaDescricao(categoryId),
                Produtos = ProductIntegration.ByCategory(categoryId)
            };
        }

        public static Reference GenerateReference(string categoryId)
        {
            return new Reference(categoryId, PrimaveraEngine.Engine.Comercial.Familias.DaDescricao(categoryId));
        }
    }
}