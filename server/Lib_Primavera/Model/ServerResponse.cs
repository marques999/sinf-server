namespace FirstREST.Lib_Primavera.Model
{
    public class ServerResponse
    {
        public ServerResponse(int paramCode, string paramMessage)
        {
            Code = paramCode;
            Message = paramMessage;
        }

        public int Code
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }
    }
}