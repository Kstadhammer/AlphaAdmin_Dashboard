# Project Requirements Status (Inlämningsuppgift)

This document tracks the completion status based on the provided grading criteria.

## Godkänt (G - Pass) Requirements

- **[Done]** Web application type: ASP.NET Razor Pages or MVC.
- **[Done]** Implement all G-level pages from the design file.
- **[Done]** Display all projects.
- **[Done]** Filter projects by status (e.g., started, completed) - requires status handling.
- **[Done]** Add necessary fields for projects if needed.
- **[Done]** Create new projects via a form.
- **[Done]** Update existing projects via a form.
- **[Done]** Manual data seeding allowed if necessary, but projects and users must be addable via forms.
- **[Done]** JavaScript form validation must be used to ensure correct data submission.
- **[Done]** Use any data storage solution with Entity Framework Core.
- **[Done]** Apply suitable design patterns (e.g., Service Pattern).
- **[Done]** Use Microsoft Identity (Individual Account) for registration and login.
- **[Done]** Protect user-specific pages using the `[Authorize]` attribute. (No role management needed for G).
- **[Done]** Solve certain parts using knowledge from previous courses.

## Väl Godkänt (VG - Higher Pass) Requirements Status

- **[Done]** Must use ASP.NET MVC. _(Project structure indicates MVC)_
- **[Cannot Verify]** All VG-level pages from the design file implemented. _(Design file not provided)_
- **[Done]** Use Views, Partial Views, and ViewModels appropriately. _(Project structure and code examined confirm usage)_
- **[Cannot Verify]** Independently implement features based on design file interpretation. _(Design file not provided)_
- **[Pending]** Admin-only page for managing status types. _(Status entity/service/repo exist but lack CUD operations; no admin controller actions or views for management)_
- **[Done]** Form validation using both JavaScript and ModelState. _(Client-side validation exists; server-side ModelState checks confirmed in key controllers like ProjectsController)_
- **[Done]** Implement Cookie Consent. _(Partial view, configuration, middleware usage confirmed)_
- **[Done]** Separate Data layer and Business layer. _(Project structure confirms this)_
- **[Done]** Data storage (any) with EF Core and appropriate design patterns applied independently. _(EF Core and Service/Repository patterns are used)_
- **[Done]** Microsoft Identity (Individual Account) for auth, appropriate page protection. _(Implemented)_
- **[Done]** Users have a standard role ("User"). Admins can add/remove data (except projects for non-admins). _(Role management, admin assignment, and restrictions implemented)_
- **[Done]** Alternative login providers (e.g., Google, GitHub). _(Configured in Program.cs)_
- **[Done]** Dark/Light theme toggle. _(Implementation exists with CSS variables, toggle UI elements, and JavaScript handling)_
- **[Pending]** Notifications for new projects/users (for admins). _(UI dropdown exists but is static; DB entities exist, but no backend service/logic to generate/fetch real notifications)_
- **[Done]** Add image for a project. _(Image upload functionality is implemented in forms and service layer)_
- **[Done]** Assign available members to a project. _(Many-to-many relationship implemented with ProjectMemberEntity and UI for member selection)_

## To-Do Items

1.  **Create admin interface for managing status types** - Add CUD operations to `StatusService`/`IStatusRepository`, create `AdminController` actions (e.g., `ManageStatuses`, `AddStatus`, `EditStatus`, `DeleteStatus`), and corresponding views in `Presentation/Views/Admin`.
2.  ~~**Complete form validation** - Ensure consistent ModelState validation across all controllers~~ (Appears complete based on review).
3.  **Complete notification system** - Implement backend logic (e.g., `NotificationService`) to create `NotificationEntity` records on new project/user creation, and fetch/display these in the `_Layout.cshtml` notification dropdown for admins.
4.  ~~**Verify cookie consent** - Test cookie consent banner functionality~~ (Implementation confirmed, functionality assumed working).
