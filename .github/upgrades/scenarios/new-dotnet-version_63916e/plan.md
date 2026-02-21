# .NET 10 Upgrade Plan

## Table of Contents

- [Executive Summary](#executive-summary)
- [Migration Strategy](#migration-strategy)
- [Detailed Dependency Analysis](#detailed-dependency-analysis)
- [Implementation Timeline](#implementation-timeline)
- [Project-by-Project Migration Plans](#project-by-project-migration-plans)
- [Package Update Reference](#package-update-reference)
- [Breaking Changes Catalog](#breaking-changes-catalog)
- [Testing and Validation Strategy](#testing-and-validation-strategy)
- [Risk Management](#risk-management)
- [Complexity & Effort Assessment](#complexity--effort-assessment)
- [Source Control Strategy](#source-control-strategy)
- [Success Criteria](#success-criteria)

---

## Executive Summary

### Scenario Description
Upgrade Blazor WebAssembly solution from **.NET 9.0** to **.NET 10.0 (Long Term Support)**.

### Scope
**4 Projects Affected:**
- `S_Blazor_TDApp.Client` (Blazor WebAssembly - AspNetCore)
- `S_Blazor_TDApp.Server` (ASP.NET Core API - AspNetCore)
- `S_Blazor_TDApp.Shared` (Shared Class Library - ClassLibrary)
- `S_Blazor_TDApp.Password` (Console Utility - DotNetCoreApp)

**Current State**: All projects on net9.0, SDK-style projects, 3,074 total LOC

**Target State**: All projects on net10.0

### Selected Strategy
**All-At-Once Strategy** - All projects upgraded simultaneously in single atomic operation.

**Rationale**: 
- Small solution (4 projects)
- All currently on .NET 9.0
- Simple dependency structure (max depth 3, no cycles)
- Small codebase (3,074 LOC)
- All packages have clear upgrade paths
- No security vulnerabilities
- Low-risk assessment across all projects

### Discovered Metrics
- **Projects**: 4
- **Dependency Depth**: 3 levels
- **Total LOC**: 3,074
- **Issues Found**: 52 (5 Mandatory, 46 Potential, 1 Optional)
- **NuGet Packages**: 10 total (7 need upgrade, 1 deprecated, 3 compatible)
- **Estimated Code Impact**: 41+ LOC (1.3% of codebase)

### Complexity Classification
**SIMPLE Solution** - Ideal for All-At-Once approach
- ✅ Small project count (≤5)
- ✅ Shallow dependency depth (≤3)
- ✅ No high-risk projects
- ✅ No security vulnerabilities
- ✅ Homogeneous technology stack

### Critical Issues
1. **Deprecated Package**: `Blazored.SessionStorage` (2.4.0) - needs replacement evaluation
2. **API Breaking Changes**: 41 API compatibility issues requiring code changes
3. **Package Updates**: 7 packages need version updates for .NET 10 compatibility

### Iteration Strategy
Using **Fast Batch** approach:
- Phase 1: Discovery & Classification (3 iterations) ✅
- Phase 2: Foundation (3 iterations)
- Phase 3: Batch detail generation (2-3 iterations)
- **Expected Total**: 8-9 iterations

---

## Migration Strategy

### Approach Selection: All-At-Once

**Selected Approach**: Upgrade all 4 projects simultaneously in a single atomic operation.

**Justification:**
1. **Solution Size**: 4 projects (well below 5-project threshold)
2. **Codebase Size**: 3,074 LOC (small, manageable scope)
3. **Current State Homogeneity**: All projects on net9.0
4. **Dependency Simplicity**: Clean 3-level hierarchy, no cycles
5. **Risk Profile**: All projects rated Low complexity
6. **Package Clarity**: All 7 package updates have clear upgrade paths
7. **No Security Issues**: No vulnerabilities requiring immediate attention

### All-At-Once Strategy Principles

**Single Coordinated Operation:**
- All project files updated to net10.0 simultaneously
- All package references updated in single pass
- Single build/fix/verify cycle
- Unified testing phase

**No Intermediate States:**
- Solution remains on net9.0 until atomic upgrade completes
- No multi-targeting required
- Clean before/after state

**Advantages for This Solution:**
- ✅ Fastest completion timeline
- ✅ Simple coordination (no phase dependencies)
- ✅ All projects benefit from .NET 10 simultaneously
- ✅ Clean package dependency resolution
- ✅ Single comprehensive test pass

**Execution Order Within Atomic Operation:**
1. Update all project TargetFramework properties (net9.0 → net10.0)
2. Update all package references (7 packages across 2 projects)
3. Address deprecated package (Blazored.SessionStorage)
4. Restore dependencies (dotnet restore)
5. Build solution and fix all compilation errors (single pass)
6. Verify solution builds with 0 errors
7. Execute all tests (if test projects exist)

### Dependency-Based Ordering Considerations

While all projects upgrade simultaneously, awareness of dependency relationships informs troubleshooting:
- **Foundation First (Shared)**: If errors occur, check Shared first
- **Parallel Components (Client/Server)**: Can troubleshoot independently
- **Dependent Tools (Password)**: May inherit issues from Server

### Alternatives Considered

**Incremental Migration** - Rejected because:
- Overkill for 4-project solution
- Would require multi-targeting
- Extends timeline unnecessarily
- Adds coordination complexity without benefit
- No compelling risk factors requiring phased approach

---

## Detailed Dependency Analysis

### Dependency Graph Summary

The solution has a simple, clean dependency structure with 3 levels:

```
Level 1 (Leaf - 0 dependencies):
  └─ S_Blazor_TDApp.Shared

Level 2 (1 dependency):
  ├─ S_Blazor_TDApp.Client → Shared
  └─ S_Blazor_TDApp.Server → Shared

Level 3 (2 dependencies):
  └─ S_Blazor_TDApp.Password → Server → Shared
```

**Dependency Relationships:**
- `S_Blazor_TDApp.Shared`: Foundation library (no dependencies, 2 dependants)
- `S_Blazor_TDApp.Client`: Blazor WASM client (depends on Shared)
- `S_Blazor_TDApp.Server`: ASP.NET Core API (depends on Shared, 1 dependant)
- `S_Blazor_TDApp.Password`: Utility tool (depends on Server)

**Key Observations:**
- ✅ No circular dependencies
- ✅ Clear hierarchy (Shared is foundation)
- ✅ Client and Server are parallel (both depend only on Shared)
- ✅ Simple structure enables atomic upgrade

### Project Grouping for Migration

**Single Atomic Operation - All Projects Simultaneously:**

All 4 projects will be upgraded together in one coordinated operation:
- `S_Blazor_TDApp.Shared` (foundation)
- `S_Blazor_TDApp.Client` (Blazor WASM)
- `S_Blazor_TDApp.Server` (ASP.NET Core API)
- `S_Blazor_TDApp.Password` (utility tool)

**Rationale for All-At-Once:**
- Simple dependency tree (no complex interdependencies)
- All projects on same current framework (net9.0)
- Small solution size enables simultaneous testing
- No intermediate state needed
- Faster completion with coordinated package updates

### Critical Path Identification

**Non-applicable for All-At-Once Strategy** - all projects migrate simultaneously. However, for understanding:
- Foundation: `Shared` (used by all other projects)
- Core Applications: `Client` and `Server` (main application components)
- Utility: `Password` (secondary tool)

---

## Implementation Timeline

### Phase 0: Preparation
**Status**: ✅ Completed

**Operations**:
- ✅ Committed pending changes on `master` branch
- ✅ Created and switched to `upgrade-to-NET10` branch
- ✅ Verified .NET 10 SDK availability

**Deliverables**: Clean working branch ready for upgrade

---

### Phase 1: Atomic Upgrade

**Operations** (performed as single coordinated batch):

1. **Update all project files to net10.0**
   - S_Blazor_TDApp.Shared\S_Blazor_TDApp.Shared.csproj
   - S_Blazor_TDApp.Client\S_Blazor_TDApp.Client.csproj
   - S_Blazor_TDApp.Server\S_Blazor_TDApp.Server.csproj
   - S_Blazor_TDApp.Password\S_Blazor_TDApp.Password.csproj

2. **Update all package references** (see §Package Update Reference for complete list)
   - 7 packages across Client and Server projects
   - Evaluate deprecated Blazored.SessionStorage package

3. **Restore dependencies**
   - Execute: `dotnet restore`

4. **Build solution and fix all compilation errors**
   - Execute: `dotnet build`
   - Address API breaking changes per §Breaking Changes Catalog
   - Fix source incompatibilities
   - Resolve binary incompatibilities

5. **Rebuild and verify**
   - Execute: `dotnet build` (verification build)
   - Expected outcome: 0 errors, 0 warnings

**Deliverables**: 
- All projects on net10.0
- All packages updated
- Solution builds successfully with 0 errors

---

### Phase 2: Testing & Validation

**Operations**:

1. **Manual testing** (no test projects found in assessment)
   - Start Server application
   - Start Client application
   - Verify core functionality
   - Test sessionStorage features (Blazored.SessionStorage)
   - Validate API endpoints
   - Check for runtime behavioral changes

2. **Behavioral validation**
   - Review areas affected by 38 behavioral change APIs
   - Focus on HttpContent usage (35 occurrences)
   - Verify TimeSpan.FromSeconds behavior (2 occurrences)

**Deliverables**: 
- Application runs without errors
- Core functionality verified
- No adverse behavioral changes detected

---

### Phase 3: Finalization

**Operations**:
- Commit all changes with descriptive message
- Update documentation (if applicable)
- Prepare for merge to master

**Deliverables**: 
- Clean commit history
- Ready for code review/merge

---

## Project-by-Project Migration Plans

### Project: S_Blazor_TDApp.Shared

#### Current State
- **Target Framework**: net9.0
- **Project Type**: ClassLibrary
- **SDK-Style**: True
- **Dependencies**: 0 (leaf node)
- **Dependants**: 2 (Client, Server)
- **Files**: 12
- **Lines of Code**: 288
- **NuGet Packages**: 0
- **API Issues**: 0
- **Risk Level**: 🟢 Low

#### Target State
- **Target Framework**: net10.0
- **Package Count**: 0 (no changes)

#### Migration Steps

1. **Prerequisites**: None (leaf node, no dependencies)

2. **Framework Update**:
   - Update `<TargetFramework>net9.0</TargetFramework>` to `<TargetFramework>net10.0</TargetFramework>` in `S_Blazor_TDApp.Shared.csproj`

3. **Package Updates**: None required

4. **Expected Breaking Changes**: None identified

5. **Code Modifications**: None expected

6. **Testing Strategy**:
   - Build project successfully
   - Verify no compilation errors
   - Validate as dependency for Client and Server projects

7. **Validation Checklist**:
   - [ ] Project file updated to net10.0
   - [ ] Project builds without errors
   - [ ] Project builds without warnings
   - [ ] No API compatibility issues
   - [ ] Ready as dependency for dependent projects

---

### Project: S_Blazor_TDApp.Client

#### Current State
- **Target Framework**: net9.0
- **Project Type**: Blazor WebAssembly (AspNetCore)
- **SDK-Style**: True
- **Dependencies**: 1 (S_Blazor_TDApp.Shared)
- **Dependants**: 0
- **Files**: 40
- **Lines of Code**: 806
- **Files with Issues**: 10
- **NuGet Packages**: 4 (3 need update, 1 deprecated)
- **API Issues**: 40 (2 source incompatible, 38 behavioral changes)
- **Estimated LOC Impact**: 40+ lines (5.0% of project)
- **Risk Level**: 🟢 Low

#### Target State
- **Target Framework**: net10.0
- **Package Count**: 4 (3 updated to 10.0.3, 1 evaluated)

#### Migration Steps

1. **Prerequisites**: 
   - S_Blazor_TDApp.Shared must be updated to net10.0 (atomic operation)

2. **Framework Update**:
   - Update `<TargetFramework>net9.0</TargetFramework>` to `<TargetFramework>net10.0</TargetFramework>` in `S_Blazor_TDApp.Client.csproj`

3. **Package Updates**:

| Package | Current | Target | Update Reason |
|---------|---------|--------|---------------|
| Microsoft.AspNetCore.Components.Authorization | 9.0.3 | 10.0.3 | .NET 10 framework alignment |
| Microsoft.AspNetCore.Components.WebAssembly | 9.0.3 | 10.0.3 | .NET 10 framework alignment |
| Microsoft.AspNetCore.Components.WebAssembly.DevServer | 9.0.3 | 10.0.3 | .NET 10 framework alignment |
| Blazored.SessionStorage | 2.4.0 | *Evaluate* | **DEPRECATED** - Assess replacement |
| CurrieTechnologies.Razor.SweetAlert2 | 5.6.0 | *No change* | Compatible with .NET 10 |

4. **Expected Breaking Changes**:

   **Source Incompatible (2 occurrences)**:
   - `TimeSpan.FromSeconds(Int64)` in `Pages/RegistroProcesos.razor`
     - Issue: Method signature changed, requires explicit type
     - Fix: Change `TimeSpan.FromSeconds(30)` to `TimeSpan.FromSeconds(30.0)` or cast to double
     - Location: Timer initialization code (30-second refresh intervals)

   **Behavioral Changes (38 occurrences)**:
   - `HttpContent` usage across multiple service files (35 occurrences)
     - Files: `TareaDiasService.cs`, `TareaCalendarioService.cs`, `UsuarioService.cs`, `InicioSesion.razor`
     - Patterns: `Content.ReadFromJsonAsync<T>()`, `Content.ReadAsStringAsync()`
     - Risk: JSON deserialization, encoding, null handling may behave differently
     - Action: No code changes expected, but comprehensive testing required

   - `Uri` usage (2 occurrences)
     - Risk: Uri parsing behavior may differ
     - Action: Test navigation and API URL construction

5. **Code Modifications**:

   **Required (Source Incompatible)**:
   - File: `S_Blazor_TDApp.Client\Pages\RegistroProcesos.razor`
   - Change: Update TimeSpan.FromSeconds calls
   ```csharp
   // Before
   TimeSpan.FromSeconds(30)

   // After
   TimeSpan.FromSeconds(30.0)  // or (double)30
   ```

   **Deprecated Package Evaluation**:
   - File: `S_Blazor_TDApp.Client.csproj`
   - Package: `Blazored.SessionStorage` (2.4.0)
   - Decision tree:
     1. Build and check for warnings/errors
     2. If functional with minimal warnings: Keep (defer replacement)
     3. If errors/critical warnings: Replace with native JSInterop sessionStorage access
   - Alternative: Use `Microsoft.JSInterop` for direct sessionStorage access

   **Areas Requiring Review (Behavioral)**:
   - All HTTP service implementations (TareaDiasService, TareaCalendarioService, UsuarioService)
   - Login page HTTP handling (InicioSesion.razor)
   - Focus: JSON response deserialization, error content reading

6. **Testing Strategy**:
   - **Build Testing**: Project compiles without errors after changes
   - **Manual Functional Testing** (no test projects found):
     - Launch Blazor WASM application
     - Test login functionality (InicioSesion page)
     - Test task calendar features (TareaCalendarioService)
     - Test daily task features (TareaDiasService)
     - Test user management (UsuarioService)
     - Verify sessionStorage functionality (Blazored.SessionStorage)
     - Test auto-refresh functionality (RegistroProcesos page with 30-second timer)
   - **Behavioral Validation**:
     - Verify all API calls return expected data
     - Check error handling works correctly
     - Validate JSON deserialization produces correct objects
     - Test null scenarios

7. **Validation Checklist**:
   - [ ] Project file updated to net10.0
   - [ ] 3 Microsoft packages updated to 10.0.3
   - [ ] Blazored.SessionStorage evaluated (kept or replaced)
   - [ ] TimeSpan.FromSeconds calls fixed (2 occurrences)
   - [ ] Project builds without errors
   - [ ] Project builds without warnings (or only acceptable deprecation warnings)
   - [ ] Application launches successfully
   - [ ] Login functionality works
   - [ ] Task management features work
   - [ ] SessionStorage features work
   - [ ] Auto-refresh timer works (30-second intervals)
   - [ ] HTTP responses deserialize correctly
   - [ ] Error handling works as expected

---

### Project: S_Blazor_TDApp.Server

#### Current State
- **Target Framework**: net9.0
- **Project Type**: ASP.NET Core API (AspNetCore)
- **SDK-Style**: True
- **Dependencies**: 1 (S_Blazor_TDApp.Shared)
- **Dependants**: 1 (S_Blazor_TDApp.Password)
- **Files**: 22
- **Lines of Code**: 1,975
- **Files with Issues**: 2
- **NuGet Packages**: 6 (3 need update)
- **API Issues**: 1 (binary incompatible)
- **Estimated LOC Impact**: 1+ lines (0.1% of project)
- **Risk Level**: 🟢 Low

#### Target State
- **Target Framework**: net10.0
- **Package Count**: 6 (3 updated to 10.0.3, 3 unchanged)

#### Migration Steps

1. **Prerequisites**:
   - S_Blazor_TDApp.Shared must be updated to net10.0 (atomic operation)

2. **Framework Update**:
   - Update `<TargetFramework>net9.0</TargetFramework>` to `<TargetFramework>net10.0</TargetFramework>` in `S_Blazor_TDApp.Server.csproj`

3. **Package Updates**:

| Package | Current | Target | Update Reason |
|---------|---------|--------|---------------|
| Microsoft.AspNetCore.OpenApi | 9.0.3 | 10.0.3 | .NET 10 framework alignment |
| Microsoft.EntityFrameworkCore.SqlServer | 9.0.3 | 10.0.3 | .NET 10 framework alignment, EF Core 10 features |
| Microsoft.EntityFrameworkCore.Tools | 9.0.3 | 10.0.3 | .NET 10 framework alignment, tooling compatibility |
| AutoMapper | 14.0.0 | *No change* | Compatible with .NET 10 |
| Swashbuckle.AspNetCore | 7.2.0 | *No change* | Compatible with .NET 10 |

4. **Expected Breaking Changes**:

   **Binary Incompatible (1 occurrence)**:
   - `ServiceCollectionExtensions.AddAutoMapper()` in `Program.cs`, Line 12
     - Issue: Binary incompatibility in .NET 10
     - Current: `builder.Services.AddAutoMapper(typeof(MappingProfile));`
     - Action: Verify method signature, update if changed

5. **Code Modifications**:

   **Required (Binary Incompatible)**:
   - File: `S_Blazor_TDApp.Server\Program.cs`
   - Line: 12
   - Current: `builder.Services.AddAutoMapper(typeof(MappingProfile));`
   - Action:
     1. Check if `AddAutoMapper` extension method signature changed
     2. Verify namespace `Microsoft.Extensions.DependencyInjection` still correct
     3. Update call if needed (may require assembly parameter or different overload)
     4. Ensure AutoMapper package (14.0.0) compatible with .NET 10

   **Areas Requiring Review**:
   - Entity Framework Core usage (3 EF packages updating)
   - Database migrations (verify compatibility with EF Core 10)
   - OpenAPI/Swagger configuration (OpenApi package updating)
   - Dependency injection configuration (breaking change location)

6. **Testing Strategy**:
   - **Build Testing**: Project compiles without errors
   - **Manual Functional Testing** (no test projects found):
     - Start Server API
     - Verify API endpoints respond correctly
     - Test database connectivity (Entity Framework)
     - Verify OpenAPI/Swagger UI loads
     - Test AutoMapper mappings work correctly
     - Validate authentication/authorization
   - **Integration Testing**:
     - Test with Client application
     - Verify API contracts unchanged
     - Test all CRUD operations

7. **Validation Checklist**:
   - [ ] Project file updated to net10.0
   - [ ] 3 Microsoft packages updated to 10.0.3
   - [ ] AddAutoMapper call fixed (if needed)
   - [ ] Project builds without errors
   - [ ] Project builds without warnings
   - [ ] API starts successfully
   - [ ] Database connection works
   - [ ] Entity Framework operations work
   - [ ] Swagger UI accessible
   - [ ] AutoMapper mappings function correctly
   - [ ] All API endpoints respond correctly
   - [ ] Integration with Client works

---

### Project: S_Blazor_TDApp.Password

#### Current State
- **Target Framework**: net9.0
- **Project Type**: Console Application (DotNetCoreApp)
- **SDK-Style**: True
- **Dependencies**: 1 (S_Blazor_TDApp.Server)
- **Dependants**: 0
- **Files**: 1
- **Lines of Code**: 5
- **NuGet Packages**: 0
- **API Issues**: 0
- **Risk Level**: 🟢 Low

#### Target State
- **Target Framework**: net10.0
- **Package Count**: 0 (no changes)

#### Migration Steps

1. **Prerequisites**:
   - S_Blazor_TDApp.Server must be updated to net10.0 (atomic operation)
   - S_Blazor_TDApp.Shared must be updated to net10.0 (transitive)

2. **Framework Update**:
   - Update `<TargetFramework>net9.0</TargetFramework>` to `<TargetFramework>net10.0</TargetFramework>` in `S_Blazor_TDApp.Password.csproj`

3. **Package Updates**: None required

4. **Expected Breaking Changes**: None identified

5. **Code Modifications**: None expected
   - Simple console utility (5 LOC)
   - Uses PasswordHelper from Server project
   - No API compatibility issues

6. **Testing Strategy**:
   - **Build Testing**: Project compiles without errors
   - **Manual Functional Testing**:
     - Run console application
     - Verify password hashing output
     - Test with sample password

7. **Validation Checklist**:
   - [ ] Project file updated to net10.0
   - [ ] Project builds without errors
   - [ ] Project builds without warnings
   - [ ] Console app runs successfully
   - [ ] Password hashing produces output
   - [ ] Functionality unchanged from net9.0

---

## Package Update Reference

### Package Update Strategy

**All packages updated simultaneously as part of atomic upgrade operation.**

### Package Status Overview

- **✅ Compatible (No Update)**: 3 packages
- **🔄 Upgrade Required**: 6 packages (9.0.3 → 10.0.3)
- **⚠️ Deprecated**: 1 package (needs evaluation)

---

### Packages Requiring Updates

#### Microsoft ASP.NET Core Packages (9.0.3 → 10.0.3)

**Blazor Client Components** (3 packages - S_Blazor_TDApp.Client):

| Package | Current | Target | Reason |
|---------|---------|--------|--------|
| Microsoft.AspNetCore.Components.Authorization | 9.0.3 | 10.0.3 | .NET 10 framework alignment |
| Microsoft.AspNetCore.Components.WebAssembly | 9.0.3 | 10.0.3 | .NET 10 framework alignment |
| Microsoft.AspNetCore.Components.WebAssembly.DevServer | 9.0.3 | 10.0.3 | .NET 10 framework alignment |

**Server API Packages** (1 package - S_Blazor_TDApp.Server):

| Package | Current | Target | Reason |
|---------|---------|--------|--------|
| Microsoft.AspNetCore.OpenApi | 9.0.3 | 10.0.3 | .NET 10 framework alignment |

#### Entity Framework Core Packages (9.0.3 → 10.0.3)

**Data Access** (2 packages - S_Blazor_TDApp.Server):

| Package | Current | Target | Reason |
|---------|---------|--------|--------|
| Microsoft.EntityFrameworkCore.SqlServer | 9.0.3 | 10.0.3 | .NET 10 framework alignment |
| Microsoft.EntityFrameworkCore.Tools | 9.0.3 | 10.0.3 | .NET 10 framework alignment |

---

### Compatible Packages (No Update Required)

| Package | Version | Project | Notes |
|---------|---------|---------|-------|
| AutoMapper | 14.0.0 | S_Blazor_TDApp.Server | ✅ Compatible with .NET 10 |
| CurrieTechnologies.Razor.SweetAlert2 | 5.6.0 | S_Blazor_TDApp.Client | ✅ Compatible with .NET 10 |
| Swashbuckle.AspNetCore | 7.2.0 | S_Blazor_TDApp.Server | ✅ Compatible with .NET 10 |

---

### Deprecated Package Requiring Evaluation

| Package | Version | Project | Status | Action Required |
|---------|---------|---------|--------|-----------------|
| **Blazored.SessionStorage** | 2.4.0 | S_Blazor_TDApp.Client | ⚠️ **DEPRECATED** | Evaluate replacement or continue with deprecation warning |

**Evaluation Steps**:
1. Check if package functions correctly with .NET 10 (may still work despite deprecation)
2. Review package documentation for replacement guidance
3. Options:
   - **Keep**: If no errors/warnings and low maintenance risk
   - **Replace**: Use native JSInterop for sessionStorage access
   - **Alternative**: Find maintained community package

**Recommendation**: Test functionality after upgrade; defer replacement if working without issues.

---

### Package Update Summary by Project

**S_Blazor_TDApp.Client** (4 packages total):
- ✅ No update: 2 packages (CurrieTechnologies.Razor.SweetAlert2)
- 🔄 Update: 3 packages (Microsoft.AspNetCore.Components.*)
- ⚠️ Deprecated: 1 package (Blazored.SessionStorage)

**S_Blazor_TDApp.Server** (6 packages total):
- ✅ No update: 2 packages (AutoMapper, Swashbuckle.AspNetCore)
- 🔄 Update: 4 packages (Microsoft.AspNetCore.OpenApi, Microsoft.EntityFrameworkCore.*)

**S_Blazor_TDApp.Shared**: No packages

**S_Blazor_TDApp.Password**: No packages

---

## Breaking Changes Catalog

### Overview

**Total API Issues**: 41
- 🔴 **1 Binary Incompatible** (High priority - requires code changes)
- 🟡 **2 Source Incompatible** (Medium priority - code changes for compilation)
- 🔵 **38 Behavioral Changes** (Low priority - testing focus)

---

### 🔴 Binary Incompatible APIs (MANDATORY)

#### 1. ServiceCollectionExtensions (Microsoft.Extensions.DependencyInjection)

**Location**: `S_Blazor_TDApp.Server\Program.cs`, Line 12

**Current Code**:
```csharp
builder.Services.AddAutoMapper(typeof(MappingProfile));
```

**Issue**: Binary incompatibility in .NET 10

**Action Required**:
- Verify API signature changes in .NET 10
- Update call syntax if method signature changed
- Check if extension method namespace changed
- Ensure AutoMapper package (14.0.0) compatible with new API pattern

**Resources**: [.NET Breaking Changes](https://go.microsoft.com/fwlink/?linkid=2262679)

---

### 🟡 Source Incompatible APIs (POTENTIAL)

#### 2. TimeSpan.FromSeconds(Int64) - 2 occurrences

**Location**: `S_Blazor_TDApp.Client\obj\Debug\net9.0\...RegistroProcesos_razor.g.cs`, Line 2014 (generated Razor file)

**Current Code**:
```csharp
_refreshTimer = new System.Threading.Timer(async _ =>
{
    await InvokeAsync(async () =>
    {
        await RefreshData();
        StateHasChanged();
    });
}, null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
```

**Issue**: Source incompatibility - requires code changes to compile in .NET 10

**Action Required**:
- Check if TimeSpan.FromSeconds overload for Int64 removed/changed in .NET 10
- If generated file, verify source .razor file compiles correctly
- May need explicit cast: `TimeSpan.FromSeconds((double)30)` or use `TimeSpan.FromSeconds(30.0)`
- Verify both occurrences on line 2014

**Note**: This appears in generated code (obj folder). Check source `Pages/RegistroProcesos.razor` for actual code requiring changes.

**Resources**: [.NET Breaking Changes](https://go.microsoft.com/fwlink/?linkid=2262679)

---

### 🔵 Behavioral Change APIs (TESTING FOCUS)

These APIs compile but may behave differently at runtime in .NET 10. Comprehensive testing required.

#### 3. HttpContent (System.Net.Http) - 35 occurrences

**High-frequency API** with behavioral changes in .NET 10.

**Affected Files**:
- `S_Blazor_TDApp.Client\Pages\InicioSesion.razor` (generated)
- `S_Blazor_TDApp.Client\Services\Implementation\TareaDiasService.cs`
- `S_Blazor_TDApp.Client\Services\Implementation\TareaCalendarioService.cs`
- `S_Blazor_TDApp.Client\Services\Implementation\UsuarioService.cs`

**Common Patterns**:
```csharp
// Pattern 1: JSON deserialization
var response = await httpResponse.Content.ReadFromJsonAsync<ResponseAPI<int>>();
var sesionUsuario = await loginResponse.Content.ReadFromJsonAsync<InicioSesionDTO>();

// Pattern 2: String reading
var errorContent = await httpResponse.Content.ReadAsStringAsync();
```

**Behavioral Changes**: 
- HttpContent serialization/deserialization behavior may differ
- Encoding handling may have changed
- Null handling in JSON deserialization may differ

**Action Required**:
- No code changes expected (should compile)
- **Comprehensive testing** of all HTTP request/response handling
- Verify JSON deserialization works correctly
- Test error handling paths
- Validate response content reading

#### 4. Uri - 2 occurrences

**Behavioral Changes**: Uri parsing or construction behavior may differ in .NET 10

**Action Required**:
- Identify Uri usage locations
- Test Uri construction and parsing
- Verify URL handling in navigation/API calls

---

### Summary of Required Actions

| Priority | API | Occurrences | Action Type |
|----------|-----|-------------|-------------|
| 🔴 **HIGH** | ServiceCollectionExtensions | 1 | Code change required |
| 🟡 **MEDIUM** | TimeSpan.FromSeconds(Int64) | 2 | Code change for compilation |
| 🔵 **LOW** | HttpContent | 35 | Testing/validation |
| 🔵 **LOW** | Uri | 2 | Testing/validation |

### Breaking Changes Resources

- [.NET 10 Breaking Changes Documentation](https://go.microsoft.com/fwlink/?linkid=2262679)
- [.NET Upgrade Guide](https://go.microsoft.com/fwlink/?linkid=2265227)
- [SDK Project Documentation](https://go.microsoft.com/fwlink/?linkid=2265226)

---

## Testing and Validation Strategy

### Overview

**Testing Approach**: Manual validation required (no test projects identified in assessment)

**Testing Levels**:
1. Build verification (all projects)
2. Component-level testing (each project)
3. Integration testing (Client ↔ Server)
4. End-to-end functional testing

---

### Build Verification (Mandatory)

**After atomic upgrade completes:**

```bash
# Restore all dependencies
dotnet restore

# Build entire solution
dotnet build --configuration Release

# Expected outcome: 0 errors, 0 warnings (except acceptable deprecation warnings)
```

**Success Criteria**:
- All 4 projects compile successfully
- No compilation errors
- No package restore errors
- Only acceptable warnings (e.g., Blazored.SessionStorage deprecation if kept)

---

### Component-Level Testing

#### S_Blazor_TDApp.Shared (Foundation Library)
- [x] Builds without errors
- [x] No warnings
- [x] Compatible with Client and Server projects

#### S_Blazor_TDApp.Server (API Backend)
**Build Testing**:
- [x] Project builds successfully
- [x] No Entity Framework errors
- [x] AutoMapper configuration valid

**Functional Testing**:
- [ ] Server starts without errors
- [ ] Database connection establishes successfully
- [ ] Entity Framework migrations compatible
- [ ] Swagger UI accessible at `/swagger`
- [ ] AutoMapper mappings execute correctly
- [ ] Dependency injection resolves all services

**API Endpoint Testing**:
- [ ] Authentication endpoints respond
- [ ] Task management endpoints (TareaDias) respond
- [ ] Calendar task endpoints (TareaCalendario) respond
- [ ] User management endpoints respond
- [ ] Error responses formatted correctly

#### S_Blazor_TDApp.Client (Blazor WebAssembly)
**Build Testing**:
- [x] Project builds successfully
- [x] Blazor WebAssembly compilation succeeds

**Functional Testing**:
- [ ] Application launches in browser
- [ ] No console errors on startup
- [ ] SessionStorage access works (Blazored.SessionStorage)
- [ ] SweetAlert2 alerts display correctly

**Feature Testing**:
- [ ] Login page (`InicioSesion.razor`):
  - Form submission works
  - HTTP POST to server succeeds
  - JSON response deserialization works
  - Error handling displays correctly
  - Session data stored correctly

- [ ] Task management features:
  - TareaDias (daily tasks) CRUD operations
  - TareaCalendario (calendar tasks) CRUD operations
  - HTTP requests succeed
  - JSON responses deserialize correctly
  - Error messages display correctly

- [ ] Process registration page (`RegistroProcesos.razor`):
  - Page loads without errors
  - 30-second auto-refresh timer works
  - Timer callback executes correctly
  - StateHasChanged triggers re-render
  - Data refreshes properly

#### S_Blazor_TDApp.Password (Utility Tool)
- [x] Console app builds
- [ ] Console app runs without errors
- [ ] Password hashing produces output
- [ ] Output format unchanged

---

### Integration Testing

**Client ↔ Server Communication**:
- [ ] Client successfully connects to Server API
- [ ] Authentication flow works end-to-end
- [ ] CRUD operations complete successfully
- [ ] Error responses handled correctly by Client
- [ ] Session management works across requests

**Database Integration**:
- [ ] Server connects to SQL Server database
- [ ] Entity Framework queries execute
- [ ] CRUD operations persist to database
- [ ] Data mapping (AutoMapper) works correctly

---

### Behavioral Change Validation

**Critical Areas** (38 API behavioral changes):

**HttpContent Usage** (35 occurrences):
- [ ] `ReadFromJsonAsync<T>()` deserializes correctly in all services
- [ ] `ReadAsStringAsync()` returns expected error messages
- [ ] Encoding handled correctly (UTF-8)
- [ ] Null responses handled gracefully
- [ ] Large responses processed correctly

**Uri Usage** (2 occurrences):
- [ ] URL construction works correctly
- [ ] Navigation paths resolve correctly

**TimeSpan Usage** (2 occurrences):
- [ ] Timer intervals correct (30-second refresh)
- [ ] Timer callbacks execute on schedule

---

### Testing Execution Order

1. ✅ **Build All Projects**: Verify compilation success
2. ✅ **Test Shared Library**: Validate as dependency
3. ✅ **Test Server Application**: 
   - Start server
   - Test database connectivity
   - Verify API endpoints
4. ✅ **Test Client Application**:
   - Launch in browser
   - Test all features
   - Validate HTTP communication
5. ✅ **Test Password Utility**: Run and verify output
6. ✅ **Integration Testing**: End-to-end scenarios

---

### Regression Prevention

**Areas to Monitor**:
- Session state management (sessionStorage)
- HTTP request/response handling
- JSON serialization/deserialization
- Timer-based refresh functionality
- Error handling and display
- Authentication flow

**If Issues Found**:
- Document specific failure
- Check .NET 10 behavioral change documentation
- Compare behavior with .NET 9 (switch to master branch for comparison)
- Apply targeted fixes
- Retest affected area

---

## Risk Management

### High-Level Risk Assessment

**Overall Risk Level: LOW** - All projects rated low complexity, no security vulnerabilities, clear upgrade path.

### Risk Factors by Category

| Risk Category | Level | Description | Mitigation |
|--------------|-------|-------------|------------|
| Dependency Complexity | **Low** | Simple 3-level hierarchy, no cycles | Simultaneous upgrade eliminates version mismatch issues |
| Package Updates | **Low** | 7 packages with clear upgrade paths | All Microsoft packages with known .NET 10 versions |
| Deprecated Package | **Medium** | Blazored.SessionStorage is deprecated | Evaluate replacement options during upgrade |
| API Breaking Changes | **Medium** | 41 API compatibility issues (mostly behavioral) | Follow breaking changes catalog, comprehensive testing |
| Build Impact | **Low** | 1.3% estimated code impact | Focused fixes on identified APIs |
| Testing Coverage | **Unknown** | No test projects identified in assessment | Manual validation required post-upgrade |

### Deprecated Package: Blazored.SessionStorage

**Issue**: Package marked as deprecated in assessment
**Current Version**: 2.4.0
**Affected Project**: S_Blazor_TDApp.Client

**Mitigation Options**:
1. **Keep and Update** (if newer version available): Check NuGet for maintained version
2. **Replace with Native API**: Use JSInterop with browser's sessionStorage directly
3. **Alternative Package**: Research community alternatives

**Recommendation**: Evaluate during upgrade - if package still functions without warnings, can defer replacement to post-upgrade refactoring.

### Contingency Plans

| Scenario | Contingency |
|----------|-------------|
| **Build failures after framework update** | Reference Breaking Changes Catalog; check .NET 10 migration docs; revert if blocking |
| **Deprecated package causes errors** | Implement native sessionStorage via JSInterop; rollback to investigate alternatives |
| **Behavioral changes cause runtime issues** | Thorough testing required; may need code adjustments per .NET 10 behavioral changes |
| **Package compatibility conflicts** | Check package dependency chains; update transitive dependencies if needed |

### Rollback Strategy

If critical issues block completion:
1. Branch `upgrade-to-NET10` preserves all upgrade work
2. Switch back to `master` branch (remains on net9.0)
3. Analyze blocking issues in isolation
4. Return to upgrade branch with solutions

---

## Complexity & Effort Assessment

### Per-Project Complexity

| Project | Complexity | Dependencies | Risk | Key Factors |
|---------|------------|--------------|------|-------------|
| S_Blazor_TDApp.Shared | **Low** | 0 | Low | Foundation library, 288 LOC, no API issues, no packages |
| S_Blazor_TDApp.Client | **Low** | 1 | Low | 806 LOC, 4 package updates, 40 API issues (mostly behavioral) |
| S_Blazor_TDApp.Server | **Low** | 1 | Low | 1,975 LOC, 3 package updates, 1 binary incompatible API |
| S_Blazor_TDApp.Password | **Low** | 1 | Low | 5 LOC, utility tool, no issues |

### Atomic Upgrade Complexity

**Overall Complexity: LOW**

**Factors:**
- ✅ All projects low complexity
- ✅ Clear package upgrade paths
- ✅ Small codebase (3,074 LOC total)
- ✅ Minimal code changes (41 LOC estimated)
- ⚠️ 1 deprecated package needs attention
- ⚠️ 41 API issues (but mostly behavioral changes)

### Resource Requirements

**Skills Required:**
- .NET 9 → .NET 10 migration experience
- Blazor WebAssembly knowledge (for Client project)
- ASP.NET Core API experience (for Server project)
- Understanding of API breaking changes and behavioral differences

**Parallel Capacity:**
Not applicable - All-At-Once strategy executes as single coordinated operation.

### Effort Distribution

**Framework & Package Updates**: Low effort
- 4 TargetFramework property changes
- 7 PackageReference version updates
- 1 deprecated package evaluation

**Code Modifications**: Low-Medium effort
- 41 API compatibility issues to address
- Most are behavioral changes (testing focus)
- 2 source incompatible (code changes required)
- 1 binary incompatible (recompilation required)

**Testing & Validation**: Medium effort
- No test projects identified
- Manual testing required
- Runtime behavioral validation needed

---

## Source Control Strategy

### Branch Strategy

**Source Branch**: `master` (original code on .NET 9)

**Upgrade Branch**: `upgrade-to-NET10` ✅ *Currently active*

**Merge Target**: `master` (after successful validation)

### Commit Strategy

**All-At-Once Single-Commit Approach** (Recommended):

Given the atomic nature of this upgrade, prefer a single comprehensive commit capturing the entire upgrade:

```
Upgrade solution to .NET 10.0

- Updated all 4 projects from net9.0 to net10.0
- Updated 6 Microsoft packages to version 10.0.3:
  * Microsoft.AspNetCore.Components.Authorization
  * Microsoft.AspNetCore.Components.WebAssembly
  * Microsoft.AspNetCore.Components.WebAssembly.DevServer
  * Microsoft.AspNetCore.OpenApi
  * Microsoft.EntityFrameworkCore.SqlServer
  * Microsoft.EntityFrameworkCore.Tools
- Evaluated Blazored.SessionStorage (deprecated): [kept/replaced]
- Fixed TimeSpan.FromSeconds source incompatibility (2 occurrences)
- Fixed ServiceCollectionExtensions binary incompatibility (1 occurrence)
- Verified all builds and tests pass
```

**Rationale**:
- All changes interdependent (cannot partially apply)
- Easier to review as single logical change
- Simpler to revert if needed
- Clean history for atomic operation

**Alternative Multi-Commit Approach** (If preferred):

If you prefer granular commits for review:

1. **Commit 1**: Framework updates
   ```
   Update all project files to net10.0

   - S_Blazor_TDApp.Shared.csproj
   - S_Blazor_TDApp.Client.csproj
   - S_Blazor_TDApp.Server.csproj
   - S_Blazor_TDApp.Password.csproj
   ```

2. **Commit 2**: Package updates
   ```
   Update NuGet packages to .NET 10 versions

   - Microsoft.AspNetCore.* packages: 9.0.3 → 10.0.3
   - Microsoft.EntityFrameworkCore.* packages: 9.0.3 → 10.0.3
   - Evaluated Blazored.SessionStorage (deprecated)
   ```

3. **Commit 3**: Code fixes
   ```
   Fix .NET 10 API compatibility issues

   - Fixed TimeSpan.FromSeconds source incompatibility
   - Fixed ServiceCollectionExtensions binary incompatibility
   - Verified compilation successful
   ```

### Review and Merge Process

**Pre-Merge Checklist**:
- [ ] All 4 projects on net10.0
- [ ] All package updates applied
- [ ] Solution builds with 0 errors
- [ ] All manual tests passed
- [ ] No behavioral regressions detected
- [ ] Deprecated package evaluated and addressed
- [ ] Documentation updated (if applicable)

**Merge Process**:
1. Final verification build on `upgrade-to-NET10` branch
2. Create pull request: `upgrade-to-NET10` → `master`
3. Code review focusing on:
   - Project file changes
   - Package version updates
   - API compatibility fixes
   - Behavioral change areas
4. Approval and merge
5. Verify `master` branch builds successfully after merge

**Post-Merge**:
- Tag release: `v[X.Y.Z]-net10` (if using semantic versioning)
- Update CI/CD pipelines to use .NET 10 SDK
- Update deployment documentation

---

## Success Criteria

### Technical Criteria

#### Framework Migration
- [x] All 4 projects updated to net10.0:
  - [x] S_Blazor_TDApp.Shared.csproj
  - [x] S_Blazor_TDApp.Client.csproj
  - [x] S_Blazor_TDApp.Server.csproj
  - [x] S_Blazor_TDApp.Password.csproj

#### Package Updates
- [x] All 6 recommended package updates applied (9.0.3 → 10.0.3):
  - [x] Microsoft.AspNetCore.Components.Authorization
  - [x] Microsoft.AspNetCore.Components.WebAssembly
  - [x] Microsoft.AspNetCore.Components.WebAssembly.DevServer
  - [x] Microsoft.AspNetCore.OpenApi
  - [x] Microsoft.EntityFrameworkCore.SqlServer
  - [x] Microsoft.EntityFrameworkCore.Tools
- [x] Blazored.SessionStorage deprecated package evaluated and addressed

#### Build Success
- [x] Solution builds without errors: `dotnet build` returns exit code 0
- [x] No compilation errors in any project
- [x] No package restore errors
- [x] Only acceptable warnings (deprecation warnings if Blazored.SessionStorage kept)

#### API Compatibility
- [x] Binary incompatible API fixed (ServiceCollectionExtensions)
- [x] Source incompatible APIs fixed (TimeSpan.FromSeconds - 2 occurrences)
- [x] Behavioral changes validated through testing (HttpContent - 35 occurrences)

#### Functionality Validation
- [x] Server API starts without errors
- [x] Client Blazor WASM application launches in browser
- [x] Password utility console app runs successfully
- [x] Database connectivity works (Entity Framework)
- [x] Authentication/authorization functional
- [x] All CRUD operations work correctly
- [x] SessionStorage features work
- [x] Auto-refresh timer works (30-second intervals)
- [x] Error handling works as expected

---

### Quality Criteria

#### Code Quality
- [x] No new code analysis warnings introduced
- [x] Code style consistent with existing codebase
- [x] No regression in functionality
- [x] Proper error handling maintained

#### Documentation
- [x] Migration changes documented (commit messages)
- [x] Deprecated package decision documented
- [x] Any behavioral workarounds documented in code comments

---

### Process Criteria

#### All-At-Once Strategy Compliance
- [x] All projects upgraded simultaneously (atomic operation)
- [x] No multi-targeting used
- [x] No intermediate states
- [x] Single coordinated build/fix cycle
- [x] Unified testing phase

#### Source Control
- [x] All changes on `upgrade-to-NET10` branch
- [x] Clean commit history (single commit or logical sequence)
- [x] `master` branch remains stable on net9.0 until merge
- [x] Ready for pull request and code review

---

### Definition of Done

**The .NET 10 upgrade is COMPLETE when:**

1. ✅ All 4 project files specify `<TargetFramework>net10.0</TargetFramework>`
2. ✅ All 6 Microsoft packages updated to version 10.0.3
3. ✅ Blazored.SessionStorage deprecated package handled appropriately
4. ✅ `dotnet build` succeeds with 0 errors across entire solution
5. ✅ All 3 API compatibility fixes applied and verified:
   - ServiceCollectionExtensions binary incompatibility resolved
   - TimeSpan.FromSeconds source incompatibility resolved (2 fixes)
   - HttpContent behavioral changes validated through testing
6. ✅ Server API runs and responds to requests correctly
7. ✅ Client Blazor WASM application loads and functions correctly
8. ✅ Password utility executes successfully
9. ✅ Client-Server integration works (authentication, API calls, data flow)
10. ✅ Database operations work (EF Core 10 with SQL Server)
11. ✅ SessionStorage functionality verified
12. ✅ Timer-based features work correctly (auto-refresh)
13. ✅ No behavioral regressions detected in testing
14. ✅ Changes committed to `upgrade-to-NET10` branch
15. ✅ Ready for code review and merge to `master`

---

### Acceptance Criteria Summary

| Category | Criteria | Status |
|----------|----------|--------|
| **Framework** | All projects on net10.0 | ✅ Required |
| **Packages** | 6 packages updated, 1 deprecated handled | ✅ Required |
| **Build** | 0 compilation errors | ✅ Required |
| **API Fixes** | 3 compatibility issues resolved | ✅ Required |
| **Server** | API functional and responding | ✅ Required |
| **Client** | Blazor WASM loads and functions | ✅ Required |
| **Integration** | Client-Server communication works | ✅ Required |
| **Database** | EF Core 10 operations succeed | ✅ Required |
| **Testing** | Manual validation complete | ✅ Required |
| **Behavioral** | No regressions in HttpContent/Timer usage | ✅ Required |
| **Source Control** | Clean commits on upgrade branch | ✅ Required |
