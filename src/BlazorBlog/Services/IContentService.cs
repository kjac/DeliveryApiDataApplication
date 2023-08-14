using BlazorBlog.Models;

namespace BlazorBlog.Services;

public interface IContentService
{
    Task<Post[]> LatestPostsAsync();

    Task<Post> PostAsync(string slug);

    Task<Author> AuthorAsync(string slug);

    Task<Post[]> LatestPostsByTagAsync(string tag);
}