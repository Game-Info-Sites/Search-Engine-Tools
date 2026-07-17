# Search Engine Tools

[![Downloads](https://img.shields.io/nuget/dt/Umbraco.Community.SearchEngineTools?color=cc9900)](https://www.nuget.org/packages/Umbraco.Community.SearchEngineTools/)
[![NuGet](https://img.shields.io/nuget/vpre/Umbraco.Community.SearchEngineTools?color=0273B3)](https://www.nuget.org/packages/Umbraco.Community.SearchEngineTools)
[![GitHub license](https://img.shields.io/github/license/GameInfoSites/Search-Engine-Tools?color=8AB803)](../LICENSE)

Search Engine Tools is a collection of properties and tools to enhance Search Engine Optimization and general Search Engine tasks.
Developers are able to pick and choose which tools and propertyes they want to use.

<!--
Including screenshots is a really good idea! 

If you put images into /docs/screenshots, then you would reference them in this readme as, for example:

<img alt="..." src="https://github.com/GameInfoSites/Search-Engine-Tools/blob/develop/docs/screenshots/screenshot.png">

And don't forget to add the screenshot files to umbraco-marketplace.json too!
-->

## Installation

Add the package to an existing Umbraco website (v17+) from nuget:

`dotnet add package Umbraco.Community.SearchEngineTools`

## Features

<b>SEO Settings Property</b>
This is an all-in-one property for configuring common SEO settings. It includes a text field for the meta description and title, along with a preview of what the search result should generally look like. In the property configuration, you can set a character limit for the meta description and title that triggers a warning should the editor exceed this limit. You can also set a global suffix to be added to the title. 
An option to set a NoIndex flag, indicating that the page should not be indexed by search engines, can be enabled. There is also a NoFollow global flag that can be enabled for the page.

<b>Content Dates</b>
The Content Dates property adds “Published On” and “Last Significant Update” date properties that are automatically updated. If the property is added to a new page, then the initial values of both dates will be set to the date and time the page is published. If the property is added to an existing page then, on the next publish, the “Published On” and “Last Significant Update” dates will be set to Umbraco’s internal creation date for the page. 
A checkbox is also available to indicate when the update being performed is considered “significant” so when this box is checked, the “Last Significant Update” value will be updated once the page is published.


## Contributing

Contributions to this package are most welcome! Please read the [Contributing Guidelines](CONTRIBUTING.md).

## Acknowledgments

The SEO Settings Property is highly inspired by the SEO Visualizer from Markus Johansson.