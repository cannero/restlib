using System;
using System.Reflection;
using System.Linq;

namespace RestLib.Utils
{
    public enum HttpMethod
    {
        GET,
        PUT,
        POST,
        DELETE
    }

    public enum ContentType
    {
        [ContentTypeMetadata(Value = "text/plain")]
        TextPlain,
        [ContentTypeMetadata(Value = "text/html")]
        TextHtml,
        [ContentTypeMetadata(Value = "text/xml")]
        TextXml,
        [ContentTypeMetadata(Value = "text/css")]
        TextCss,
        [ContentTypeMetadata(Value = "application/xml")]
        ApplicationXml,
        //default encoding UTF-8
        [ContentTypeMetadata(Value = "application/json")]
        ApplicationJson,
        //for JSONP
        [ContentTypeMetadata(Value = "application/javascript")]
        ApplicationJs
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    class ContentTypeMetadata : Attribute
    {
        public const string DefaultValue = "text/plain";
        
        public string Value { get; set; }

        public ContentTypeMetadata()
        {
            Value = DefaultValue;
        }
    }

    public static class ContentTypeExtensions
    {
        private static Type typeContentType = typeof(ContentType);
        private static Type typeContentTypeMetadata = typeof(ContentTypeMetadata);
        
        static ContentTypeMetadata GetMetadata(this ContentType contentType)
        {
            MemberInfo mi = typeContentType.GetMember(contentType.ToString())
                .FirstOrDefault();
            if (mi != null)
            {
                return (ContentTypeMetadata)mi.GetCustomAttributes(typeContentTypeMetadata, false)
                    .FirstOrDefault();
            }
            return null;
        }

        public static string GetValue(this ContentType contentType)
        {
            ContentTypeMetadata md = contentType.GetMetadata();
            if (md != null)
            {
                return md.Value;
            }
            return ContentTypeMetadata.DefaultValue;
        }
    }
}