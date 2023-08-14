using Umbraco.Cms.Core.DeliveryApi;
using Umbraco.Cms.Core.Models.DeliveryApi;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PropertyEditors.DeliveryApi;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace RazorBlog.PropertyEditors;

public class MyAuthorPostsPropertyEditorValueConverter : PropertyValueConverterBase, IDeliveryApiPropertyValueConverter
{
    private readonly IPublishedSnapshotAccessor _publishedSnapshotAccessor;
    private readonly IApiContentBuilder _apiContentBuilder;

    public MyAuthorPostsPropertyEditorValueConverter(IPublishedSnapshotAccessor publishedSnapshotAccessor, IApiContentBuilder apiContentBuilder)
    {
        _publishedSnapshotAccessor = publishedSnapshotAccessor;
        _apiContentBuilder = apiContentBuilder;
    }

    #region PropertyValueConverterBase implementation

    public override bool IsConverter(IPublishedPropertyType propertyType)
        => propertyType.EditorAlias.Equals("My.AuthorPosts");

    public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType)
        => PropertyCacheLevel.Elements;

    public override Type GetPropertyValueType(IPublishedPropertyType propertyType)
        => typeof(IEnumerable<IPublishedContent>);

    public override object? ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object? inter, bool preview)
        => GetAuthorPosts(owner);

    // must set this to true explicitly, since the value converter *always* expect to have a value - and expect to be invoked accordingly
    public override bool? IsValue(object? value, PropertyValueLevel level) => true;

    private IPublishedContent[] GetAuthorPosts(IPublishedElement owner)
    {
        var publishedContentCache = _publishedSnapshotAccessor.GetRequiredPublishedSnapshot().Content
                                    ?? throw new InvalidOperationException("Could not obtain the published content cache");
        var posts = publishedContentCache.GetAtRoot().FirstOrDefault(c => c.ContentType.Alias == "posts")
                    ?? throw new InvalidOperationException("Could not obtain the posts root content");

        // find the ten most recently updated posts that are written by the author (.Where(p => p.Value<IPublishedContent>("author")?.Key == owner.Key))
        return posts
            .Children
            .OfType<Post>()
            .Where(p => p.Value<IPublishedContent>("author")?.Key == owner.Key)
            .OrderBy(p => p.UpdateDate)
            .Take(10)
            .Cast<IPublishedContent>()
            .ToArray();
    }

    #endregion

    #region IDeliveryApiPropertyValueConverter implementation

    public PropertyCacheLevel GetDeliveryApiPropertyCacheLevel(IPublishedPropertyType propertyType)
        => PropertyCacheLevel.Elements;

    public Type GetDeliveryApiPropertyValueType(IPublishedPropertyType propertyType)
        => typeof(IEnumerable<IApiContent>);

    public object? ConvertIntermediateToDeliveryApiObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object? inter, bool preview, bool expanding)
        => expanding
            ? GetAuthorPosts(owner).Select(_apiContentBuilder.Build).WhereNotNull().ToArray()
            : Array.Empty<IApiContent>();

    #endregion
}