# BestFit Width Specification

## Overview

This specification defines how automatic best-fit column widths are calculated in XAF Blazor grids.

## BestFit Concept

**Definition**: Automatically size column to fit its content without wrapping or truncation.

**Goals**:
1. Show all content without horizontal scrolling within the column
2. Avoid excessive whitespace
3. Maintain reasonable column widths across varying data

**Challenges**:
1. Content width varies by row
2. Font rendering differs across browsers
3. Performance impact of measuring all content
4. Dynamic data changes

## Width Measurement Strategy

### Two-Phase Approach

#### Phase 1: Server-Side Estimation

Estimate width based on:
- Character count
- Data type
- Font metrics (average character width)

**Pros**: Fast, no DOM access needed
**Cons**: Less accurate, doesn't account for actual rendering

#### Phase 2: Client-Side Measurement (Optional)

Measure actual rendered width using:
- Canvas text measurement
- Hidden DOM element measurement
- Browser's `getComputedTextLength()` for SVG

**Pros**: Accurate
**Cons**: Slower, requires JavaScript interop

**Default**: Use Phase 1 only for initial render, optionally refine with Phase 2

## Server-Side Width Estimation

### Character-Based Estimation

```csharp
public class ServerSideWidthEstimator
{
    // Average character widths for common fonts (in pixels at 14px font size)
    private const double AverageCharWidth = 8.0;  // Approximation for typical sans-serif
    private const double HeaderPadding = 32.0;    // Icon + sort indicator + padding
    private const double CellPadding = 16.0;      // Cell padding

    public int EstimateWidth(
        string headerText,
        IEnumerable<object> sampleData,
        int sampleSize,
        bool includeHeader)
    {
        var maxContentWidth = 0.0;

        // Measure header
        if (includeHeader && !string.IsNullOrEmpty(headerText))
        {
            maxContentWidth = Math.Max(maxContentWidth,
                headerText.Length * AverageCharWidth + HeaderPadding);
        }

        // Measure sample data
        var samples = sampleData.Take(sampleSize).ToList();
        foreach (var item in samples)
        {
            var text = FormatValue(item);
            if (!string.IsNullOrEmpty(text))
            {
                var estimatedWidth = EstimateTextWidth(text);
                maxContentWidth = Math.Max(maxContentWidth, estimatedWidth + CellPadding);
            }
        }

        return (int)Math.Ceiling(maxContentWidth);
    }

    private double EstimateTextWidth(string text)
    {
        // More sophisticated estimation considering character types
        var width = 0.0;

        foreach (var c in text)
        {
            if (char.IsUpper(c) || char.IsDigit(c))
                width += AverageCharWidth * 1.1;  // Upper case wider
            else if (c == 'i' || c == 'l' || c == 't')
                width += AverageCharWidth * 0.5;  // Narrow characters
            else if (c == 'w' || c == 'm' || c == 'W' || c == 'M')
                width += AverageCharWidth * 1.3;  // Wide characters
            else
                width += AverageCharWidth;
        }

        return width;
    }

    private string FormatValue(object value)
    {
        if (value == null)
            return string.Empty;

        // Use display format if available
        return value.ToString() ?? string.Empty;
    }
}
```

### Type-Specific Estimation

Different data types have different typical widths:

```csharp
public class TypeAwareWidthEstimator
{
    private static readonly Dictionary<Type, int> DefaultWidths = new()
    {
        [typeof(bool)] = 60,        // Checkbox
        [typeof(DateTime)] = 140,   // "01/31/2025 12:00 PM"
        [typeof(int)] = 80,         // Up to 9 digits
        [typeof(long)] = 100,       // Up to 15 digits
        [typeof(decimal)] = 100,    // Up to 2 decimal places
        [typeof(Guid)] = 280,       // Full GUID display
    };

    public int EstimateByType(Type type, object sampleValue)
    {
        if (DefaultWidths.TryGetValue(type, out var defaultWidth))
            return defaultWidth;

        if (type.IsEnum)
        {
            // Use longest enum value name
            var enumNames = Enum.GetNames(type);
            var maxLength = enumNames.Max(n => n.Length);
            return (int)(maxLength * AverageCharWidth + CellPadding);
        }

        // Fallback to content-based estimation
        return EstimateFromContent(sampleValue);
    }
}
```

