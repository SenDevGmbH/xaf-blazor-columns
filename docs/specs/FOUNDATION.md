# Foundation Specification

## Overview

This specification defines the foundational infrastructure for the column width enhancement module.

## Core Attributes

### RelativeColumnWidthAttribute

**Purpose**: Mark model properties to use relative/proportional column widths

**Location**: `SenDev.Xaf.Blazor.Columns.Attributes`

**Implementation**:
```csharp
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class RelativeColumnWidthAttribute : Attribute
{
    /// <summary>
    /// Gets the relative width specification (e.g., "25%", "2*", "1fr")
    /// </summary>
    public string Width { get; }

    /// <summary>
    /// Optional minimum width in pixels
    /// </summary>
    public int? MinWidth { get; set; }

    /// <summary>
    /// Optional maximum width in pixels
    /// </summary>
    public int? MaxWidth { get; set; }

    public RelativeColumnWidthAttribute(string width)
    {
        Width = width ?? throw new ArgumentNullException(nameof(width));
    }
}
```

**Validation Rules**:
- Width must not be null or empty
- Width must match one of these patterns:
  - Percentage: `\d+%` (e.g., "25%")
  - Star notation: `\d*\*` (e.g., "2*", "*")
  - Fractional: `\d+fr` (e.g., "1fr")
- MinWidth must be > 0 if specified
- MaxWidth must be > MinWidth if both specified

### BestFitColumnWidthAttribute

**Purpose**: Mark model properties to use automatic best-fit column widths

**Location**: `SenDev.Xaf.Blazor.Columns.Attributes`

**Implementation**:
```csharp
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class BestFitColumnWidthAttribute : Attribute
{
    /// <summary>
    /// Include header text in width calculation
    /// </summary>
    public bool IncludeHeader { get; set; } = true;

    /// <summary>
    /// Number of rows to sample for width calculation (0 = all rows)
    /// </summary>
    public int SampleSize { get; set; } = 100;

    /// <summary>
    /// Minimum width in pixels
    /// </summary>
    public int MinWidth { get; set; } = 50;

    /// <summary>
    /// Maximum width in pixels
    /// </summary>
    public int MaxWidth { get; set; } = 500;
}
```

**Validation Rules**:
- SampleSize must be >= 0
- MinWidth must be > 0
- MaxWidth must be > MinWidth

## Service Interfaces

### IColumnWidthCalculator

**Purpose**: Strategy interface for calculating column widths

**Location**: `SenDev.Xaf.Blazor.Columns.Services`

**Implementation**:
```csharp
public interface IColumnWidthCalculator
{
    /// <summary>
    /// Calculate pixel width for a column
    /// </summary>
    /// <param name="context">Calculation context</param>
    /// <returns>Calculated width in pixels or CSS value</returns>
    string CalculateWidth(ColumnWidthContext context);

    /// <summary>
    /// Check if this calculator can handle the given column
    /// </summary>
    bool CanHandle(IModelColumn column);
}
```

### ColumnWidthContext

**Purpose**: Provides context for width calculation

**Implementation**:
```csharp
public class ColumnWidthContext
{
    public IModelColumn Column { get; init; }
    public IModelListView ListView { get; init; }
    public int AvailableWidth { get; init; }
    public IEnumerable<IModelColumn> AllColumns { get; init; }
    public object DataSource { get; init; }
}
```

## Width Calculation Services

### RelativeWidthCalculator

**Purpose**: Calculate relative column widths

**Location**: `SenDev.Xaf.Blazor.Columns.Services`

**Responsibilities**:
- Parse relative width specifications (%, *, fr)
- Calculate pixel values based on available space
- Handle min/max constraints
- Coordinate with other columns for total width distribution

### BestFitCalculator

**Purpose**: Calculate best-fit column widths

**Location**: `SenDev.Xaf.Blazor.Columns.Services`

**Responsibilities**:
- Measure header text width
- Sample data rows for content width
- Apply min/max constraints
- Cache calculations for performance

### ColumnWidthManager

**Purpose**: Coordinate all width calculations

**Location**: `SenDev.Xaf.Blazor.Columns.Services`

