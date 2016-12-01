using System;
using System.Data.Common;
using System.Collections.Generic;

using FirstREST.QueryBuilder.Enums;

namespace FirstREST.QueryBuilder
{
    public class WhereStatement : List<List<WhereClause>>
    {
        public int ClauseLevels
        {
            get
            {
                return Count;
            }
        }

        private void AssertLevelExistance(int level)
        {
            if (Count < (level - 1))
            {
                throw new Exception("Level " + level + " not allowed because level " + (level - 1) + " does not exist.");
            }
            else if (Count < level)
            {
                Add(new List<WhereClause>());
            }
        }

        public void Add(WhereClause clause)
        {
            Add(clause, 1);
        }

        public void Add(WhereClause clause, int level)
        {
            AddWhereClauseToLevel(clause, level);
        }

        public WhereClause Add(string field, Comparison @operator, object compareValue)
        {
            return this.Add(field, @operator, compareValue, 1);
        }

        public WhereClause Add(Enum field, Comparison @operator, object compareValue)
        {
            return this.Add(field.ToString(), @operator, compareValue, 1);
        }

        public WhereClause Add(string field, Comparison @operator, object compareValue, int level)
        {
            var NewWhereClause = new WhereClause(field, @operator, compareValue);
            AddWhereClauseToLevel(NewWhereClause, level);
            return NewWhereClause;
        }

        private void AddWhereClause(WhereClause clause)
        {
            AddWhereClauseToLevel(clause, 1);
        }

        private void AddWhereClauseToLevel(WhereClause clause, int level)
        {
            AssertLevelExistance(level);
            this[level - 1].Add(clause);
        }

        public string BuildWhereStatement()
        {
            DbCommand dummyCommand = null;
            return BuildWhereStatement(false, ref dummyCommand);
        }

        public string BuildWhereStatement(bool useCommandObject, ref DbCommand usedDbCommand)
        {
            string Result = "";

            foreach (List<WhereClause> WhereStatement in this)
            {
                string LevelWhere = "";

                foreach (WhereClause Clause in WhereStatement)
                {
                    string WhereClause = "";

                    if (useCommandObject)
                    {
                        string parameterName = string.Format(
                            "@p{0}_{1}",
                            usedDbCommand.Parameters.Count + 1,
                            Clause.FieldName.Replace('.', '_')
                            );
                        var parameter = usedDbCommand.CreateParameter();

                        parameter.ParameterName = parameterName;
                        parameter.Value = Clause.Value;
                        usedDbCommand.Parameters.Add(parameter);
                        WhereClause += CreateComparisonClause(Clause.FieldName, Clause.ComparisonOperator, new SqlLiteral(parameterName));
                    }
                    else
                    {
                        WhereClause = CreateComparisonClause(Clause.FieldName, Clause.ComparisonOperator, Clause.Value);
                    }

                    foreach (WhereClause.SubClause SubWhereClause in Clause.SubClauses)
                    {
                        switch (SubWhereClause.LogicOperator)
                        {
                        case LogicOperator.And:
                            WhereClause += " AND ";
                            break;
                        case LogicOperator.Or:
                            WhereClause += " OR ";
                            break;
                        }

                        if (useCommandObject)
                        {
                            string parameterName = string.Format(
                                "@p{0}_{1}",
                                usedDbCommand.Parameters.Count + 1,
                                Clause.FieldName.Replace('.', '_')
                                );
                            var parameter = usedDbCommand.CreateParameter();

                            parameter.ParameterName = parameterName;
                            parameter.Value = SubWhereClause.Value;
                            usedDbCommand.Parameters.Add(parameter);
                            WhereClause += CreateComparisonClause(Clause.FieldName, SubWhereClause.ComparisonOperator, new SqlLiteral(parameterName));
                        }
                        else
                        {
                            WhereClause += CreateComparisonClause(Clause.FieldName, SubWhereClause.ComparisonOperator, SubWhereClause.Value);
                        }
                    }

                    LevelWhere += "(" + WhereClause + ") AND ";
                }

                LevelWhere = LevelWhere.Substring(0, LevelWhere.Length - 5);

                if (WhereStatement.Count > 1)
                {
                    Result += " (" + LevelWhere + ") ";
                }
                else
                {
                    Result += " " + LevelWhere + " ";
                }

                Result += " OR";
            }

            Result = Result.Substring(0, Result.Length - 2);

            return Result;
        }

