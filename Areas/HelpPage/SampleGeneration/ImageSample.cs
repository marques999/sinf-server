using System;

namespace FirstREST.Areas.HelpPage
{
    public class ImageSample
    {
        public ImageSample(string src)
        {
            Src = src;
        }

        public string Src
        {
            get;
            private set;
        }

        public override bool Equals(object obj)
        {
            return (obj as ImageSample) != null && Src == (obj as ImageSample).Src;
        }

        public override int GetHashCode()
        {
            return Src.GetHashCode();
        }

        public override string ToString()
        {
            return Src;
        }
    }
}