using System;

namespace FirstREST.Areas.HelpPage
{
    public class TextSample
    {
        public TextSample(string text)
        {
            Text = text;
        }

        public string Text
        {
            get;
            private set;
        }

        public override bool Equals(object obj)
        {
            return (obj as TextSample) != null && Text == (obj as TextSample).Text;
        }

        public override int GetHashCode()
        {
            return Text.GetHashCode();
        }

        public override string ToString()
        {
            return Text;
        }
    }
}