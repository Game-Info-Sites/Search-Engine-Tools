import { manifests as contentDates } from './ContentDates/manifest.js';
import { manifests as seoSettings } from './SeoSettings/manifest.js';

export const manifests: Array<UmbExtensionManifest> =
[
  ...contentDates,
  ...seoSettings,
];
