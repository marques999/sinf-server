using System.Collections.Generic;

using Interop.StdBE900;

using FirstREST.QueryBuilder;
using FirstREST.QueryBuilder.Enums;
using FirstREST.LibPrimavera.Model;

namespace FirstREST.LibPrimavera.Integration
{
    public class CategoryIntegration
    {
        private static SqlColumn[] sqlColumns =
        {
            new SqlColumn("FAMILIAS.Familia", null),
            new SqlColumn("FAMILIAS.Descricao", null),
            new SqlColumn("COUNT(*)", "Count"),
        };

        public static List<Category> List()
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var queryResult = new List<Category>();
            var queryObject = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("FAMILIAS")
                .Columns(sqlColumns)
                .InnerJoin("ARTIGO", "Familia", Comparison.Equals, "FAMILIAS", "Familia")
                .GroupBy(new string[] { "FAMILIAS.Familia", "FAMILIAS.Descricao" }));

            while (!queryObject.NoFim())
            {
                queryResult.Add(new Category()
                {
                    Identifier = TypeParser.String(queryObject.Valor("Familia")),
                    Name = TypeParser.String(queryObject.Valor("Descricao")),
                    NumberProducts = queryObject.Valor("Count")
                });

                queryObject.Seguinte();
            }

            queryResult.Sort(delegate(Category lhs, Category rhs)
            {
                if (lhs.Identifier == null || rhs.Identifier == null)
                {
                    return -1;
                }

                return lhs.Identifier.CompareTo(rhs.Identifier);
            });

            return queryResult;
        }

        public static CategoryReference GenerateReference(StdBELista queryResult)
        {
            return new CategoryReference
            {
                Identifier = TypeParser.String(queryResult.Valor("IdFamilia")),
                Name = TypeParser.String(queryResult.Valor("Familia"))
            };
        }
    }
}