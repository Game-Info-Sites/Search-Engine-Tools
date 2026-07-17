import { UMB_ACTION_EVENT_CONTEXT } from "@umbraco-cms/backoffice/action";
import { UMB_DOCUMENT_WORKSPACE_CONTEXT, UmbDocumentUrlModel, UmbDocumentUrlRepository, UmbDocumentVariantModel, UmbDocumentWorkspaceContext } from "@umbraco-cms/backoffice/document";
import { UmbEntityUnique } from "@umbraco-cms/backoffice/entity";
import { css, customElement, html, state, when } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { UmbPropertyEditorConfigCollection, UmbPropertyEditorUiElement } from "@umbraco-cms/backoffice/property-editor";
import { UmbFormControlMixin } from "@umbraco-cms/backoffice/validation";
import { SeoSettingsConfig } from "../../Models/seoSettingsConfig";
import { tryParseInt } from "../../Utilities/tryParseInt";
import { UMB_PROPERTY_CONTEXT } from "@umbraco-cms/backoffice/property";
import { UmbEntityActionEvent, UmbRequestReloadStructureForEntityEvent } from "@umbraco-cms/backoffice/entity-action";
import { debounce } from "@umbraco-cms/backoffice/utils";
import { UmbChangeEvent } from "@umbraco-cms/backoffice/event";
import { UUIInputElement, UUITextareaElement, UUIToggleElement } from "@umbraco-cms/backoffice/external/uui";
import { UmbTextStyles } from "@umbraco-cms/backoffice/style";
import { SeoSettingsPropertyEditorValue } from "../../Models/seoSettingsPropertyEditorValue";

@customElement('seo-settings-property-editor-ui')
export class SeoSettingsPropertyEditorUiElement extends UmbFormControlMixin<SeoSettingsPropertyEditorValue | undefined, typeof UmbLitElement>(UmbLitElement, undefined) implements UmbPropertyEditorUiElement
{
  #documentUrlRepository = new UmbDocumentUrlRepository(this);
  #workspaceContext?: UmbDocumentWorkspaceContext;
  #eventContext?: typeof UMB_ACTION_EVENT_CONTEXT.TYPE;
  @state() _unique?: UmbEntityUnique;
  @state() _urls?: UmbDocumentUrlModel[];
  @state() _culture?: string;
  @state() _variants?: UmbDocumentVariantModel[];
  @state() _previewTitle?: string;
  @state() _previewUrl?: string;
  private static readonly protocolRegex = /^https?:\/\//;

  @state() _config: SeoSettingsConfig =
    {
      titleCharacterLimit: 60,
      titleSuffix: '',
      countSuffixLength: true,
      metaDescriptionCharacterLimit: 160,
      showNoFollowOption: false,
      showNoIndexOption: false
    };

  public set config(config: UmbPropertyEditorConfigCollection | undefined)
  {
    if (!config) { return; }
    const titleSuffix = config.getValueByAlias<string>('titleSuffix') ?? "";
    const countSuffixLength = config.getValueByAlias<boolean>('countSuffixLength') ?? true;
    let titleCharacterLimit = tryParseInt(config.getValueByAlias('titleCharacterLimit'), 60);
    if (countSuffixLength && titleSuffix.length > 0) { titleCharacterLimit -= (titleSuffix.length + 1); }

    this._config =
    {
      titleCharacterLimit,
      titleSuffix,
      countSuffixLength,
      metaDescriptionCharacterLimit: tryParseInt(config.getValueByAlias('metaDescriptionCharacterLimit'), 160),
      showNoFollowOption: !!config.getValueByAlias('showNoFollowOption'),
      showNoIndexOption: !!config.getValueByAlias('showNoIndexOption')
    };
  }

  constructor()
  {
    super();

    this.consumeContext(UMB_DOCUMENT_WORKSPACE_CONTEXT, (workspaceContext) =>
    {
      this.#workspaceContext = workspaceContext;
      this.#observeContent();
    });

    this.consumeContext(UMB_PROPERTY_CONTEXT, (propertyContext) =>
    {
      this.observe(propertyContext?.variantId, (variantId) =>
      {
        this._culture = variantId?.culture ?? undefined;
        this.#setPropertiesByCulture();
      }, 'SearchEngineToolsVariantIdSubscription');
    });

    this.consumeContext(UMB_ACTION_EVENT_CONTEXT, (eventContext) =>
    {
      this.#eventContext = eventContext;
      this.#eventContext?.removeEventListener(UmbRequestReloadStructureForEntityEvent.TYPE, this.#onReloadRequest as unknown as EventListener);
      this.#eventContext?.addEventListener(UmbRequestReloadStructureForEntityEvent.TYPE, this.#onReloadRequest as unknown as EventListener);
    });
  }

