using System;
using System.Collections.Generic;

using Interop.CrmBE900;
using Interop.StdBE900;

using FirstREST.QueryBuilder;
using FirstREST.QueryBuilder.Enums;
using FirstREST.LibPrimavera.Model;

namespace FirstREST.LibPrimavera.Integration
{
    public class ContactIntegration
    {
        private static SqlColumn[] sqlColumnsFull =
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

        private static SqlColumn[] sqlColumnsListing =
        {
            new SqlColumn("CONTACTOS.Contacto", null),
            new SqlColumn("CONTACTOS.Titulo", null),
            new SqlColumn("CONTACTOS.PrimeiroNome", null),
            new SqlColumn("CONTACTOS.UltimoNome", null),
            new SqlColumn("CONTACTOS.DataUltContacto", null),
            new SqlColumn("CONTACTOS.Email", null),
            new SqlColumn("CONTACTOS.Telemovel", null),
            new SqlColumn("CONTACTOS.Pais", null),
            new SqlColumn("CONTACTOS.Distrito", null),
            new SqlColumn("CONTACTOS.Morada", null),
        };

        private static SqlColumn[] sqlColumnsReference =
        {
            new SqlColumn("CONTACTOS.Contacto", null),
            new SqlColumn("CONTACTOS.PrimeiroNome", null),
            new SqlColumn("CONTACTOS.UltimoNome", null),
        };

        private static Contact GenerateFull(StdBELista queryResult)
        {
            return new Contact()
            {
                Identifier = TypeParser.String(queryResult.Valor("Contacto")),
                Name = TypeParser.String(queryResult.Valor("PrimeiroNome")) + " " + queryResult.Valor("UltimoNome"),
                Title = TypeParser.String(queryResult.Valor("Titulo")),
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
                }
            };
        }

        private static ContactListing GenerateListing(StdBELista queryResult)
        {
            return new ContactListing()
            {
                Identifier = TypeParser.String(queryResult.Valor("Contacto")),
                Name = TypeParser.String(queryResult.Valor("PrimeiroNome")) + " " + queryResult.Valor("UltimoNome"),
                Title = TypeParser.String(queryResult.Valor("Titulo")),
                Email = TypeParser.String(queryResult.Valor("Email")),
                DateModified = TypeParser.Date(queryResult.Valor("DataUltContacto")),
                MobilePhone = TypeParser.String(queryResult.Valor("Telemovel")),
                Address = TypeParser.String(queryResult.Valor("Morada")),
                Country = TypeParser.String(queryResult.Valor("Pais")),
                State = TypeParser.String(queryResult.Valor("Distrito"))
            };
        }

        private static Reference GenerateReference(StdBELista queryResult)
        {
            return new Reference
            {
                Identifier = TypeParser.String(queryResult.Valor("Contacto")),
                Name = TypeParser.String(queryResult.Valor("PrimeiroNome")) + " " + queryResult.Valor("UltimoNome"),
            };
        }

        public static List<ContactListing> List(string sessionId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var queryResult = new List<ContactListing>();
            var queryObject = PrimaveraEngine.Consulta(new SqlBuilder().FromTable("CONTACTOS").Columns(sqlColumnsListing));

            while (!queryObject.NoFim())
            {
                queryResult.Add(GenerateListing(queryObject));
                queryObject.Seguinte();
            }

            queryResult.Sort(delegate(ContactListing lhs, ContactListing rhs)
            {
                if (lhs.Identifier == null || rhs.Identifier == null)
                {
                    return -1;
                }

                return lhs.Identifier.CompareTo(rhs.Identifier);
            });

            return queryResult;
        }

        public static Contact View(string sessionId, string paramId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            if (PrimaveraEngine.Engine.CRM.Contactos.Existe(paramId) == false)
            {
                return null;
            }

            return GenerateFull(PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("CONTACTOS")
                .Columns(sqlColumnsFull)
                .Where("CONTACTOS.Contacto", Comparison.Equals, paramId)));
        }

        public static Reference Reference(string paramId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            if (PrimaveraEngine.Engine.CRM.Contactos.Existe(paramId) == false)
            {
                throw new NotFoundException();
            }

            return GenerateReference(PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("CONTACTOS")
                .Columns(sqlColumnsReference)
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

        public static bool Update(string sessionId, Contact paramObject)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var selectedId = paramObject.Identifier;
            var selectedTable = PrimaveraEngine.Engine.CRM.Contactos;

            if (selectedTable.Existe(selectedId) == false)
            {
                return false;
            }

            var selectedRow = selectedTable.Edita(selectedId);

            selectedRow.set_EmModoEdicao(true);
            SetFields(selectedRow, paramObject);
            selectedTable.Actualiza(selectedRow);

            return true;
        }

        public static bool Insert(string sessionId, Contact paramObject)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var selectedRow = new CrmBEContacto();
            var selectedId = paramObject.Identifier;
            var selectedTable = PrimaveraEngine.Engine.CRM.Contactos;

            if (selectedTable.Existe(selectedId))
            {
                return false;
            }

            selectedRow.set_Contacto(selectedId);
            SetFields(selectedRow, paramObject);
            selectedTable.Actualiza(selectedRow);

            return true;
        }

        public static bool Delete(string p, string customerId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            System.Diagnostics.Debug.Print("TESTING DELETE METHOD!");

            return true;
        }
    }
}