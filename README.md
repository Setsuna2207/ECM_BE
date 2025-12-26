# ECM_BE

## 📌 Overview
This is a backend API built with **.NET 8**, using **Entity Framework Core** with **SQL Server** as the database.

Frontend repository: [https://github.com/Setsuna2207/ECM_FE]

---

## 🚀 Tech Stack
- [.NET 8](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [ASP.NET Core Identity](https://docs.microsoft.com/aspnet/core/security/authentication/identity)
- [JWT Authentication](https://jwt.io/)

---

## 📂 Project Structure

```
.
├── Controllers/               # API Controllers
├── Services/                  # Business Logic Services
├── Models/                    # Data Models & DTOs & Mappers
├── Data/                      # DbContext & Database Configuration
├── Configurations/            # Application Configuration
├── Exceptions/                # Custom Exception Handling
├── Extensions/                # Extension Methods
├── Hubs/                      # SignalR Hubs
├── Migrations/                # Migrations for SQL Server
├── Migrations.PostgreSQL/     # Migrations for PostgresSQL
└── Program.cs                 # Application Entry Point
uploads/                       # Local storage (keep to test, else: remove all items in this folder when clone)
```
---

## ⚙️ Configuration

### 1. Core Configuration — General settings such as logging, email, and JWT.
Create an `appsettings.json` file in the project root and configure as follows:

```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "EmailConfiguration": {
    "From": "your-email",
    "SmtpServer": "smtp.gmail.com",
    "Port": 587,
    "Username": "your-email",
    "Password": "your_password"
  },
  "BaseUrl": "https://localhost:7169",
  "AllowedHosts": "*",
  "ClientUrls": [
    "http://localhost:5173"
  ],
  "JWT": {
    "Issuer": "https://localhost:7169",
    "Audience": "https://localhost:7169",
    "SigningKey": "Your-512-bit-Key"
  }
}
```
### 2. Database connection configration
- Set `"DatabaseProvider"` to "SqlServer"` depending on the database provider you use.
- set connection string for database connection.

```
"DatabaseProvider": "SqlServer",
"ConnectionStrings": {
  "SqlServerConnection": "Server=your-server;Database=ECM;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True",
}
```

---

## 📦 Installation & Setup

### 1. Prerequisites
- Visual Studio 2022
- .NET 8 SDK
- SQL Server

### 2. Database Setup
1. Open Package Manager Console in Visual Studio
2. Run the following command:
```
update-database

```

### 3. Running the Application
1. Open the solution in Visual Studio 2022
2. Build the solution
3. Press F5 to run the application

The API will be available at:
- HTTPS: [https://localhost:7624](https://localhost:7624)
- Swagger UI: [https://localhost:7624/swagger](https://localhost:7624/swagger)

---

## 🔑 Authentication

* The backend uses **JWT** for authentication.
* On successful login, the server returns a **JWT token**.
* Include the token in request headers for protected routes:
```
Authorization: Bearer <token>
```

## 👥 Authorization Policies

- `AdminPolicy`: Requires "Admin" role
- `UserPolicy`: Requires "User" or "Admin" role

---

## 🌐 Client Integration

The frontend application is available at: [https://github.com/Setsuna2207/ECM_FE]

The frontend is expected to run at:
[http://localhost:5173](http://localhost:5173) (configured in `appsettings.json`).

---

## 📝 Notes

- Ensure proper CORS configuration for your production environment
- Secure your JWT signing key, database connection strings, and secret keys
- Configure email settings with valid SMTP credentials
- For production deployment, update the JWT issuer and audience URLs