## Client-Side Width Measurement

### JavaScript Interop

```javascript
// wwwroot/columnWidth.js
export function measureTextWidth(text, fontStyle) {
    const canvas = document.createElement('canvas');
    const context = canvas.getContext('2d');
    context.font = fontStyle || '14px -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto';

    const metrics = context.measureText(text);
    return Math.ceil(metrics.width);
}

export function measureColumnContentWidth(columnId, sampleSize) {
    const cells = document.querySelectorAll(`[data-column-id="${columnId}"]`);
    let maxWidth = 0;

    const sampled = Array.from(cells).slice(0, sampleSize);
    sampled.forEach(cell => {
        const width = cell.scrollWidth;
        if (width > maxWidth) {
            maxWidth = width;
        }
    });

    return maxWidth;
}
```

### C# Wrapper

```csharp
public class ClientSideWidthMeasurer
{
    private readonly IJSRuntime _jsRuntime;
    private IJSObjectReference _module;

    public async Task<int> MeasureTextWidthAsync(string text, string fontStyle = null)
    {
        await EnsureModuleLoadedAsync();
        return await _module.InvokeAsync<int>("measureTextWidth", text, fontStyle);
    }

    public async Task<int> MeasureColumnContentWidthAsync(string columnId, int sampleSize)
    {
        await EnsureModuleLoadedAsync();
        return await _module.InvokeAsync<int>("measureColumnContentWidth", columnId, sampleSize);
    }

    private async Task EnsureModuleLoadedAsync()
    {
        if (_module == null)
        {
            _module = await _jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/SenDev.Xaf.Blazor.Columns/columnWidth.js");
        }
    }
}
```

## BestFit Calculator Implementation

```csharp
public class BestFitCalculator : IColumnWidthCalculator
{
    private readonly ServerSideWidthEstimator _serverEstimator;
    private readonly ClientSideWidthMeasurer _clientMeasurer;
    private readonly ILogger<BestFitCalculator> _logger;

    public bool CanHandle(IModelColumn column)
    {
        return GetBestFitAttribute(column) != null ||
               (column is IModelColumnColumnWidth widthCol && widthCol.BestFit);
    }

    public async Task<string> CalculateWidthAsync(ColumnWidthContext context)
    {
        var config = GetBestFitConfiguration(context.Column);

        // Get sample data
        var sampleData = GetSampleData(
            context.DataSource,
            context.Column.PropertyName,
            config.SampleSize);

        // Server-side estimation
        var estimatedWidth = _serverEstimator.EstimateWidth(
            context.Column.Caption,
            sampleData,
            config.SampleSize,
            config.IncludeHeader);

        // Apply constraints
        var constrainedWidth = ApplyConstraints(
            estimatedWidth,
            config.MinWidth,
            config.MaxWidth);

        _logger.LogDebug(
            "BestFit for column {Column}: estimated={Estimated}px, constrained={Constrained}px",
            context.Column.Id,
            estimatedWidth,
            constrainedWidth);

        return $"{constrainedWidth}px";
    }

    private BestFitConfiguration GetBestFitConfiguration(IModelColumn column)
    {
        // Check model
        if (column is IModelColumnColumnWidth widthCol)
        {
            return new BestFitConfiguration
            {
                IncludeHeader = true,  // Default
                SampleSize = 100,      // Default
                MinWidth = widthCol.MinWidth ?? 50,
                MaxWidth = widthCol.MaxWidth ?? 500
            };
        }

        // Check attribute (from TypesInfo metadata)
        var attr = GetBestFitAttribute(column);
        if (attr != null)
        {
            return new BestFitConfiguration
            {
                IncludeHeader = attr.IncludeHeader,
                SampleSize = attr.SampleSize,
                MinWidth = attr.MinWidth,
                MaxWidth = attr.MaxWidth
            };
        }

        throw new InvalidOperationException($"Column {column.Id} is not configured for BestFit");
    }

    private IEnumerable<object> GetSampleData(
        object dataSource,
        string propertyName,
        int sampleSize)
    {
        if (dataSource == null)
            return Enumerable.Empty<object>();

        if (dataSource is IEnumerable enumerable)
        {
            return enumerable
                .Cast<object>()
                .Take(sampleSize)
                .Select(item => GetPropertyValue(item, propertyName))
                .Where(value => value != null);
        }

        return Enumerable.Empty<object>();
    }

    private object GetPropertyValue(object item, string propertyName)
    {
        var property = item.GetType().GetProperty(propertyName);
        return property?.GetValue(item);
    }

    private int ApplyConstraints(int width, int minWidth, int maxWidth)
    {
        if (width < minWidth)
            return minWidth;

        if (width > maxWidth)
            return maxWidth;

        return width;
    }

    private BestFitColumnWidthAttribute GetBestFitAttribute(IModelColumn column)
    {
        // Implementation depends on metadata storage approach
        // This would retrieve the attribute from TypesInfo or cached metadata
        return null;
    }
}

public class BestFitConfiguration
{
    public bool IncludeHeader { get; set; }
    public int SampleSize { get; set; }
    public int MinWidth { get; set; }
    public int MaxWidth { get; set; }
}
```

