using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Threading.Tasks;

using ADODB;

using Interop.ErpBS900;
using Interop.StdPlatBS900;
using Interop.StdBE900;
using Interop.IGcpBS900;
using Interop.ICrmBS900;

using FirstREST.QueryBuilder;
using FirstREST.LibPrimavera.Model;
using System.Data.SQLite;

namespace FirstREST.LibPrimavera
{
    public class PrimaveraEngine
    {
        public static StdPlatBS Platform
        {
            get;
            set;
        }

        public static ErpBS Engine
        {
            get;
            set;
        }

        public static StdBELista Consulta(SqlBuilder queryString)
        {
            string query = queryString.BuildQuery();
            System.Diagnostics.Debug.Print(query);
            return Engine.Consulta(query);
        }

        public static SQLiteDataReader ConsultaSQLite(SqlBuilder queryString)
        {
            using (var sqlQuery = sqliteConnection.CreateCommand())
            {
                System.Diagnostics.Debug.Print(queryString.BuildQuery());
                sqliteConnection.Open();
                sqlQuery.CommandText = queryString.BuildQuery();
                return sqlQuery.ExecuteReader();
            }
        }

        private static SQLiteConnection sqliteConnection;

        private static bool sqliteInitialized = false;

        public static void InitializeSQLite()
        {
            if (sqliteInitialized)
            {
                return;
            }

            sqliteConnection = new SQLiteConnection("Data Source=sinFORCE.sqlite;Version=3;");

            using (var sqlQuery = sqliteConnection.CreateCommand())
            {
                sqliteConnection.Open();
                sqlQuery.CommandText = "SELECT name FROM sqlite_master WHERE name='account'";

                var queryResult = sqlQuery.ExecuteScalar();

                if (queryResult != null && queryResult.ToString() == "account")
                {
                    sqliteInitialized = true;
                }
                else
                {
                    sqlQuery.CommandText = "CREATE TABLE users (username VARCHAR(64), password VARCHAR(64), references INT)";
                    sqlQuery.ExecuteNonQuery();
                    sqliteInitialized = true;
                }

                sqliteConnection.Close();
            }
        }

        public static bool InitializeCompany(string Company, string User, string Password)
        {
            if (Platform != null && Platform.Inicializada)
            {
                return true;
            }

            bool blnModoPrimario = true;
            var objAplConf = new StdBSConfApl();

            objAplConf.Instancia = "Default";
            objAplConf.AbvtApl = "GCP";
            objAplConf.PwdUtilizador = Password;
            objAplConf.Utilizador = User;
            objAplConf.LicVersaoMinima = "9.00";

            var MotorLE = new ErpBS();
            var Plataforma = new StdPlatBS();
            var objStdTransac = new StdBETransaccao();
            var tipoPlataforma = EnumTipoPlataforma.tpProfissional;

            try
            {
                Plataforma.AbrePlataformaEmpresa(ref Company, ref objStdTransac, ref objAplConf, ref tipoPlataforma, "");
            }
            catch
            {
                return false;
            }

            if (Plataforma.Inicializada)
            {
                Platform = Plataforma;
                MotorLE.AbreEmpresaTrabalho(EnumTipoPlataforma.tpProfissional, ref Company, ref User, ref Password, ref objStdTransac, "Default", ref blnModoPrimario);
                MotorLE.set_CacheActiva(true);
                Engine = MotorLE;
            }

            return Plataforma.Inicializada;
        }

        /*  public static bool IsAuthenticated()
          {
              try
              {
                  return string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) == false;
              }
              catch
              {
                  return false;
              }
          }
          */

        internal static SQLiteConnection getAuthenticationService()
        {
            return sqliteConnection;
        }

        public static string generateGUID()
        {
            return Guid.NewGuid().ToString("D").ToUpper();
        }

        private static HashGenerator hashGenerator = new HashGenerator();

        public static string GenerateHash()
        {
            return hashGenerator.EncodeLong(DateTime.Now.Ticks);
        }
    }
}