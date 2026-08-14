const e = {
  type: "propertyEditorSchema",
  name: "Content Dates Schema",
  alias: "GameInfoSites.SearchEngineTools.ContentDatesSchema",
  meta: { defaultPropertyEditorUiAlias: "GameInfoSites.SearchEngineTools.ContentDatesUi" }
}, t = {
  type: "propertyEditorUi",
  name: "Content Dates Property Editor UI",
  alias: "GameInfoSites.SearchEngineTools.ContentDatesUi",
  element: () => import("./contentDatesPropertyEditorUi.element-j_hHAGZW.js"),
  elementName: "content-dates-property-editor-ui",
  meta: {
    label: "Content Dates",
    icon: "icon-calendar-alt",
    group: "Search Engine Tools",
    propertyEditorSchemaAlias: "GameInfoSites.SearchEngineTools.ContentDatesSchema"
  }
}, o = [
  e,
  t
], i = {
  type: "propertyEditorSchema",
  name: "SEO Settings Property Editor Schema",
  alias: "GameInfoSites.SearchEngineTools.SeoSettingsSchema",
  meta: {
    defaultPropertyEditorUiAlias: "GameInfoSites.SearchEngineTools.SeoSettingsUi",
    settings: {
      properties: [
        {
          alias: "titleCharacterLimit",
          label: "Recommended title character limit.",
          description: "The maximum characters allowed before warning about title length.",
          propertyEditorUiAlias: "Umb.PropertyEditorUi.Integer"
        },
        {
          alias: "titleSuffix",
          label: "Title suffix.",
          description: "Suffix to append to the title, will automatically add a space before the suffix.",
          propertyEditorUiAlias: "Umb.PropertyEditorUi.TextBox"
        },
        {
          alias: "countSuffixLength",
          label: "Count title suffix against character limit?",
          description: "If enabled, the title suffix will be counted against the recommended title character limit.",
          propertyEditorUiAlias: "Umb.PropertyEditorUi.Toggle"
        },
        {
          alias: "metaDescriptionCharacterLimit",
          label: "Recommended meta description character limit.",
          description: "The maximum characters allowed before warning about meta description length.",
          propertyEditorUiAlias: "Umb.PropertyEditorUi.Integer"
        },
        {
          alias: "showNoFollowOption",
          label: "Show 'nofollow' toggle option",
          description: "Enable to show a toggle to set the 'nofollow' option for the document.",
          propertyEditorUiAlias: "Umb.PropertyEditorUi.Toggle"
        },
        {
          alias: "showNoIndexOption",
          label: "Show 'noindex' toggle option",
          description: "Enable to show a toggle to set the 'noindex' option for the document.",
          propertyEditorUiAlias: "Umb.PropertyEditorUi.Toggle"
        }
      ],
      defaultData: [
        {
          alias: "titleCharacterLimit",
          value: "60"
        },
        {
          alias: "metaDescriptionCharacterLimit",
          value: "160"
        }
      ]
    }
  }
}, a = {
  type: "propertyEditorUi",
  alias: "GameInfoSites.SearchEngineTools.SeoSettingsUi",
  name: "SEO Settings Property Editor UI",
  element: () => import("./seoSettingsPropertyEditorUi.element-D2Y8dPA9.js"),
  elementName: "seo-settings-property-editor-ui",
  meta: {
    label: "SEO Settings",
    icon: "icon-search",
    group: "Search Engine Tools",
    propertyEditorSchemaAlias: "GameInfoSites.SearchEngineTools.SeoSettingsSchema"
  }
}, r = [
  i,
  a
], n = [
  ...o,
  ...r
], s = [
  ...n
];
export {
  s as manifests
};
//# sourceMappingURL=search-engine-tools.js.map