## Sampling Strategies

### Fixed Sample Size

Take first N rows:

```csharp
var sample = dataSource.Take(sampleSize);
```

**Pros**: Fast, deterministic
**Cons**: May miss wider values later in dataset

### Random Sampling

Take random N rows:

```csharp
var random = new Random();
var sample = dataSource
    .OrderBy(x => random.Next())
    .Take(sampleSize);
```

**Pros**: More representative
**Cons**: Non-deterministic, slower

### Smart Sampling

Sample from different parts of dataset:

```csharp
public static IEnumerable<T> SmartSample<T>(IEnumerable<T> source, int sampleSize)
{
    var list = source.ToList();
    var count = list.Count;

    if (count <= sampleSize)
        return list;

    var step = count / sampleSize;

    for (int i = 0; i < sampleSize; i++)
    {
        yield return list[i * step];
    }
}
```

**Pros**: Representative of entire dataset
**Cons**: Requires materializing collection

## Caching and Performance

### Cache Key Strategy

```csharp
public class BestFitWidthCache
{
    private readonly Dictionary<string, CachedWidth> _cache = new();

    public string GetCacheKey(IModelColumn column, object dataSource)
    {
        var dataHash = ComputeDataHash(dataSource);
        return $"{column.Id}:{column.GetHashCode()}:{dataHash}";
    }

    private string ComputeDataHash(object dataSource)
    {
        if (dataSource is IEnumerable enumerable)
        {
            // Hash based on count + first few items
            var items = enumerable.Cast<object>().Take(10).ToList();
            return $"{items.Count}:{string.Join(",", items.Select(i => i?.GetHashCode() ?? 0))}";
        }

        return dataSource?.GetHashCode().ToString() ?? "null";
    }
}
```

### Invalidation Strategy

Invalidate cache when:
1. Data source changes (add/remove/update)
2. Column configuration changes
3. Font size changes
4. Window width changes (if percentage-constrained)

```csharp
public void InvalidateCache(InvalidationReason reason, string columnId = null)
{
    _logger.LogDebug("Invalidating cache: {Reason}, Column: {Column}", reason, columnId ?? "all");

    if (columnId != null)
    {
        _cache.Keys
            .Where(k => k.StartsWith($"{columnId}:"))
            .ToList()
            .ForEach(k => _cache.Remove(k));
    }
    else
    {
        _cache.Clear();
    }
}

public enum InvalidationReason
{
    DataChanged,
    ConfigurationChanged,
    FontChanged,
    WindowResized
}
```

### Progressive Enhancement

1. **Initial Render**: Use server-side estimation
2. **After Render**: Optionally refine with client-side measurement
3. **Update**: If client measurement differs significantly, update width

```csharp
public async Task ProgressivelyCalculateWidthAsync(ColumnWidthContext context)
{
    // Phase 1: Quick server estimation
    var estimatedWidth = CalculateServerSideWidth(context);
    await ApplyWidthAsync(context.Column.Id, estimatedWidth);

    // Phase 2: Refine with client measurement (if enabled)
    if (_options.EnableClientRefinement)
    {
        var measuredWidth = await _clientMeasurer.MeasureColumnContentWidthAsync(
            context.Column.Id,
            _options.ClientMeasurementSampleSize);

        if (Math.Abs(measuredWidth - estimatedWidth) > _options.RefinementThreshold)
        {
            var refinedWidth = ApplyConstraints(measuredWidth, context.Column);
            await ApplyWidthAsync(context.Column.Id, refinedWidth);

            _logger.LogInformation(
                "Refined width for {Column}: {Estimated}px -> {Measured}px",
                context.Column.Id,
                estimatedWidth,
                refinedWidth);
        }
    }
}
```

