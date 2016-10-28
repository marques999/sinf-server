using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;

using FirstREST.Lib_Primavera.Enums;

namespace FirstREST.Lib_Primavera
{
    public class QueryBuilder : IQueryBuilder
    {
        protected bool _distinct = false;
        protected TopClause _topClause = new TopClause(100, TopUnit.Percent);
        private List<SqlColumn> _selectedColumns = new List<SqlColumn>();
        protected List<string> _selectedTables = new List<string>();
        protected List<JoinClause> _joins = new List<JoinClause>();
        protected WhereStatement _whereStatement = new WhereStatement();
        protected List<OrderByClause> _orderByStatement = new List<OrderByClause>();
        protected List<string> _groupByColumns = new List<string>();
        protected WhereStatement _havingStatement = new WhereStatement();

        internal WhereStatement WhereStatement
        {
            get
            {
                return _whereStatement;
            }
            set
            {
                _whereStatement = value;
            }
        }

        public QueryBuilder()
        {
        }

        private bool Distinct
        {
            get
            {
                return _distinct;
            }
            set
            {
                _distinct = value;
            }
        }

        public int TopRecords
        {
            get
            {
                return _topClause.Quantity;
            }
            set
            {
                _topClause.Quantity = value;
                _topClause.Unit = TopUnit.Records;
            }
        }

        public TopClause TopClause
        {
            get
            {
                return _topClause;
            }
            set
            {
                _topClause = value;
            }
        }

        public string[] SelectedTables
        {
            get
            {
                return _selectedTables.ToArray();
            }
        }

        public void SelectAllColumns()
        {
            _selectedColumns.Clear();
        }

        public QueryBuilder SelectCount()
        {
            return Column("count(*)", "Count");
        }

        public QueryBuilder Column(string column)
        {
            return Column(column, null);
        }

        public QueryBuilder Column(string column, string columnAs)
        {
            _selectedColumns.Add(new SqlColumn
            {
                Name = column,
                Alias = columnAs
            });

            return this;
        }

        public QueryBuilder Columns(params string[] columns)
        {
            _selectedColumns.Clear();

            foreach (string column in columns)
            {
                _selectedColumns.Add(new SqlColumn
                {
                    Name = column,
                    Alias = null
                });
            }

            return this;
        }

        public QueryBuilder Columns(params SqlColumn[] columns)
        {
            _selectedColumns.Clear();
            _selectedColumns.AddRange(columns);

            return this;
        }

        public QueryBuilder FromTable(string table)
        {
            _selectedTables.Add(table);

            return this;
        }

        public QueryBuilder SelectFromTables(params string[] tables)
        {
            _selectedTables.Clear();
            _selectedTables.AddRange(tables);

            return this;
        }

        public QueryBuilder AddJoin(JoinClause newJoin)
        {
            _joins.Add(newJoin);

            return this;
        }

        public QueryBuilder LeftJoin(string toTableName, string toColumnName, Comparison @operator, string fromTableName, string fromColumnName)
        {
            return Join(JoinType.LeftJoin, toTableName, toColumnName, @operator, fromTableName, fromColumnName);
        }

        public QueryBuilder RightJoin(string toTableName, string toColumnName, Comparison @operator, string fromTableName, string fromColumnName)
        {
            return Join(JoinType.RightJoin, toTableName, toColumnName, @operator, fromTableName, fromColumnName);
        }

        public QueryBuilder InnerJoin(string toTableName, string toColumnName, Comparison @operator, string fromTableName, string fromColumnName)
        {
            return Join(JoinType.InnerJoin, toTableName, toColumnName, @operator, fromTableName, fromColumnName);
        }

        public QueryBuilder OuterJoin(string toTableName, string toColumnName, Comparison @operator, string fromTableName, string fromColumnName)
        {
            return Join(JoinType.OuterJoin, toTableName, toColumnName, @operator, fromTableName, fromColumnName);
        }

        private QueryBuilder Join(JoinType join, string toTableName, string toColumnName, Comparison @operator, string fromTableName, string fromColumnName)
        {
            _joins.Add(new JoinClause(join, toTableName, toColumnName, @operator, fromTableName, fromColumnName));

            return this;
        }

        public WhereStatement MyWhere
        {
            get
            {
                return _whereStatement;
            }
            set
            {
                _whereStatement = value;
            }
        }

        public QueryBuilder Where(WhereClause clause)
        {
            return Where(clause, 1);
        }

        public QueryBuilder Where(WhereClause clause, int level)
        {
            _whereStatement.Add(clause, level);

            return this;
        }

        public QueryBuilder Where(string field, Comparison @operator, object compareValue)
        {
            Where(field, @operator, compareValue, 1);

            return this;
        }

        public WhereClause Where(Enum field, Comparison @operator, object compareValue)
        {
            return Where(field.ToString(), @operator, compareValue, 1);
        }

        public WhereClause Where(string field, Comparison @operator, object compareValue, int level)
        {
            WhereClause NewWhereClause = new WhereClause(field, @operator, compareValue);
            _whereStatement.Add(NewWhereClause, level);
            return NewWhereClause;
        }

