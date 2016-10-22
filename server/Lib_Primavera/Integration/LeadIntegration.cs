using FirstREST.Lib_Primavera.Model;
using Interop.GcpBE900;
using System;
using System.Collections.Generic;

namespace FirstREST.Lib_Primavera.Integration
{
    public class LeadIntegration
    {
        public static List<Lead> GetLeads()
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                return null;
            }

            var accounts = new List<Lead>();
            var queryResult = PriEngine.Consulta("SELECT Cliente, Nome, Moeda, NumContrib as NumContribuinte, Fac_Mor AS campo_exemplo FROM CLIENTES");

            while (!queryResult.NoFim())
            {
                accounts.Add(new Lead
                {
                    Nome = queryResult.Valor("Nome"),
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
                return null;
            }

            if (PriEngine.Clientes.Existe(paramId) == false)
            {
                return null;
            }

            var queryResult = PriEngine.Clientes.Edita(paramId);

            return new Lead
            {
                Nome = queryResult.get_Nome(),

                Morada = new Address
                {
                    Rua = queryResult.get_Morada(),
                    CodigoPostal = queryResult.get_CodigoPostal(),
                    Pais = queryResult.get_Pais(),
                    Zona = queryResult.get_Zona()
                }
            };
        }

        public static bool updateLead(string leadId, Lead leadInstance)
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseException("LeadIntegration");
            }

            if (PriEngine.Clientes.Existe(leadId) == false)
            {
                return false;
            }

            var myLead = PriEngine.Clientes.Edita(leadId);

            myLead.set_EmModoEdicao(true);
            myLead.set_Nome(leadInstance.Nome);
            myLead.set_Morada(leadInstance.Morada.Rua);
            myLead.set_Zona(leadInstance.Morada.Zona);
            myLead.set_Pais(leadInstance.Morada.Pais);
            myLead.set_CodigoPostal(leadInstance.Morada.CodigoPostal);
            myLead.set_Pais(leadInstance.Morada.Coordinates);
            PriEngine.Clientes.Actualiza(myLead);

            return true;
        }

        public static bool createLead(string leadId, Lead leadModel)
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseException("LeadIntegration");
            }

            var myAccount = new GcpBECliente();

            myAccount.set_Cliente(leadId);
            myAccount.set_Nome(leadModel.Nome);
            myAccount.set_Morada(leadModel.Morada.Rua);
            PriEngine.Clientes.Actualiza(myAccount);

            return true;
        }
    }
}