## Edge Cases

### Empty Dataset

**Scenario**: No data rows available

**Resolution**: Use header width + type-based minimum

```csharp
if (!sampleData.Any())
{
    var headerWidth = EstimateHeaderWidth(column.Caption);
    var typeMinimum = GetTypeMinimumWidth(column.PropertyType);
    return Math.Max(headerWidth, typeMinimum);
}
```

### Very Long Content

**Scenario**: Some cells have extremely long content

**Resolution**: Use MaxWidth constraint

**Example**: 5000-character description field

```csharp
[BestFitColumnWidth(MaxWidth = 400)]
public string Description { get; set; }
```

### Formatted Values

**Scenario**: Display format differs from raw value

**Example**: `[DisplayFormat(DataFormatString = "${0:N2}")]`

**Resolution**: Use formatted value for measurement

```csharp
private string FormatValue(object value, IModelColumn column)
{
    // Check for display format attribute
    var formatAttr = column.ModelClass.TypeInfo
        .FindMember(column.PropertyName)
        .FindAttribute<DisplayFormatAttribute>();

    if (formatAttr != null && !string.IsNullOrEmpty(formatAttr.DataFormatString))
    {
        return string.Format(formatAttr.DataFormatString, value);
    }

    return value?.ToString() ?? string.Empty;
}
```

## Testing Requirements

### Unit Tests

1. **Estimation Tests**
   - Character-based width estimation
   - Type-specific estimation
   - Format handling

2. **Constraint Tests**
   - MinWidth enforcement
   - MaxWidth enforcement
   - Both constraints

3. **Sampling Tests**
   - Fixed sampling
   - Empty dataset
   - Single row dataset

### Integration Tests

1. BestFit with various data types
2. BestFit with formatted values
3. Cache invalidation
4. Performance benchmarks

### Performance Benchmarks

Target: Calculate BestFit for 100 columns with 1000 rows in < 100ms

```csharp
[Benchmark]
public void BestFit_100Columns_1000Rows()
{
    var calculator = new BestFitCalculator(/* dependencies */);

    foreach (var column in _columns)
    {
        var width = calculator.CalculateWidth(new ColumnWidthContext
        {
            Column = column,
            DataSource = _testData
        });
    }
}
```

## Configuration Options

```csharp
public class BestFitOptions
{
    /// <summary>
    /// Default sample size for width calculation
    /// </summary>
    public int DefaultSampleSize { get; set; } = 100;

    /// <summary>
    /// Default minimum width
    /// </summary>
    public int DefaultMinWidth { get; set; } = 50;

    /// <summary>
    /// Default maximum width
    /// </summary>
    public int DefaultMaxWidth { get; set; } = 500;

    /// <summary>
    /// Enable client-side measurement refinement
    /// </summary>
    public bool EnableClientRefinement { get; set; } = false;

    /// <summary>
    /// Sample size for client-side measurement
    /// </summary>
    public int ClientMeasurementSampleSize { get; set; } = 20;

    /// <summary>
    /// Threshold (in pixels) to trigger width update from client measurement
    /// </summary>
    public int RefinementThreshold { get; set; } = 20;

    /// <summary>
    /// Sampling strategy
    /// </summary>
    public SamplingStrategy SamplingStrategy { get; set; } = SamplingStrategy.Fixed;
}

public enum SamplingStrategy
{
    Fixed,    // Take first N rows
    Random,   // Random N rows
    Smart     // Distributed across dataset
}
```

## Acceptance Criteria

- [ ] Server-side width estimation implemented
- [ ] Type-aware estimation working
- [ ] Sample data extraction implemented
- [ ] Min/max constraints enforced
- [ ] BestFitCalculator integrated with ColumnWidthManager
- [ ] Caching implemented
- [ ] Edge cases handled (empty data, long content, formatted values)
- [ ] Unit tests pass with >85% coverage
- [ ] Performance benchmark meets target (< 100ms for 100 columns)
- [ ] Optional client-side refinement working

---

**Status**: Draft
**Version**: 1.0
**Last Updated**: 2025-10-26