        public void AddOrderBy(OrderByClause clause)
        {
            _orderByStatement.Add(clause);
        }

        public void AddOrderBy(Enum field, Sorting order)
        {
            this.AddOrderBy(field.ToString(), order);
        }

        public void AddOrderBy(string field, Sorting order)
        {
            _orderByStatement.Add(new OrderByClause(field, order));
        }

        public QueryBuilder GroupBy(params string[] columns)
        {
            foreach (string Column in columns)
            {
                _groupByColumns.Add(Column);
            }

            return this;
        }

        public WhereStatement Having
        {
            get
            {
                return _havingStatement;
            }
            set
            {
                _havingStatement = value;
            }
        }

        public void AddHaving(WhereClause clause)
        {
            AddHaving(clause, 1);
        }

        public void AddHaving(WhereClause clause, int level)
        {
            _havingStatement.Add(clause, level);
        }

        public WhereClause AddHaving(string field, Comparison @operator, object compareValue)
        {
            return AddHaving(field, @operator, compareValue, 1);
        }

        public WhereClause AddHaving(Enum field, Comparison @operator, object compareValue)
        {
            return AddHaving(field.ToString(), @operator, compareValue, 1);
        }

        public WhereClause AddHaving(string field, Comparison @operator, object compareValue, int level)
        {
            WhereClause NewWhereClause = new WhereClause(field, @operator, compareValue);
            _havingStatement.Add(NewWhereClause, level);
            return NewWhereClause;
        }

        public string BuildQuery()
        {
            string Query = "SELECT ";

            if (_distinct)
            {
                Query += "DISTINCT ";
            }

            if (!(_topClause.Quantity == 100 & _topClause.Unit == TopUnit.Percent))
            {
                Query += "TOP " + _topClause.Quantity;

                if (_topClause.Unit == TopUnit.Percent)
                {
                    Query += " PERCENT";
                }

                Query += " ";
            }

            if (_selectedColumns.Count == 0)
            {
                if (_selectedTables.Count == 1)
                {
                    Query += _selectedTables[0] + ".";
                }

                Query += "*";
            }
            else
            {
                foreach (var CurrentColumn in _selectedColumns)
                {
                    if (CurrentColumn.Alias == null)
                    {
                        Query += CurrentColumn.Name + ',';
                    }
                    else
                    {
                        Query += CurrentColumn.Name + " AS " + CurrentColumn.Alias + ',';
                    }
                }

                Query = Query.TrimEnd(',');
                Query += ' ';
            }

            if (_selectedTables.Count > 0)
            {
                Query += " FROM ";

                foreach (string TableName in _selectedTables)
                {
                    Query += TableName + ',';
                }

                Query = Query.TrimEnd(',');
                Query += ' ';
            }

            if (_joins.Count > 0)
            {
                foreach (JoinClause Clause in _joins)
                {
                    string JoinString = "";

                    switch (Clause.JoinType)
                    {
                        case JoinType.InnerJoin:
                            JoinString = "INNER JOIN";
                            break;
                        case JoinType.OuterJoin:
                            JoinString = "OUTER JOIN";
                            break;
                        case JoinType.LeftJoin:
                            JoinString = "LEFT JOIN";
                            break;
                        case JoinType.RightJoin:
                            JoinString = "RIGHT JOIN";
                            break;
                    }

                    JoinString += " " + Clause.ToTable + " ON ";
                    JoinString += WhereStatement.CreateComparisonClause(Clause.FromTable + '.' + Clause.FromColumn, Clause.ComparisonOperator, new SqlLiteral(Clause.ToTable + '.' + Clause.ToColumn));
                    Query += JoinString + ' ';
                }
            }

            if (_whereStatement.ClauseLevels > 0)
            {
                Query += " WHERE " + _whereStatement.BuildWhereStatement();
            }

            if (_groupByColumns.Count > 0)
            {
                Query += " GROUP BY ";

                foreach (string Column in _groupByColumns)
                {
                    Query += Column + ',';
                }

                Query = Query.TrimEnd(',');
                Query += ' ';
            }

            if (_havingStatement.ClauseLevels > 0)
            {
                if (_groupByColumns.Count == 0)
                {
                    throw new Exception("Having statement was set without Group By");
                }

                Query += " HAVING " + _havingStatement.BuildWhereStatement();
            }

            if (_orderByStatement.Count > 0)
            {
                Query += " ORDER BY ";

                foreach (OrderByClause Clause in _orderByStatement)
                {
                    string OrderByClause = "";

                    switch (Clause.SortOrder)
                    {
                        case Sorting.Ascending:
                            OrderByClause = Clause.FieldName + " ASC";
                            break;
                        case Sorting.Descending:
                            OrderByClause = Clause.FieldName + " DESC";
                            break;
                    }

                    Query += OrderByClause + ',';
                }

                Query = Query.TrimEnd(',');
                Query += ' ';
            }

            return Query;
        }
    }
}