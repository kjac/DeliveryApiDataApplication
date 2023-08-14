namespace BlazorBlog.Models;

public abstract class ContentEntity
{
    public Guid Id { get; init; }

    public required string Slug { get; init; }
}
