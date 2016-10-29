using FirstREST.QueryBuilder.Enums;

namespace FirstREST.QueryBuilder
{
    public struct JoinClause
    {
        public string FromTable;
        public string FromColumn;
        public string ToTable;
        public string ToColumn;

        public JoinClause(JoinType join, string toTableName, string toColumnName, Comparison @operator, string fromTableName, string fromColumnName)
        {
            JoinType = join;
            FromTable = fromTableName;
            FromColumn = fromColumnName;
            ComparisonOperator = @operator;
            ToTable = toTableName;
            ToColumn = toColumnName;
        }

        public JoinType JoinType;
        public Comparison ComparisonOperator;
    }
}