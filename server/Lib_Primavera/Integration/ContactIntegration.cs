using System;
using System.Collections.Generic;

using Interop.StdBE900;
using Interop.CrmBE900;

using FirstREST.Lib_Primavera.Enums;
using FirstREST.Lib_Primavera.Model;

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
                Identifier = TypeParser.String(queryResult.Valor("Contacto")),
                Name = TypeParser.String(queryResult.Valor("PrimeiroNome")) + " " + queryResult.Valor("UltimoNome"),
                Email = TypeParser.String(queryResult.Valor("Email")),
                Phone = TypeParser.String(queryResult.Valor("Telefone")),
                DateModified = TypeParser.Date(queryResult.Valor("DataUltContacto")),
                MobilePhone = TypeParser.String(queryResult.Valor("Telemovel")),

                Location = new Address
                {
                    PostalCode = TypeParser.String(queryResult.Valor("CodPostal")),
                    Street = TypeParser.String(queryResult.Valor("Morada")),
                    Country = TypeParser.String(queryResult.Valor("Pais")),
                    Parish = TypeParser.String(queryResult.Valor("Localidade")),
                    State = TypeParser.String(queryResult.Valor("Distrito")),
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

            if (PriEngine.Engine.CRM.Contactos.Existe(paramId) == false)
            {
                throw new NotFoundException();
            }

            return Generate(PriEngine.Consulta(new QueryBuilder()
                .FromTable("CONTACTOS")
                .Columns(sqlColumns)
                .Where("CONTACTOS.Contacto", Comparison.Equals, paramId)));
        }

        private static void SetFields(CrmBEContacto selectedRow, Contact paramObject)
        {
            if (paramObject.Name != null)
            {
                selectedRow.set_Nome(paramObject.Name.Trim());
            }

            if (paramObject.Email != null)
            {
                selectedRow.set_Email(paramObject.Email.Trim());
            }

            if (paramObject.Phone != null)
            {
                selectedRow.set_Telefone(paramObject.Phone.Trim());
            }

            if (paramObject.MobilePhone != null)
            {
                selectedRow.set_Telemovel(paramObject.MobilePhone.Trim());
            }

            if (paramObject.DateModified != null)
            {
                selectedRow.set_DataUltContacto(paramObject.DateModified);
            }

            if (paramObject.Location != null)
            {
                var objectLocation = paramObject.Location;

                if (objectLocation.Street != null)
                {
                    selectedRow.set_Morada(paramObject.Location.Street.Trim());
                }

                if (objectLocation.State != null)
                {
                    selectedRow.set_Distrito(paramObject.Location.State.Trim());
                }

                if (objectLocation.Parish != null)
                {
                    selectedRow.set_Localidade(paramObject.Location.Parish.Trim());
                }

                if (objectLocation.PostalCode != null)
                {
                    selectedRow.set_CodPostal(paramObject.Location.PostalCode.Trim());
                }

                if (objectLocation.Country != null)
                {
                    selectedRow.set_Pais(paramObject.Location.Country.Trim());
                }
            }
        }

        public static bool UpdateContact(string paramId, Contact paramObject)
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var selectedTable = PriEngine.Engine.CRM.Contactos;

            if (selectedTable.Existe(paramId) == false)
            {
                return false;
            }

            var selectedRow = selectedTable.Edita(paramId);

            selectedRow.set_EmModoEdicao(true);
            SetFields(selectedRow, paramObject);
            selectedTable.Actualiza(selectedRow);

            return true;
        }

        public static bool CreateContact(string paramId, Contact paramObject)
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var selectedRow = new CrmBEContacto();
            var selectedTable = PriEngine.Engine.CRM.Contactos;

            if (selectedTable.Existe(paramId))
            {
                return false;
            }

            selectedRow.set_Contacto(paramId);
            SetFields(selectedRow, paramObject);
            selectedTable.Actualiza(selectedRow);

            return true;
        }
    }
}