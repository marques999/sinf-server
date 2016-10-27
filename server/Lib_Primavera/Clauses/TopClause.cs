using FirstREST.Lib_Primavera.Enums;

namespace FirstREST.Lib_Primavera
{
    public struct TopClause
    {
        public TopUnit Unit;

        public TopClause(int nr)
        {
            Quantity = nr;
            Unit = TopUnit.Records;
        }

        public int Quantity;

        public TopClause(int nr, TopUnit aUnit)
        {
            Quantity = nr;
            Unit = aUnit;
        }
    }
}