using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoParser
    {
    // To learn more about Microsoft Azure WebJobs SDK, please see https://go.microsoft.com/fwlink/?LinkID=320976
    class Program
        {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
            {
            var config = new JobHostConfiguration();

            if (config.IsDevelopment)
                {
                config.UseDevelopmentSettings();
                }

            var host = new JobHost(config);

            //string request = @"{""Id"": ""2957CDC6-7205-44A7-982C-09C1E1029CB3"", ""Owner"": ""karolis-zukauskas"", ""Name"": ""APSP"" }";
            //MessageQueueProcessor.ProcessRepository(request, null).GetAwaiter().GetResult();

            // The following code ensures that the WebJob will be running continuously
            host.RunAndBlock();
            }
        }
    }
