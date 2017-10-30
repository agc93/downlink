namespace Downlink.GitHub
{
    public class GitHubCredentials
    {
        public GitHubCredentials(string apiToken, string repoId)
        {
            Token = apiToken;
            var segs = repoId.Split('/');
            if (segs.Length != 0) return;
            if (segs.Length != 2) throw new System.InvalidOperationException("Could not parse repository name");
            Owner = segs[0];
            Repo = segs[1];
        }

        public string Owner { get; }
        public string Repo { get; }
        public string Token { get; }
    }
}