        internal static string CreateComparisonClause(string fieldName, Comparison comparisonOperator, object value)
        {
            string Output = "";

            if (value != null && value != System.DBNull.Value)
            {
                switch (comparisonOperator)
                {
                case Comparison.Equals:
                    Output = fieldName + " = " + FormatSQLValue(value);
                    break;
                case Comparison.NotEquals:
                    Output = fieldName + " <> " + FormatSQLValue(value);
                    break;
                case Comparison.GreaterThan:
                    Output = fieldName + " > " + FormatSQLValue(value);
                    break;
                case Comparison.GreaterOrEquals:
                    Output = fieldName + " >= " + FormatSQLValue(value);
                    break;
                case Comparison.LessThan:
                    Output = fieldName + " < " + FormatSQLValue(value);
                    break;
                case Comparison.LessOrEquals:
                    Output = fieldName + " <= " + FormatSQLValue(value);
                    break;
                case Comparison.Like:
                    Output = fieldName + " LIKE " + FormatSQLValue(value);
                    break;
                case Comparison.NotLike:
                    Output = "NOT " + fieldName + " LIKE " + FormatSQLValue(value);
                    break;
                case Comparison.In:
                    Output = fieldName + " IN (" + FormatSQLValue(value) + ")";
                    break;
                }
            }
            else
            {
                if ((comparisonOperator != Comparison.Equals) && (comparisonOperator != Comparison.NotEquals))
                {
                    throw new Exception("Cannot use comparison operator " + comparisonOperator.ToString() + " for NULL values.");
                }
                else
                {
                    switch (comparisonOperator)
                    {
                    case Comparison.Equals:
                        Output = fieldName + " IS NULL";
                        break;
                    case Comparison.NotEquals:
                        Output = "NOT " + fieldName + " IS NULL";
                        break;
                    }
                }
            }

            return Output;
        }

        internal static string FormatSQLValue(object someValue)
        {
            string FormattedValue = "";

            if (someValue == null)
            {
                FormattedValue = "NULL";
            }
            else
            {
                switch (someValue.GetType().Name)
                {
                    case "String":
                        FormattedValue = "'" + ((string)someValue).Replace("'", "''") + "'";
                        break;
                    case "DateTime":
                        FormattedValue = "'" + ((DateTime)someValue).ToString("yyyy/MM/dd hh:mm:ss") + "'";
                        break;
                    case "DBNull":
                        FormattedValue = "NULL";
                        break;
                    case "Boolean":
                        FormattedValue = (bool)someValue ? "1" : "0";
                        break;
                    case "SqlLiteral":
                        FormattedValue = ((SqlLiteral)someValue).Value;
                        break;
                    default:
                        FormattedValue = someValue.ToString();
                        break;
                }
            }
            return FormattedValue;
        }

        public static WhereStatement CombineStatements(WhereStatement statement1, WhereStatement statement2)
        {
            var result = WhereStatement.Copy(statement1);

            for (int i = 0; i < statement2.ClauseLevels; i++)
            {
                var level = statement2[i];

                foreach (var clause in level)
                {
                    for (int j = 0; j < result.ClauseLevels; j++)
                    {
                        result.AddWhereClauseToLevel(clause, j);
                    }
                }
            }

            return result;
        }

        public static WhereStatement Copy(WhereStatement statement)
        {
            int currentLevel = 0;
            var result = new WhereStatement();

            foreach (var level in statement)
            {
                currentLevel++;
                result.Add(new List<WhereClause>());

                foreach (var clause in statement[currentLevel - 1])
                {
                    var clauseCopy = new WhereClause(clause.FieldName, clause.ComparisonOperator, clause.Value);

                    foreach (var subClause in clause.SubClauses)
                    {
                        clauseCopy.SubClauses.Add(new WhereClause.SubClause(subClause.LogicOperator, subClause.ComparisonOperator, subClause.Value));
                    }

                    result[currentLevel - 1].Add(clauseCopy);
                }
            }

            return result;
        }
    }
}