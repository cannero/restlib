using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestLib.Client;
using RestLib.Utils;

namespace ClientExample
{
    class Program
    {
        static void Main(string[] args)
        {
            SetLoggingEvents();
            Client client = new Client("http://localhost:1234", null);

            RestRequest request = new RestRequest
            {
                Method = HttpMethod.GET,
                Resource = "foo/{id}",
                ContentType = ContentType.TextPlain
            };

            request.AddParameter("id", "myId1");
            request.AddQuery("location", "room");
            request.AddQuery("location", "outside");

            Response response = client.SendRequest(request);

            WriteResponse(response);

            request.Resource = "ex/someException";
            request.ClearQuery();

            response = client.SendRequest(request);
            WriteResponse(response);

            Console.WriteLine("press any key");
            Console.ReadLine();
        }

        private static void SetLoggingEvents()
        {
            RestLogger.ExceptionEvent += (caller, ex) =>
            {
                Console.WriteLine("Server::" + caller + ": ");
                Console.WriteLine(ex);
            };
            RestLogger.WarningEvent += (warning) => Console.WriteLine(warning);
            RestLogger.InfoEvent += (info) => Console.WriteLine(info);
        }

        private static void WriteResponse(Response response)
        {
            if (response.CallSuccessful)
            {
                Console.WriteLine("Call was successful:");
                Console.WriteLine(response.StatusCode);
                Console.WriteLine(response.Content);
            }
            else
            {
                Console.WriteLine("Error during call:");
                Console.WriteLine(response.ErrorStatus);
                Console.WriteLine(response.CompleteErrorMessage);
                Console.WriteLine(response.Content);
            }
        }
    }
}
