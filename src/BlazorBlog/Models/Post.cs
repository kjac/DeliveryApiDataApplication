using Microsoft.AspNetCore.Components;

namespace BlazorBlog.Models;

public class Post : PostSummary
{
    public required Author Author { get; init; }

    public required MarkupString Content { get; init; }

    public required PostSummary[] LatestPosts { get; init; }
}
