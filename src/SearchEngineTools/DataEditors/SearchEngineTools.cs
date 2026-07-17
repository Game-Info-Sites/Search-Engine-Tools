using Umbraco.Cms.Core.PropertyEditors;

namespace SearchEngineTools.DataEditors
{
    [DataEditor("GameInfoSites.SearchEngineTools.ContentDatesSchema", ValueEditorIsReusable = true, ValueType = "JSON")]
    public class ContentDatesDataEditor(IDataValueEditorFactory dataValueEditorFactory) : DataEditor(dataValueEditorFactory);

    [DataEditor("GameInfoSites.SearchEngineTools.SeoSettingsSchema", ValueEditorIsReusable = true, ValueType = "JSON")]
    public class SeoSettingsDataEditor(IDataValueEditorFactory dataValueEditorFactory) : DataEditor(dataValueEditorFactory);
}
