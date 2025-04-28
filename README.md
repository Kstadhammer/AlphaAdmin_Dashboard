# AlphaAdmin Dashboard - ASP.NET Assignment

[![.NET Build and Test](https://github.com/kimhammerstad/AlphaAdmin_Dashboard/actions/workflows/dotnet.yml/badge.svg)](https://github.com/kimhammerstad/AlphaAdmin_Dashboard/actions/workflows/dotnet.yml)

![AlphaAdmin Dashboard Preview](Presentation/wwwroot/images/admin/dashboard-preview.png)

## Project Overview

AlphaAdmin Dashboard is a web application developed as part of the ASP.NET course assignment at EC-Utbildning. It's built using ASP.NET Core MVC (.NET 9) and follows a clean, multi-layered architecture (Presentation, Business, Data, Domain).

The goal of this project is to create a functional administrative dashboard for managing projects, clients, and team members, fulfilling specific requirements for "Godkänt" (Pass) and "Väl Godkänt" (Higher Pass) grades as outlined in the assignment brief.

**Note on AI Assistance:** Parts of this codebase, particularly involving implementation details and refactoring efforts, were developed with assistance from the AI model Gemini 2.5 Pro. This usage adheres to the assignment guidelines regarding AI-generated code.

## Key Features & Assignment Requirements

This application implements features based on the assignment criteria:

- **Core Functionality (G/VG):**
  - ASP.NET MVC Framework
  - Project Listing & Filtering (by Status)
  - Project Creation & Updates via Forms
  - Client & Member Management (implicit for VG)
  - EF Core for Data Access (SQLite configured)
  - Service & Repository Patterns
  - User Registration & Login (Microsoft Identity)
  - Authorization (`[Authorize]` attribute)
- **Advanced Features (VG):**
  - Clean Architecture (Separate Data & Business Layers)
  - Views, Partial Views, and ViewModels
  - Admin-Only Management Pages (e.g., Status Types - _Partially Implemented_)
  - Server-Side (ModelState) & Client-Side (JavaScript) Validation
  - Cookie Consent Banner
  - Role-Based Authorization (Admin vs. User roles)
  - External Login Providers (Google, GitHub configured)
  - Dark/Light Theme Toggle
  - Project Image Uploads
  - Assigning Members to Projects
  - Notifications for Admins (New Projects/Users - _Partially Implemented_)

For a detailed breakdown of requirement completion status, see [PROJECT_REQUIREMENTS_STATUS.md](PROJECT_REQUIREMENTS_STATUS.md).

## Technologies Used

- **Backend**: ASP.NET Core MVC (.NET 9)
- **Database**: Entity Framework Core with SQLite (Connection string in `appsettings.json`)
- **Frontend**: HTML5, CSS3, JavaScript
- **Authentication**: ASP.NET Core Identity, Google & GitHub OAuth
- **Architecture**: Multi-layered (Presentation, Business, Data, Domain) with Service, Repository, and Factory patterns.

## Prerequisites

- .NET 9 SDK (or compatible version as per `global.json`)
- A compatible IDE (e.g., Visual Studio 2022, JetBrains Rider)

## Setup and Installation

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/kimhammerstad/AlphaAdmin_Dashboard.git
    cd AlphaAdmin_Dashboard
    ```
2.  **Ensure Database Connection:**
    - The project is configured to use SQLite (`Data Source=app.db`). The database file will be created automatically in the `Presentation` directory upon first run if it doesn't exist.
    - No changes are needed in `appsettings.json` unless you wish to switch to a different provider.
3.  **Build and Run:**
    - **Using IDE:** Open the solution (`AlphaAdmin_Dashboard.sln`) and run the `Presentation` project.
    - **Using .NET CLI:**
      `bash
dotnet run --project Presentation
`
      The application will build, apply necessary EF Core migrations automatically (including seeding initial roles, statuses, and a default admin user), and start the web server.
4.  **Access the application:**
    - Open your browser and navigate to the URL provided by the Kestrel server (e.g., `https://localhost:7147` or `http://localhost:5147`).

## Default Admin Credentials

A default administrator account is seeded into the database on the first run:

- **Email:** `admin@admin.se`
- **Password:** `Admin123!`

Use these credentials to log in and access administrative features.

## Project Structure

```
AlphaAdmin_Dashboard/
├── Business/              # Business logic, services, factories, models
├── Data/                  # Data access layer, EF Core context, entities, repositories
├── Domain/                # Core domain models, extensions
└── Presentation/          # ASP.NET MVC project, controllers, views, wwwroot
    ├── app.db             # SQLite database file (created on run)
    └── ...
├── .gitignore
├── AlphaAdmin_Dashboard.sln
├── global.json            # Specifies .NET SDK version
├── PROJECT_REQUIREMENTS_STATUS.md # Tracks assignment progress
└── README.md              # This file
```

## License

This project was created solely for educational purposes as part of an assignment for EC-Utbildning.

## Features

- **Project Management**: Create, edit, and track projects with detailed information
- **Team Collaboration**: Assign team members to projects and manage their roles
- **Client Management**: Maintain a database of clients and associate them with projects
- **User Authentication**: Secure login/registration with identity management
- **Responsive Design**: Works seamlessly across desktop and mobile devices
- **Dark/Light Mode**: Toggle between dark and light themes for comfortable viewing
- **Real-time Notifications**: Get notified about new projects and updates
- **Role-based Access Control**: Different permissions for administrators and regular users

## Screenshots

### Dashboard

![Dashboard](Presentation/wwwroot/images/admin/dashboard.png)

### Project Management

![Projects](Presentation/wwwroot/images/admin/projects.png)

### Dark Mode

![Dark Mode](Presentation/wwwroot/images/admin/dark-mode.png)

## Acknowledgements

- Design inspiration from [source of design files if applicable]
- Icons from FontAwesome
- Avatars created with [avatar generator tool if applicable]

---

_Note: This project was created as an assignment for the ASP.NET course at EC-Utbildning._