**Implementation**:
```csharp
public class ColumnWidthManager
{
    private readonly IEnumerable<IColumnWidthCalculator> _calculators;

    public ColumnWidthManager(IEnumerable<IColumnWidthCalculator> calculators)
    {
        _calculators = calculators;
    }

    public IDictionary<string, string> CalculateColumnWidths(
        IModelListView listView,
        int availableWidth,
        object dataSource)
    {
        // 1. Identify columns with custom width attributes
        // 2. Delegate to appropriate calculators
        // 3. Return dictionary: columnId -> width CSS value
    }
}
```

## Model Extensions

### IModelColumnColumnWidth (extends IModelColumn)

**Purpose**: Extend model to store column width settings

**Properties**:
```csharp
public interface IModelColumnColumnWidth : IModelColumn
{
    [Category("Column Width")]
    [Description("Relative width (e.g., 25%, 2*, 1fr)")]
    string RelativeWidth { get; set; }

    [Category("Column Width")]
    [Description("Use best-fit automatic sizing")]
    bool BestFit { get; set; }

    [Category("Column Width")]
    [Description("Minimum width in pixels")]
    int? MinWidth { get; set; }

    [Category("Column Width")]
    [Description("Maximum width in pixels")]
    int? MaxWidth { get; set; }
}
```

## Module Customization

### TypesInfo Customization

In `SenDevXafBlazorColumnsModule.CustomizeTypesInfo()`:

1. Scan all business object types
2. Find properties with `RelativeColumnWidthAttribute` or `BestFitColumnWidthAttribute`
3. Store attribute metadata for runtime use

### Model Customization

In module's `Model.DesignedDiffs.xafml`:

```xml
<Application>
  <Views>
    <ListView>
      <Columns>
        <ColumnColumnWidth />
      </Columns>
    </ListView>
  </Views>
</Application>
```

## Configuration Options

### Module Options

```csharp
public class ColumnWidthOptions
{
    /// <summary>
    /// Enable/disable module functionality globally
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Default minimum column width
    /// </summary>
    public int DefaultMinWidth { get; set; } = 50;

    /// <summary>
    /// Default maximum column width for best-fit
    /// </summary>
    public int DefaultMaxWidth { get; set; } = 500;

    /// <summary>
    /// Default sample size for best-fit calculation
    /// </summary>
    public int DefaultSampleSize { get; set; } = 100;
}
```

Store in module as:
```csharp
public ColumnWidthOptions Options { get; } = new();
```

## Testing Requirements

### Unit Tests

1. **Attribute Validation Tests**
   - Valid width specifications
   - Invalid width specifications
   - Min/max constraints

2. **Parser Tests**
   - Parse percentage values
   - Parse star notation
   - Parse fractional units
   - Handle edge cases

3. **Calculator Tests**
   - Basic calculations
   - Constraint enforcement
   - Multiple columns distribution

### Integration Tests

1. Module registration and initialization
2. Attribute discovery from model
3. Service resolution from DI container

## File Structure

```
src/SenDev.Xaf.Blazor.Columns/
├── Attributes/
│   ├── RelativeColumnWidthAttribute.cs
│   └── BestFitColumnWidthAttribute.cs
├── Services/
│   ├── IColumnWidthCalculator.cs
│   ├── ColumnWidthContext.cs
│   ├── RelativeWidthCalculator.cs
│   ├── BestFitCalculator.cs
│   └── ColumnWidthManager.cs
├── Model/
│   └── IModelColumnColumnWidth.cs
├── Options/
│   └── ColumnWidthOptions.cs
├── Module.cs
└── Module.Designer.cs
```

## Acceptance Criteria

- [ ] All attribute classes implemented with validation
- [ ] Service interfaces defined
- [ ] Calculator classes created (stub implementations OK)
- [ ] Model extensions defined
- [ ] Module configuration options available
- [ ] Unit tests pass with >80% coverage
- [ ] Code compiles without warnings
- [ ] XML documentation complete

---

**Status**: Draft
**Version**: 1.0
**Last Updated**: 2025-10-26
