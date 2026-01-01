# ECM_BE - English Course Management Backend

Backend API built with **.NET 8** and **Entity Framework Core**.

Frontend: [https://github.com/Setsuna2207/ECM_FE]

---

## 🚀 Tech Stack
- .NET 8
- Entity Framework Core
- ASP.NET Core Identity
- JWT Authentication
- SQL Server

---

## 📂 Project Structure

```
.
├── Controllers/               # API Controllers
├── Services/                  # Business Logic Services
│   └── Interfaces/            # Service Interfaces
├── Models/                    # Data Models, DTOs & Mappers
│   ├── Entities/              # Database Entities
│   ├── DTOs/                  # Data Transfer Objects
│   └── Mapper/                # Entity-DTO Mappers
├── Data/                      # DbContext & Database Configuration
├── Configuration/             # Application Configuration
├── Exceptions/                # Custom Exception Handling
├── Extensions/                # Extension Methods
├── Migrations/                # EF Core Migrations
├── uploads/                   # File Upload Storage (Local)
├── wwwroot/                   # Static Files (Local)
├── appsettings.json           # Application Settings
└── Program.cs                 # Application Entry Point
```

---

## ⚙️ Configuration

Create `appsettings.json`:

```json
{
  "EmailConfiguration": {
    "From": "your-email@gmail.com",
    "SmtpServer": "smtp.gmail.com",
    "Port": 587,
    "Username": "your-email@gmail.com",
    "Password": "your-app-password"
  },
  "BaseUrl": "https://localhost:7264",
  "ClientUrls": ["http://localhost:5173"],
  "JWT": {
    "Issuer": "https://localhost:7264",
    "Audience": "https://localhost:7264",
    "SigningKey": "Your-512-bit-Secret-Key-Here-Must-Be-At-Least-64-Characters-Long"
  },
  "DatabaseProvider": "SqlServer",
  "ConnectionStrings": {
    "SqlServerConnection": "Server=localhost;Database=ECM;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
    "OpenAI": {
    "ApiKey": "YOUR_API_KEY"
  }
}
```

**Note**: Use Gmail [App Password](https://support.google.com/accounts/answer/185833) for SMTP

---

## 📦 Setup

### Prerequisites
- Visual Studio 2022 or VS Code
- .NET 8 SDK
- SQL Server

### Installation
```bash
git clone https://github.com/Setsuna2207/ECM_BE.git
cd ECM_BE
```

### Database
```bash
# Package Manager Console
Update-Database

# .NET CLI
dotnet ef database update
```

### Run
```bash
dotnet restore
dotnet build
dotnet run
```

Available at:
- **HTTPS**: [https://localhost:7264](https://localhost:7264)
- **Swagger**: [https://localhost:7264/swagger](https://localhost:7264/swagger)

---

## 🔑 Authentication

Uses **JWT tokens**. Include in request headers:
```http
Authorization: Bearer <token>
```

**Policies**:
- `AdminPolicy`                 - Admin role
- `UserPolicy`                  - User or Admin role

---

## 📡 API Endpoints

- `/api/User`                   - Authentication & user management
- `/api/Course`                 - Courses
- `/api/Lesson`                 - Lessons
- `/api/Quiz`                   - Quizzes
- `/api/PlacementTest`          - Placement tests
- `/api/Review`                 - Reviews
- `/api/TestResult`             - Test results
- `/api/QuizResult`             - Quiz results
- `/api/History`                - Learning history
- `/api/Following`              - Course following

Full docs: [https://localhost:7264/swagger](https://localhost:7264/swagger)

---

## 🗄️ Database

Main entities: Users, Courses, Lessons, Quizzes, PlacementTests, Reviews, TestResults, QuizResults, History, Following

---

## 🔧 Development

```bash
# Add migration
Add-Migration MigrationName

# Apply migration
Update-Database
```

## 🐛 Troubleshooting

- **Database Connection Failed**:       Check SQL Server and connection string
- **JWT Token Invalid**:                Verify SigningKey is 512-bit (64+ characters)
- **CORS Errors**:                      Add frontend URL to `ClientUrls` and restart

---

## 🔗 Links
- Frontend: [https://github.com/Setsuna2207/ECM_FE]
- .NET: [https://docs.microsoft.com/dotnet/]
- EF Core: [https://docs.microsoft.com/ef/core/]
