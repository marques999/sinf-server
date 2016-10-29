namespace FirstREST.QueryBuilder
{
    public class SqlLiteral
    {
        private string _value;

        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        public static string StatementRowsAffected = "SELECT @@ROWCOUNT";

        public SqlLiteral(string value)
        {
            _value = value;
        }
    }
}