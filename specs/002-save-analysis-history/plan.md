# Implementation Plan: Save Analysis History

**Branch**: `002-save-analysis-history` | **Date**: 2026-11-19 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/002-save-analysis-history/spec.md`

## Summary

Add durable save-and-history support to the existing Razor Pages Analyze experience. Store validated analysis snapshots in a local SQLite database through Entity Framework Core, expose them through a level-2 REST API, and add accessible client-side save and history views using the project’s existing page and styling conventions.

## Technical Context

**Language/Version**: C# with ASP.NET Core Razor Pages on .NET 10

**Primary Dependencies**: Entity Framework Core SQLite provider, ASP.NET Core MVC controller infrastructure, existing Bootstrap/site assets

**Storage**: Local SQLite database file in the application data folder, created automatically at startup; database file ignored by Git

**Testing**: xUnit; SQLite in-memory tests for the service and controller tests for validation/status codes; existing test suite must remain green

**Target Platform**: Existing ASP.NET Core web application running locally or on a supported .NET 10 host

**Project Type**: Single-project Razor Pages web application with a companion xUnit test project

**Performance Goals**: History reads and saves should complete as normal local CRUD operations; no additional scale target is required for this local feature

**Constraints**: Do not use MySQL; keep new CSS classes prefixed `history-`; use DTO validation; do not add accounts, editing, deleting, or Compare functionality; preserve keyboard and screen-reader access

**Scale/Scope**: One local saved-analysis collection for the existing application; one API controller, one History Razor Page, Analyze-page integration, and focused unit/controller tests

## Constitution Check

The constitution file contains placeholder principles and no ratified project-specific gates. The plan follows the explicit feature constraints, keeps the design simple, adds tests for the service/API contract, and avoids unrelated changes.

## Project Structure

### Documentation (this feature)

```text
specs/002-save-analysis-history/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/history-api.md
└── tasks.md                         # Created by /speckit-tasks
```

### Source Code (repository root)

```text
Cincy Biz Advisor/
├── Controllers/HistoryController.cs
├── Data/HistoryDbContext.cs
├── Models/HistoryEntry.cs
├── Models/HistoryDtos.cs
├── Pages/Analyze.cshtml             # Save button and result data hooks
├── Pages/History.cshtml             # History table shell
├── Pages/Shared/_Layout.cshtml      # History navigation link
├── Services/IHistoryService.cs
├── Services/HistoryService.cs
├── wwwroot/css/history.css
└── wwwroot/js/history.js             # Analyze save and History loading

Cincy_Biz_Advisor.Tests/
├── HistoryServiceTests.cs
└── HistoryControllerTests.cs
```

**Structure Decision**: Extend the existing single Razor Pages project with conventional Controllers, Data, Models, Services, Pages, and wwwroot files. Keep persistence behind `IHistoryService` so the controller and client contract do not depend on EF Core details, while tests can use SQLite in-memory and fake the service where appropriate.

## Complexity Tracking

No constitution violations require justification. The requested design fits the existing application and test projects without introducing another project or abstraction beyond the required service interface.
