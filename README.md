# 🎬 CineVerse

A movie review platform built with **ASP.NET Core MVC** and **Clean Architecture**.
Visitors can browse movies, filter them by genre or tag, see what's in theaters, and read reviews.
Registered users can rate and review movies, and admins/editors manage the whole catalog from an admin panel.

> Built as a learning project to practice layered architecture, the Repository & Unit of Work patterns,
> EF Core and ASP.NET Core Identity.

![Demo posters](docs/demo-posters.jpg)

---

## Features

### For visitors
- **Home page** with a carousel of the most talked-about movies, "In Theaters" and "Latest Movies" sections
- **Movie list**, **In Theaters** page and **genre / tag** filter pages
- **Movie details** page with poster, overview, tags, average rating and reviews
- Clean, SEO-friendly URLs using slugs (`/movie/inception`)

### For users
- Register, log in (with "remember me" and lockout after failed attempts), log out
- Rate movies (1–5 stars) and write reviews
- Redirected back to the page they were on after logging in

### Admin panel (`/administrator`)
- **Movies:** create, edit, delete, upload posters (JPG/PNG/WEBP, max 2 MB), mark as "In Theaters", attach tags
- **Genres** and **Tags:** full CRUD with unique-name validation
- **Reviews:** moderation (delete inappropriate reviews)
- **Role-based access:** `Admin` can do everything, `Editor` can add content and moderate reviews but cannot edit or delete catalog items

---

## Tech Stack

| Area | Technology |
|---|---|
| Framework | .NET 10, ASP.NET Core MVC |
| Data access | Entity Framework Core 10 (SQL Server / LocalDB) |
| Authentication | ASP.NET Core Identity (cookie based) |
| Validation | FluentValidation 12 |
| UI | Razor Views, Bootstrap 5.3, Bootstrap Icons, custom dark theme |

---

## Architecture

The solution follows Clean Architecture. Dependencies only point inwards:

```
CineVerse.MVC  ──►  CineVerse.Application  ──►  CineVerse.Domain
      │                       ▲
      └──►  CineVerse.Infrastructure ─┘
```

| Project | Responsibility |
|---|---|
| **CineVerse.Domain** | Entities (`Movie`, `Genre`, `Tag`, `MovieTag`, `Review`, `AppUser`, `AppRole`) and the shared `BaseEntity` |
| **CineVerse.Application** | Service interfaces and implementations, DTOs (request/response records), FluentValidation validators, repository & Unit of Work contracts, `Result` pattern |
| **CineVerse.Infrastructure** | `AppDbContext`, entity configurations, generic `Repository<T>` and concrete repositories, `UnitOfWork`, migrations |
| **CineVerse.MVC** | Controllers, views, view components, admin area, Identity setup, seeders |

### Design decisions worth mentioning
- **Soft delete** – records are never physically removed (`IsActive = false`, `DeletedOn`), and a global query filter hides them.
- **Filtered unique indexes** (`[IsActive] = 1`) – a deleted genre/tag/movie name can be reused without violating the unique constraint.
- **Read vs. write loading** – detail queries use `Include` for display, while updates load the bare entity so navigation properties can't silently override foreign key changes.
- **Result pattern** – services return `Result` / `Result<T>` instead of throwing exceptions for expected failures.
- **Security** – anti-forgery tokens on every POST, deletes are POST-only, the reviewer's identity comes from the auth cookie claims (never from the form), local-only redirects after login, and HTTPS redirection.
- **View Components** – the genre menu and sidebar widgets load their own data, so every page can show them without extra controller code.

---

## Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server LocalDB (installed with Visual Studio's "ASP.NET and web development" workload)
- Visual Studio 2026 (or any IDE that supports .NET 10)

### Run the project
1. Clone the repository
   ```bash
   git clone https://github.com/batueren0/CineVerse.git
   ```
2. Open `CineVerse.slnx` in Visual Studio.
3. Create the database. In **Package Manager Console**, select **CineVerse.Infrastructure** as the *Default project* and run:
   ```powershell
   Update-Database
   ```
   <details>
   <summary>Using the .NET CLI instead</summary>

   ```bash
   dotnet tool install --global dotnet-ef
   dotnet ef database update --project CineVerse.Infrastructure --startup-project CineVerse.MVC
   ```
   </details>
4. Set **CineVerse.MVC** as the startup project and press **F5**.
   The app opens at `https://localhost:7122`.

On the first run, the app automatically creates the roles, an admin account and (in the Development environment) demo data: 20 movies with posters, genres, tags, 6 users and 60+ reviews.

### Demo accounts

| Role | Email | Password |
|---|---|---|
| Admin | `system.admin@cineverse.com` | `Admin@123` |
| User | `pete@example.com` | `Demo@123` |

> The connection string lives in `CineVerse.MVC/appsettings.json` and points to `(localdb)\MSSQLLocalDB`.

---

## Project Structure

```
CineVerse
├── CineVerse.Domain
│   ├── Common/BaseEntity.cs
│   └── Entities/
├── CineVerse.Application
│   ├── Common/          → IRepository, IUnitOfWork, Result, Roles, StringExtensions
│   ├── Contracts/       → repository & service interfaces
│   ├── DTOs/            → request & response records
│   ├── Features/        → service implementations
│   └── Validators/      → FluentValidation rules
├── CineVerse.Infrastructure
│   ├── Migrations/
│   └── Persistence/     → AppDbContext, configurations, repositories, UnitOfWork
└── CineVerse.MVC
    ├── Areas/Administrator/   → admin panel (controllers + views)
    ├── Controllers/           → public site
    ├── ViewComponents/        → genre menu, tag cloud
    ├── Views/
    ├── wwwroot/
    ├── DbSeeder.cs            → roles + admin account
    └── DemoDataSeeder.cs      → demo movies, users and reviews
```

---

## Notes

- The movie posters in `wwwroot/images/posters` are **original placeholder artwork created for this project**, not the official posters.
- Posters uploaded from the admin panel are saved to `wwwroot/uploads/posters`, which is excluded from Git.
- The demo accounts and passwords exist only for local development.
