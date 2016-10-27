using System.Collections.Generic;

using Interop.StdBE900;

using FirstREST.Lib_Primavera.Enums;
using FirstREST.Lib_Primavera.Model;

namespace FirstREST.Lib_Primavera.Integration
{
    public class WarehouseIntegration
    {
        private static SqlColumn[] sqlColumns =
        {
            new SqlColumn("ARMAZENS.Armazem", null),
            new SqlColumn("ARMAZENS.Descricao", null),
            new SqlColumn("ARMAZENS.Morada", null),
            new SqlColumn("ARMAZENS.Localidade", null),
            new SqlColumn("ARMAZENS.Cp", null),
            new SqlColumn("ARMAZENS.Distrito", null),
            new SqlColumn("ARMAZENS.Pais", null),
            new SqlColumn("ARTIGOARMAZEM.StkActual", null)
        };

        public static List<Warehouse> GetWarehouses(string productId)
        {
            var queryResult = new List<Warehouse>();
            var queryObject = PriEngine.Consulta(new QueryBuilder()
                .FromTable("ARTIGOARMAZEM")
                .Columns(sqlColumns)
                .Where("ARTIGOARMAZEM.Artigo", Comparison.Equals, productId)
                .LeftJoin("ARMAZENS", "Armazem", Comparison.Equals, "ARTIGOARMAZEM", "Armazem"));

            while (!queryObject.NoFim())
            {
                queryResult.Add(new Warehouse
                {
                    Identifier = queryObject.Valor("Armazem"),
                    Name = queryObject.Valor("Descricao"),
                    Stock = queryObject.Valor("StkActual"),

                    Location = new Address
                    {
                        PostalCode = queryObject.Valor("Cp"),
                        Street = queryObject.Valor("Morada"),
                        Country = queryObject.Valor("Pais"),
                        Parish = queryObject.Valor("Localidade"),
                        State = queryObject.Valor("Distrito"),
                    }
                });

                queryObject.Seguinte();
            }

            queryResult.Sort(delegate(Warehouse lhs, Warehouse rhs)
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