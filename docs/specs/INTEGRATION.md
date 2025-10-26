# Blazor Integration Specification

## Overview

This specification defines how the column width module integrates with XAF Blazor list views and DxGrid component.

## Integration Points

### 1. ListView Controller

Create a view controller to intercept list view rendering and apply custom column widths.

**Location**: `SenDev.Xaf.Blazor.Columns.Controllers`

```csharp
public class ColumnWidthController : ViewController<ListView>
{
    private readonly ColumnWidthManager _widthManager;
    private readonly ILogger<ColumnWidthController> _logger;

    public ColumnWidthController(
        ColumnWidthManager widthManager,
        ILogger<ColumnWidthController> logger)
    {
        _widthManager = widthManager;
        _logger = logger;

        TargetViewType = ViewType.ListView;
    }

    protected override void OnViewControlsCreated()
    {
        base.OnViewControlsCreated();

        if (View.Editor is not BlazorListEditor blazorEditor)
            return;

        ApplyColumnWidths(blazorEditor);
    }

    private void ApplyColumnWidths(BlazorListEditor editor)
    {
        try
        {
            var availableWidth = GetAvailableWidth(editor);
            var dataSource = View.CollectionSource?.Collection;

            var widths = _widthManager.CalculateColumnWidths(
                View.Model,
                availableWidth,
                dataSource);

            ApplyWidthsToGrid(editor, widths);

            _logger.LogDebug(
                "Applied column widths to {View}: {Widths}",
                View.Id,
                string.Join(", ", widths.Select(kvp => $"{kvp.Key}={kvp.Value}")));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to apply column widths to {View}", View.Id);
        }
    }

    private int GetAvailableWidth(BlazorListEditor editor)
    {
        // Default to standard desktop width
        // This can be refined with JavaScript interop to get actual container width
        return 1200;
    }

    private void ApplyWidthsToGrid(
        BlazorListEditor editor,
        IDictionary<string, string> widths)
    {
        // Apply widths to grid columns
        // Implementation depends on how we can access DxGrid columns
        foreach (var kvp in widths)
        {
            var columnId = kvp.Key;
            var width = kvp.Value;

            // Find column and set width
            // This may require extending BlazorListEditor or using model customization
        }
    }
}
```

### 2. Model Customization

Extend the Application Model to include column width settings.

**Model.DesignedDiffs.xafml**:

```xml
<?xml version="1.0" encoding="utf-8"?>
<Application Title="SenDev.Xaf.Blazor.Columns">
  <Views>
    <ListView>
      <Columns>
        <ColumnColumnWidth IsNewNode="True">
          <RelativeWidth />
          <BestFit />
          <MinWidth />
          <MaxWidth />
        </ColumnColumnWidth>
      </Columns>
    </ListView>
  </Views>
</Application>
```

**Model Interface Extension**:

```csharp
public interface IModelColumnColumnWidth : IModelColumn
{
    [Category("Column Width")]
    [Description("Relative width specification (e.g., '25%', '2*', '1fr'). Leave empty for default pixel width.")]
    string RelativeWidth { get; set; }

    [Category("Column Width")]
    [Description("Enable automatic best-fit sizing based on content")]
    bool BestFit { get; set; }

    [Category("Column Width")]
    [Description("Minimum width in pixels")]
    [Range(1, int.MaxValue)]
    int? MinWidth { get; set; }

    [Category("Column Width")]
    [Description("Maximum width in pixels")]
    [Range(1, int.MaxValue)]
    int? MaxWidth { get; set; }
}
```

### 3. DxGrid Customization

#### Option A: Component Wrapper

Create a custom grid component that wraps DxGrid:

```razor
@inherits DxGrid<TData>

<DxGrid Data="@Data"
        CssClass="@GetGridCssClass()"
        @attributes="@AdditionalAttributes">
    <Columns>
        @foreach (var column in GetConfiguredColumns())
        {
            <DxGridDataColumn Field="@column.Field"
                             Caption="@column.Caption"
                             Width="@column.Width"
                             CssClass="@column.CssClass" />
        }
    </Columns>
</DxGrid>

@code {
    private string GetGridCssClass()
    {
        return "xaf-column-width-enabled";
    }

    private IEnumerable<ColumnConfiguration> GetConfiguredColumns()
    {
        // Get columns from model with calculated widths
    }
}
```

#### Option B: CSS Class Application

Generate CSS classes for each column:

