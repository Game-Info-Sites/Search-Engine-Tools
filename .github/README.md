# Search Engine Tools

[![Downloads](https://img.shields.io/nuget/dt/Umbraco.Community.SearchEngineTools?color=cc9900)](https://www.nuget.org/packages/Umbraco.Community.SearchEngineTools/)
[![NuGet](https://img.shields.io/nuget/vpre/Umbraco.Community.SearchEngineTools?color=0273B3)](https://www.nuget.org/packages/Umbraco.Community.SearchEngineTools)
[![GitHub license](https://img.shields.io/github/license/Game-Info-Sites/Search-Engine-Tools?color=8AB803)](../LICENSE)

Search Engine Tools is a collection of properties and tools to enhance Search Engine Optimization and general Search Engine tasks.
Developers are able to pick and choose which tools and propertyes they want to use.

<!--
Including screenshots is a really good idea! 

If you put images into /docs/screenshots, then you would reference them in this readme as, for example:

<img alt="..." src="https://github.com/GameInfoSites/Search-Engine-Tools/blob/develop/docs/screenshots/screenshot.png">

And don't forget to add the screenshot files to umbraco-marketplace.json too!
-->

## Installation

Add the package to an existing Umbraco website (v17+) from NuGet:

```dotnet add package Umbraco.Community.SearchEngineTools```

Add the following JSON to the root of your appsettings file.

```json
 "SearchEngineTools": {
    "Enabled": true,
    "IndexNow": {
      "Enabled": true
    }
  }
```

## Features

<b>SEO Settings Property:</b>
<br />An all-in-one property for common page level SEO settings. It includes title and meta description fields, a preview of roughly how the page will appear in search results, as well as optional NoIndex and NoFollow toggles.
A developer can configure the following settings for the data type:

<ul>
	<li>Set character limits for the title and meta description fields. Content editors see a warning when they exceed the limit but can still publish.</li>
	<li>Create a suffix that's automatically appended to the title field, and choose whether it counts toward the title limit.</li>
	<li>Enable the NoIndex and NoFollow toggles so content editors can set them per page.</li>
</ul>

To display the SEO Property (change `seoSettings` to your propertie's alias):
```razor
@{ var seoData = Model.Value<SeoSettingsValue>("seoSettings"); }
<title>@seoData?.Title</title>
<meta name="description" content="@seoData?.MetaDescription" />
@if(seoData.NoFollowOption) {<meta name="robots" content="nofollow" />}
@if(seoData.NoIndexOption) {<meta name="robots" content="noindex" />}
```

**Content Dates:**
<br />A data type that adds two read-only date properties “Published On” and “Last Significant Update” that are automatically updated.

<ul>
	<li>New content: Both dates are set when the page is first published.</li>
	<li>Existing content: When added to a document type that already has published pages, both dates are set to Umbraco’s internal creation date when each page is next published.</li>
</ul>

A checkbox is also added that allows content editors to update the “Last Significant Update” value the next time the page is published. The checkbox will reset after publishing but will persist when the page is saved without publishing.

To display the content dates (change `contentDates` to yoru propertie's alias):
```razor
@{ var publishDates = Model.Value<ContentDatesValue>("contentDates"); }
<p>Published On: @publishDates.PublishedOn.ToString("MMMM d, yyyy")</p>
<p>Last Updated: @publishedDates.LastSignificantUpdate.ToString("MMM d, yyyy")</p>
```


**IndexNow:**
<br />The IndexNow function submits content nodes that are published, unpublished, moved, or deleted to IndexNow for distribution to any search engines that support it. The plugin automatically generates and serves the verification key. The IndexNow function is disabled by default, to turn it on you must enable it in your appsettings.json file.

```json
 "SearchEngineTools": {
    "Enabled": true,
    "IndexNow": {
      "Enabled": true
    }
  }
```

## Contributing

Contributions to this package are most welcome! Please read the [Contributing Guidelines](CONTRIBUTING.md).

## Acknowledgments

The SEO Settings Property is highly inspired by the SEO Visualizer from Markus Johansson.