
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static RepoStats.Constants;

namespace RepoStats
    {
    class RepositoryState
        {
        public string Id { get; set; }
        public string Url { get; set; }
        public int ParseStatus { get; set; }
        }

    static class DbStatements
        {
        public static RepositoryState GetRepository(SqlConnection connection, string repoUrl)
            {
            var command = new SqlCommand($"SELECT TOP 1 * FROM RepoStats.{TABLE_REPOS} WHERE {COLUMN_REPO_URL}=@0", connection);
            command.Parameters.AddWithValue("@0", repoUrl);

            using (SqlDataReader reader = command.ExecuteReader())
                {
                if (reader.Read())
                    {
                    var state = new RepositoryState
                    {
                        Id = reader[COLUMN_REPO_ID].ToString(),
                        Url = reader[COLUMN_REPO_URL] as string,
                        ParseStatus = (int)reader[COLUMN_PARSE_STATUS],
                    };

                    return state;
                    }
                }

            return new RepositoryState { ParseStatus = PARSE_STATUS_NONE };
            }

        public static int InsertRepository(SqlConnection connection, RepositoryState repo)
            {
            var command = new SqlCommand($"INSERT INTO RepoStats.{TABLE_REPOS} ({COLUMN_REPO_ID}, {COLUMN_REPO_URL}, {COLUMN_PARSE_STATUS}) VALUES (@0, @1, @2)", connection);
            command.Parameters.AddWithValue("@0", repo.Id);
            command.Parameters.AddWithValue("@1", repo.Url);
            command.Parameters.AddWithValue("@2", repo.ParseStatus);

            return command.ExecuteNonQuery();
            }
        }
    }
