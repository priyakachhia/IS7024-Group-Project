## :bust_in_silhouette:  (11/19/2026 2:31 PM)
/speckit.specify Add a Save Analysis and History feature to MetroBiz Advisor (supports Requirement 103).

Users who run an analysis on the Analyze page can save it, then see all their saved analyses on a History page.

Save:
- After an analysis shows results, a "Save Analysis" button appears. Clicking it saves the neighborhood, business type, date range, crime count, permit count, safety score, growth score, suitability score and the date and time it was saved.
- After saving, the user sees a clear confirmation message. Each click saves a new entry.

History page:
- A new History page, reachable from the top menu, lists saved analyses newest first with: neighborhood, business type, period, suitability score and saved date.
- When nothing has been saved yet, the page shows "No saved analyses found".

Saving and reading go through a REST API: POST /api/history saves an analysis and GET /api/history returns the saved list as JSON. Invalid data is rejected with a clear error and nothing is saved.

Saved analyses are kept in a local SQLite database, not MySQL, so the app needs no separate database server. Saved items are still there after the app restarts.

Accessibility: the button and form controls have clear labels, the history list uses a real table with column headers, the empty message and the save confirmation are announced to screen readers, and everything works with the keyboard.

Out of scope: editing or deleting saved analyses, user accounts, and the Compare page.
## :speech_balloon: GitHub Copilot

1. Read copilot-instructions.md, lines 1-200 :white_check_mark:
1. Read constitution.md, lines 1-240 :white_check_mark:
1. Read skill 'speckit-specify', lines 1-260 :white_check_mark:
1. Read 2 projects :white_check_mark:
1. Read skill 'speckit-specify', lines 100-345 :white_check_mark:
1. Read 0 files for '.specify/extensions.yml', '.specify/init-options.json', 'specs' :white_check_mark:
1. Read 92 files in Cincy Biz Advisor/Cincy Biz Advisor.csproj :white_check_mark:
1. Command executed: $ext = Test-Path '.specify/extensions.yml'; $init = Test-Path '.specify/init-options.json'; Write-Output "extensions=$ext"; Write-Output "init=$init"; if ($init) { Get-Content '.specify/init-options.json' }; if (Test-Path 'specs') { Get-ChildItem 'specs' -Directory | Select-Object -ExpandProperty Name } :white_check_mark:
1. Command executed: & '.specify/scripts/powershell/resolve-template.ps1' spec-template -Json :negative_squared_cross_mark:
1. Command executed: powershell.exe -ExecutionPolicy Bypass -File '.specify/scripts/powershell/resolve-template.ps1' spec-template -Json :white_check_mark:
1. Command executed: $featureDir = 'specs/002-save-analysis-history'; New-Item -ItemType Directory -Force -Path $featureDir | Out-Null; $resolved = powershell.exe -ExecutionPolicy Bypass -File '.specify/scripts/powershell/resolve-template.ps1' spec-template -Json | ConvertFrom-Json; $resolved.TEMPLATE_CONTENT | Set-Content -Path "$featureDir/spec.md" -Encoding utf8; @{ feature_directory = $featureDir } | ConvertTo-Json | Set-Content -Path '.specify/feature.json' -Encoding utf8; Write-Output "Created $featureDir/spec.md and .specify/feature.json" :white_check_mark:
1. Read spec.md, lines 1-240 :white_check_mark:
1. Command executed: Get-Date -Format 'yyyy-MM-dd' :white_check_mark:
I’m creating the feature specification and its quality checklist from the requested save/history behavior. No application code changes are being made in this `.specify` workflow.
 1. Modifying spec.md
 1. Modifying requirements.md

