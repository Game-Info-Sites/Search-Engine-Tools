import { customElement, html, PropertyValues, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { UmbPropertyEditorUiElement } from "@umbraco-cms/backoffice/property-editor";
import { UmbFormControlMixin } from "@umbraco-cms/backoffice/validation";
import { ContentDatesPropertyEditorValue } from "../../Models/contentDatesPropertyEditorValue";
import { UmbChangeEvent } from "@umbraco-cms/backoffice/event";
import { UUICheckboxElement } from "@umbraco-cms/backoffice/external/uui";
import { formatDateTime } from "../../Utilities/formatDateTime";

@customElement('content-dates-property-editor-ui')
export class ContentDatesPropertyEditorUiElement extends UmbFormControlMixin<ContentDatesPropertyEditorValue | undefined, typeof UmbLitElement>(UmbLitElement, undefined) implements UmbPropertyEditorUiElement
{
  @state() _publishedOnLabel = '';
  @state() _lastSignificantUpdateLabel = '';

  constructor() { super(); }

  #onSignificantUpdateCheckboxChange(event: Event)
  {
    this.value = { ...this.value, isSignificantUpdate: (event.target as UUICheckboxElement).checked };
    this.dispatchEvent(new UmbChangeEvent());
  }

  override willUpdate(changedProperties: PropertyValues)
  {
    super.willUpdate(changedProperties);

    if (changedProperties.has('value'))
    {
      this._publishedOnLabel = formatDateTime(this.value?.publishedOn);
      this._lastSignificantUpdateLabel = formatDateTime(this.value?.lastSignificantUpdate);
    }
  }

  override render()
  {
    return html`
      <div id="contentDates">
        <p><strong>Published On:</strong> <uui-input type="text" id="publishedOnDate" label="Published On Date" readonly .value=${this._publishedOnLabel}></uui-input></p>
        <p><strong>Last Significant Update:</strong> <uui-input type="text" id="lastSignificantUpdateDate" label="Last Significant Update Date" readonly .value=${this._lastSignificantUpdateLabel}></uui-input></p>
        <uui-checkbox label="Is Significant Update" id="significantUpdateCheckbox" .checked=${this.value?.isSignificantUpdate ?? false} @change=${this.#onSignificantUpdateCheckboxChange}></uui-checkbox>
      </div>
    `;
  }
}

export default ContentDatesPropertyEditorUiElement;

declare global
{
  interface HTMLElementTagNameMap
  {
    'content-dates-property-editor-ui': ContentDatesPropertyEditorUiElement;
  }
}
