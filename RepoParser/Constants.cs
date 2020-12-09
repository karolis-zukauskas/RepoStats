namespace RepoParser
    {
    static class Constants
        {
        public const string TABLE_REPOS = "Repos";
        public const string TABLE_LANGUAGES = "Repo_Languages";
        public const string TABLE_CONTRIBUTORS = "Repo_Contributors";

        public const string COLUMN_REPO_ID = "RepoId";
        public const string COLUMN_REPO_URL = "RepoUrl";
        public const string COLUMN_PARSE_STATUS = "ParseStatus";
        public const string COLUMN_IFRAME_URL = "IFrameUrl";
        public const string COLUMN_LANGUAGE = "Language";
        public const string COLUMN_BYTES = "Bytes";
        public const string COLUMN_USERNAME = "UserName";
        public const string COLUMN_CONTRIBUTIONS = "Contributions";

        public const int PARSE_STATUS_NONE = 0;
        public const int PARSE_STATUS_QUEUED = 1;
        public const int PARSE_STATUS_PARSING = 2;
        public const int PARSE_STATUS_FAILED = 3;
        public const int PARSE_STATUS_DONE = 4;

        public const string JSON_ID = "Id";
        public const string JSON_OWNER = "Owner";
        public const string JSON_NAME = "Name";
        }
    }
