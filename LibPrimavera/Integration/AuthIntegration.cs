using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data.SqlClient;

using FirstREST.QueryBuilder;
using FirstREST.QueryBuilder.Enums;
using FirstREST.LibPrimavera.Model;

namespace FirstREST.LibPrimavera.Integration
{
    public class AuthIntegration
    {
        private static SqlColumn[] userLoginFields = new SqlColumn[]
        {
            new SqlColumn("username", null),
            new SqlColumn("passord", null)
        };

        public static SessionInfo Authenticate(UserLogin userLogin)
        {
            try
            {
                PrimaveraEngine.InitializeSQLite();
            }
            catch
            {
                throw new DatabaseConnectionException();
            }

            var sqlQuery = new SqlBuilder()
                .FromTable("users")
                .Columns(userLoginFields)
                .Where("username", Comparison.Equals, userLogin.Username);

            using (var queryResult = PrimaveraEngine.ConsultaSQLite(sqlQuery))
            {
                var userPasword = queryResult.GetOrdinal("password");

                if (queryResult.Read() == false)
                {
                    return null;
                }

                if (queryResult.GetString(userPasword).Equals(userLogin.Password) == false)
                {
                    return null;
                }

                if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
                {
                    throw new DatabaseConnectionException();
                }

                var representativesTable = PrimaveraEngine.Engine.Comercial.Vendedores;
                var representativeId = queryResult.GetString(queryResult.GetOrdinal("representative"));
                var representativeInfo = representativesTable.Edita(representativeId);
                var userToken = Authentication.CreateSession(userLogin.Username, representativeId);

                return new SessionInfo
                {
                    Token = userToken,
                    NomeCompleto = representativeInfo.get_Nome(),
                    Email = representativeInfo.get_Email(),
                    Identificador = representativeInfo.get_Vendedor(),
                    Telefone = representativeInfo.get_Telefone(),
                    Telemovel = representativeInfo.get_Telemovel(),
                    Fotografia = representativeInfo.get_LocalizacaoFoto()
                };
            }
        }

        public static bool Logout(string sessionToken)
        {
            return true;
        }

        private static string changePassword = "UPDATE users SET password = ? WHERE username = ?";

        public static bool ChangePassword(string sessionUsername, UserPassword userPassword)
        {
            try
            {
                PrimaveraEngine.InitializeSQLite();
            }
            catch
            {
                throw new DatabaseConnectionException();
            }

            var sqlQuery = new SqlBuilder()
                .FromTable("users")
                .Column("password")
                .Where("username", Comparison.Equals, sessionUsername);

            using (var queryResult = PrimaveraEngine.ConsultaSQLite(sqlQuery))
            {
                if (queryResult.Read())
                {
                    var oldPassword = queryResult.GetString(0);

                    if (oldPassword.Equals(userPassword.PasswrdAntiga) == false)
                    {
                        return false;
                    }

                    using (var sqlCommand = new SQLiteCommand(changePassword, PrimaveraEngine.getAuthenticationService()))
                    {
                        sqlCommand.Parameters.Add(sessionUsername);
                        sqlCommand.Parameters.Add(userPassword.PasswordNova);
                        sqlCommand.ExecuteNonQuery();
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
    }
}