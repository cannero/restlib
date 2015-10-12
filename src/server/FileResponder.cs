using System;
using System.IO;
using System.Net;
using RestLib.Utils;

namespace RestLib.Server
{
    public class FileResponder
    {
        readonly bool rootExists = false;
        readonly string root = "";
        ResponseWriter responseWriter = new ResponseWriter();

        public FileResponder(string root)
        {
            if (!string.IsNullOrEmpty(root))
            {
                root = Path.GetFullPath(root);
                if (!Directory.Exists(root))
                {
                    Console.WriteLine("creating " + root);
                    Directory.CreateDirectory(root);
                }
                Console.WriteLine("using " + root);
                this.rootExists = true;
                this.root = root;
            }
        }

        public bool FileExists(string url)
        {
            if (!rootExists ||
                string.IsNullOrEmpty(url))
            {
                return false;
            }

            url = url.TrimStart('/');
            string filename = url.Replace('/', Path.DirectorySeparatorChar);
            string path = Path.Combine(root, filename);
            Console.WriteLine("searching for file " + path);
            return File.Exists(path);
        }

        public void SendFileResponse(HttpListenerContext context)
        {
            string url = context.Request.RawUrl;

            if (!FileExists(url))
            {
                return;
            }

            url = url.TrimStart('/');
            string filename = url.Replace('/', Path.DirectorySeparatorChar);
            string path = Path.Combine(root, filename);

            DateTime lastWriteTime = File.GetLastWriteTimeUtc(path);
            string lastModified = lastWriteTime.ToString("R");
            string expires = lastWriteTime.AddHours(23).ToString("R");

            if (RequestParser.LastModifiedSinceEquals(context.Request, lastModified))
            {
                responseWriter.WriteNotModified(context.Response);
                return;
            }

            string fileContent = "";
            using(StreamReader reader = new StreamReader(path))
            {
                fileContent = reader.ReadToEnd();
            }

            ContentType contentType = ContentType.TextPlain;
            string extension = Path.GetExtension(path).ToLower().TrimStart('.');
            if (extension == "css")
            {
                contentType = ContentType.TextCss;
            }
            else if (extension == "js")
            {
                contentType = ContentType.ApplicationJs;
            }

            responseWriter.AddLastModifiedAndExpires(context.Response, lastModified, expires);

            if (RequestParser.GzipCanBeUsed(context.Request) &&
                fileContent.Length > 1024)
            {
                responseWriter.WriteZippedResponse(context.Response,
                                                   new ResponseData(fileContent,
                                                                    contentType));
            }
            else
            {
                responseWriter.WriteResponse(context.Response,
                                             new ResponseData(fileContent,
                                                              contentType));
            }
        }
    }
}
