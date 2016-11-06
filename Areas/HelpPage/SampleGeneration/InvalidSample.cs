using System;

namespace FirstREST.Areas.HelpPage
{
    public class InvalidSample
    {
        public InvalidSample(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public string ErrorMessage
        {
            get;
            private set;
        }

        public override bool Equals(object obj)
        {
            return (obj as InvalidSample) != null && ErrorMessage == (obj as InvalidSample).ErrorMessage;
        }

        public override int GetHashCode()
        {
            return ErrorMessage.GetHashCode();
        }

        public override string ToString()
        {
            return ErrorMessage;
        }
    }
}