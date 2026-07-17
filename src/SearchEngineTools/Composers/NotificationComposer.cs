using SearchEngineTools.Handlers;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;

namespace SearchEngineTools.Composers
{
    public class NotificationComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.AddNotificationHandler<ContentPublishingNotification, ContentPublishingHandler>();
        }
    }
}
