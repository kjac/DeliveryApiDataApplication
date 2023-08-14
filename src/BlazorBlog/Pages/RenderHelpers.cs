using BlazorBlog.Models;

namespace BlazorBlog.Pages;

internal static class RenderHelpers
{
    public static string PostLink(PostSummary post) => $"/post{post.Slug}";

    public static string AuthorLink(Author author) => $"/author{author.Slug}";

    public static string TagLink(string tag) => $"/tags/{tag.ToLowerInvariant()}";
}