# Project Requirements Status (Inlämningsuppgift)

This document tracks the completion status based on the provided grading criteria.

## Godkänt (G - Pass) Requirements

- Web application type: ASP.NET Razor Pages or MVC.
- Implement all G-level pages from the design file.
- Display all projects.
- Filter projects by status (e.g., started, completed) - requires status handling.
- Add necessary fields for projects if needed.
- Create new projects via a form.
- Update existing projects via a form.
- Manual data seeding allowed if necessary, but projects and users must be addable via forms.
- JavaScript form validation must be used to ensure correct data submission.
- Use any data storage solution with Entity Framework Core.
- Apply suitable design patterns (e.g., Service Pattern).
- Use Microsoft Identity (Individual Account) for registration and login.
- Protect user-specific pages using the `[Authorize]` attribute. (No role management needed for G).
- Solve certain parts using knowledge from previous courses.

## Väl Godkänt (VG - Higher Pass) Requirements Status

- **[Done]** Must use ASP.NET MVC. _(Project structure indicates MVC)_
- **[Cannot Verify]** All VG-level pages from the design file implemented. _(Design file not provided)_
- **[Done]** Use Views, Partial Views, and ViewModels appropriately. _(Project structure and code examined confirm usage)_
- **[Cannot Verify]** Independently implement features based on design file interpretation. _(Design file not provided)_
- **[Pending]** Admin-only page for managing status types. _(Not implemented yet)_
- **[Partially Done / Pending]** Form validation using both JavaScript and ModelState. _(Basic JS/HTML validation exists, but explicit ModelState checks in controllers might be missing)_
- **[Pending]** Implement Cookie Consent. _(Not implemented yet)_
- **[Done]** Separate Data layer and Business layer. _(Project structure confirms this)_
- **[Done]** Data storage (any) with EF Core and appropriate design patterns applied independently. _(EF Core and Service/Repository patterns are used)_
- **[Done]** Microsoft Identity (Individual Account) for auth, appropriate page protection. _(Implemented)_
- **[Done]** Users have a standard role ("User"). Admins can add/remove data (except projects for non-admins). _(Role management, admin assignment, and restrictions implemented)_
- **[Done]** Alternative login providers (e.g., Google, GitHub). _(Configured in Program.cs)_
- **[Pending]** Dark/Light theme toggle. _(Not implemented yet)_
- **[Pending]** Notifications for new projects/users (for admins). _(Not implemented yet)_
- **[Likely Done / Needs Verification]** Add image for a project. _(Image upload logic exists for members, likely adaptable/present for projects)_
- **[Likely Done / Needs Verification]** Assign available members to a project. _(Member selection component exists, likely used in project forms)_
