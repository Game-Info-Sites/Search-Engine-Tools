using System.Text.Json;
using SearchEngineTools.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;

namespace SearchEngineTools.Converters
{
    public class SeoSettingsValueConverter : PropertyValueConverterBase
    {
        public override bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias == Constants.SeoSettings;
        }

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType)
        {
            return typeof(SeoSettingsValue);
        }

        public override object? ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel cacheLevel, object? inter, bool preview)
        {
            if (inter is not string json || string.IsNullOrWhiteSpace(json)) { return null; }
            var stored = JsonSerializer.Deserialize<SeoSettingsProperty>(json);
            if (stored is null) { return null; }

            return new SeoSettingsValue
            {
                MetaDescription = stored.MetaDescription,
                NoFollowOption = stored.NoFollowOption,
                NoIndexOption = stored.NoIndexOption,
                Title = stored.Title
            };
        }
    }
}
