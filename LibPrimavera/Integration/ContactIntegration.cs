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
            new SqlColumn("Contacto", null),
            new SqlColumn("Titulo", null),
            new SqlColumn("PrimeiroNome", null),
            new SqlColumn("UltimoNome", null),
            new SqlColumn("DataUltContacto", null),
            new SqlColumn("Email", null),
            new SqlColumn("Telemovel", null),
            new SqlColumn("Pais", null),
            new SqlColumn("Morada", null),
        };

        private static bool CheckPermissions(CrmBEContacto contactInfo, string sessionId)
        {
            var representativeId = contactInfo.get_CriadoPor();

            if (representativeId != null && representativeId != sessionId)
            {
                return false;
            }

            return true;
        }

        public static List<ContactListing> List(string sessionId)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var contactList = new List<ContactListing>();
            var contactInfo = PrimaveraEngine.Consulta(new SqlBuilder().FromTable("CONTACTOS").Columns(sqlColumnsListing));

            while (!contactInfo.NoFim())
            {
                contactList.Add(new ContactListing()
                {
                    Identificador = TypeParser.String(contactInfo.Valor("Contacto")),
                    Nome = TypeParser.String(contactInfo.Valor("PrimeiroNome")) + " " + contactInfo.Valor("UltimoNome"),
                    Titulo = TypeParser.String(contactInfo.Valor("Titulo")),
                    Email = TypeParser.String(contactInfo.Valor("Email")),
                    ModificadoEm = TypeParser.Date(contactInfo.Valor("DataUltContacto")),
                    Telemovel = TypeParser.String(contactInfo.Valor("Telemovel")),
                    Localizacao = TypeParser.String(contactInfo.Valor("Morada")),
                    Pais = TypeParser.String(contactInfo.Valor("Pais")),
                });

                contactInfo.Seguinte();
            }

            contactList.Sort(delegate(ContactListing lhs, ContactListing rhs)
            {
                if (lhs.Nome == null || rhs.Nome == null)
                {
                    return -1;
                }

                return lhs.Nome.CompareTo(rhs.Nome);
            });

            return contactList;
        }

        public static ContactInfo View(string sessionId, string contactId)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var contactsTable = PrimaveraEngine.Engine.CRM.Contactos;

            if (contactsTable.Existe(contactId) == false)
            {
                return null;
            }

            return GenerateContact(contactsTable.Edita(contactId));
        }

        private static ContactInfo GenerateContact(CrmBEContacto contactInfo)
        {
            return new ContactInfo()
            {
                Identficador = contactInfo.get_Contacto(),
                Responsavel = UserIntegration.Reference(contactInfo.get_CriadoPor()),
                DataCriacao = contactInfo.get_DataUltContacto(),
                DataModificacao = contactInfo.get_DataUltContacto(),
                Nome = contactInfo.get_PrimeiroNome() + " " + contactInfo.get_UltimoNome(),
                Titulo = contactInfo.get_Titulo(),
                Email = contactInfo.get_Email(),
                Telefone = contactInfo.get_Telefone(),
                Telefone2 = contactInfo.get_Telefone2(),
                Telemovel = contactInfo.get_Telemovel(),
                Localizacao = new Address
                {
                    CodigoPostal = contactInfo.get_CodPostal(),
                    Morada = contactInfo.get_Morada(),
                    Pais = contactInfo.get_Pais(),
                    Localidade = contactInfo.get_Localidade(),
                    Distrito = LocationIntegration.Reference(contactInfo.get_Distrito())
                }
            };
        }

        public static EntityReference Reference(string contactId)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var contactsTable = PrimaveraEngine.Engine.CRM.Contactos;

            if (contactsTable.Existe(contactId) == false)
            {
                return null;
            }

            return new EntityReference(contactId, EntityType.Contact, contactsTable.DaNomeContacto(contactId));
        }

        private static void SetFields(CrmBEContacto contactInfo, Contact jsonObject)
        {
            if (jsonObject.Nome != null)
                contactInfo.set_Nome(jsonObject.Nome);
            if (jsonObject.Titulo != null)
                contactInfo.set_Titulo(jsonObject.Titulo);
            if (jsonObject.Email != null)
                contactInfo.set_Email(jsonObject.Email);
            if (jsonObject.Telefone != null)
                contactInfo.set_Telefone(jsonObject.Telefone);
            if (jsonObject.Telefone2 != null)
                contactInfo.set_Telefone2(jsonObject.Telefone2);
            if (jsonObject.Telemovel != null)
                contactInfo.set_Telemovel(jsonObject.Telemovel);
            if (jsonObject.Localizacao.Pais != null)
                contactInfo.set_Pais(jsonObject.Localizacao.Pais);
            if (jsonObject.Localizacao.Morada != null)
                contactInfo.set_Morada(jsonObject.Localizacao.Morada);
            if (jsonObject.Localizacao.Distrito != null)
                contactInfo.set_Distrito(jsonObject.Localizacao.Distrito);
            if (jsonObject.Localizacao.Localidade != null)
                contactInfo.set_Localidade(jsonObject.Localizacao.Localidade);
            if (jsonObject.Localizacao.CodigoPostal != null)
                contactInfo.set_CodPostal(jsonObject.Localizacao.CodigoPostal);
            if (jsonObject.Localizacao.Localidade != null)
                contactInfo.set_CodPostalLocal(jsonObject.Localizacao.Localidade);
        }

        public static ContactInfo Update(string sessionId, string contactId, Contact jsonObject)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var contactsTable = PrimaveraEngine.Engine.CRM.Contactos;

            if (contactsTable.Existe(contactId) == false)
            {
                return null;
            }

            var contactInfo = contactsTable.Edita(contactId);

            if (CheckPermissions(contactInfo, sessionId) == false)
            {
                return null;
            }

            contactInfo.set_EmModoEdicao(true);
            SetFields(contactInfo, jsonObject);
            contactsTable.Actualiza(contactInfo);

            return GenerateContact(contactInfo);
        }

        public static ContactListing Insert(string sessionId, Contact jsonObject)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var contactInfo = new CrmBEContacto();
            var contactId = PrimaveraEngine.GenerateHash();
            var contactsTable = PrimaveraEngine.Engine.CRM.Contactos;

            if (contactsTable.Existe(contactId))
            {
                return null;
            }

            contactInfo.set_Contacto(contactId);
            contactInfo.set_CriadoPor(sessionId);
            contactInfo.set_ID(PrimaveraEngine.generateGUID());
            SetFields(contactInfo, jsonObject);
            contactsTable.Actualiza(contactInfo);

            return new ContactListing()
            {
                Identificador = contactId,
                Nome = contactInfo.get_PrimeiroNome() + " " + contactInfo.get_UltimoNome(),
                Titulo = contactInfo.get_Titulo(),
                Email = contactInfo.get_Email(),
                ModificadoEm = contactInfo.get_DataUltContacto(),
                Telemovel = contactInfo.get_Telemovel(),
                Localizacao = contactInfo.get_Morada(),
                Pais = contactInfo.get_Pais()
            };
        }

        public static bool Delete(string sessionId, string contactId)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var contactsTable = PrimaveraEngine.Engine.CRM.Contactos;

            if (contactsTable.Existe(contactId) == false)
            {
                return false;
            }

            if (CheckPermissions(contactsTable.Edita(contactId), sessionId) == false)
            {
                return false;
            }

            contactsTable.Remove(contactId);

            return true;
        }
    }
}