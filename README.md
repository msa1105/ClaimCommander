# 📋 ClaimCommander - Contract Monthly Claim System

<div align="center">

![ClaimCommander](https://img.shields.io/badge/ClaimCommander-CMCS%20Prototype-2563eb?style=for-the-badge&logo=clipboard-check)

[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-MVC-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-Programming-239120?style=flat-square&logo=c-sharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-7952B3?style=flat-square&logo=bootstrap)](https://getbootstrap.com/)

**A web-based Contract Monthly Claim System prototype for academic institutions**

</div>

## 🎯 Overview

ClaimCommander is a prototype Contract Monthly Claim System (CMCS) built with ASP.NET Core MVC. This application streamlines the claim submission and approval process for academic institutions with separate dashboards for lecturers and administrators.

**Author:** Muhammed Saif Alexander  
**Student ID:** ST10275164  
**Module:** PROG6212 - Programming 2B  

## ✨ Key Features

- **Lecturer Dashboard**: Submit claims, track status, view monthly summaries
- **Admin Dashboard**: Review claims, approve/deny submissions, manage users
- **Document Management**: Upload and manage supporting documents
- **Role-Based Access**: Separate interfaces for lecturers and administrators
- **Responsive Design**: Modern dark-themed UI with mobile support

## 🏗️ Architecture

Built using **ASP.NET Core MVC** with clear separation of concerns:

- **Models**: Data entities (User, Claim, Subject, Document)
- **Views**: Razor pages for user interface
- **Controllers**: Handle user input and business logic
- **Repository Pattern**: Data access abstraction layer

## 📊 Database Design

Core entities with relationships:
- **Users** → **Claims** (1:many)
- **Subjects** → **Claims** (1:many)  
- **Claims** → **Documents** (1:many)

## 🚀 Quick Start

### Prerequisites
- .NET 7.0 SDK
- Visual Studio 2022
- SQL Server LocalDB

### Installation
```bash
# Clone repository
git clone https://github.com/msa1105/ClaimCommander.git

# Navigate to directory
cd ClaimCommander

# Restore packages
dotnet restore

# Run application
dotnet run
```

Access at: `https://localhost:7001`

## 📁 Project Structure

```
ClaimCommander/
├── Controllers/          # MVC Controllers
├── Models/              # Data models and ViewModels
├── Views/               # Razor views (.cshtml)
├── wwwroot/             # Static files (CSS, JS, images)
├── Data/                # Database context and migrations
└── Services/            # Business logic services
```

## 🎨 UI Design

- **Clean, modern interface** with dark theme
- **Card-based layout** for easy navigation
- **Role-specific dashboards** tailored to user needs
- **Responsive design** works on desktop and mobile
- **Intuitive workflow** for claim submission and review

## 🔧 Development Status

**Phase:** Part 1 - Prototype Development  
**Status:** Front-end prototype with mock data  
**Next Phase:** Database integration and backend functionality

## 📋 Current Limitations

- Non-functional prototype (UI only)
- Mock data in controllers
- No persistent database connections
- Form submissions don't save data

## 👨‍💻 Author

**Muhammed Saif Alexander**  
Student ID: ST10275164  
Module: PROG6212 - Programming 2B  
GitHub: [@msa1105](https://github.com/msa1105)

---

*This is Part 1 of the Portfolio of Evidence for PROG6212*
