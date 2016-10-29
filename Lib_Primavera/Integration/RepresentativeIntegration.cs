using System;
using System.Collections.Generic;

using FirstREST.LibPrimavera.Model;

namespace FirstREST.LibPrimavera.Integration
{
    public class RepresentativeIntegration
    {
        public static List<User> GetRepresentatives()
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            return new List<User>();
        }

        public static Opportunity GetRepresentative(string paramId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            if (PrimaveraEngine.Engine.Comercial.Clientes.Existe(paramId) == false)
            {
                throw new NotFoundException();
            }

            return null;
        }

        public static bool UpdateOpportunity(string paramId, Opportunity paramObject)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            if (PrimaveraEngine.Engine.Comercial.Clientes.Existe(paramId) == false)
            {
                return false;
            }

            return true;
        }

        public static bool CreateRepresentative(string paramId, User paramObject)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            if (PrimaveraEngine.Engine.Comercial.Clientes.Existe(paramId))
            {
                return false;
            }

            return true;
        }
    }
}