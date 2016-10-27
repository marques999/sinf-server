namespace FirstREST.Lib_Primavera
{
    public struct SqlColumn
    {
        public string Name;
        public string Alias;

        public SqlColumn(string paramName, string paramAlias)
        {
            Name = paramName;
            Alias = paramAlias;
        }
    }
}