```css
/* Generated dynamically */
.xaf-list-view .column-A { width: 25%; min-width: 100px; }
.xaf-list-view .column-B { width: 2fr; }
.xaf-list-view .column-C { width: 300px; }
```

Apply via JavaScript:

```javascript
export function applyColumnWidths(gridId, columnWidths) {
    const grid = document.getElementById(gridId);
    if (!grid) return;

    columnWidths.forEach(cw => {
        const column = grid.querySelector(`[data-column-id="${cw.columnId}"]`);
        if (column) {
            column.style.width = cw.width;
            if (cw.minWidth) column.style.minWidth = cw.minWidth;
            if (cw.maxWidth) column.style.maxWidth = cw.maxWidth;
        }
    });
}
```

#### Option C: Grid Template Columns (Recommended)

Use CSS Grid layout for the entire table:

```css
.xaf-column-grid-container {
    display: grid;
    grid-template-columns: var(--column-template);
}
```

Set CSS variable dynamically:

```javascript
export function setColumnTemplate(gridId, template) {
    const grid = document.getElementById(gridId);
    if (grid) {
        grid.style.setProperty('--column-template', template);
    }
}
```

Example template:
```
200px 2fr 1fr 300px
```

### 4. JavaScript Interop

**wwwroot/columnWidthInterop.js**:

```javascript
let gridInstances = new Map();

export function initializeGrid(gridId, dotNetHelper) {
    gridInstances.set(gridId, {
        dotNetHelper: dotNetHelper,
        resizeObserver: null
    });

    setupResizeObserver(gridId);
}

export function setColumnWidths(gridId, columnWidths) {
    const grid = document.getElementById(gridId);
    if (!grid) {
        console.error(`Grid not found: ${gridId}`);
        return;
    }

    // Apply widths to columns
    columnWidths.forEach(({ columnId, width, minWidth, maxWidth }) => {
        const cells = grid.querySelectorAll(`[data-column-id="${columnId}"]`);
        cells.forEach(cell => {
            if (width) cell.style.width = width;
            if (minWidth) cell.style.minWidth = minWidth;
            if (maxWidth) cell.style.maxWidth = maxWidth;
        });
    });
}

export function getGridWidth(gridId) {
    const grid = document.getElementById(gridId);
    return grid ? grid.clientWidth : 0;
}

function setupResizeObserver(gridId) {
    const grid = document.getElementById(gridId);
    if (!grid) return;

    const instance = gridInstances.get(gridId);
    if (!instance) return;

    const resizeObserver = new ResizeObserver(debounce(() => {
        const width = grid.clientWidth;
        instance.dotNetHelper.invokeMethodAsync('OnGridResized', width);
    }, 250));

    resizeObserver.observe(grid);
    instance.resizeObserver = resizeObserver;
}

export function disposeGrid(gridId) {
    const instance = gridInstances.get(gridId);
    if (instance?.resizeObserver) {
        instance.resizeObserver.disconnect();
    }
    gridInstances.delete(gridId);
}

function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}
```

**C# Wrapper**:

```csharp
public class ColumnWidthJsInterop : IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private IJSObjectReference _module;
    private DotNetObjectReference<ColumnWidthJsInterop> _dotNetRef;

    public event EventHandler<int> GridResized;

    public async Task InitializeGridAsync(string gridId)
    {
        await EnsureModuleAsync();
        _dotNetRef = DotNetObjectReference.Create(this);
        await _module.InvokeVoidAsync("initializeGrid", gridId, _dotNetRef);
    }

    public async Task SetColumnWidthsAsync(string gridId, IEnumerable<ColumnWidthInfo> widths)
    {
        await EnsureModuleAsync();
        await _module.InvokeVoidAsync("setColumnWidths", gridId, widths);
    }

    public async Task<int> GetGridWidthAsync(string gridId)
    {
        await EnsureModuleAsync();
        return await _module.InvokeAsync<int>("getGridWidth", gridId);
    }

    [JSInvokable]
    public void OnGridResized(int width)
    {
        GridResized?.Invoke(this, width);
    }

    private async Task EnsureModuleAsync()
    {
        if (_module == null)
        {
            _module = await _jsRuntime.InvokeAsync<IJSObjectReference>(
                "import",
                "./_content/SenDev.Xaf.Blazor.Columns/columnWidthInterop.js");
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_module != null)
        {
            await _module.InvokeVoidAsync("disposeGrid", "all");
            await _module.DisposeAsync();
        }
        _dotNetRef?.Dispose();
    }
}

public record ColumnWidthInfo(string ColumnId, string Width, string MinWidth, string MaxWidth);
```

