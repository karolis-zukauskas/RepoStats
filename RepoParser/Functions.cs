using System.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.Azure.WebJobs;
using Octokit;
using Newtonsoft.Json;
using static RepoParser.Constants;

namespace RepoParser
    {
    public class ProcessRepositoryRequest
        {
        public string Id { get; set; }
        public string Owner { get; set; }
        public string Name { get; set; }
        }

    public class MessageQueueProcessor
        {
        public static GitHubClient GetClient()
            {
            var client = new GitHubClient(new ProductHeaderValue("repo-stats"));

            string token = ConfigurationManager.AppSettings.Get("GitHubToken");
            client.Credentials = new Credentials(token);

            return client;
            }

        public static async Task Test()
            {
            GitHubClient client = GetClient();
            var repo = JsonConvert.DeserializeObject<ProcessRepositoryRequest>("{ \"repoOwner\": \"karolis\", \"repoName\": \"repo-name\" }");

            var result = await client.Repository.Statistics.GetParticipation("karolis-zukauskas", "APSP");

            return;
            }

        public static async Task ProcessRepository([QueueTrigger("queue")] string message, TextWriter log)
            {
            // Examples: https://docs.github.com/en/free-pro-team@latest/rest/reference/repos#list-repository-languages
            // TOKEN: ff6388318e6086f012f1548da41f1775210100c5

            var connectionString = ConfigurationManager.ConnectionStrings["DbConnectionString"].ToString();
            var repo = JsonConvert.DeserializeObject<ProcessRepositoryRequest>(message);
            var client = GetClient();

            using (SqlConnection connection = new SqlConnection(connectionString))
                {
                connection.Open();

                try
                    {
                    DbStatements.UpdateRepoStatus(connection, repo, PARSE_STATUS_PARSING);

                    var languages = await client.Repository.GetAllLanguages(repo.Owner, repo.Name);
                    foreach (var language in languages)
                        DbStatements.InsertLanguage(connection, repo, language);

                    var contributors = await client.Repository.GetAllContributors(repo.Owner, repo.Name);
                    foreach (var contributor in contributors)
                        DbStatements.InsertContributor(connection, repo, contributor);

                    DbStatements.UpdateRepoStatus(connection, repo, PARSE_STATUS_DONE);
                    }
                catch (Exception e)
                    {
                    log.WriteLine($"Failed to process repository: {repo.Owner}/{repo.Name}. Error: {e.Message}");

                    DbStatements.UpdateRepoStatus(connection, repo, PARSE_STATUS_FAILED);
                    }
                }
            }
        }
    }