  #observeContent()
  {
    if (!this.#workspaceContext) { return; }

    this.observe(this.#workspaceContext.unique, async (unique) =>
    {
      this._unique = unique;
      await this.#requestUrls();
    }, '_documentUrls');

    this.observe(this.#workspaceContext.variants, (variants) =>
    {
      this._variants = variants;
      this.#setPropertiesByCulture();
    }, '_variants');
  }

  #debounceRequestUrls = debounce(() => this.#requestUrls(), 50);

  #onReloadRequest = (event: UmbEntityActionEvent) =>
  {
    if (event.getUnique() !== this.#workspaceContext?.getUnique() || event.getEntityType() !== this.#workspaceContext.getEntityType()) { return; }
    this.#debounceRequestUrls();
  }

  async #requestUrls()
  {
    if (!this._unique) { return; }
    const { data } = await this.#documentUrlRepository.requestItems([this._unique]);

    if (data?.length)
    {
      const item = data[0];
      this._urls = item.urls;
      this.#setPropertiesByCulture();
    }
  }

  #setPropertiesByCulture()
  {
    this.#setUrl();
    this.#setTitle();
  }

  #setUrl()
  {
    if (!this._urls?.length)
    {
      this._previewUrl = 'https://www.example.com/';
      return;
    }

    const url = this._urls.find(x => x.culture === this._culture)?.url ?? this._urls[0]?.url ?? '';
    this._previewUrl = this.#prependProtocolAndHost(url);
  }

  #setTitle()
  {
    let title = this.value?.title ?? "";

    if (!title)
    {
      const node = this._variants?.find(x => x.culture === this._culture);
      if (node) { title = node.name; }
    }

    if (title === '')
    {
      this._previewTitle = '';
      return;
    }

    if (this._config.titleSuffix) { title = `${title} ${this._config.titleSuffix}`; }
    this._previewTitle = title;
  }

  #prependProtocolAndHost(url: string): string
  {
    if (SeoSettingsPropertyEditorUiElement.protocolRegex.test(url)) { return url; }
    return `${window.location.protocol}//${window.location.host}${url}`;
  }

  #updateValues(updates: Partial<SeoSettingsPropertyEditorValue>)
  {
    this.value =
    {
      noFollowOption: false,
      noIndexOption: false,
      ...this.value,
      ...updates,
    };

    this.dispatchEvent(new UmbChangeEvent());
  }

  #onTitleChange(e: Event)
  {
    this.#updateValues({ title: (e.target as UUIInputElement).value.toString() });
    this.#setTitle();
  }

  #onMetaDescriptionChange(e: Event) { this.#updateValues({ metaDescription: (e.target as UUITextareaElement).value.toString() }); }
  #onNoIndexOptionChange(e: Event) { this.#updateValues({ noIndexOption: (e.target as UUIToggleElement).checked }); }
  #onNoFollowOptionChange(e: Event) { this.#updateValues({ noFollowOption: (e.target as UUIToggleElement).checked }); }

  override destroy()
  {
    this.#eventContext?.removeEventListener(UmbRequestReloadStructureForEntityEvent.TYPE, this.#onReloadRequest as unknown as EventListener,);
    super.destroy();
  }

  render()
  {
    return html`
      <div id="seoSettingsProperties">
        <div id="form">
            ${this.#renderTitleField()}
            ${this.#renderMetaDescriptionField()}
            ${this.#renderToggleOptions()}
        </div>

        ${this.#renderSearchPreview()}
      </div>
    `;
  }

#renderTitleField()
{
  const titleLength = this.value?.title?.length ?? 0;
  const remaining = this._config.titleCharacterLimit - titleLength;

  return html`
    <div id="title">
      <uui-label>Meta Title: <small>(${remaining} characters remaining)</small></uui-label>
      <uui-input type="text" @input=${this.#onTitleChange} .value=${this.value?.title ?? ""} label="Page's Meta Title"></uui-input>
      ${when(remaining < 0, () => html`<p class="error">The title has exceeded the recommended length</p>`)}
    </div>
  `;
}

#renderMetaDescriptionField()
{
  const descLength = this.value?.metaDescription?.length ?? 0;
  const remaining = this._config.metaDescriptionCharacterLimit - descLength;

  return html`
    <div id="metaDescription">
      <uui-label>Meta Description: <small>(${remaining} characters remaining)</small></uui-label>
      <uui-textarea @input=${this.#onMetaDescriptionChange} .value=${this.value?.metaDescription ?? ""} label="Page's Meta Description" rows="6"></uui-textarea>
      ${when(remaining < 0, () => html`<p class="error">The meta-description has exceeded the recommended length</p>`)}
    </div>
  `;
}

#renderToggleOptions()
{
  return html`
    ${when(this._config.showNoFollowOption, () => html`
      <div id="noFollowToggle">
        <uui-toggle @change=${this.#onNoFollowOptionChange} label="Prevent search engines from following links on this page" ?checked=${this.value?.noFollowOption}></uui-toggle>
      </div>
    `)}
    ${when(this._config.showNoIndexOption, () => html`
      <div id="noIndexOptionToggle">
        <uui-toggle @change=${this.#onNoIndexOptionChange} label="Prevent search engines from indexing this page" ?checked=${this.value?.noIndexOption}></uui-toggle>
      </div>
    `)}
  `;
}

#renderSearchPreview()
{
  return html`
    <div id="searchPreview">
      <div>
        <h6>${this._previewTitle}</h6>
        <p class="link">${this._previewUrl}</p>
        <p>${this.value?.metaDescription}</p>
      </div>
    </div>
  `;
}

  static styles = [UmbTextStyles, css`
    :host > div
    {
      display: flex;
      gap: 40px;
    }

    #form { width: 400px; }
    #form > div + div { margin-top: 10px; }
    #form uui-input, #form uui-textarea { width: 100%; }
    #searchPreview { max-width: 600px; }

    #searchPreview h6
    {
      color: var(--uui-color-focus);
      font-size: 20px;
      line-height: 1.3;
      margin: 0 0 3px 0;
      padding: 0;
      text-decoration: none;
    }

    #searchPreview p
    {
      font-size: 14px;
      line-height: 1.57;
      margin: 0 0 3px 0;
      padding: 0;
      word-wrap: break-word;
     }

     #searchPreview p.link { color: var(--uui-color-text-alt); }

     p.error
     {
      color: var(--uui-color-invalid);
      margin: 0;
     }
  `]
}

export default SeoSettingsPropertyEditorUiElement;

declare global
{
  interface HTMLElementTagNameMap
  {
    'seo-settings-property-editor-ui': SeoSettingsPropertyEditorUiElement;
  }
}
