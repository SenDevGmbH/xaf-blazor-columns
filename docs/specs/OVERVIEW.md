# SenDev.Xaf.Blazor.Columns - Project Specification Overview

## Project Purpose

This XAF module addresses a critical limitation in DevExpress XAF Blazor applications: the inability to specify column widths using anything other than pixel values. This module extends XAF Blazor to support two additional column width modes:

1. **Relative Width Mode** - Proportional column widths based on percentage or weight
2. **BestFit Mode** - Automatic column width based on content

## Problem Statement

### Current Limitation
In XAF Blazor applications, grid columns can only be sized using pixel values (`ColumnWidth` property). This creates several issues:

- **Responsive Design**: Pixel-based widths don't adapt to different screen sizes
- **Content Overflow**: Fixed widths may truncate content or leave excessive whitespace
- **User Experience**: Users cannot benefit from automatic sizing based on actual data
- **Maintenance**: Developers must manually adjust pixel values for different scenarios

### Why This Matters
Modern web applications require flexible, responsive layouts. Business applications built with XAF need to present data efficiently across various devices and screen resolutions.

## Solution Goals

### Primary Goals
1. Enable percentage/proportional column widths (e.g., "25%", "2*")
2. Enable automatic best-fit sizing based on column content
3. Maintain backward compatibility with existing pixel-based widths
4. Provide both attribute-based and runtime configuration options

### Secondary Goals
1. Support mixed mode grids (some columns pixel, some relative, some best-fit)
2. Provide customization hooks for advanced scenarios
3. Include comprehensive documentation and examples
4. Support both .NET 8.0 and .NET 9.0

## Technical Approach

### Architecture Components

1. **Column Width Attributes**
   - `RelativeColumnWidthAttribute` - For proportional sizing
   - `BestFitColumnWidthAttribute` - For automatic sizing

2. **Column Width Services**
   - `IColumnWidthCalculator` - Interface for width calculation strategies
   - `RelativeWidthCalculator` - Implements relative width logic
   - `BestFitCalculator` - Implements best-fit logic

3. **Blazor Integration**
   - List view customization for Blazor grids
   - JavaScript interop for client-side measurements (if needed)
   - CSS class generation for responsive layouts

4. **Module Infrastructure**
   - Model customization via Model.DesignedDiffs
   - Type info customization for attributes
   - View controller for runtime configuration

### Implementation Phases

#### Phase 1: Foundation (Spec: FOUNDATION.md)
- Define attribute classes
- Create basic module structure
- Implement service interfaces
- Add unit test infrastructure

#### Phase 2: Relative Width (Spec: RELATIVE-WIDTH.md)
- Implement percentage-based widths (e.g., "25%")
- Implement proportional weights (e.g., "2*")
- Handle edge cases (total != 100%, mixed units)
- Add configuration options

#### Phase 3: BestFit Width (Spec: BESTFIT-WIDTH.md)
- Implement automatic sizing logic
- Consider header vs. data width
- Add min/max constraints
- Performance optimization for large datasets

#### Phase 4: Integration & Polish (Spec: INTEGRATION.md)
- Blazor DxGrid integration
- Model editor support
- Documentation and examples
- Performance testing

## Success Criteria

### Functional Requirements
- [ ] Developer can apply `[RelativeColumnWidth("25%")]` to model properties
- [ ] Developer can apply `[BestFitColumnWidth]` to model properties
- [ ] Columns render with correct widths in Blazor list views
- [ ] Mixed-mode grids work correctly
- [ ] Works with both .NET 8.0 and .NET 9.0

### Non-Functional Requirements
- [ ] No performance degradation for grids with standard pixel widths
- [ ] Minimal impact on initial page load time
- [ ] Clear error messages for invalid configurations
- [ ] Comprehensive XML documentation
- [ ] 80%+ code coverage with unit tests

### Documentation Requirements
- [ ] README with quick start guide
- [ ] Detailed specification documents
- [ ] Code examples for common scenarios
- [ ] API reference documentation
- [ ] Migration guide from pixel widths

## Out of Scope (v1.0)

The following are explicitly excluded from the initial release:

- Windows Forms / WinForms support (Blazor only)
- Mobile (MAUI) support
- Angular/React client support
- Automatic column reordering
- Column width persistence across sessions
- Advanced features like column groups/bands

These may be considered for future versions based on user feedback.

## Dependencies

### Required Packages
- DevExpress.ExpressApp.Blazor (24.2.3 or higher)
- .NET 8.0 or 9.0 SDK

### Development Tools
- Visual Studio 2022 or JetBrains Rider
- DevExpress XAF installation

## Timeline & Milestones

This is an open-source project with flexible timeline. Suggested milestones:

1. **M1**: Foundation + Relative Width (2-3 weeks)
2. **M2**: BestFit Width (2-3 weeks)
3. **M3**: Integration & Polish (1-2 weeks)
4. **M4**: Documentation & Examples (1 week)

## Contributing

This is an open-source project. Contributions are welcome following standard GitHub workflow:

1. Fork the repository
2. Create a feature branch
3. Implement changes with tests
4. Submit pull request with clear description

## License

MIT License - See LICENSE file for details

## References

- [DevExpress XAF Documentation](https://docs.devexpress.com/eXpressAppFramework/112670/expressapp-framework)
- [DevExpress Blazor Grid Documentation](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxGrid)
- [XAF Module Development](https://docs.devexpress.com/eXpressAppFramework/118046/modules)

---

**Version**: 1.0
**Last Updated**: 2025-10-26
**Status**: Draft
