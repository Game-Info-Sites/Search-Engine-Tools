const contentDatesSchema: UmbExtensionManifest =
{
  type: 'propertyEditorSchema',
  name: 'Content Dates Schema',
  alias: 'GameInfoSites.SearchEngineTools.ContentDatesSchema',
  meta: { defaultPropertyEditorUiAlias: 'GameInfoSites.SearchEngineTools.ContentDatesUi' }
}

const contentDatesUi: UmbExtensionManifest =
{
  type: 'propertyEditorUi',
  name: 'Content Dates Property Editor UI',
  alias: 'GameInfoSites.SearchEngineTools.ContentDatesUi',
  element: () => import('./contentDatesPropertyEditorUi.element.ts'),
  elementName: 'content-dates-property-editor-ui',
  meta:
  {
    label: 'Content Dates',
    icon: 'icon-calendar-alt',
    group: 'Search Engine Tools',
    propertyEditorSchemaAlias: 'GameInfoSites.SearchEngineTools.ContentDatesSchema'
  }
}

export const manifests: Array<UmbExtensionManifest> =
  [
    contentDatesSchema,
    contentDatesUi
];
