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

        private static Contact GenerateFull(CrmBEContacto contactInfo)
        {
            return new Contact()
            {
                Identficador = contactInfo.get_Contacto(),
                NomeFiscal = contactInfo.get_PrimeiroNome() + " " + contactInfo.get_UltimoNome(),
                Titulo = contactInfo.get_Titulo(),
                Email = contactInfo.get_Email(),
                Telefone = contactInfo.get_Telefone(),
                Telefone2 = contactInfo.get_Telefone2(),
                Telemovel = contactInfo.get_Telemovel(),
                ModificadoEm = contactInfo.get_DataUltContacto(),
                Localizacao = new Address
                {
                    CodigoPostal = contactInfo.get_CodPostal(),
                    Morada = contactInfo.get_Morada(),
                    Pais = contactInfo.get_Pais(),
                    Localidade = contactInfo.get_Localidade(),
                    Distrito = contactInfo.get_Distrito()
                }
            };
        }

        private static ContactListing GenerateListing(StdBELista contactInfo)
        {
            return new ContactListing()
            {
                Identifier = TypeParser.String(contactInfo.Valor("Contacto")),
                Name = TypeParser.String(contactInfo.Valor("PrimeiroNome")) + " " + contactInfo.Valor("UltimoNome"),
                Title = TypeParser.String(contactInfo.Valor("Titulo")),
                Email = TypeParser.String(contactInfo.Valor("Email")),
                DateModified = TypeParser.Date(contactInfo.Valor("DataUltContacto")),
                MobilePhone = TypeParser.String(contactInfo.Valor("Telemovel")),
                Address = TypeParser.String(contactInfo.Valor("Morada")),
                Country = TypeParser.String(contactInfo.Valor("Pais")),
                State = TypeParser.String(contactInfo.Valor("Distrito"))
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
                if (lhs.Name == null || rhs.Name == null)
                {
                    return -1;
                }

                return lhs.Name.CompareTo(rhs.Name);
            });

            return queryResult;
        }

        public static Contact View(string sessionId, string contactId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var contactsTable = PrimaveraEngine.Engine.CRM.Contactos;

            if (contactsTable.Existe(contactId) == false)
            {
                return null;
            }

            return GenerateFull(contactsTable.Edita(contactId));
        }

        public static Reference Reference(string contactId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var contactsTable = PrimaveraEngine.Engine.CRM.Contactos;

            if (contactsTable.Existe(contactId) == false)
            {
                return null;
            }

            return new Reference(contactId, contactsTable.DaNomeContacto(contactId));
        }

        private static void SetFields(CrmBEContacto contactInfo, Contact jsonObject)
        {
            if (jsonObject.NomeFiscal != null)
            {
                contactInfo.set_Nome(jsonObject.NomeFiscal.Trim());
            }

            if (jsonObject.Email != null)
            {
                contactInfo.set_Email(jsonObject.Email.Trim());
            }

            if (jsonObject.Telefone != null)
            {
                contactInfo.set_Telefone(jsonObject.Telefone.Trim());
            }

            if (jsonObject.Telemovel != null)
            {
                contactInfo.set_Telemovel(jsonObject.Telemovel.Trim());
            }

            if (jsonObject.ModificadoEm != null)
            {
                contactInfo.set_DataUltContacto(jsonObject.ModificadoEm);
            }

            if (jsonObject.Localizacao != null)
            {
                var objectLocation = jsonObject.Localizacao;

                if (objectLocation.Morada != null)
                {
                    contactInfo.set_Morada(jsonObject.Localizacao.Morada.Trim());
                }

                if (objectLocation.Distrito != null)
                {
                    contactInfo.set_Distrito(jsonObject.Localizacao.Distrito.Trim());
                }

                if (objectLocation.Localidade != null)
                {
                    contactInfo.set_Localidade(jsonObject.Localizacao.Localidade.Trim());
                }

                if (objectLocation.CodigoPostal != null)
                {
                    contactInfo.set_CodPostal(jsonObject.Localizacao.CodigoPostal.Trim());
                }

                if (objectLocation.Pais != null)
                {
                    contactInfo.set_Pais(jsonObject.Localizacao.Pais.Trim());
                }
            }
        }

        public static bool Update(string sessionId, string contactId, Contact jsonObject)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var contactsTable = PrimaveraEngine.Engine.CRM.Contactos;

            if (contactsTable.Existe(contactId) == false)
            {
                return false;
            }

            var contactInfo = contactsTable.Edita(contactId);

            if (contactInfo.get_CriadoPor() != sessionId)
            {
                return false;
            }

            jsonObject.ModificadoEm = DateTime.Now;
            contactInfo.set_EmModoEdicao(true);
            SetFields(contactInfo, jsonObject);
            contactsTable.Actualiza(contactInfo);

            return true;
        }

        public static bool Insert(string sessionId, Contact jsonObject)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var contactInfo = new CrmBEContacto();
            var contactId = new HashGenerator().EncodeLong(DateTime.Now.Ticks);
            var contactsTable = PrimaveraEngine.Engine.CRM.Contactos;

            if (contactsTable.Existe(contactId))
            {
                return false;
            }

            jsonObject.ModificadoEm = DateTime.Now;
            contactInfo.set_Contacto(contactId);
            SetFields(contactInfo, jsonObject);
            contactsTable.Actualiza(contactInfo);

            return true;
        }

        public static bool Delete(string sessionId, string contactId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var contactsTable = PrimaveraEngine.Engine.CRM.Contactos;

            if (contactsTable.Existe(contactId) == false)
            {
                return false;
            }

            if (contactsTable.Edita(contactId).get_CriadoPor() != sessionId)
            {
                return false;
            }

            contactsTable.Remove(contactId);

            return true;
        }
    }
}