# Relative Width Specification

## Overview

This specification defines how relative/proportional column widths are calculated and applied in XAF Blazor grids.

## Width Notation Formats

### Percentage Notation

**Format**: `{number}%`

**Examples**: `"25%"`, `"33.5%"`, `"100%"`

**Semantics**:
- Direct percentage of available grid width
- Sum of all percentage columns can be ≤ 100%
- If sum < 100%, remaining space distributed to other columns

**Calculation**:
```
column_width_px = (available_width_px × percentage) / 100
```

### Star Notation

**Format**: `{number}*` or `*`

**Examples**: `"2*"`, `"*"`, `"0.5*"`

**Semantics**:
- Proportional units (CSS Grid-like)
- `*` is equivalent to `1*`
- Space divided proportionally among star columns

**Calculation**:
```
total_stars = sum(all star values)
column_width_px = (available_width_px × star_value) / total_stars
```

**Example**:
```
Available width: 1000px
Columns: A (2*), B (1*), C (1*)
Total stars: 4
A width: 1000 × 2/4 = 500px
B width: 1000 × 1/4 = 250px
C width: 1000 × 1/4 = 250px
```

### Fractional Notation

**Format**: `{number}fr`

**Examples**: `"1fr"`, `"2fr"`, `"0.5fr"`

**Semantics**:
- CSS Grid fractional units
- Identical behavior to star notation
- Provided for CSS compatibility

## Width Specification Parser

### Implementation

**Location**: `SenDev.Xaf.Blazor.Columns.Services.Parsers`

```csharp
public class WidthSpecificationParser
{
    private static readonly Regex PercentageRegex = new(@"^(\d+(?:\.\d+)?)%$");
    private static readonly Regex StarRegex = new(@"^(\d+(?:\.\d+)?)?(\*)$");
    private static readonly Regex FractionalRegex = new(@"^(\d+(?:\.\d+)?)fr$");

    public WidthSpecification Parse(string width)
    {
        if (string.IsNullOrWhiteSpace(width))
            throw new ArgumentException("Width specification cannot be empty", nameof(width));

        // Try percentage
        var match = PercentageRegex.Match(width);
        if (match.Success)
        {
            var value = double.Parse(match.Groups[1].Value);
            if (value < 0 || value > 100)
                throw new ArgumentException($"Percentage must be between 0 and 100: {width}");

            return new WidthSpecification
            {
                Type = WidthType.Percentage,
                Value = value
            };
        }

        // Try star notation
        match = StarRegex.Match(width);
        if (match.Success)
        {
            var valueStr = match.Groups[1].Value;
            var value = string.IsNullOrEmpty(valueStr) ? 1.0 : double.Parse(valueStr);

            if (value < 0)
                throw new ArgumentException($"Star value must be non-negative: {width}");

            return new WidthSpecification
            {
                Type = WidthType.Star,
                Value = value
            };
        }

        // Try fractional
        match = FractionalRegex.Match(width);
        if (match.Success)
        {
            var value = double.Parse(match.Groups[1].Value);

            if (value < 0)
                throw new ArgumentException($"Fractional value must be non-negative: {width}");

            return new WidthSpecification
            {
                Type = WidthType.Fractional,
                Value = value
            };
        }

        throw new ArgumentException($"Invalid width specification: {width}");
    }
}

public class WidthSpecification
{
    public WidthType Type { get; init; }
    public double Value { get; init; }
}

public enum WidthType
{
    Percentage,
    Star,
    Fractional,
    Pixel,
    BestFit
}
```

## Relative Width Calculator Implementation

### Core Algorithm

