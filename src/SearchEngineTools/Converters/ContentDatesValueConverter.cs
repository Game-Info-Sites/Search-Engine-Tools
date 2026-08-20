using System.Text.Json;
using SearchEngineTools.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;

namespace SearchEngineTools.Converters
{
    public class ContentDatesValueConverter : PropertyValueConverterBase
    {
        public override bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias == Constants.ContentDates;
        }

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType)
        {
            return typeof(ContentDatesValue);
        }

        public override object? ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel cacheLevel, object? inter, bool preview)
        {
            ContentDatesProperty? stored = null;

            if (inter is string json && !string.IsNullOrWhiteSpace(json))
            {
                try
                {
                    stored = JsonSerializer.Deserialize<ContentDatesProperty>(json);
                }
                catch (JsonException)
                {
                    //A Json issue was found, so clear stored value so we can fallback to another value later.
                    stored = null;
                }
            }

            //Set a fallback PublishedOn date if no value is stored but the property exists on the page.
            var fallback = owner is IPublishedContent content ? content.CreateDate : (DateTime?)null;
            var publishedOn = stored?.PublishedOn ?? fallback;

            return new ContentDatesValue
            {
                PublishedOn = publishedOn,
                LastSignificantUpdate = stored?.LastSignificantUpdate ?? publishedOn
            };
        }
    }
}
