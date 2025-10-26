# SenDev.Xaf.Blazor.Columns

An XAF module that enhances column width management in DevExpress XAF Blazor applications. It adds **Proportional** column width mode to the standard grid list editor, enabling responsive column sizing based on relative proportions.

## Problem

By default, XAF Blazor applications only support fixed pixel-based column widths. This leads to:

- Columns that overflow or waste space on different screen sizes
- Manual maintenance of pixel values when adding or removing columns
- Poor responsiveness across devices

## Solution

This module extends the XAF model with a `ColumnWidthMode` option and replaces the default `DxGridListEditor` with a custom implementation that handles proportional width calculations transparently.

## Features

- **Proportional width mode** — column widths are treated as proportional values; the available grid width is distributed among columns according to the ratio of their widths
- **Two-level configuration** — set the width mode globally (application options) or per list view (columns node)

## Installation

Install the NuGet package:

```
dotnet add package SenDev.Xaf.Blazor.Columns
```

Then register the module using the XAF fluent builder in your `Startup.cs`:

```csharp
services.AddXaf(Configuration, builder =>
{
    builder.UseApplication<MyBlazorApplication>();
    builder.Modules
        .Add<SenDevXafBlazorColumnsModule>();
    // ...
});
```

## Requirements

- .NET 8.0 or .NET 9.0
- DevExpress XAF 24.2+

## Configuration

### Column Width Mode

The module adds a `ColumnWidthMode` property to two places in the XAF model:

| Location | Effect |
|---|---|
| `Application > Options` | Sets the default mode for all list views |
| `Views > [ListView] > Columns` | Overrides the mode for a specific list view |

**Available modes:**

| Mode | Description |
|---|---|
| `Default` | Standard XAF behavior (pixel widths) |
| `Pixel` | Explicit fixed pixel widths |
| `Proportional` | Column widths are treated as proportional values relative to each other |

### Setting Up Proportional Widths

1. In the XAF model (`Model.xafml`), navigate to **Application > Options** and set `ColumnWidthMode` to `Proportional` (applies globally), or navigate to a specific **ListView > Columns** node to set it per view.
2. Set column `Width` values to any numeric values that express the desired proportions, for example:

| Column | Width value | Share of grid width |
|---|---|---|
| Name | `400` | 400 / 1000 = 40% |
| Status | `200` | 200 / 1000 = 20% |
| CreatedOn | `200` | 200 / 1000 = 20% |
| Description | `200` | 200 / 1000 = 20% |

Only the ratios between the values matter — `400, 200, 200, 200` is equivalent to `2, 1, 1, 1` or `40, 20, 20, 20`. The actual pixel widths are calculated at runtime by distributing the available grid width proportionally:

```
column_width_px = available_width_px × column_value / sum_of_all_column_values
```

When the user resizes a column interactively, the new pixel width is converted back to a proportional value and persisted in the model, keeping all column ratios consistent.

## How It Works

The module hooks into the XAF grid rendering pipeline at two points:

1. **`ReplaceDxGridWithSenDevUpdater`** — a `ModelNodesGeneratorUpdater` that runs during model generation and replaces all `DxGridListEditor` references with the custom `SenDevGridListEditor`.

2. **`SenDevGridColumnWrapper`** — overrides the `Width` property getter and setter:
   - **Get**: returns the stored proportional value directly (e.g., `400`), which the grid interprets relative to the other columns.
   - **Set**: when the user resizes a column, the new pixel width is converted to a proportional value using `new_value × 100 / total_width` and stored in the model.

Only visible columns (`Index >= 0`) are included in the total width calculation.

## License

MIT. See [LICENSE](LICENSE) for details.
