﻿using System;
using System.Collections.Generic;

using Interop.CrmBE900;
using Interop.StdBE900;

using FirstREST.QueryBuilder;
using FirstREST.QueryBuilder.Enums;
using FirstREST.LibPrimavera.Model;

namespace FirstREST.LibPrimavera.Integration
{
    public class LeadIntegration
    {
        private static SqlColumn[] sqlColummns =
        {
            new SqlColumn("ENTIDADESEXTERNAS.Entidade", null),
            new SqlColumn("ENTIDADESEXTERNAS.Nome", null),
            new SqlColumn("ENTIDADESEXTERNAS.Email", null),
            new SqlColumn("ENTIDADESEXTERNAS.Activo", null),
            new SqlColumn("ENTIDADESEXTERNAS.DataCriacao", null),
            new SqlColumn("ENTIDADESEXTERNAS.DataUltAct", null),
            new SqlColumn("ENTIDADESEXTERNAS.Telefone", null),
            new SqlColumn("ENTIDADESEXTERNAS.Telemovel", null),
            new SqlColumn("ENTIDADESEXTERNAS.CodPostal", null),
            new SqlColumn("ENTIDADESEXTERNAS.Distrito", null),
            new SqlColumn("ENTIDADESEXTERNAS.Localidade", null),
            new SqlColumn("ENTIDADESEXTERNAS.Morada", null),
            new SqlColumn("ENTIDADESEXTERNAS.Pais", null)            
        };

        private static Lead Generate(StdBELista queryResult)
        {
            return new Lead()
            {
                Identifier = TypeParser.String(queryResult.Valor("Entidade")),
                Active = TypeParser.Boolean(queryResult.Valor("Activo")),
                Name = TypeParser.String(queryResult.Valor("Nome")),
                Email = TypeParser.String(queryResult.Valor("Email")),
                Phone = TypeParser.String(queryResult.Valor("Telefone")),
                DateCreated = TypeParser.Date(queryResult.Valor("DataCriacao")),
                DateModified = TypeParser.Date(queryResult.Valor("DataUltAct")),
                MobilePhone = TypeParser.String(queryResult.Valor("Telemovel")),

                Location = new Address
                {
                    PostalCode = TypeParser.String(queryResult.Valor("CodPostal")),
                    State = TypeParser.String(queryResult.Valor("Distrito")),
                    Parish = TypeParser.String(queryResult.Valor("Localidade")),
                    Street = TypeParser.String(queryResult.Valor("Morada")),
                    Country = TypeParser.String(queryResult.Valor("Pais"))
                },
            };
        }

        public static List<Lead> GetLeads(string sessionUsername)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var queryResult = new List<Lead>();
            var queryObject = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("ENTIDADESEXTERNAS")
                .Columns(sqlColummns)
                .Where("PotencialCliente", Comparison.Equals, "TRUE"));

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

        public static Lead GetLead(string sessionUsername, string paramId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            if (PrimaveraEngine.Engine.CRM.EntidadesExternas.Existe(paramId) == false)
            {
                throw new NotFoundException();
            }

            return Generate(PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("ENTIDADESEXTERNAS")
                .Columns(sqlColummns)
                .Where("Entidade", Comparison.Equals, paramId)
                .Where("PotencialCliente", Comparison.Equals, "TRUE")));
        }

        private static SqlColumn[] sqlReference =
        {
            new SqlColumn("ENTIDADESEXTERNAS.Entidade", null),
            new SqlColumn("ENTIDADESEXTERNAS.Nome", null),
        };

        public static UserReference GetReference(string paramId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            if (PrimaveraEngine.Engine.CRM.EntidadesExternas.Existe(paramId) == false)
            {
                throw new NotFoundException();
            }

            var queryObject = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("ENTIDADESEXTERNAS")
                .Columns(sqlReference)
                .Where("ENTIDADESEXTERNAS.Entidade", Comparison.Equals, paramId)
                .Where("PotencialCliente", Comparison.Equals, "TRUE"));

            return new UserReference
            {
                Identifier = TypeParser.String(queryObject.Valor("Entidade")),
                Name = TypeParser.String(queryObject.Valor("Nome"))
            };
        }

        private static void SetFields(CrmBEEntidadeExterna selectedRow, Lead paramObject)
        {
            selectedRow.set_Activo(paramObject.Active);
            selectedRow.set_PotencialCliente(true);

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
                selectedRow.set_DataUltAct(paramObject.DateModified);
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

        public static bool Update(string sessionUsername, Lead paramObject)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var selectedId = paramObject.Identifier;
            var selectedTable = PrimaveraEngine.Engine.CRM.EntidadesExternas;

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

        public static bool Insert(string sessionUsername, Lead paramObject)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var selectedId = paramObject.Identifier;
            var selectedRow = new CrmBEEntidadeExterna();
            var selectedTable = PrimaveraEngine.Engine.CRM.EntidadesExternas;

            if (selectedTable.Existe(selectedId))
            {
                return false;
            }

            selectedRow.set_Entidade(selectedId);
            SetFields(selectedRow, paramObject);
            selectedTable.Actualiza(selectedRow);

            return true;
        }

        public static bool Delete(string sessionId, string paramId)
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