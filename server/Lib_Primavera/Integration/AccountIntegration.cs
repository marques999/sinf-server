using FirstREST.Lib_Primavera.Model;
using System;
using System.Collections.Generic;

namespace FirstREST.Lib_Primavera.Integration
{
    public class AccountIntegration
    {
        public static List<Account> listAccounts()
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                return null;
            }

            var accounts = new List<Account>();
            var queryResult = PriEngine.Engine.Consulta("SELECT Cliente, Nome, Moeda, NumContrib as NumContribuinte, Fac_Mor AS campo_exemplo FROM  CLIENTES");

            while (!queryResult.NoFim())
            {
                accounts.Add(new Account
                {
                    ID = queryResult.Valor("Cliente"),
                    NomeCliente = queryResult.Valor("Nome"),
                    Moeda = queryResult.Valor("Moeda"),
                    NumContribuinte = queryResult.Valor("NumContribuinte"),
                    Morada = queryResult.Valor("campo_exemplo")
                });

                queryResult.Seguinte();
            }

            return accounts;
        }

        public static Account getAccount(string paramId)
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                return null;
            }

            if (PriEngine.Engine.Comercial.Clientes.Existe(paramId) == false)
            {
                return null;
            }

            var queryResult = PriEngine.Engine.Comercial.Clientes.Edita(paramId);
           
            return new Account
            {
                ID = queryResult.get_Cliente();
                Nome = queryResult.get_Nome();
                Currency = queryResult.get_Moeda();
                myAccount.NumContribuinte = queryResult.get_NumContribuinte();
                myAccount.Morada = queryResult.get_Morada();
            };
        }

        public static ServerResponse updateAccount(Account myCustomer)
        {
            try
            {
                if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
                {
                    return new ServerResponse(1, "Erro ao abrir a empresa");
                }

                if (PriEngine.Engine.Comercial.Clientes.Existe(myCustomer.ID) == false)
                {
                    return new ServerResponse(1, "O cliente não existe");
                }

                var myAccount = PriEngine.Engine.Comercial.Clientes.Edita(myCustomer.ID);

                myAccount.set_EmModoEdicao(true);
                myAccount.set_Nome(myCustomer.Name);
                myAccount.set_NumContribuinte(myCustomer.NumContribuinte);
                myAccount.set_Moeda(myCustomer.Currency);
                myAccount.set_CodigoPostal(myCustomer.Address.PostalCode);
                myAccount.set_Pais(myCustomer.Address.Coordinates);

                PriEngine.Engine.Comercial.Clientes.Actualiza(myAccount);
            }
            catch (Exception ex)
            {
                return new ServerResponse(1, ex.Message);
            }

            return new ServerResponse(0, "Success");
        }

        public static ServerResponse deleteAccount(string accountId)
        {
            try
            {
                if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
                {
                    return new ServerResponse(1, "Error loading company database!");
                }

                if (PriEngine.Engine.Comercial.Clientes.Existe(accountId))
                {
                    PriEngine.Engine.Comercial.Clientes.Remove(accountId);
                }
                else
                {
                    return new ServerResponse(1, "Customer not found!");
                }
            }
            catch (Exception ex)
            {
                return new ServerResponse(1, ex.Message);
            }

            return new ServerResponse(0, "Success");
        }
    }
}
