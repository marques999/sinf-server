namespace FirstREST.LibPrimavera.Model
{
    public class Session
    {
        public Session()
        {
        }

        public Session(string userName, string representativeId)
        {
            Username = userName;
            ID = representativeId;
        }

        public string Username
        {
            get;
            set;
        }

        public string ID
        {
            get;
            set;
        }
    }
}