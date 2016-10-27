using System;
using System.Collections.Generic;

using FirstREST.Lib_Primavera.Enums;
using FirstREST.Lib_Primavera.Model;

using Interop.ErpBS900;
using Interop.StdBE900;
using Interop.GcpBE900;

namespace FirstREST.Lib_Primavera.Integration
{
    public class ContactIntegration
    {
        private static SqlColumn[] sqlColumns =
        {
            new SqlColumn("CONTACTOS.Contacto", null),
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

        private static Contact Generate(StdBELista queryResult)
        {
            return new Contact()
            {
                Identifier = queryResult.Valor("Contacto"),
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

        public static List<Contact> GetContacts()
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var queryResult = new List<Contact>();
            var queryObject = PriEngine.Consulta(new QueryBuilder().FromTable("CONTACTOS").Columns(sqlColumns));

            while (!queryObject.NoFim())
            {
                queryResult.Add(Generate(queryObject));
                queryObject.Seguinte();
            }

            queryResult.Sort(delegate(Contact lhs, Contact rhs)
            {
                if (lhs.Identifier == null || rhs.Identifier == null)
                {
                    return -1;
                }

                return lhs.Identifier.CompareTo(rhs.Identifier);
            });

            return queryResult;
        }

        public static Contact GetContact(string paramId)
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            /* var accountsTable = PriEngine.Engine.Comercial.Clientes;

             if (accountsTable.Existe(paramId) == false)
             {
                 throw new NotFoundException();
             }*/

            return Generate(PriEngine.Consulta(new QueryBuilder()
                .FromTable("CONTACTOS")
                .Columns(sqlColumns)
                .Where("CONTACTOS.Contacto", Comparison.Equals, paramId)));
        }

        public static void UpdateContact(string paramId, Contact paramObject)
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var accountsTable = PriEngine.Engine.Comercial.Clientes;

            if (accountsTable.Existe(paramId) == false)
            {
                throw new NotFoundException();
            }

            var newInstance = accountsTable.Edita(paramId);

            newInstance.set_EmModoEdicao(true);
            newInstance.set_Nome(paramObject.Name);
            newInstance.set_Morada(paramObject.Location.Street);
            newInstance.set_Zona(paramObject.Location.State);
            newInstance.set_Pais(paramObject.Location.Country);
            newInstance.set_CodigoPostal(paramObject.Location.PostalCode);
            newInstance.set_Pais(paramObject.Location.Country);
            accountsTable.Actualiza(newInstance);
        }

        public static void CreateContact(string paramId, Contact paramObject)
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var newInstance = new GcpBECliente();
            var accountsTable = PriEngine.Engine.Comercial.Clientes;

            if (accountsTable.Existe(paramId))
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
            accountsTable.Actualiza(newInstance);
        }
    }
}