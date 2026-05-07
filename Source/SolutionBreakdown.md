# OpenWebStudio — Solution Breakdown

> Generated: 2026-04-16  
> Version: 2.2.18  
> Solution file: `Source.sln`

---

## What Is It?

**Open Web Studio (OWS)** is a runtime engine and visual editor framework for rapidly building web application modules inside the **DotNetNuke (DNN)** portal platform. Rather than writing code for each module, developers and administrators declare module behavior in XML configuration files. The engine reads those configs at runtime and executes queries, applies data transformations, renders output, and runs server-side actions — all without custom compiled code per module.

Originally developed by **R2Integrated Inc. (copyright 2007–2013)**, OWS targets **.NET Framework 4.8** and supports DNN versions **4.7 through 10.x**.

---

## Architecture at a Glance

```
DotNetNuke Platform
       │
       ▼
Wrapper.DotNetNuke          ← DNN module integration (ASCX/ASPX controls)
       │
       ▼
UI (r2i.OWS.UI)             ← Generic web controls + Visual Editor (C#)
       │
       ▼
Engine (r2i.OWS.Engine)     ← Runtime: executes configs, plugins, lifecycle
       │
       ▼
Framework (r2i.OWS.Framework) ← Abstractions: interfaces, base classes, utilities
       │
       ▼
SqlDataProvider             ← SQL Server data access
```

---

## Project Inventory

| Project | Language | Output DLL | Role |
|---|---|---|---|
| `r2i.OWS.Framework` | VB.NET | `r2i.OWS.Framework.dll` | Core interfaces & base classes |
| `r2i.OWS.Engine` | VB.NET | `r2i.OWS.Engine.dll` | Runtime engine & plugin system |
| `r2i.OWS.UI` | VB.NET | `r2i.OWS.UI.dll` | Generic web UI controls |
| `r2i.OWS.UI.Editor` | C# | `r2i.OWS.UI.Editor.dll` | Visual XML config editor |
| `r2i.OWS.Wrapper.DotNetNuke` | VB.NET | `r2i.OWS.Wrapper.DotNetNuke.dll` | DNN module host |
| `r2i.OWS.Wrapper.DotNetNuke.Interface` | VB.NET | `…Interface.dll` | DNN interface definitions |
| `r2i.OWS.Wrapper.ASPNET` | VB.NET | `r2i.OWS.Wrapper.ASPNET.dll` | Standalone ASP.NET wrapper (no DNN) |
| `r2i.OWS.SqlDataProvider` | VB.NET | `r2i.OWS.SqlDataProvider.dll` | SQL Server data provider |
| `r2i.OWS.Wrapper.DotNetNuke.Extensions.4.3` | VB.NET | `….4.3.dll` | DNN 4.3 compatibility shim |
| `r2i.OWS.Wrapper.DotNetNuke.Extensions.4.7` | VB.NET | `….4.7.dll` | DNN 4.7 compatibility shim |
| `r2i.OWS.Wrapper.DotNetNuke.Professional.Extensions` | VB.NET | `….Professional….dll` | Pro edition extensions |

All projects target **Debug / Release** configs and output to `/Build/`.

---

## Layer Details

### 1. Framework (`r2i.OWS.Framework`) — ~94 files

The base layer everyone else depends on. Provides:

- **Config.vb** (691 lines) — Parses and manages `openwebstudio.config` XML; defines sections for Formats, Queries, Tokens, Actions, and the Wrapper.
- **DataAccess interfaces** — `IConfiguration`, `IModule`, `IUser`, `IRole`, `IPortalSettings`, `ILog`, etc.  Decouples the engine from any specific platform.
- **Entity interfaces** — `IDataController`, `ILogController`, `IModuleController`, etc.
- **Utilities** — JSON helpers (Newtonsoft wrapper), security functions, object mapping, and backward-compatibility adapters.
- **Plugin base classes** — Abstract `FormatterBase`, `QueryBase`, `RendererBase` that concrete plugins in the Engine extend.

