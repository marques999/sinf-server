using System.Data.SQLite;

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
        private static string queryPassword = "UPDATE users SET password = :password WHERE username = :username";

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

            using (var queryResult = PrimaveraEngine.ConsultaSQLite(sqlQuery))
            {
                if (queryResult.Read() == false)
                {
                    return null;
                }

                var userPasword = queryResult.GetString(queryResult.GetOrdinal(fieldPassword));

                if (userPasword.Equals(userLogin.Password) == false)
                {
                    return null;
                }

                var representativesTable = PrimaveraEngine.Engine.Comercial.Vendedores;
                var representativeId = queryResult.GetString(queryResult.GetOrdinal(fieldRepresentative));
                var representativeInfo = representativesTable.Edita(representativeId);
                var userToken = Authentication.CreateSession(userLogin.Username, representativeId);

                return new SessionInfo
                {
                    Token = userToken,
                    Email = representativeInfo.get_Email(),
                    Nome = representativeInfo.get_Nome(),
                    Comissao = representativeInfo.get_Comissao(),
                    Telefone = representativeInfo.get_Telefone(),
                    Telemovel = representativeInfo.get_Telemovel(),
                    Identificador = representativeInfo.get_Vendedor(),
                    Fotografia = representativeInfo.get_LocalizacaoFoto()
                };
            }
        }

        public static bool Logout(string sessionToken)
        {
            return true;
        }

        public static bool ChangePassword(string sessionUsername, UserPassword userPassword)
        {
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
                if (queryResult.Read())
                {
                    var oldPassword = queryResult.GetString(queryResult.GetOrdinal(fieldPassword));

                    if (userPassword.PasswordNova.Equals(oldPassword))
                    {
                        return false;
                    }

                    if (userPassword.PasswordAntiga.Equals(oldPassword) == false)
                    {
                        return false;
                    }

                    using (var sqlCommand = new SQLiteCommand(queryPassword, PrimaveraEngine.getAuthenticationService()))
                    {
                        sqlCommand.Parameters.Add(new SQLiteParameter(fieldUsername, sessionUsername));
                        sqlCommand.Parameters.Add(new SQLiteParameter(fieldPassword, userPassword.PasswordNova));
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