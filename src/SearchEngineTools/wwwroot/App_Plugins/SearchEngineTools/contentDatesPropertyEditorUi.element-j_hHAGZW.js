import { html as f, state as u, customElement as v } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement as m } from "@umbraco-cms/backoffice/lit-element";
import { UmbFormControlMixin as _ } from "@umbraco-cms/backoffice/validation";
import { UmbChangeEvent as b } from "@umbraco-cms/backoffice/event";
const g = new Intl.DateTimeFormat(void 0, {
  dateStyle: "medium",
  timeStyle: "short"
});
function d(t) {
  if (!t)
    return "";
  const e = new Date(t);
  return isNaN(e.getTime()) ? "" : g.format(e);
}
var U = Object.defineProperty, S = Object.getOwnPropertyDescriptor, c = (t) => {
  throw TypeError(t);
}, p = (t, e, i, r) => {
  for (var a = r > 1 ? void 0 : r ? S(e, i) : e, s = t.length - 1, o; s >= 0; s--)
    (o = t[s]) && (a = (r ? o(e, i, a) : o(a)) || a);
  return r && a && U(e, i, a), a;
}, y = (t, e, i) => e.has(t) || c("Cannot " + i), D = (t, e, i) => e.has(t) ? c("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(t) : e.set(t, i), C = (t, e, i) => (y(t, e, "access private method"), i), l, h;
let n = class extends _(m, void 0) {
  constructor() {
    super(), D(this, l), this._publishedOnLabel = "", this._lastSignificantUpdateLabel = "";
  }
  willUpdate(t) {
    super.willUpdate(t), t.has("value") && (this._publishedOnLabel = d(this.value?.publishedOn), this._lastSignificantUpdateLabel = d(this.value?.lastSignificantUpdate));
  }
  render() {
    return f`
      <div id="contentDates">
        <p><strong>Published On:</strong> <uui-input type="text" id="publishedOnDate" label="Published On Date" readonly .value=${this._publishedOnLabel}></uui-input></p>
        <p><strong>Last Significant Update:</strong> <uui-input type="text" id="lastSignificantUpdateDate" label="Last Significant Update Date" readonly .value=${this._lastSignificantUpdateLabel}></uui-input></p>
        <uui-checkbox label="Is Significant Update" id="significantUpdateCheckbox" .checked=${this.value?.isSignificantUpdate ?? !1} @change=${C(this, l, h)}></uui-checkbox>
      </div>
    `;
  }
};
l = /* @__PURE__ */ new WeakSet();
h = function(t) {
  this.value = { ...this.value, isSignificantUpdate: t.target.checked }, this.dispatchEvent(new b());
};
p([
  u()
], n.prototype, "_publishedOnLabel", 2);
p([
  u()
], n.prototype, "_lastSignificantUpdateLabel", 2);
n = p([
  v("content-dates-property-editor-ui")
], n);
const x = n;
export {
  n as ContentDatesPropertyEditorUiElement,
  x as default
};
//# sourceMappingURL=contentDatesPropertyEditorUi.element-j_hHAGZW.js.map