### 2. Engine (`r2i.OWS.Engine`) — ~147 files

The heart of the system. Reads XML configuration and executes it.

**Core:**
- **Engine.vb** (1,603 lines) — Main runtime class. Manages the request lifecycle, paging/pagination, ViewState, thread-scoped storage, action variables, and request routing.
- **EngineSingleton.vb** — Singleton wrapper so a single engine instance serves all requests.

**Plugin types (all live under `/Plugins/`):**

#### Actions (~20 plugins)
Server-side operations wired together declaratively.

| Plugin | What it does |
|---|---|
| `Action.Assignment` | Sets/copies variables |
| `Action.Condition.If/Else` | Branching logic |
| `Action.Loop` | Iterates over data |
| `Action.Email` | Sends SMTP email |
| `Action.Input` | Reads HTTP request data |
| `Action.Output` | Writes to the response |
| `Action.Execute` | Runs arbitrary code |
| `Action.File` | File read/write/delete |
| `Action.Database` | Executes SQL |
| `Action.Redirect` | HTTP redirects |
| `Action.Message` | DNN messaging |
| `Action.Template.Variable` | Template-scoped variables |

#### Queries (4 plugins)
Data source abstraction — returns tabular results to the engine.

| Plugin | Source |
|---|---|
| `Query.Database` | SQL Server (stored procs / inline SQL) |
| `Query.Directory` | File system directory listings |
| `Query.XML` | XML files / feeds |
| `Query.JSON` | JSON files / APIs |

#### Formatters (~50 plugins)
Stateless transforms applied to individual values in the rendering pipeline.

Categories:
- **String** — `Upper`, `Lower`, `Trim`, `Replace`, `Left/Right/Mid`, `Pad`, `Split`
- **Encoding** — `EncodeHtml`, `EncodeUri`, `Escape`, `DecodeHtml`, `DecodeUri`
- **Security** — `Encrypt`, `Decrypt`, `MD5Hash`, `Firewall`
- **Validation** — `IsDate`, `IsNumeric`, `IsEmpty`, `IsInRole`, `IsSuperUser`
- **Inspection** — `Count`, `Length`, `Contains`, `StartsWith/EndsWith`, `IndexOf/LastIndexOf`
- **Path/URL** — `MapPath`, `MapUrl`, `ParentPath`, `ReversePath`, `FriendlyUrl`
- **Specialized** — `SqlFind`, `Diff`, `List`, `Tab`, `File`

#### Renderers (~25 plugins)
Output generation — turn data + templates into HTML/text.

| Plugin | Output |
|---|---|
| `Render.Variable` | Emits a stored variable |
| `Render.Format` | Applies formatter chain |
| `Render.IIF` | Inline if-then-else |
| `Render.Coalesce` | First non-empty value |
| `Render.Math` | Arithmetic expressions |
| `Render.Count / Sum` | Aggregate operations |
| `Render.Sort / Filter / Columns` | Data manipulation before rendering |
| `Render.Select / CheckList / Radio` | Form control HTML |
| `Render.Action / Actions` | Embeds action tokens in output |
| `Render.OpenControl` | Includes another OWS control (nesting) |
| `Render.SubQuery` | Nested data queries within a template |
| `Render.Alternate` | Alternating row styles |
| `Render.TextEditor` | Rich text (HTML) editor widget |
| `Render.Locale` | Localized string lookup |

### 3. UI Layer

**r2i.OWS.UI** — Generic ASCX controls and renderers that host the engine output inside DNN pages.

