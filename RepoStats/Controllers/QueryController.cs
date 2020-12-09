using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using static RepoStats.Constants;

namespace RepoStats.Controllers
    {
    [ApiController]
    [Route("[controller]")]
    public class QueryController : ControllerBase
        {
        private readonly IOptions<ConnectionConfig> m_config;

         public QueryController(IOptions<ConnectionConfig> config)
            {
            this.m_config = config;
            }

        [HttpGet("Report")]
        public async Task<ActionResult> Report(string repoUrl)
            {
            try {
                RepoParserAPI.EnsureIsRunning();

                string[] parts = repoUrl.Split("/");
                if (parts.Length < 2)
                    return BadRequest("Wrong repository identifier. Value must be in the form: '{repositoryOwner}/{repositoryName}'.");

                string repoOwner = parts[0];
                string repoName = parts[1];
                string baseUrl = "https://app.powerbi.com/reportEmbed?reportId=f24e2ae2-f708-43c0-8923-ae68091c7e9f&autoAuth=true&ctid=82c51a82-548d-43ca-bcf9-bf4b7eb1d012&config=eyJjbHVzdGVyVXJsIjoiaHR0cHM6Ly93YWJpLW5vcnRoLWV1cm9wZS1yZWRpcmVjdC5hbmFseXNpcy53aW5kb3dzLm5ldC8ifQ%3D%3D";

                using (SqlConnection connection = new SqlConnection(m_config.Value.DbConnectionString))
                    {
                    connection.Open();

                    RepositoryState repo = DbStatements.GetRepository(connection, $"{repoOwner}/{repoName}");
                    switch (repo.ParseStatus)
                        {
                        case PARSE_STATUS_DONE:
                            {
                            string iframeUrl = $"{baseUrl}&$filter=Languages/RepoId%20eq%20%27{repo.Id}%27%20and%20Contributors/RepoId%20eq%20%27{repo.Id}%27";
                            // TODO: Check date time when repository was processed - restart if expired

                            return Ok(iframeUrl);
                            }
                        case PARSE_STATUS_QUEUED:
                        case PARSE_STATUS_PARSING:
                            return Accepted();
                        case PARSE_STATUS_FAILED:
                            return Forbid(); // TODO: Handle repos that failed to parse
                        case PARSE_STATUS_NONE:
                            {
                            repo.Id = Guid.NewGuid().ToString();
                            repo.Url = $"{repoOwner}/{repoName}";
                            repo.ParseStatus = PARSE_STATUS_QUEUED;

                            DbStatements.InsertRepository(connection, repo);
                            RepoParserAPI.ProcessRepository(m_config.Value.StorageConnectionString, repo.Id, repoOwner, repoName);

                            return Accepted();
                            }
                        default:
                            throw new NotImplementedException();
                        }
                    }
                }
            catch (Exception e)
                {
                return BadRequest(e);
                }
            }
        }
    }
