using System;

namespace FirstREST.Areas.HelpPage
{
    public class InvalidSample
    {
        public InvalidSample(string errorMessage)
        {
            if (errorMessage == null)
            {
                throw new ArgumentNullException("errorMessage");
            }

            ErrorMessage = errorMessage;
        }

        public string ErrorMessage
        {
            get;
            private set;
        }

        public override bool Equals(object obj)
        {
            var other = obj as InvalidSample;
            return other != null && ErrorMessage == other.ErrorMessage;
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