**r2i.OWS.UI.Editor** (C#) — Browser-based visual configuration editor (`Editor.aspx`). Lets non-developers build OWS modules by clicking rather than hand-editing XML.

Frontend stack used by the editor:
- jQuery 1.4.4
- jQuery UI (with themes)
- jQuery Layout (panel management)
- jstree (tree navigation)
- jQuery Templates (dynamic content)
- jQuery Cookie, Hotkeys, Theme Switcher
- Custom `OWS.jQuery.Extended.Editor.js`

### 4. DNN Wrapper (`r2i.OWS.Wrapper.DotNetNuke`)

Bridges OWS to the DotNetNuke lifecycle:

- **`dnn.ascx`** — The actual DNN module control; hosts the OWS `OpenControl`.
- **`Module.vb`** — Implements `IPortalModuleBase`; wires DNN events into the OWS engine.
- **`Admin.aspx`** — Module administration page.
- **`IM.aspx`** — Inter-module communication handler.
- **`ModuleSettings.vb`** — Persists per-module settings to DNN's settings store.
- **`SkinObject.vb`** — Allows OWS modules to appear as DNN skin objects.
- **`PackageCreator.ascx`** — Generates installable DNN module packages.
- **`installerDnn.ascx`** — Handles module installation into DNN.

Version-specific wrappers (`Extensions.4.3`, `Extensions.4.7`, `Wrapper.9.2`) handle API differences across DNN versions.

---

## Configuration System

All behavior is declared in XML. The primary config file is **`openwebstudio.config`** (root of the solution). Version-specific configs (`openwebstudio.216.config`, `217`, `218`, `2126`, `2127`) carry release-specific overrides or additions.

**Config sections:**

| Section | Contains |
|---|---|
| `<Formats>` | Registered Formatter plugins + metadata |
| `<Queries>` | Registered Query plugins |
| `<Tokens>` | Registered Renderer plugins |
| `<Actions>` | Registered Action plugins |
| `<Wrapper>` | DNN-specific connection and module settings |

At runtime, the engine reads a module's instance config (stored in DNN), resolves tokens, executes queries, pipes results through formatters, calls actions, and emits the final rendered HTML.

---

## External Dependencies

| Library | Version | Purpose |
|---|---|---|
| DotNetNuke.dll | 9.2.0.366 (+ version variants) | Portal platform APIs |
| `r2i.OWS.Newtonsoft.Json.dll` | Custom wrap | JSON serialization |
| `SharpZipLib.dll` | 0.81 | ZIP packaging |
| `Microsoft.ApplicationBlocks.Data.dll` | — | ADO.NET helper blocks |

DNN assemblies are kept per-version under `/References/DNN47/`, `/DNN6/`, `/DNN9/`, `/DNN10/`.

---

## Build & Output

- **Build output**: `/Build/`
- **Framework target**: .NET Framework 4.8 across all projects
- **Build system**: MSBuild (ToolsVersion 12.0)
- **Configs**: Debug and Release

---

## Key Metrics (approximate)

| Layer | Source Files | Est. Lines of Code |
|---|---|---|
| Framework | 94 | ~8,000 |
| Engine | 147 | ~15,000 |
| UI (both) | 12 | ~2,000 |
| Wrappers (all) | 362 | ~20,000 |
| **Total** | **~615** | **~45,000** |

---

## Runtime Flow (typical page request)

1. DNN loads the `dnn.ascx` control and fires the ASP.NET page lifecycle.
2. `Module.vb` initializes the OWS engine and resolves the module's XML config.
3. The engine parses `<Queries>` → executes database/file/JSON/XML queries → stores result sets.
4. Token rendering iterates result rows, applies Formatters to each field value.
5. Action pipeline runs (`If/Else`, `Loop`, `Email`, `Redirect`, etc.) in declared order.
6. Final HTML is written to the page response.
7. The Visual Editor (`Editor.aspx`) lets administrators inspect and modify the config in a browser UI without touching XML directly.

---

## Project History Highlights

| Date | Milestone |
|---|---|
| Jan 2009 | Multi-formatter support; JSON/XML query plugins; digest auth |
| Mar 2009 | Progressive enhancement; scheduled task execution; email SSL |
| Apr 2009 | SYSTEM queries; modeless publishing; OpenControl (nesting) token |
| May–Jun 2009 | Paging fixes; Pager token; inter-module communication corrections |
| Jul–Aug 2009 | Graphics library; custom pager rendering |
| (ongoing) | Version progression 2.01.10 → 2.01.13 → 2.2.18 |
