using System.Net.Http.Formatting;

namespace FirstREST.Controllers
{
    public class FormatterConfig
    {
        public static void RegisterJsonNetMediaTypeFormatter(MediaTypeFormatterCollection formatters)
        {
            formatters.Clear();
            formatters.Add(new JsonFormatter());
        }
    }
}