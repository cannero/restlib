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
        ResponseWriter responseWriter;

        public FileResponder(string root, ResponseWriter responseWriter)
        {
            if (!string.IsNullOrEmpty(root))
            {
                if (responseWriter == null)
                {
                    throw new ArgumentNullException("responseWriter");
                }
                this.responseWriter = responseWriter;

                root = Path.GetFullPath(root);
                if (!Directory.Exists(root))
                {
                    RestLogger.LogInfo("FileResponder: creating directory " + root);
                    Directory.CreateDirectory(root);
                }
                RestLogger.LogInfo("FileResponder: web root " + root);
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

            if (url.Contains(".."))
            {
                RestLogger.LogWarning("FileResponder::FileExists: request with step back{0}{1}",
                    Environment.NewLine, url);
                return false;
            }

            string path = GetFullPath(url);
            return File.Exists(path);
        }

        public void SendFileResponse(HttpListenerContext context)
        {
            string url = context.Request.RawUrl;

            if (!FileExists(url))
            {
                return;
            }

            FileInfo fileInfo = new FileInfo(GetFullPath(url));
            string lastModified = fileInfo.LastWriteTime.ToString("R");

            if (RequestParser.LastModifiedSinceEquals(context.Request, lastModified))
            {
                responseWriter.SendNotModified(context.Response);
            }
            else
            {
                SendFileContent(context, fileInfo);
            }
        }

        private string GetFullPath(string url)
        {
            url = url.TrimStart('/');
            string filename = url.Replace('/', Path.DirectorySeparatorChar);
            string path = Path.Combine(root, filename);
            return path;
        }

        private void SendFileContent(HttpListenerContext context, FileInfo fileInfo)
        {
            byte[] fileContent = GetContent(fileInfo);
            ContentType contentType = GetContentType(fileInfo.Extension);
            string lastModified = fileInfo.LastWriteTime.ToString("R");
            string expires = fileInfo.LastWriteTime.AddHours(23).ToString("R");

            responseWriter.AddLastModifiedAndExpires(context.Response, lastModified,
                                                     expires);

            if (RequestParser.GzipCanBeUsed(context.Request) &&
                fileContent.Length > 1024)
            {
                responseWriter.SendZippedResponse(context.Response,
                                                   new ByteResponseData(fileContent,
                                                                        contentType));
            }
            else
            {
                responseWriter.SendResponse(context.Response,
                                             new ByteResponseData(fileContent,
                                                                  contentType));
            }
        }

        private byte[] GetContent(FileInfo fileInfo)
        {
            ContentType contentType = GetContentType(fileInfo.Extension);
            
            byte[] fileContent;
            if(contentType.IsBinary())
            {
                using (BinaryReader reader = new BinaryReader(File.Open(fileInfo.FullName,
                                                                        FileMode.Open,
                                                                        FileAccess.Read)))
                {
                    fileContent = reader.ReadBytes((int)fileInfo.Length);
                }
            }
            else
            {
                using (StreamReader reader = new StreamReader(fileInfo.FullName))
                {
                    fileContent = responseWriter.Encode(reader.ReadToEnd());
                }
            }
            return fileContent;
        }

        private ContentType GetContentType(string extension)
        {
            extension = extension.ToLower().TrimStart('.');
            ContentType contentType = ContentType.TextPlain;

            if (extension == "css")
            {
                contentType = ContentType.TextCss;
            }
            else if (extension == "js")
            {
                contentType = ContentType.ApplicationJs;
            }
            else if (extension == "ico")
            {
                contentType = ContentType.ImageXIcon;
            }
            return contentType;
        }
    }
}
