# ECM_BE - English Course Management Backend

Backend API built with **.NET 8** and **Entity Framework Core** for managing English courses, lessons, quizzes, and user progress.

Frontend: [ECM_FE](https://github.com/Setsuna2207/ECM_FE)

---

## 🚀 Tech Stack
- **.NET 8** + **Entity Framework Core**
- **ASP.NET Core Identity** + **JWT Authentication**
- **SQL Server**
- **OpenAI API** - AI recommendations
- **iTextSharp** - PDF processing

---

## �  Project Structure

```
ECM_BE/
├── Controllers/          # API endpoints
├── Services/             # Business logic
│   └── Interfaces/
├── Models/
│   ├── Entities/         # Database models
│   ├── DTOs/             # Data transfer objects
│   └── Mapper/
├── Data/                 # DbContext
├── Configuration/
├── Exceptions/
├── Migrations/           # EF migrations
├── uploads/              # File storage
│   ├── videos/
│   ├── documents/
│   └── images/
└── appsettings.json
```

---

## 📦 Setup

### Prerequisites
- **.NET 8 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **SQL Server**

### Installation
```bash
git clone https://github.com/Setsuna2207/ECM_BE.git
cd ECM_BE
dotnet restore
```

### Configuration
Update `appsettings.json`:
```json
{
  "EmailConfiguration": {
    "From": "your-email@gmail.com",
    "SmtpServer": "smtp.gmail.com",
    "Port": 587,
    "Username": "your-email@gmail.com",
    "Password": "your-gmail-app-password"
  },
  "BaseUrl": "https://localhost:7264",
  "ClientUrls": ["http://localhost:5173"],
  "JWT": {
    "SigningKey": "Your-64-Character-Secret-Key-Here"
  },
  "ConnectionStrings": {
    "SqlServerConnection": "Server=localhost;Database=ECM;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "OpenAI": {
    "ApiKey": "sk-your-key"
  }
}
```

### Run
```bash
dotnet ef database update
dotnet build
dotnet run
```

API: https://localhost:7264 | Swagger: https://localhost:7264/swagger

---

## 📡 Key API Endpoints

**Auth**: `POST /api/User/register`, `POST /api/User/login`

**Courses**: `GET/POST/PUT/DELETE /api/Course`

**Lessons**: `GET/POST/PUT/DELETE /api/Lesson`

**Quizzes**: `GET/POST/PUT/DELETE /api/Quiz`

**Tests**: `GET/POST /api/PlacementTest`

**File Upload**: `POST /api/FileUpload/upload?type={video|document|image}`
- Max sizes: Video (5GB), Document (100MB), Image (10MB)

**AI**: `GET /api/AITestRcm/recommend-test`, `GET /api/AICourseRcm/recommend-course`

Full docs: https://localhost:7264/swagger

---

## 🔑 Authentication

JWT token required for protected endpoints:
```http
Authorization: Bearer <token>
```

**Roles**: Admin (full access), User (limited access)

---

## 🔧 Development

**Migrations**:
```bash
dotnet ef migrations add MigrationName
dotnet ef database update
```

**File Storage**:
- `/uploads/videos/` - Video files
- `/uploads/documents/` - Documents
- `/uploads/images/` - Images

---

## 🐛 Troubleshooting

- **DB Connection**: Check SQL Server running + connection string
- **JWT Invalid**: Ensure SigningKey is 64+ characters
- **CORS**: Add frontend URL to `ClientUrls`
- **File Upload**: Check size limits + file permissions
- **Email**: Use Gmail App Password

---

## 🔗 Links
- Frontend: [ECM_FE](https://github.com/Setsuna2207/ECM_FE)
- [.NET Docs](https://docs.microsoft.com/dotnet/) | [EF Core](https://docs.microsoft.com/ef/core/)
