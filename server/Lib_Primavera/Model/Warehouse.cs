namespace FirstREST.Lib_Primavera.Model
{
    public class Warehouse
    {
        public string ID
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public Address Address
        {
            get
            {
                return Address;
            }
            set
            {
                if (value != null)
                {
                    Address = value;
                }
            }
        }
    }
}