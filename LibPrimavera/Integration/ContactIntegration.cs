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
        private static bool CheckPermissions(CrmBEContacto contactInfo, string sessionId)
        {
            //return contactInfo.get_CriadoPor() == null || contactInfo.get_CriadoPor().Equals(sessionId);
            return true;
        }

        private static SqlColumn[] sqlColumnsListing =
        {
            new SqlColumn("Contacto", null),
            new SqlColumn("PrimeiroNome", null),
            new SqlColumn("UltimoNome", null),
            new SqlColumn("Email", null),
            new SqlColumn("Pais", null),
            new SqlColumn("Telemovel", null),
            new SqlColumn("DataUltContacto", null)
        };

        public static List<ContactListing> List(string sessionId)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var contactList = new List<ContactListing>();
            var contactInfo = PrimaveraEngine.Consulta(new SqlBuilder().FromTable("CONTACTOS").Columns(sqlColumnsListing));

            if (contactInfo == null || contactInfo.Vazia())
            {
                return contactList;
            }

            while (!contactInfo.NoFim())
            {
                contactList.Add(new ContactListing()
                {
                    Identificador = TypeParser.String(contactInfo.Valor("Contacto")),
                    Nome = TypeParser.String(contactInfo.Valor("PrimeiroNome")) + " " + contactInfo.Valor("UltimoNome"),
                    Email = TypeParser.String(contactInfo.Valor("Email")),
                    Telefone = TypeParser.String(contactInfo.Valor("Telemovel")),
                    DataModificacao = TypeParser.Date(contactInfo.Valor("DataUltContacto")),
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
                throw new NotFoundException("contacto", false);
            }

            var contactInfo = contactsTable.Edita(contactId);

            /*if (contactInfo.get_CriadoPor().Equals(sessionId) == false)
            {
                return null;
            }*/

            return GenerateContact(contactsTable.Edita(contactId));
        }

        private static ContactInfo GenerateContact(CrmBEContacto contactInfo)
        {
            return new ContactInfo()
            {
                Identficador = contactInfo.get_Contacto(),
                Cliente = CustomerIntegration.Reference("SILVA"),
                DataModificacao = contactInfo.get_DataUltContacto(),
                Responsavel = UserIntegration.Reference(contactInfo.get_CriadoPor()),
                Nome = contactInfo.get_PrimeiroNome() + " " + contactInfo.get_UltimoNome(),
                Titulo = contactInfo.get_Titulo(),
                Email = contactInfo.get_Email(),
                Telefone = contactInfo.get_Telefone(),
                Telefone2 = contactInfo.get_Telefone2(),
                Telemovel = contactInfo.get_Telemovel(),
                Localizacao = new Address
                {
                    Pais = contactInfo.get_Pais(),
                    Morada = contactInfo.get_Morada(),
                    CodigoPostal = contactInfo.get_CodPostal(),
                    Localidade = contactInfo.get_Localidade(),
                    Distrito = LocationIntegration.DistritoReference(contactInfo.get_Distrito())
                }
            };
        }

        public static EntityReference Reference(string contactId)
        {
            if (string.IsNullOrEmpty(contactId))
            {
                return null;
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
            contactInfo.set_Nome(jsonObject.Nome);
            contactInfo.set_Email(jsonObject.Email);
            contactInfo.set_Titulo(jsonObject.Titulo);
            contactInfo.set_Telefone(jsonObject.Telefone);
            contactInfo.set_Telefone2(jsonObject.Telefone2);
            contactInfo.set_Telemovel(jsonObject.Telemovel);
            contactInfo.set_Pais(jsonObject.Localizacao.Pais);
            contactInfo.set_Morada(jsonObject.Localizacao.Morada);
            contactInfo.set_CodPostal(jsonObject.Localizacao.CodigoPostal);

            if (jsonObject.Localizacao.Pais.Equals("PT"))
            {
                if (jsonObject.Localizacao.Distrito == null)
                {
                    contactInfo.set_Distrito(null);
                    contactInfo.set_Localidade(null);
                    contactInfo.set_CodPostalLocal(null);
                }
                else
                {
                    contactInfo.set_Distrito(jsonObject.Localizacao.Distrito);
                    contactInfo.set_Localidade(jsonObject.Localizacao.Localidade);
                    contactInfo.set_CodPostalLocal(jsonObject.Localizacao.Localidade);
                }
            }
            else
            {
                contactInfo.set_Distrito(null);
                contactInfo.set_Localidade(null);
                contactInfo.set_CodPostalLocal(null);
            }
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
                throw new NotFoundException("contacto", false);
            }

            var contactInfo = contactsTable.Edita(contactId);

            if (CheckPermissions(contactInfo, sessionId) == false)
            {
                return null;
            }

            contactInfo.set_EmModoEdicao(true);
            contactInfo.set_DataUltContacto(DateTime.Now);
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
                throw new EntityExistsException("contacto", false);
            }

            contactInfo.set_Contacto(contactId);
            contactInfo.set_CriadoPor(sessionId);
            contactInfo.set_DataUltContacto(DateTime.Now);
            contactInfo.set_ID(PrimaveraEngine.generateGUID());
            SetFields(contactInfo, jsonObject);
            // falta associar contacto a um cliente
            contactsTable.Actualiza(contactInfo);

            return new ContactListing()
            {
                Identificador = contactId,
                Nome = contactInfo.get_PrimeiroNome() + " " + contactInfo.get_UltimoNome(),
                Email = contactInfo.get_Email(),
                Telefone = contactInfo.get_Telemovel(),
                DataModificacao = contactInfo.get_DataUltContacto()
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
                throw new NotFoundException("contacto", false);
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