### 5. Dependency Injection

Register services in the module:

```csharp
public sealed partial class SenDevXafBlazorColumnsModule : ModuleBase
{
    public override void Setup(XafApplication application)
    {
        base.Setup(application);

        if (application is BlazorApplication blazorApp)
        {
            blazorApp.ServiceCollectionConfigurator.AddServices(ConfigureServices);
        }
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Register calculators
        services.AddTransient<IColumnWidthCalculator, RelativeWidthCalculator>();
        services.AddTransient<IColumnWidthCalculator, BestFitCalculator>();

        // Register manager
        services.AddSingleton<ColumnWidthManager>();

        // Register helpers
        services.AddTransient<WidthSpecificationParser>();
        services.AddTransient<ServerSideWidthEstimator>();

        // Register interop
        services.AddScoped<ColumnWidthJsInterop>();

        // Register options
        services.Configure<ColumnWidthOptions>(options =>
        {
            options.Enabled = true;
            options.DefaultMinWidth = 50;
            options.DefaultMaxWidth = 500;
        });
    }
}
```

## Attribute Processing

### TypesInfo Customization

Process attributes and store metadata:

```csharp
public override void CustomizeTypesInfo(ITypesInfo typesInfo)
{
    base.CustomizeTypesInfo(typesInfo);

    foreach (var typeInfo in typesInfo.PersistentTypes)
    {
        foreach (var memberInfo in typeInfo.Members)
        {
            ProcessRelativeWidthAttribute(memberInfo);
            ProcessBestFitAttribute(memberInfo);
        }
    }
}

private void ProcessRelativeWidthAttribute(IMemberInfo memberInfo)
{
    var attr = memberInfo.FindAttribute<RelativeColumnWidthAttribute>();
    if (attr == null) return;

    // Store in metadata for runtime access
    memberInfo.AddAttribute(new ModelDefaultAttribute(
        nameof(IModelColumnColumnWidth.RelativeWidth),
        attr.Width));

    if (attr.MinWidth.HasValue)
    {
        memberInfo.AddAttribute(new ModelDefaultAttribute(
            nameof(IModelColumnColumnWidth.MinWidth),
            attr.MinWidth.Value.ToString()));
    }

    if (attr.MaxWidth.HasValue)
    {
        memberInfo.AddAttribute(new ModelDefaultAttribute(
            nameof(IModelColumnColumnWidth.MaxWidth),
            attr.MaxWidth.Value.ToString()));
    }
}

private void ProcessBestFitAttribute(IMemberInfo memberInfo)
{
    var attr = memberInfo.FindAttribute<BestFitColumnWidthAttribute>();
    if (attr == null) return;

    memberInfo.AddAttribute(new ModelDefaultAttribute(
        nameof(IModelColumnColumnWidth.BestFit),
        "True"));

    memberInfo.AddAttribute(new ModelDefaultAttribute(
        nameof(IModelColumnColumnWidth.MinWidth),
        attr.MinWidth.ToString()));

    memberInfo.AddAttribute(new ModelDefaultAttribute(
        nameof(IModelColumnColumnWidth.MaxWidth),
        attr.MaxWidth.ToString()));
}
```

## Blazor Component Integration

### Custom List Editor (Advanced)

Create a custom BlazorListEditor for full control:

```csharp
public class ColumnWidthBlazorListEditor : BlazorListEditor
{
    private readonly ColumnWidthManager _widthManager;
    private readonly ColumnWidthJsInterop _jsInterop;

    public ColumnWidthBlazorListEditor(
        IModelListView model,
        ColumnWidthManager widthManager,
        ColumnWidthJsInterop jsInterop)
        : base(model)
    {
        _widthManager = widthManager;
        _jsInterop = jsInterop;
    }

    protected override async Task OnGridInitializedAsync()
    {
        await base.OnGridInitializedAsync();
        await ApplyColumnWidthsAsync();
    }

    private async Task ApplyColumnWidthsAsync()
    {
        var gridWidth = await _jsInterop.GetGridWidthAsync(GridId);

        var widths = _widthManager.CalculateColumnWidths(
            Model,
            gridWidth,
            DataSource);

        var widthInfos = widths.Select(kvp => new ColumnWidthInfo(
            kvp.Key,
            kvp.Value,
            GetMinWidth(kvp.Key),
            GetMaxWidth(kvp.Key)));

        await _jsInterop.SetColumnWidthsAsync(GridId, widthInfos);
    }
}
```

