using System;
using System.Collections.Generic;

using Interop.GcpBE900;

using FirstREST.Lib_Primavera.Model;

namespace FirstREST.Lib_Primavera.Integration
{
    public class LeadIntegration
    {
        public static List<Lead> GetLeads()
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var accounts = new List<Lead>();
            var queryResult = PriEngine.Consulta("SELECT Cliente, Nome, Moeda, NumContrib as NumContribuinte, Fac_Mor AS campo_exemplo FROM CLIENTES");

            while (!queryResult.NoFim())
            {
                accounts.Add(new Lead
                {
                    Name = queryResult.Valor("Nome"),
                    /*                    Address = new Address
                                        {
                                            Street = queryResult.Valor("Morada"),
                                            PostalCode = queryResult.Valor("CodigoPostal"),
                                            Country = queryResult.Valor("Pais"),
                                            City = queryResult.Valor("Zona")
                                        }*/
                });

                queryResult.Seguinte();
            }

            return accounts;
        }

        public static Lead GetLead(string paramId)
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            if (PriEngine.Clientes.Existe(paramId) == false)
            {
                throw new NotFoundException();
            }

            var queryResult = PriEngine.Clientes.Edita(paramId);

            return new Lead
            {
                Name = queryResult.get_Nome(),

                Location = new Address
                {
                    Street = queryResult.get_Morada(),
                    PostalCode = queryResult.get_CodigoPostal(),
                    Country = queryResult.get_Pais(),
                    City = queryResult.get_Zona()
                }
            };
        }

        public static void updateLead(string paramId, Lead paramObject)
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            if (PriEngine.Clientes.Existe(paramId) == false)
            {
                throw new NotFoundException();
            }

            var myLead = PriEngine.Clientes.Edita(paramId);

            myLead.set_EmModoEdicao(true);
            myLead.set_Nome(paramObject.Name);
            myLead.set_Morada(paramObject.Location.Street);
            myLead.set_Zona(paramObject.Location.City);
            myLead.set_Pais(paramObject.Location.Country);
            myLead.set_CodigoPostal(paramObject.Location.PostalCode);
            myLead.set_Pais(paramObject.Location.Coordinates);
            PriEngine.Clientes.Actualiza(myLead);
        }

        public static void createLead(string paramId, Lead paramObject)
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            if (PriEngine.Clientes.Existe(paramId) == true)
            {
                throw new EntityExistsException();
            }

            var myLead = new GcpBECliente();

            myLead.set_Cliente(paramId);
            myLead.set_Nome(paramObject.Name);
            myLead.set_Morada(paramObject.Location.Street);
            PriEngine.Clientes.Actualiza(myLead);
        }
    }
}