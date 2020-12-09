using System;
using System.IO;
using System.Net;
using System.Text;
using Azure.Storage.Queues;
using static RepoStats.Constants;

namespace RepoStats
    {
    public class RepoParserAPI
        {
        private const string webJobLogin = "$RepoStats:5iRlfMqocveaNQNC5kGmiaABmbb8pvSpwicNsFG2XwcHwjRR7gbl7Kn48j0Y";

        private static bool IsRunning()
            {
            var request = WebRequest.Create("https://repostats.scm.azurewebsites.net/api/triggeredwebjobs/RepoParser");
            request.Method = "GET";
            var byteArray = Encoding.ASCII.GetBytes(webJobLogin);
            request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(byteArray));
            request.ContentLength = 0;

            var response = request.GetResponse();
            using (var reader = new StreamReader(response.GetResponseStream()))
                {
                string data = reader.ReadToEnd();
                return data.Contains("\"status\":\"Running\"");
                }
            }

        public static void EnsureIsRunning()
            {
            if (IsRunning())
                return;

            var request = WebRequest.Create("https://repostats.scm.azurewebsites.net/api/triggeredwebjobs/RepoParser/run");
            request.Method = "POST";
            var byteArray = Encoding.ASCII.GetBytes(webJobLogin);
            request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(byteArray));
            request.ContentLength = 0;

            request.GetResponse();
            }

        public static void ProcessRepository(string storageConnectionString, string repoId, string repoOwner, string repoName)
            {
            string requestJson = $@"{{ ""{JSON_ID}"": ""{repoId}"", ""{JSON_OWNER}"": ""{repoOwner}"", ""{JSON_NAME}"": ""{repoName}"" }}";
            var requestBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(requestJson));

            QueueClient queueClient = new QueueClient(storageConnectionString, "queue");
            queueClient.SendMessage(requestBase64);
            }
        }
    }
