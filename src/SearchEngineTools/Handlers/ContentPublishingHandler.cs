using System.Text.Json;
using Microsoft.Extensions.Logging;
using SearchEngineTools.Models;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace SearchEngineTools.Handlers
{
    public class ContentPublishingHandler(ILogger<ContentPublishingHandler> logger) : INotificationHandler<ContentPublishingNotification>
    {
        private readonly ILogger<ContentPublishingHandler> _logger = logger;

        public void Handle(ContentPublishingNotification notification)
        {
            var currentDateTime = DateTime.UtcNow;

            foreach (var contentNode in notification.PublishedEntities)
            {
                //Get the Content Dates property from the node.
                var contentDatesProperty = contentNode.Properties.FirstOrDefault(p => p.PropertyType.PropertyEditorAlias == Constants.ContentDates);

                //Skip the node if property does not exist
                if (contentDatesProperty is null) { continue; }

                ContentDatesProperty? contentDatesData;

                //If the content dates property exists, deserialize the data otherwise fall back to a fresh instance.
                var contentDatesJson = contentDatesProperty.GetValue(published: false)?.ToString();

                //Try to deserialize the property contents and break the publish if it fails.
                try
                {
                    contentDatesData = contentDatesJson is not null ? JsonSerializer.Deserialize<ContentDatesProperty>(contentDatesJson) : new ContentDatesProperty();
                }
                catch (JsonException ex)
                {
                    _logger.LogError
                    (
                        ex,
                        "Failed to deserialize the ContentDates property on node {NodeId} ('{NodeName}') with a stored value of '{StoredValue}'.",
                        contentNode.Id,
                        contentNode.Name,
                        contentDatesJson
                    );
                    throw;
                }

                //Log a warning if the stored value deserializes to null for some reason and skip processing on this node.
                if (contentDatesData is null)
                {
                    _logger.LogWarning
                    (
                        "ContentDates property on node {NodeId} ('{NodeName}') deserialized to null from a stored value of '{StoredValue}'. Skipping content dates processing for this node.",
                        contentNode.Id,
                        contentNode.Name,
                        contentDatesJson
                    );
                    continue;
                }

                if (contentDatesData.PublishedOn is null)
                {
                    contentDatesData.PublishedOn = contentNode is { HasIdentity: true, Published: true } ? contentNode.CreateDate : currentDateTime;
                    contentDatesData.LastSignificantUpdate = contentDatesData.PublishedOn;
                }
                else if (contentDatesData.IsSignificantUpdate)
                {
                    contentDatesData.LastSignificantUpdate = currentDateTime;
                }

                //Reset Significant Update flag so it doesn't persist
                contentDatesData.IsSignificantUpdate = false;
                contentDatesProperty?.SetValue(JsonSerializer.Serialize(contentDatesData));
            }
        }
    }
}
