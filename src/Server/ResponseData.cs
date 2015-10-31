using System;
using RestLib.Utils;

namespace RestLib.Server
{
    public struct StringResponseData
    {
        public readonly string Content;
        public readonly ContentType ContentType;

        public StringResponseData(string content)
        {
            this.Content = content;
            this.ContentType = ContentType.TextPlain;
        }

        public StringResponseData(string content, ContentType contentType)
        {
            this.Content = content;
            this.ContentType = contentType;
        }
    }

    public struct ByteResponseData
    {
        public readonly byte[] Content;
        public readonly ContentType ContentType;

        public ByteResponseData(byte[] content)
        {
            this.Content = content;
            this.ContentType = ContentType.TextPlain;
        }

        public ByteResponseData(byte[] content, ContentType contentType)
        {
            this.Content = content;
            this.ContentType = contentType;
        }
    }
}