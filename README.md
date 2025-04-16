# AlphaAdmin Dashboard

![AlphaAdmin Dashboard](Presentation/wwwroot/images/admin/dashboard-preview.png)

## Project Overview

AlphaAdmin Dashboard is a modern project management web application built with ASP.NET Core MVC. It provides a comprehensive solution for managing projects, clients, and team members with a clean, responsive user interface that supports both light and dark themes.

## Features

- **Project Management**: Create, edit, and track projects with detailed information
- **Team Collaboration**: Assign team members to projects and manage their roles
- **Client Management**: Maintain a database of clients and associate them with projects
- **User Authentication**: Secure login/registration with identity management
- **Responsive Design**: Works seamlessly across desktop and mobile devices
- **Dark/Light Mode**: Toggle between dark and light themes for comfortable viewing
- **Real-time Notifications**: Get notified about new projects and updates
- **Role-based Access Control**: Different permissions for administrators and regular users

## Technologies Used

- **Backend**: ASP.NET Core MVC (.NET 9)
- **Database**: Entity Framework Core with SQL Server
- **Frontend**: HTML5, CSS3, JavaScript, Bootstrap
- **Authentication**: ASP.NET Core Identity with external login providers
- **Architecture**: Multi-layered architecture with:
  - Presentation Layer (MVC)
  - Business Layer (Services)
  - Data Layer (Repositories)
  - Domain Layer (Models)

## Prerequisites

- .NET 9 SDK
- SQL Server (or alternative database that works with EF Core)
- Visual Studio 2022 or JetBrains Rider

## Setup and Installation

1. **Clone the repository**

   ```
   git clone https://github.com/yourusername/AlphaAdmin_Dashboard.git
   cd AlphaAdmin_Dashboard
   ```

2. **Configure the database connection**

   - Update the connection string in `appsettings.json` file according to your local database setup

3. **Apply migrations to create the database**

   ```
   dotnet ef database update
   ```

4. **Run the application**

   ```
   dotnet run --project Presentation
   ```

5. **Access the application**
   - Open your browser and navigate to `https://localhost:5001` or `http://localhost:5000`

## Project Structure

```
AlphaAdmin_Dashboard/
├── Business/              # Business logic layer
│   ├── DataSeeders/       # Seed data for development
│   ├── Factories/         # Factory pattern implementations
│   ├── Interfaces/        # Service interfaces
│   ├── Models/            # Business models
│   └── Services/          # Service implementations
├── Data/                  # Data access layer
│   ├── Contexts/          # EF Core DbContext
│   ├── Entities/          # Database entities
│   ├── Interfaces/        # Repository interfaces
│   ├── Migrations/        # EF Core migrations
│   ├── Models/            # Data models
│   └── Repositories/      # Repository implementations
├── Domain/                # Domain layer
│   ├── Extensions/        # Extension methods
│   └── Models/            # Domain models
└── Presentation/          # Presentation layer (MVC)
    ├── Controllers/       # MVC controllers
    ├── ViewModels/        # View models
    ├── Views/             # Razor views
    └── wwwroot/           # Static assets
```

## Features Implemented

### Base Requirements (Godkänt/Passed)

- ✅ All required pages from the design file
- ✅ Project listing with filtering by status
- ✅ Project creation and editing functionality
- ✅ Form validation using JavaScript
- ✅ Entity Framework Core with appropriate design patterns
- ✅ Authentication and authorization with ASP.NET Identity

### Advanced Requirements (Väl Godkänt/Well-Passed)

- ✅ All pages required for the advanced level
- ✅ Proper use of Views, Partial Views, and ViewModels
- ✅ Complete implementation based on the design file
- ✅ Admin interface for managing system data
- ✅ Form validation with both JavaScript and ModelState
- ✅ Cookie Consent implementation
- ✅ Separate data and business layers
- ✅ Role-based access control (Admin vs Standard users)
- ✅ Third-party authentication providers
- ✅ Dark/Light theme toggle
- ✅ Notification system for new projects and users
- ✅ Project image upload and team member assignment

## Screenshots

### Dashboard

![Dashboard](Presentation/wwwroot/images/admin/dashboard.png)

### Project Management

![Projects](Presentation/wwwroot/images/admin/projects.png)

### Dark Mode

![Dark Mode](Presentation/wwwroot/images/admin/dark-mode.png)

## License

This project is part of an educational assignment for EC-Utbildning.

## Acknowledgements

- Design inspiration from [source of design files if applicable]
- Icons from FontAwesome
- Avatars created with [avatar generator tool if applicable]

---

_Note: This project was created as an assignment for the ASP.NET course at EC-Utbildning._
