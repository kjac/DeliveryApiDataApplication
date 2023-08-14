namespace Umbraco.Cms.Web.Common.PublishedModels;

public partial class Post
{
    public string FormatTags() => string.Join(" | ", Tags ?? Array.Empty<string>());

    public Post[] PostsByAuthor { get; set; }
}