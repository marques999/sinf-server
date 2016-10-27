using FirstREST.Lib_Primavera.Enums;

namespace FirstREST.Lib_Primavera
{
    public struct OrderByClause
    {
        public string FieldName;

        public OrderByClause(string field)
        {
            FieldName = field;
            SortOrder = Sorting.Ascending;
        }

        public Sorting SortOrder;

        public OrderByClause(string field, Sorting order)
        {
            FieldName = field;
            SortOrder = order;
        }
    }
}