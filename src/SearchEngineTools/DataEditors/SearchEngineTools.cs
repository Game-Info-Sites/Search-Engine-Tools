using Umbraco.Cms.Core.PropertyEditors;

namespace SearchEngineTools.DataEditors
{
    [DataEditor(Constants.ContentDates, ValueEditorIsReusable = true, ValueType = "JSON")]
    public class ContentDatesDataEditor(IDataValueEditorFactory dataValueEditorFactory) : DataEditor(dataValueEditorFactory);

    [DataEditor(Constants.SeoSettings, ValueEditorIsReusable = true, ValueType = "JSON")]
    public class SeoSettingsDataEditor(IDataValueEditorFactory dataValueEditorFactory) : DataEditor(dataValueEditorFactory);
}
