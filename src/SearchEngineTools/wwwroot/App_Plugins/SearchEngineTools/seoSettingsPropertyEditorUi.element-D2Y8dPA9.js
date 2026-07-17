import { UMB_ACTION_EVENT_CONTEXT as B } from "@umbraco-cms/backoffice/action";
import { UmbDocumentUrlRepository as V, UMB_DOCUMENT_WORKSPACE_CONTEXT as W } from "@umbraco-cms/backoffice/document";
import { html as a, when as g, css as Y, state as p, customElement as X } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement as z } from "@umbraco-cms/backoffice/lit-element";
import { UmbFormControlMixin as G } from "@umbraco-cms/backoffice/validation";
import { UMB_PROPERTY_CONTEXT as H } from "@umbraco-cms/backoffice/property";
import { UmbRequestReloadStructureForEntityEvent as y } from "@umbraco-cms/backoffice/entity-action";
import { debounce as K } from "@umbraco-cms/backoffice/utils";
import { UmbChangeEvent as J } from "@umbraco-cms/backoffice/event";
import { UmbTextStyles as Q } from "@umbraco-cms/backoffice/style";
function S(t, e) {
  const n = Number(t);
  return !isNaN(n) && n > 0 ? n : e;
}
var Z = Object.defineProperty, j = Object.getOwnPropertyDescriptor, U = (t) => {
  throw TypeError(t);
}, u = (t, e, n, l) => {
  for (var c = l > 1 ? void 0 : l ? j(e, n) : e, w = t.length - 1, x; w >= 0; w--)
    (x = t[w]) && (c = (l ? x(e, n, c) : x(c)) || c);
  return l && c && Z(e, n, c), c;
}, C = (t, e, n) => e.has(t) || U("Cannot " + n), o = (t, e, n) => (C(t, e, "read from private field"), e.get(t)), d = (t, e, n) => e.has(t) ? U("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(t) : e.set(t, n), $ = (t, e, n, l) => (C(t, e, "write to private field"), e.set(t, n), n), r = (t, e, n) => (C(t, e, "access private method"), n), E, h, f, i, N, T, v, O, _, b, P, D, m, L, M, I, F, k, q, R, A;
let s = class extends G(z, void 0) {
  constructor() {
    super(), d(this, i), d(this, E, new V(this)), d(this, h), d(this, f), this._config = {
      titleCharacterLimit: 60,
      titleSuffix: "",
      countSuffixLength: !0,
      metaDescriptionCharacterLimit: 160,
      showNoFollowOption: !1,
      showNoIndexOption: !1
    }, d(this, T, K(() => r(this, i, O).call(this), 50)), d(this, v, (t) => {
      t.getUnique() !== o(this, h)?.getUnique() || t.getEntityType() !== o(this, h).getEntityType() || o(this, T).call(this);
    }), this.consumeContext(W, (t) => {
      $(this, h, t), r(this, i, N).call(this);
    }), this.consumeContext(H, (t) => {
      this.observe(t?.variantId, (e) => {
        this._culture = e?.culture ?? void 0, r(this, i, _).call(this);
      }, "SearchEngineToolsVariantIdSubscription");
    }), this.consumeContext(B, (t) => {
      $(this, f, t), o(this, f)?.removeEventListener(y.TYPE, o(this, v)), o(this, f)?.addEventListener(y.TYPE, o(this, v));
    });
  }
  set config(t) {
    if (!t)
      return;
    const e = t.getValueByAlias("titleSuffix") ?? "", n = t.getValueByAlias("countSuffixLength") ?? !0;
    let l = S(t.getValueByAlias("titleCharacterLimit"), 60);
    n && e.length > 0 && (l -= e.length + 1), this._config = {
      titleCharacterLimit: l,
      titleSuffix: e,
      countSuffixLength: n,
      metaDescriptionCharacterLimit: S(t.getValueByAlias("metaDescriptionCharacterLimit"), 160),
      showNoFollowOption: !!t.getValueByAlias("showNoFollowOption"),
      showNoIndexOption: !!t.getValueByAlias("showNoIndexOption")
    };
  }
  destroy() {
    o(this, f)?.removeEventListener(y.TYPE, o(this, v)), super.destroy();
  }
  render() {
    return a`
      <div id="seoSettingsProperties">
        <div id="form">
            ${r(this, i, k).call(this)}
            ${r(this, i, q).call(this)}
            ${r(this, i, R).call(this)}
        </div>

        ${r(this, i, A).call(this)}
      </div>
    `;
  }
};
E = /* @__PURE__ */ new WeakMap();
h = /* @__PURE__ */ new WeakMap();
f = /* @__PURE__ */ new WeakMap();
i = /* @__PURE__ */ new WeakSet();
N = function() {
  o(this, h) && (this.observe(o(this, h).unique, async (t) => {
    this._unique = t, await r(this, i, O).call(this);
  }, "_documentUrls"), this.observe(o(this, h).variants, (t) => {
    this._variants = t, r(this, i, _).call(this);
  }, "_variants"));
};
T = /* @__PURE__ */ new WeakMap();
v = /* @__PURE__ */ new WeakMap();
O = async function() {
  if (!this._unique)
    return;
  const { data: t } = await o(this, E).requestItems([this._unique]);
  if (t?.length) {
    const e = t[0];
    this._urls = e.urls, r(this, i, _).call(this);
  }
};
_ = function() {
  r(this, i, b).call(this), r(this, i, P).call(this);
};
b = function() {
  if (!this._urls?.length) {
    this._previewUrl = "https://www.example.com/";
    return;
  }
  const t = this._urls.find((e) => e.culture === this._culture)?.url ?? this._urls[0]?.url ?? "";
  this._previewUrl = r(this, i, D).call(this, t);
};
P = function() {
  let t = this.value?.title ?? "";
  if (!t) {
    const e = this._variants?.find((n) => n.culture === this._culture);
    e && (t = e.name);
  }
  if (t === "") {
    this._previewTitle = "";
    return;
  }
  this._config.titleSuffix && (t = `${t} ${this._config.titleSuffix}`), this._previewTitle = t;
};
D = function(t) {
  return s.protocolRegex.test(t) ? t : `${window.location.protocol}//${window.location.host}${t}`;
};
m = function(t) {
  this.value = {
    noFollowOption: !1,
    noIndexOption: !1,
    ...this.value,
    ...t
  }, this.dispatchEvent(new J());
};
L = function(t) {
  r(this, i, m).call(this, { title: t.target.value.toString() }), r(this, i, P).call(this);
};
M = function(t) {
  r(this, i, m).call(this, { metaDescription: t.target.value.toString() });
};
I = function(t) {
  r(this, i, m).call(this, { noIndexOption: t.target.checked });
};
F = function(t) {
  r(this, i, m).call(this, { noFollowOption: t.target.checked });
};
k = function() {
  const t = this.value?.title?.length ?? 0, e = this._config.titleCharacterLimit - t;
  return a`
    <div id="title">
      <uui-label>Meta Title: <small>(${e} characters remaining)</small></uui-label>
      <uui-input type="text" @input=${r(this, i, L)} .value=${this.value?.title ?? ""} label="Page's Meta Title"></uui-input>
      ${g(e < 0, () => a`<p class="error">The title has exceeded the recommended length</p>`)}
    </div>
  `;
};
q = function() {
  const t = this.value?.metaDescription?.length ?? 0, e = this._config.metaDescriptionCharacterLimit - t;
  return a`
    <div id="metaDescription">
      <uui-label>Meta Description: <small>(${e} characters remaining)</small></uui-label>
      <uui-textarea @input=${r(this, i, M)} .value=${this.value?.metaDescription ?? ""} label="Page's Meta Description" rows="6"></uui-textarea>
      ${g(e < 0, () => a`<p class="error">The meta-description has exceeded the recommended length</p>`)}
    </div>
  `;
};
R = function() {
  return a`
    ${g(this._config.showNoFollowOption, () => a`
      <div id="noFollowToggle">
        <uui-toggle @change=${r(this, i, F)} label="Prevent search engines from following links on this page" ?checked=${this.value?.noFollowOption}></uui-toggle>
      </div>
    `)}
    ${g(this._config.showNoIndexOption, () => a`
      <div id="noIndexOptionToggle">
        <uui-toggle @change=${r(this, i, I)} label="Prevent search engines from indexing this page" ?checked=${this.value?.noIndexOption}></uui-toggle>
      </div>
    `)}
  `;
};
A = function() {
  return a`
    <div id="searchPreview">
      <div>
        <h6>${this._previewTitle}</h6>
        <p class="link">${this._previewUrl}</p>
        <p>${this.value?.metaDescription}</p>
      </div>
    </div>
  `;
};
s.protocolRegex = /^https?:\/\//;
s.styles = [Q, Y`
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
  `];
u([
  p()
], s.prototype, "_unique", 2);
u([
  p()
], s.prototype, "_urls", 2);
u([
  p()
], s.prototype, "_culture", 2);
u([
  p()
], s.prototype, "_variants", 2);
u([
  p()
], s.prototype, "_previewTitle", 2);
u([
  p()
], s.prototype, "_previewUrl", 2);
u([
  p()
], s.prototype, "_config", 2);
s = u([
  X("seo-settings-property-editor-ui")
], s);
const ut = s;
export {
  s as SeoSettingsPropertyEditorUiElement,
  ut as default
};
//# sourceMappingURL=seoSettingsPropertyEditorUi.element-D2Y8dPA9.js.map