## Responsive Behavior

### Window Resize Handling

```csharp
public class GridResizeHandler
{
    private readonly ColumnWidthJsInterop _jsInterop;
    private readonly ColumnWidthManager _widthManager;
    private Timer _recalculateTimer;

    public void Initialize(string gridId, IModelListView listView, object dataSource)
    {
        _jsInterop.GridResized += (sender, newWidth) =>
        {
            // Debounce recalculation
            _recalculateTimer?.Dispose();
            _recalculateTimer = new Timer(_ =>
            {
                RecalculateWidths(gridId, listView, dataSource, newWidth);
            }, null, 250, Timeout.Infinite);
        };
    }

    private async void RecalculateWidths(
        string gridId,
        IModelListView listView,
        object dataSource,
        int newWidth)
    {
        var widths = _widthManager.CalculateColumnWidths(
            listView,
            newWidth,
            dataSource);

        var widthInfos = widths.Select(kvp => new ColumnWidthInfo(
            kvp.Key,
            kvp.Value,
            null,
            null));

        await _jsInterop.SetColumnWidthsAsync(gridId, widthInfos);
    }
}
```

## Testing Integration

### Component Tests

```csharp
[Fact]
public async Task Controller_AppliesColumnWidths_ToListView()
{
    // Arrange
    var testApp = new TestBlazorApplication();
    var module = new SenDevXafBlazorColumnsModule();
    testApp.Modules.Add(module);
    testApp.Setup();

    var objectSpace = testApp.CreateObjectSpace(typeof(TestEntity));
    var listView = testApp.CreateListView(typeof(TestEntity));

    // Act
    listView.CreateControls();

    // Assert
    var editor = (BlazorListEditor)listView.Editor;
    // Verify widths applied
}
```

### JavaScript Interop Tests

Use bUnit for Blazor component testing:

```csharp
[Fact]
public async Task JsInterop_SetsColumnWidths()
{
    using var ctx = new TestContext();

    var jsRuntime = ctx.Services.GetRequiredService<IJSRuntime>();
    var interop = new ColumnWidthJsInterop(jsRuntime);

    var widths = new[]
    {
        new ColumnWidthInfo("col1", "200px", "100px", "300px"),
        new ColumnWidthInfo("col2", "2fr", null, null)
    };

    await interop.SetColumnWidthsAsync("test-grid", widths);

    // Verify JS calls
}
```

## Documentation

### Usage Examples

**Example 1: Attribute-Based**

```csharp
public class Customer : BaseObject
{
    [RelativeColumnWidth("25%", MinWidth = 100)]
    public string FirstName { get; set; }

    [RelativeColumnWidth("25%", MinWidth = 100)]
    public string LastName { get; set; }

    [BestFitColumnWidth(MinWidth = 150, MaxWidth = 300)]
    public string Email { get; set; }

    [RelativeColumnWidth("2*")]
    public string Address { get; set; }

    [BestFitColumnWidth]
    public string Phone { get; set; }
}
```

**Example 2: Model Editor**

1. Open Model Editor
2. Navigate to Views | Customer_ListView | Columns
3. Select column
4. In properties:
   - Set RelativeWidth = "25%"
   - Set MinWidth = 100

**Example 3: Runtime Configuration**

```csharp
public class CustomerListViewController : ViewController<ListView>
{
    protected override void OnActivated()
    {
        base.OnActivated();

        if (View.ObjectTypeInfo.Type == typeof(Customer))
        {
            var firstNameCol = View.Model.Columns["FirstName"] as IModelColumnColumnWidth;
            if (firstNameCol != null)
            {
                firstNameCol.RelativeWidth = "30%";
                firstNameCol.MinWidth = 120;
            }
        }
    }
}
```

## Acceptance Criteria

- [ ] ColumnWidthController implemented and registered
- [ ] Model customization working (properties visible in Model Editor)
- [ ] JavaScript interop functional
- [ ] DxGrid integration working
- [ ] Attributes processed from business objects
- [ ] Dependency injection configured
- [ ] Window resize handling implemented
- [ ] Integration tests pass
- [ ] Documentation complete with examples

---

**Status**: Draft
**Version**: 1.0
**Last Updated**: 2025-10-26
