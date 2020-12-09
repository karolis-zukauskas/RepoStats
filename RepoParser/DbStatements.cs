using System.Data.SqlClient;
using Octokit;

using static RepoParser.Constants;

namespace RepoParser
    {
    static class DbStatements
        {
        public static int UpdateRepoStatus(SqlConnection connection, ProcessRepositoryRequest repo, int parseStatus)
            {
            var command = new SqlCommand($"UPDATE RepoStats.{TABLE_REPOS} SET {COLUMN_PARSE_STATUS} = @0 WHERE {COLUMN_REPO_ID} = @1", connection);
            command.Parameters.AddWithValue("@0", parseStatus);
            command.Parameters.AddWithValue("@1", repo.Id);

            return command.ExecuteNonQuery();
            }

        public static int InsertLanguage(SqlConnection connection, ProcessRepositoryRequest repo, RepositoryLanguage language)
            {
            var command = new SqlCommand($"INSERT INTO RepoStats.{TABLE_LANGUAGES} ({COLUMN_REPO_ID}, {COLUMN_LANGUAGE}, {COLUMN_BYTES}) VALUES (@0, @1, @2)", connection);
            command.Parameters.AddWithValue("@0", repo.Id);
            command.Parameters.AddWithValue("@1", language.Name);
            command.Parameters.AddWithValue("@2", language.NumberOfBytes);

            return command.ExecuteNonQuery();
            }

        public static int InsertContributor(SqlConnection connection, ProcessRepositoryRequest repo, RepositoryContributor contributor)
            {
            var command = new SqlCommand($"INSERT INTO RepoStats.{TABLE_CONTRIBUTORS} ({COLUMN_REPO_ID}, {COLUMN_USERNAME}, {COLUMN_CONTRIBUTIONS}) VALUES (@0, @1, @2)", connection);
            command.Parameters.AddWithValue("@0", repo.Id);
            command.Parameters.AddWithValue("@1", contributor.Login);
            command.Parameters.AddWithValue("@2", contributor.Contributions);

            return command.ExecuteNonQuery();
            }
        }
    }
