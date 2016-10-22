using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http.Headers;
using System.IO;
using System.Collections;
using System.Net;
using System.Net.Http.Formatting;

using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System.Threading.Tasks;
using System.Net.Http;

internal class JsonNetMediaTypeFormatter : MediaTypeFormatter
{
    private JsonSerializerSettings serializerSettings;

    public JsonNetMediaTypeFormatter()
        : this(new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })
    {
    }

    public override bool CanReadType(Type type)
    {
        return true;
    }

    public override bool CanWriteType(Type type)
    {
        return true;
    }

    public JsonNetMediaTypeFormatter(JsonSerializerSettings paramSettings)
    {
        serializerSettings = paramSettings;
        SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/json"));
        SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
        SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/bson"));
    }

    public override Task WriteToStreamAsync(Type type, object value, Stream stream, HttpContent contentHeaders, TransportContext context)
    {
        var serializer = JsonSerializer.Create(serializerSettings);
        var writer = GetWriter(contentHeaders.Headers, stream);
        
        return Task.Factory.StartNew(() =>
        {
             if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(IQueryable<>)))
             {
                 serializer.Serialize(writer, ((IEnumerable)value).OfType<object>().ToList());
             }
             else
             {
                 serializer.Serialize(writer, value);
             }

             writer.Flush();
         });
    }

    public override Task<object> ReadFromStreamAsync(Type type, Stream stream, HttpContent contentHeaders, IFormatterLogger formatterLogger)
    {
        return Task.Factory.StartNew(() =>
        {
            return JsonSerializer.Create(serializerSettings).Deserialize(GetReader(contentHeaders.Headers, stream), type);
        });
    }
    
    private static JsonReader GetReader(HttpContentHeaders contentHeaders, Stream stream)
    {
        if (contentHeaders.ContentType.MediaType.EndsWith("json"))
        {
            return new JsonTextReader(new StreamReader(stream));
        }

        return new BsonReader(stream);
    }

    private JsonWriter GetWriter(HttpContentHeaders contentHeaders, Stream stream)
    {
        if (contentHeaders.ContentType.MediaType.EndsWith("json"))
        {
            return new JsonTextWriter(new StreamWriter(stream));
        }

        return new BsonWriter(stream);
    }
}