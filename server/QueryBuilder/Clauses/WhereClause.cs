using System;
using System.Collections.Generic;

using FirstREST.QueryBuilder.Enums;

namespace FirstREST.QueryBuilder
{
    public class WhereClause
    {
        private object m_Value;
        private string m_FieldName;

        internal struct SubClause
        {
            public object Value;

            public SubClause(LogicOperator logic, Comparison compareOperator, object compareValue)
            {
                LogicOperator = logic;
                ComparisonOperator = compareOperator;
                Value = compareValue;
            }

            public LogicOperator LogicOperator;
            public Comparison ComparisonOperator;
        }

        internal List<SubClause> SubClauses;

        public string FieldName
        {
            get
            {
                return m_FieldName;
            }
            private set
            {
                m_FieldName = value;
            }
        }

        private Comparison m_ComparisonOperator;

        public Comparison ComparisonOperator
        {
            get
            {
                return m_ComparisonOperator;
            }
            private set
            {
                m_ComparisonOperator = value;
            }
        }

        public object Value
        {
            get
            {
                return m_Value;
            }
            private set
            {
                m_Value = value;
            }
        }

        public WhereClause(string field, Comparison firstCompareOperator, object firstCompareValue)
        {
            m_FieldName = field;
            m_Value = firstCompareValue;
            m_ComparisonOperator = firstCompareOperator;
            SubClauses = new List<SubClause>();
        }

        public WhereClause(Enum field, Comparison firstCompareOperator, object firstCompareValue)
        {
            m_FieldName = field.ToString();
            m_Value = firstCompareValue;
            m_ComparisonOperator = firstCompareOperator;
            SubClauses = new List<SubClause>();
        }

        public WhereClause AddClause(LogicOperator logic, Comparison compareOperator, object compareValue)
        {
            SubClauses.Add(new SubClause(logic, compareOperator, compareValue));
            return this;
        }
    }
}