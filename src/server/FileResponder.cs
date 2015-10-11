using System;
using System.IO;
using System.Net;

namespace RestLib.Server
{
    public class FileResponder
    {
        readonly bool rootExists = false;
        readonly string root = "";

        public FileResponder(string root)
        {
            if (!string.IsNullOrEmpty(root))
            {
                if (!Directory.Exists(root))
                {
                    Directory.CreateDirectory(root);
                }
                this.rootExists = true;
                this.root = root;
            }
        }

        public bool FileExists(string path)
        {
            if (!rootExists)
            {
                return false;
            }
            throw new NotImplementedException();
        }

        public void SendFileResponse(HttpListenerContext context)
        {
            throw new NotImplementedException();
        }
    }
}
