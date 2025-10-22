# 📋 ClaimCommander - Contract Monthly Claim System

\<div align-center"\>

[](https://dotnet.microsoft.com/)
[](https://docs.microsoft.com/en-us/dotnet/csharp/)
[](https://getbootstrap.com/)

**A functional web-based prototype of a Contract Monthly Claim System for academic institutions.**

\</div\>

## 🎯 Overview

ClaimCommander is a functional prototype of a Contract Monthly Claim System (CMCS) built with ASP.NET Core MVC. This application streamlines the claim submission and approval process for academic institutions, featuring distinct dashboards and workflows for Lecturers, Coordinators, and Managers. The system uses an in-memory data service to provide a fully interactive experience.

**Author:** Muhammed Saif Alexander  
**Student ID:** ST10275164  
**Module:** PROG6212 - Programming 2B

## ✨ Key Features

  - **Homepage Navigation**: A central landing page to direct users to the correct role-based view.
  - **Lecturer Dashboard**:
      - Simplified, **subject-based claim submission** with predefined hourly rates.
      - Intuitive **progress bar** to track claim approval status.
      - Dashboard summary of total hours, total amount, and pending claims.
  - **Manager & Coordinator Dashboards**: Review, approve, or reject claims within a multi-step approval workflow.
  - **Document Management**: Ability to attach supporting documents to claims.
  - **Role-Based Access**: Separate, tailored interfaces for Lecturers, Coordinators, and Managers.
  - **Responsive Design**: A modern, readable dark-themed UI that works on desktop and mobile.

## 🏗️ Architecture

Built using **ASP.NET Core MVC** with a clear separation of concerns:

  - **Models**: Defines the data entities (User, Claim, DocumentInfo) and ViewModels.
  - **Views**: Razor pages (`.cshtml`) for the user interface.
  - **Controllers**: Handle user input, business logic, and data flow.
  - **In-Memory Service**: A service (`InClaimStorageService`) acts as a temporary data store, simulating backend functionality with mock data.

## 📊 Database Design

The application is designed around core entities with the following relationships:

  - **Users** → **Claims** (1-to-many)
  - **Claims** → **Documents** (1-to-many)

## 🚀 Quick Start

### Prerequisites

  - .NET 7.0 SDK
  - Visual Studio 2022

### Installation

```bash
# Clone the repository
git clone https://github.com/msa1105/ClaimCommander.git

# Navigate to the project directory
cd ClaimCommander/ClaimCommander

# Restore dependencies
dotnet restore

# Run the application
dotnet run
```

Access the application at `https://localhost:xxxx` (port number may vary).

## 📁 Project Structure

```
ClaimCommander/
├── Controllers/      # Handles requests for Lecturer, Manager, Coordinator
├── Models/           # Contains data models and ViewModels
├── Views/            # Razor views for each role and action
├── wwwroot/          # Static assets (CSS, JavaScript, libraries)
└── Services/         # Contains the in-memory data storage service
```

## 🎨 UI Design

  - **Clean, modern interface** with a high-contrast dark theme for readability.
  - **Card-based layout** for displaying claims and summaries.
  - **Intuitive progress bars** for at-a-glance status tracking.
  - **Role-specific dashboards** tailored to the needs of each user type.
  - **Responsive design** ensures a seamless experience on both desktop and mobile devices.

## 🔧 Development Status

**Phase:** Part 1 - Functional Prototype  
**Status:** The application is a fully interactive prototype using an in-memory data service. All core UI and workflow features are implemented.  
**Next Phase:** Integration with a persistent database (e.g., SQL Server) and implementation of user authentication.

## 📋 Current Limitations

  - **No Data Persistence**: The application uses an in-memory service. All data, including new claims, will be reset when the application is restarted.
  - **No Authentication**: The application does not currently have a user login or authentication system.
  - **Mock Data**: The initial set of claims is hard-coded for demonstration purposes.

## 👨‍💻 Author

**Muhammed Saif Alexander** Student ID: ST10275164  
Module: PROG6212 - Programming 2B  
GitHub: [@msa1105](https://github.com/msa1105)

-----

*This is Part 1 of the Portfolio of Evidence for PROG6212*
