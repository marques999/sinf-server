using System;
using System.Data.SQLite;

using Interop.GcpBE900;

using FirstREST.QueryBuilder;
using FirstREST.QueryBuilder.Enums;
using FirstREST.LibPrimavera.Model;

namespace FirstREST.LibPrimavera.Integration
{
    public class LoginIntegration
    {
        private static string tableUsers = "users";
        private static string fieldUsername = "username";
        private static string fieldPassword = "password";
        private static string fieldRepresentative = "representative";

        private static SqlColumn[] loginColumns = new SqlColumn[]
        {
            new SqlColumn(fieldUsername, null),
            new SqlColumn(fieldPassword, null),
            new SqlColumn(fieldRepresentative, null)
        };

        public static SessionInfo Authenticate(UserLogin userLogin)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var sqlQuery = new SqlBuilder()
                .FromTable(tableUsers)
                .Columns(loginColumns)
                .Where(fieldUsername, Comparison.Equals, userLogin.Username);

            using (var userInfo = PrimaveraEngine.ConsultaSQLite(sqlQuery))
            {
                if (userInfo.Read() == false)
                {
                    return null;
                }

                var userPasword = userInfo.GetString(userInfo.GetOrdinal(fieldPassword));

                if (userPasword.Equals(userLogin.Password) == false)
                {
                    return null;
                }

                var representativesTable = PrimaveraEngine.Engine.Comercial.Vendedores;
                var representativeId = userInfo.GetString(userInfo.GetOrdinal(fieldRepresentative));
                var representativeInfo = representativesTable.Edita(representativeId);
                var userToken = Authentication.CreateSession(userLogin.Username, representativeId);

                return new SessionInfo
                {
                    Token = userToken,
                    Username = userLogin.Username,
                    Nome = representativeInfo.get_Nome(),
                    Email = representativeInfo.get_Email(),
                    Morada = representativeInfo.get_Morada(),
                    Comissao = representativeInfo.get_Comissao(),
                    Telefone = representativeInfo.get_Telefone(),
                    Telemovel = representativeInfo.get_Telemovel(),
                    Identificador = representativeInfo.get_Vendedor(),
                    Empresa = PrimaveraEngine.Engine.Licenca.get_Nome(),
                    CodigoPostal = representativeInfo.get_CodigoPostal(),
                    Fotografia = representativeInfo.get_LocalizacaoFoto(),
                    Localidade = representativeInfo.get_LocalidadeCodigoPostal(),
                };
            }
        }

        public static bool Logout(string sessionToken)
        {
            return true;
        }

        public static Representative Update(string sessionUsername, UserInfo userInfo)
        {
            string queryPassword = "UPDATE users SET ";

            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var sqlQuery = new SqlBuilder()
                .FromTable(tableUsers)
                .Column(fieldPassword)
                .Where(fieldUsername, Comparison.Equals, sessionUsername);

            using (var queryResult = PrimaveraEngine.ConsultaSQLite(sqlQuery))
            {
                int changedFields = 0;

                if (queryResult.Read())
                {
                    var currentPassword = queryResult.GetString(queryResult.GetOrdinal(fieldPassword));

                    if (userInfo.Password.Equals(currentPassword))
                    {
                        return null;
                    }

                    if (userInfo.Password == null)
                    {
                        queryPassword += "password = :password";
                        changedFields++;
                    }

                    if (userInfo.Representante != null)
                    {
                        if (changedFields > 0)
                        {
                            queryPassword += " AND ";
                        }

                        queryPassword += "representative = :representative";
                        changedFields++;
                    }

                    if (changedFields < 1)
                    {
                        return null;
                    }

                    queryPassword += "WHERE username = :username";

                    using (var sqlCommand = new SQLiteCommand(queryPassword, PrimaveraEngine.getAuthenticationService()))
                    {
                        sqlCommand.Parameters.Add(new SQLiteParameter(fieldUsername, sessionUsername));

                        if (userInfo.Password != null)
                        {
                            sqlCommand.Parameters.Add(new SQLiteParameter(fieldPassword, userInfo.Password));
                        }

                        if (userInfo.Representante != null)
                        {
                            sqlCommand.Parameters.Add(new SQLiteParameter(fieldRepresentative, userInfo.Representante));
                        }

                        sqlCommand.ExecuteNonQuery();
                    }
                }
                else
                {
                    return null;
                }
            }

            return UserIntegration.View(userInfo.Representante);
        }
    }
}