```csharp
public class RelativeWidthCalculator : IColumnWidthCalculator
{
    private readonly WidthSpecificationParser _parser;

    public bool CanHandle(IModelColumn column)
    {
        // Check for RelativeColumnWidthAttribute or RelativeWidth model property
        return GetRelativeWidth(column) != null;
    }

    public string CalculateWidth(ColumnWidthContext context)
    {
        var relativeWidth = GetRelativeWidth(context.Column);
        var spec = _parser.Parse(relativeWidth);

        return spec.Type switch
        {
            WidthType.Percentage => CalculatePercentageWidth(spec, context),
            WidthType.Star or WidthType.Fractional => CalculateProportionalWidth(spec, context),
            _ => throw new NotSupportedException($"Width type {spec.Type} not supported")
        };
    }

    private string CalculatePercentageWidth(WidthSpecification spec, ColumnWidthContext context)
    {
        var widthPx = (int)Math.Round(context.AvailableWidth * spec.Value / 100.0);

        // Apply constraints
        widthPx = ApplyConstraints(widthPx, context.Column);

        return $"{widthPx}px";
    }

    private string CalculateProportionalWidth(WidthSpecification spec, ColumnWidthContext context)
    {
        // Calculate total proportional units
        var totalUnits = 0.0;
        var proportionalColumns = new List<(IModelColumn Column, double Units)>();

        foreach (var col in context.AllColumns)
        {
            var relWidth = GetRelativeWidth(col);
            if (relWidth != null)
            {
                var colSpec = _parser.Parse(relWidth);
                if (colSpec.Type == WidthType.Star || colSpec.Type == WidthType.Fractional)
                {
                    totalUnits += colSpec.Value;
                    proportionalColumns.Add((col, colSpec.Value));
                }
            }
        }

        if (totalUnits == 0)
            return "auto";

        // Calculate width for this column
        var widthPx = (int)Math.Round(context.AvailableWidth * spec.Value / totalUnits);

        // Apply constraints
        widthPx = ApplyConstraints(widthPx, context.Column);

        return $"{widthPx}px";
    }

    private int ApplyConstraints(int width, IModelColumn column)
    {
        var minWidth = GetMinWidth(column);
        var maxWidth = GetMaxWidth(column);

        if (minWidth.HasValue && width < minWidth.Value)
            width = minWidth.Value;

        if (maxWidth.HasValue && width > maxWidth.Value)
            width = maxWidth.Value;

        return width;
    }

    private string GetRelativeWidth(IModelColumn column)
    {
        // Check model first
        if (column is IModelColumnColumnWidth widthColumn &&
            !string.IsNullOrEmpty(widthColumn.RelativeWidth))
        {
            return widthColumn.RelativeWidth;
        }

        // Check attribute (stored during TypesInfo customization)
        // Implementation depends on metadata storage approach
        return null;
    }

    private int? GetMinWidth(IModelColumn column)
    {
        if (column is IModelColumnColumnWidth widthColumn)
            return widthColumn.MinWidth;
        return null;
    }

    private int? GetMaxWidth(IModelColumn column)
    {
        if (column is IModelColumnColumnWidth widthColumn)
            return widthColumn.MaxWidth;
        return null;
    }
}
```

## Mixed Width Scenarios

### Scenario 1: Mix of Pixel and Percentage

**Example**:
```
Available width: 1000px
Column A: 200px (fixed)
Column B: 50%
Column C: 50%
```

**Resolution**:
1. Subtract fixed widths: 1000 - 200 = 800px available
2. Distribute 800px according to percentages:
   - B: 800 × 50% = 400px
   - C: 800 × 50% = 400px

**Algorithm**:
```csharp
public IDictionary<string, string> CalculateMixedWidths(
    IEnumerable<IModelColumn> columns,
    int availableWidth)
{
    var result = new Dictionary<string, string>();
    var remainingWidth = availableWidth;

    // Phase 1: Calculate fixed pixel widths
    foreach (var col in columns.Where(IsPixelWidth))
    {
        var width = GetPixelWidth(col);
        result[col.Id] = $"{width}px";
        remainingWidth -= width;
    }

    // Phase 2: Calculate percentage widths from remaining space
    foreach (var col in columns.Where(IsPercentageWidth))
    {
        var context = new ColumnWidthContext
        {
            Column = col,
            AvailableWidth = remainingWidth,
            AllColumns = columns
        };
        result[col.Id] = CalculateWidth(context);
    }

    // Phase 3: Calculate proportional widths from remaining space
    foreach (var col in columns.Where(IsProportionalWidth))
    {
        var context = new ColumnWidthContext
        {
            Column = col,
            AvailableWidth = remainingWidth,
            AllColumns = columns
        };
        result[col.Id] = CalculateWidth(context);
    }

    return result;
}
```

### Scenario 2: Percentage Total > 100%

**Example**:
```
Available width: 1000px
Column A: 60%
Column B: 60%
Total: 120%
```

**Resolution**: Normalize to 100%
```
A: (60 / 120) × 100% = 50% → 500px
B: (60 / 120) × 100% = 50% → 500px
```

### Scenario 3: Percentage Total < 100%

**Example**:
```
Available width: 1000px
Column A: 30%
Column B: 30%
Total: 60%
```

**Resolution Options**:

1. **Option A (Default)**: Leave remainder as flexible space
   - A: 300px
   - B: 300px
   - Remaining 400px: Distributed equally or left as auto

2. **Option B**: Proportional expansion
   - A: (30 / 60) × 100% = 50% → 500px
   - B: (30 / 60) × 100% = 50% → 500px

**Configuration**:
```csharp
public class RelativeWidthOptions
{
    /// <summary>
    /// How to handle when percentage total < 100%
    /// </summary>
    public PercentageUnderfillStrategy UnderfillStrategy { get; set; }
        = PercentageUnderfillStrategy.LeaveFlexible;
}

public enum PercentageUnderfillStrategy
{
    LeaveFlexible,    // Keep percentages as-is, leave remainder
    ExpandProportionally  // Scale up to 100%
}
```

## CSS Integration

### Grid Layout Approach

For maximum flexibility, use CSS Grid:

```css
.xaf-column-grid {
    display: grid;
    grid-template-columns: [computed from widths];
}
```

**Example Output**:
```
Columns: A (200px), B (2*), C (1*)
CSS: grid-template-columns: 200px 2fr 1fr;
```

### Benefits:
- Native browser layout engine
- Responsive by default
- Handles mixed units naturally

## Edge Cases

### Empty or Hidden Columns

**Rule**: Exclude from width calculations

```csharp
var visibleColumns = columns.Where(c => c.Index >= 0);
```

### Constraints Conflict

**Scenario**: MinWidth > MaxWidth

**Resolution**: Use MinWidth, log warning

### Insufficient Space

**Scenario**: Sum of MinWidths > Available Width

**Resolution**:
1. Honor MinWidths (allow horizontal scroll)
2. OR: Proportionally reduce MinWidths
3. Configuration option for strategy

## Performance Considerations

### Caching

Cache calculated widths until:
- Window resize
- Column configuration change
- Data source change (for best-fit columns)

```csharp
public class WidthCalculationCache
{
    private readonly Dictionary<string, CacheEntry> _cache = new();

    public string GetOrCalculate(string cacheKey, Func<string> calculator)
    {
        if (_cache.TryGetValue(cacheKey, out var entry) && !entry.IsExpired)
            return entry.Value;

        var value = calculator();
        _cache[cacheKey] = new CacheEntry { Value = value, Timestamp = DateTime.UtcNow };
        return value;
    }

    public void Invalidate(string cacheKey = null)
    {
        if (cacheKey == null)
            _cache.Clear();
        else
            _cache.Remove(cacheKey);
    }
}
```

### Debouncing

Debounce window resize events:

```javascript
let resizeTimeout;
window.addEventListener('resize', () => {
    clearTimeout(resizeTimeout);
    resizeTimeout = setTimeout(() => {
        DotNet.invokeMethodAsync('SenDev.Xaf.Blazor.Columns', 'HandleResize');
    }, 250);
});
```

## Testing Requirements

### Unit Tests

1. **Parser Tests**
   - Valid format parsing
   - Invalid format rejection
   - Edge values (0%, 100%, etc.)

2. **Calculator Tests**
   - Simple percentage calculation
   - Simple proportional calculation
   - Mixed width scenarios
   - Constraint application

### Integration Tests

1. Grid rendering with relative widths
2. Window resize handling
3. Column visibility changes

### Test Data

```csharp
public static TheoryData<string, WidthType, double> ValidWidthSpecs => new()
{
    { "25%", WidthType.Percentage, 25.0 },
    { "50%", WidthType.Percentage, 50.0 },
    { "100%", WidthType.Percentage, 100.0 },
    { "33.5%", WidthType.Percentage, 33.5 },
    { "*", WidthType.Star, 1.0 },
    { "2*", WidthType.Star, 2.0 },
    { "0.5*", WidthType.Star, 0.5 },
    { "1fr", WidthType.Fractional, 1.0 },
    { "2fr", WidthType.Fractional, 2.0 },
};
```

## Acceptance Criteria

- [ ] Width specification parser implemented and tested
- [ ] RelativeWidthCalculator implemented
- [ ] Percentage width calculations correct
- [ ] Proportional (star/fr) calculations correct
- [ ] Mixed width scenarios handled
- [ ] Min/max constraints enforced
- [ ] Unit tests pass with >90% coverage
- [ ] Integration tests pass
- [ ] Performance acceptable (< 16ms for calculation)

---

**Status**: Draft
**Version**: 1.0
**Last Updated**: 2025-10-26
