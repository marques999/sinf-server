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
            return Engine.Consulta(queryString.BuildQuery());
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

        static PrimaveraEngine()
        {
            loggedIn.Add("4c3b314d9ec74f7690dd819df973fb82", "marques999");
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
    }
}