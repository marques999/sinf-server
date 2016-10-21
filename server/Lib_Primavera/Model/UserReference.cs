namespace FirstREST.Lib_Primavera.Model
{
    public class UserReference
    {
        public UserReference(string paramId, string paramFirst, string paramLast)
        {
            ID = paramId;
            FirstName = paramFirst;
            LastName = paramLast;
        }

        public string ID
        {
            get;
            set;
        }

        public string FirstName
        {
            get;
            set;
        }

        public string LastName
        {
            get;
            set;
        }

        public string Name
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
    }
}