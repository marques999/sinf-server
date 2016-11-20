using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data.SqlClient;

using FirstREST.QueryBuilder;
using FirstREST.QueryBuilder.Enums;
using FirstREST.LibPrimavera.Model;

namespace FirstREST.LibPrimavera.Integration
{
    public class UserIntegration
    {
        public static List<Representative> List()
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var queryResult = new List<Representative>();
            var queryObject = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("VENDEDORES"));

            while (!queryObject.NoFim())
            {
                queryResult.Add(new Representative
                {
                    NomeCompleto = TypeParser.String(queryObject.Valor("Nome")),
                    Email = TypeParser.String(queryObject.Valor("Email")),
                    Fotografia = TypeParser.String(queryObject.Valor("LocalizacaoFoto")),
                    Identificador = TypeParser.String(queryObject.Valor("Vendedor"))
                });

                queryObject.Seguinte();
            }

            return queryResult;
        }

        private static Representative generateUser(SQLiteDataReader sqliteQuery, Interop.StdBE900.StdBELista queryObject)
        {
            if (sqliteQuery.Read())
            {
                return new Representative
                {
                    NomeCompleto = queryObject.Valor("Nome"),
                    Email = queryObject.Valor("Email"),
                    Telemovel = queryObject.Valor("Telemovel"),
                    Identificador = queryObject.Valor("Vendedor")
                };
            }
            else
            {
                return null;
            }
        }

        public static Representative View(string sessionUsername)
        {
            try
            {
                PrimaveraEngine.InitializeSQLite();
            }
            catch
            {
                throw new DatabaseConnectionException();
            }

            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var sqlQuery = new SqlBuilder()
                .FromTable("users")
                .Column("reperesentative", null)
                .Where("username", Comparison.Equals, sessionUsername);

            using (var queryResult = PrimaveraEngine.ConsultaSQLite(sqlQuery))
            {
                if (queryResult.Read())
                {
                    var representativesTable = PrimaveraEngine.Engine.Comercial.Vendedores;
                    var representativeId = queryResult.GetString(queryResult.GetOrdinal("representative"));
                    var representativeInfo = representativesTable.Edita(representativeId);

                    return new Representative
                    {
                        NomeCompleto = representativeInfo.get_Nome(),
                        Email = representativeInfo.get_Email(),
                        Identificador = representativeInfo.get_Vendedor(),
                        Telefone = representativeInfo.get_Telefone(),
                        Telemovel = representativeInfo.get_Telemovel(),
                        Fotografia = representativeInfo.get_LocalizacaoFoto()
                    };
                }
            }

            return null;
        }

        private static string registerUser = "INSERT INTO users(username, password, reference) VALUES (?,?,?)";

        public static bool Insert(UserInfo paramObject)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            try
            {
                PrimaveraEngine.InitializeSQLite();
            }
            catch
            {
                throw new DatabaseConnectionException();
            }

            if (PrimaveraEngine.Engine.Comercial.Vendedores.Existe(paramObject.Representante) == false)
            {
                return false;
            }

            using (var sqlCommand = new SQLiteCommand(registerUser, PrimaveraEngine.getAuthenticationService()))
            {
                sqlCommand.Parameters.Add(paramObject.Username);
                sqlCommand.Parameters.Add(paramObject.Password);
                sqlCommand.Parameters.Add(paramObject.Representante);
                sqlCommand.ExecuteNonQuery();
            }

            return true;
        }
    }
}