1. Read spec.md, lines 1-220 :white_check_mark:
1. Read requirements.md, lines 1-120 :white_check_mark:
1. Read feature.json, lines 1-30 :white_check_mark:
1. Found 7 matches for \[FEATURE NAME\]|\[DATE\]|\[NEEDS CLARIFICATION|\[specific |\[Brief Title\]|\$ARGUMENTS :white_check_mark:
 1. Modifying spec.md

1. Found 3 matches for \[FEATURE NAME\]|\[DATE\]|\[NEEDS CLARIFICATION|\[specific |\[Brief Title\]|\$ARGUMENTS|\[Describe |\[Explain |\[Assumption :white_check_mark:
1. Read spec.md, lines 1-180 :white_check_mark:
 1. Modifying spec.md

1. No matches found for \[FEATURE NAME\]|\[DATE\]|\[NEEDS CLARIFICATION|\[specific |\[Brief Title\]|\$ARGUMENTS|\[Describe |\[Explain |\[Assumption|ACTION REQUIRED :white_check_mark:
1. Read spec.md, lines 1-180 :white_check_mark:
1. Read requirements.md, lines 1-60 :white_check_mark:
1. Read spec.md, lines 100-110 :white_check_mark:
Created the feature specification for Save Analysis and History.

Files added/updated:
- `specs/002-save-analysis-history/spec.md`
- `specs/002-save-analysis-history/checklists/requirements.md`
- `.specify/feature.json`

The specification covers:
- Save Analysis flow and confirmation
- `POST /api/history` and `GET /api/history`
- SQLite persistence and restart behavior
- Validation and duplicate saves
- History page and empty state
- Accessibility and keyboard support
- Out-of-scope functionality

No application code was changed.

___

## :bust_in_silhouette:
/speckit.plan Use ASP.NET Core Razor Pages on .NET 10, the same project as the Analyze page.

Storage: SQLite through Entity Framework Core (Microsoft.EntityFrameworkCore.Sqlite). Create a HistoryEntry model and a HistoryDbContext. Create the database file automatically on first run in the app's data folder, and add the .db file to .gitignore. Do not use MySQL.

Service: add an IHistoryService interface and a HistoryService implementation (AddAsync and GetAllAsync, newest first), registered with dependency injection so tests can use a fake.

REST API: a controller at /api/history with POST (returns 201 Created with the saved entry, or 400 with validation errors) and GET (returns 200 with a JSON list, newest first). Use DTOs with validation attributes (neighborhood required, scores 0 to 100, counts not negative, period end not before period start). Use proper HTTP verbs and status codes (Richardson Maturity Level 2).

Analyze page: a "Save Analysis" button below the results calls POST /api/history with JavaScript fetch and shows a confirmation or error message in an aria-live region. Put the script in wwwroot/js and keep all new CSS classes prefixed with "history-" so other pages are not affected.

History page: a new Pages/History.cshtml with a top menu link. The page loads the list with JavaScript fetch from GET /api/history, shows a real table with column headers, and shows "No saved analyses found" when the list is empty.

Tests: xUnit tests for HistoryService using SQLite in-memory, and for the controller validation and status codes. Existing tests must keep passing.

Keep the code simple and match the style of the existing Analyze feature.
## :speech_balloon: GitHub Copilot

1. Read skill 'speckit-plan', lines 1-360 :white_check_mark:
1. Command executed: powershell.exe -ExecutionPolicy Bypass -File '.specify/scripts/powershell/setup-plan.ps1' -Json :white_check_mark:
1. Read spec.md, lines 1-140 :white_check_mark:
1. Read plan.md, lines 1-260 :white_check_mark:
1. Read constitution.md, lines 1-80 :white_check_mark:
1. Read 6 files for 'extensions.yml', 'Cincy Biz Advisor.csproj', 'Analyze.cshtml', 'Analyze.cshtml.cs', '_Layout.cshtml' :white_check_mark:
1. Read Cincy Biz Advisor.csproj, lines 1-160 :white_check_mark:
1. Read Program.cs, lines 1-220 :white_check_mark:
1. Read Analyze.cshtml.cs, lines 1-260 :white_check_mark:
1. Read Analyze.cshtml, lines 1-320 :white_check_mark:
1. Read _Layout.cshtml, lines 1-180 :white_check_mark:
1. Read 7 files in Cincy_Biz_Advisor.Tests/Cincy_Biz_Advisor.Tests.csproj :white_check_mark:
1. Read NeighborhoodAnalysis.cs, lines 1-180 :white_check_mark:
1. Read Cincy_Biz_Advisor.Tests.csproj, lines 1-140 :white_check_mark:
1. Read analyze-charts.js, lines 1-220 :white_check_mark:
1. Read .gitignore, lines 1-160 :white_check_mark:
1. Read Analyze.cshtml, lines 120-240 :white_check_mark:
 1. Modifying plan.md
 1. Modifying research.md
 1. Modifying data-model.md
 1. Modifying history-api.md
 1. Modifying quickstart.md

1. Found 6 matches for \[FEATURE\]|\[DATE\]|\[###|ACTION REQUIRED|Option [123]|NEEDS CLARIFICATION|\[link\]|\[e\.g\. :white_check_mark:
1. Read plan.md, lines 1-180 :white_check_mark:
1. Read 0 files for 'research.md', 'data-model.md', 'history-api.md', 'quickstart.md' :white_check_mark:
 1. Modifying plan.md

1. No matches found for \[FEATURE\]|\[DATE\]|\[###|ACTION REQUIRED|Option [123]|NEEDS CLARIFICATION|\[link\]|\[e\.g\. :white_check_mark:
1. Read plan.md, lines 1-180 :white_check_mark:
1. Read research.md, lines 1-180 :white_check_mark:
1. Read data-model.md, lines 1-140 :white_check_mark:
1. Read history-api.md, lines 1-180 :white_check_mark:
1. Read quickstart.md, lines 1-160 :white_check_mark:
Created the implementation plan and design artifacts for Save Analysis History.

Generated:
- `specs/002-save-analysis-history/plan.md`
- `specs/002-save-analysis-history/research.md`
- `specs/002-save-analysis-history/data-model.md`
- `specs/002-save-analysis-history/contracts/history-api.md`
- `specs/002-save-analysis-history/quickstart.md`

The plan covers:

- ASP.NET Core Razor Pages on .NET 10
- EF Core SQLite persistence with automatic database initialization
- `HistoryEntry` and `HistoryDbContext`
- `IHistoryService` and `HistoryService`
- DTO validation and REST API status codes
- Analyze-page `fetch` integration
- Accessible History page and navigation
- `history-` CSS naming convention
- xUnit service and controller tests
- Existing project structure and test compatibility

No application code was changed.

___

## :bust_in_silhouette:
/speckit.tasks
## :speech_balloon: GitHub Copilot

1. Read skill 'speckit-tasks', lines 1-360 :white_check_mark:
1. Command executed: powershell.exe -ExecutionPolicy Bypass -File '.specify/scripts/powershell/setup-tasks.ps1' -Json :white_check_mark:
1. Couldn't run get_file :negative_squared_cross_mark:
1. Couldn't run get_file :negative_squared_cross_mark:
1. Couldn't run get_file :negative_squared_cross_mark:
1. Couldn't run get_file :negative_squared_cross_mark:
1. Couldn't run get_file :negative_squared_cross_mark:
1. Couldn't run get_file :negative_squared_cross_mark:
1. Read tasks.md, lines 1-300 :white_check_mark:
1. Read feature.json, lines 1-20 :white_check_mark:
1. Read 0 files for '002-save-analysis-history/tasks.md' :white_check_mark:
 1. Modifying tasks.md

1. Couldn't run get_file :negative_squared_cross_mark:
1. Couldn't run grep_search :negative_squared_cross_mark:
1. Couldn't run grep_search :negative_squared_cross_mark:
1. Read tasks.md, lines 1-260 :white_check_mark:
1. Found 30 matches for ^- \[ \] T[0-9]{3} :white_check_mark:
1. Found 16 matches for T[0-9]{3}.*\[US[123]\] :white_check_mark:
Created `specs/002-save-analysis-history/tasks.md` with 30 dependency-ordered tasks.

The task list includes:

- EF Core SQLite setup and `.gitignore`
- `HistoryEntry`, DTOs, `HistoryDbContext`, and service abstraction
- DI and automatic database initialization
- xUnit SQLite in-memory service tests
- Controller validation and status-code tests
- Analyze-page save button and `fetch` integration
- Accessible History page and navigation
- `history-` CSS isolation
- Persistence/restart validation
- Full test, build, and quickstart verification

Tasks are organized by setup, foundational work, and prioritized user stories with parallel execution markers and dependencies.
