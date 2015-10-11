using System;
using RestLib.Utils;

namespace RestLib.Server
{
    public struct ResponseData
    {
        public readonly string Content;
        public readonly ContentType ContentType;

        public ResponseData(string content)
        {
            this.Content = content;
            this.ContentType = ContentType.TextPlain;
        }

        public ResponseData(string content, ContentType contentType)
        {
            this.Content = content;
            this.ContentType = contentType;
        }
    }
}