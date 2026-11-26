
# Metro Biz Advisor Constitution

**Status:** Proposed — Pending Team Review

## Core Principles

### I. Reliable and Traceable Data
Metro Biz Advisor must use legitimate, identifiable data sources for neighborhood analysis and comparison. Crime and building permit information must come from the configured Cincinnati Open Data datasets. The application must not invent missing values. Reporting periods and unavailable data must be communicated clearly.

### II. Modular Feature Development
Each major feature, including Analyze and Compare, should have clearly defined responsibilities and interfaces. Shared functionality should be reused rather than duplicated. Compare should consume the existing neighborhood analysis service for crime counts, permit counts, and suitability scores.

### III. Testable and Maintainable Code
The application should use readable, maintainable C# code and ASP.NET Core Razor Pages conventions. New features should include relevant automated tests. Existing tests must be run after integration changes. Input validation and error handling should be included.

### IV. Collaborative Branch Development
Team members should develop assigned features on identifiable Git branches. Pull requests should be used for code review before merging into the main branch. Cross-branch integration should be performed when features depend on one another, while preserving individual contributions and review history.

### V. User-Centered Design
The application should provide an understandable and consistent experience. Neighborhood selections, business types, charts, and comparison tables must use clear labels. Missing data and invalid selections must be explained without misleading the user.

## Technology and Project Constraints

- Framework: ASP.NET Core Razor Pages with C#.
- Frontend: Razor views, HTML, CSS, JavaScript, and Bootstrap where appropriate.
- Data: Cincinnati Open Data and other approved project datasets.
- Database: MySQL for required application data storage.
- Version Control: Git and GitHub.
- Deployment Target: Microsoft Azure.
- Feature documentation: Spec-Driven Development artifacts, including specifications, plans, and tasks.

## Development Workflow and Quality Gates

1. Document feature requirements and acceptance criteria.
2. Prepare an implementation plan and task breakdown.
3. Implement features using modular services and models.
4. Add relevant automated tests and run existing tests.
5. Verify user-facing functionality in the application.
6. Commit changes to the appropriate feature or integration branch.
7. Open a pull request and request teammate review.
8. Address review feedback before merging into main.

## Governance

This constitution documents the proposed development principles for Metro Biz Advisor.

Changes to these principles should be discussed and approved by the project team. Pull requests should be reviewed for consistency with the agreed principles, project requirements, and testing expectations.

**Version:** 1.0.0 (Proposed)
**Ratified:** Pending team approval
**Last Amended:** Not applicable
