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

        public static IGcpBSVendedores TabelaVendedores
        {
            get
            {
                return Engine.Comercial.Vendedores;
            }
        }

        public static ICrmBSContactos TabelaContactos
        {
            get
            {
                return Engine.CRM.Contactos;
            }
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
            string query = queryString.BuildQuery();
            var sqlQuery = sqliteConnection.CreateCommand();

            System.Diagnostics.Debug.Print(query);
            sqliteConnection.Open();      
            sqlQuery.CommandText = query; 
  
            var sqlResult = sqlQuery.ExecuteReader();
 
            sqliteConnection.Close();

            return sqlResult;
        }

        private static Dictionary<string, string> loggedIn = new Dictionary<string, string>();

        public static bool ValidateSession(string sesssionId)
        {
            return loggedIn.ContainsKey(sesssionId);
        }

        public static string GetUsername(string sesssionId)
        {
            if (loggedIn.ContainsKey(sesssionId))
            {
                return loggedIn[sesssionId];
            }

            return null;
        }

        private static SQLiteConnection sqliteConnection;

        static PrimaveraEngine()
        {
            loggedIn.Add("4c3b314d9ec74f7690dd819df973fb82", "marques999");
        }

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

        public static bool IsAuthenticated()
        {
            return true;
            /*try
            {
                var sessionUsername = HttpContext.Current.User.Identity.Name;
                return sessionUsername != null && string.IsNullOrWhiteSpace(sessionUsername) == false;
            }
            catch
            {
                return false;
            }*/
        }

        internal static bool ApiLogin(string username, string password)
        {
            return username == "merda" && password == "merda";
        }

        internal static SQLiteConnection getAuthenticationService()
        {return sqliteConnection;
        }
    }
}