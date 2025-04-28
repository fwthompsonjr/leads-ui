using Octokit;

namespace git.project.reader.models
{
    public class ReleaseModel
    {
        public string Url { get; set; } = string.Empty;

        public string HtmlUrl { get; set; } = string.Empty;

        public string AssetsUrl { get; set; } = string.Empty;

        public string UploadUrl { get; set; } = string.Empty;

        public long Id { get; set; }

        //
        // Summary:
        //     GraphQL Node Id
        public string NodeId { get; set; } = string.Empty;

        public string TagName { get; set; } = string.Empty;

        public string TargetCommitish { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Body { get; set; } = string.Empty;

        public bool Draft { get; set; }

        public bool Prerelease { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? PublishedAt { get; set; }

        public Author Author { get; set; } = new();

        public string TarballUrl { get; set; } = string.Empty;

        public string ZipballUrl { get; set; } = string.Empty;
    }
}
