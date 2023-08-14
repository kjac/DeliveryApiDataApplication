using Umbraco.Cms.Core.DeliveryApi;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Serialization;

namespace RazorBlog.Filters;

// see https://docs.umbraco.com/umbraco-cms/reference/content-delivery-api/extension-api-for-querying#custom-filter
public class TagsFilter : IFilterHandler, IContentIndexHandler
{
    private const string FilterSpecifier = "tag:";
    private const string FieldName = "tags";

    private readonly IJsonSerializer _jsonSerializer;

    public TagsFilter(IJsonSerializer jsonSerializer) => _jsonSerializer = jsonSerializer;

    // Querying
    public bool CanHandle(string query)
        => query.StartsWith(FilterSpecifier, StringComparison.OrdinalIgnoreCase);

    public FilterOption BuildFilterOption(string filter)
    {
        var fieldValue = filter.Substring(FilterSpecifier.Length);

        // There might be several values for the filter
        var values = fieldValue.ToLowerInvariant().Split(',');

        return new FilterOption
        {
            FieldName = FieldName,
            Values = values,
            Operator = FilterOperation.Is
        };
    }

    // Indexing
    public IEnumerable<IndexFieldValue> GetFieldValues(IContent content, string? culture)
    {
        var tagsValue = content.GetValue<string>("tags");
        if (tagsValue.IsNullOrWhiteSpace() || tagsValue.DetectIsJson() is false)
        {
            return Array.Empty<IndexFieldValue>();
        }

        var tags = _jsonSerializer.Deserialize<string[]>(tagsValue);
        if (tags?.Any() is not true)
        {
            return Array.Empty<IndexFieldValue>();
        }

        return new[]
        {
            new IndexFieldValue
            {
                FieldName = FieldName,
                Values = tags.Select(tag => tag.ToLowerInvariant()).ToArray()
            }
        };
    }

    public IEnumerable<IndexField> GetFields() => new[]
    {
        new IndexField
        {
            FieldName = FieldName,
            FieldType = FieldType.StringRaw,
            VariesByCulture = false
        }
    };
}