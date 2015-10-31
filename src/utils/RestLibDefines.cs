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
        [ContentTypeMetadata(Value = "text/plain", Binary = false)]
        TextPlain,
        [ContentTypeMetadata(Value = "text/html", Binary = false)]
        TextHtml,
        [ContentTypeMetadata(Value = "text/xml", Binary = false)]
        TextXml,
        [ContentTypeMetadata(Value = "text/css", Binary = false)]
        TextCss,
        [ContentTypeMetadata(Value = "application/xml", Binary = false)]
        ApplicationXml,
        //default encoding UTF-8
        [ContentTypeMetadata(Value = "application/json", Binary = false)]
        ApplicationJson,
        //for JSONP
        [ContentTypeMetadata(Value = "application/javascript", Binary = false)]
        ApplicationJs,
        //Image because of IE
        [ContentTypeMetadata(Value = "image/x-icon", Binary = true)]
        ImageXIcon
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    class ContentTypeMetadata : Attribute
    {
        public const string DefaultValue = "text/plain";
        
        public string Value { get; set; }
        public bool Binary { get; set; }

        public ContentTypeMetadata()
        {
            Value = DefaultValue;
            Binary = false;
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

        public static bool IsBinary(this ContentType contentType)
        {
            ContentTypeMetadata md = contentType.GetMetadata();
            if (md != null)
            {
                return md.Binary;
            }
            return false;
        }
    }
}