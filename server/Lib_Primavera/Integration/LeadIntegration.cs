using System;
using System.Collections.Generic;

using Interop.GcpBE900;
using Interop.StdBE900;

using FirstREST.Lib_Primavera.Enums;
using FirstREST.Lib_Primavera.Model;

namespace FirstREST.Lib_Primavera.Integration
{
    public class LeadIntegration
    {
        private static SqlColumn[] sqlColummns =
        {
            new SqlColumn("CONTACTOS.Contacto", null),
            new SqlColumn("CONTACTOS.Titulo", null),
            new SqlColumn("CONTACTOS.PrimeiroNome", null),
            new SqlColumn("CONTACTOS.UltimoNome", null),
            new SqlColumn("CONTACTOS.DataUltContacto", null),
            new SqlColumn("CONTACTOS.Email", null),
            new SqlColumn("CONTACTOS.Telefone", null),
            new SqlColumn("CONTACTOS.Telemovel", null),
            new SqlColumn("CONTACTOS.CodPostal", null),
            new SqlColumn("CONTACTOS.Morada", null),
            new SqlColumn("CONTACTOS.Pais", null),
            new SqlColumn("CONTACTOS.Localidade", null),
            new SqlColumn("CONTACTOS.Distrito", null)
        };

        private static Lead Generate(StdBELista queryResult)
        {
            return new Lead()
            {
                Identifier = queryResult.Valor("Contacto"),
                Title = queryResult.Valor("Titulo"),
                Name = queryResult.Valor("PrimeiroNome") + " " + queryResult.Valor("UltimoNome"),
                Email = queryResult.Valor("Email"),
                Phone = queryResult.Valor("Telefone"),
                LastContact = queryResult.Valor("DataUltContacto"),
                MobilePhone = queryResult.Valor("Telemovel"),

                Location = new Address
                {
                    PostalCode = queryResult.Valor("CodPostal"),
                    Street = queryResult.Valor("Morada"),
                    Country = queryResult.Valor("Pais"),
                    Parish = queryResult.Valor("Localidade"),
                    State = queryResult.Valor("Distrito"),
                },
            };
        }

        public static List<Lead> GetLeads()
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var queryResult = new List<Lead>();
            var queryObject = PriEngine.Consulta(new QueryBuilder().FromTable("CONTACTOS").Columns(sqlColummns));

            while (!queryObject.NoFim())
            {
                queryResult.Add(Generate(queryObject));
                queryObject.Seguinte();
            }

            queryResult.Sort(delegate(Lead lhs, Lead rhs)
            {
                if (lhs.Identifier == null || rhs.Identifier == null)
                {
                    return -1;
                }

                return lhs.Identifier.CompareTo(rhs.Identifier);
            });

            return queryResult;
        }

        public static Lead GetLead(string paramId)
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            /*if (PriEngine.Clientes.Existe(paramId) == false)
            {
                throw new NotFoundException();
            }*/

            return Generate(PriEngine.Consulta(new QueryBuilder()
                .FromTable("CONTACTOS")
                .Columns(sqlColummns)
                .Where("CONTACTOS.Contacto", Comparison.Equals, paramId)));
        }

        public static void UpdateLead(string paramId, Lead paramObject)
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var tabelaLeads = PriEngine.Engine.Comercial.Clientes;

            if (tabelaLeads.Existe(paramId) == false)
            {
                throw new NotFoundException();
            }

            var newInstance = tabelaLeads.Edita(paramId);

            newInstance.set_EmModoEdicao(true);
            newInstance.set_Nome(paramObject.Name);
            newInstance.set_Morada(paramObject.Location.Street);
            newInstance.set_Zona(paramObject.Location.State);
            newInstance.set_Pais(paramObject.Location.Country);
            newInstance.set_CodigoPostal(paramObject.Location.PostalCode);
            newInstance.set_Pais(paramObject.Location.Country);
            tabelaLeads.Actualiza(newInstance);
        }

        public static void CreateLead(string paramId, Lead paramObject)
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var newInstance = new GcpBECliente();
            var tabelaLeads = PriEngine.Engine.Comercial.Clientes;

            if (tabelaLeads.Existe(paramId))
            {
                throw new EntityExistsException();
            }

            newInstance.set_Cliente(paramId);
            newInstance.set_Nome(paramObject.Name);
            newInstance.set_Morada(paramObject.Location.Street);
            newInstance.set_Zona(paramObject.Location.State);
            newInstance.set_Pais(paramObject.Location.Country);
            newInstance.set_CodigoPostal(paramObject.Location.PostalCode);
            newInstance.set_Pais(paramObject.Location.Country);
            tabelaLeads.Actualiza(newInstance);
        }
    }
}