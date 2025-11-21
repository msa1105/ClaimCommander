# 🌙 **ClaimCommander**

### *A Modern, Secure, Role-Driven Lecturer Claim Management System*

<p align="center">
  <img src="https://img.shields.io/badge/Platform-.NET%208-blue" />
  <img src="https://img.shields.io/badge/Framework-ASP.NET%20Core%20MVC-purple" />
  <img src="https://img.shields.io/badge/UI-Bootstrap%205-teal" />
  <img src="https://img.shields.io/badge/Security-AES%20Encryption-red" />
</p>

ClaimCommander is a fully-featured lecturer claim management system built with **clean architecture**, **secure document handling**, and a **polished dark-mode UI**. The system supports the full lifecycle of hourly claims—submission, approval workflow, HR processing, and reporting.

---

## 🚀 **Key Highlights**

* **Layered Architecture with DI**
* **Session-based Authentication & RBAC**
* **AES Encryption for Uploaded Files**
* **Modern Dark Mode UI with Vibrant Accents**
* **Role-Specific Dashboards & Workflows**
* **HR Reporting + Invoicing System**
* **CSV Export, File Uploads, Auto-Rates & Auto-Calculations**

---

# 🏗️ **System Architecture**

### **Layered, Scalable, and Clean**

✔ Controllers → Services → Models
✔ Dependency Injection across all core components
✔ No database required (In-Memory storage for demo mode)

**In-Memory Storage Services:**

* `InMemoryClaimStorageService`
* `InMemoryLecturerService`
* `InMemoryUserService`

---

# 🔐 **Authentication & Security**

### **Role-Based Access Control (RBAC)**

Four distinct system roles:

| Role                      | Responsibilities                                   |
| ------------------------- | -------------------------------------------------- |
| **Lecturer**              | Submit claims, upload files, track statuses        |
| **Programme Coordinator** | Review and approve/reject claims                   |
| **Academic Manager**      | High-level approval and audit trail                |
| **HR / Finance**          | Manage lecturer details, rates, payments & reports |

### **Security Features**

* **AES Encryption** on all uploaded documents
* **Session-based authentication**
* **Authorization checks** on every controller action
* **Server-side validation** for form inputs
* **CSRF protection** with anti-forgery tokens
* **Friendly error handling** for all states

---

# 🎨 **User Interface**

### **Dark Theme + Vibrant Accents**

A custom, high-contrast dark UI enhanced with:

* Electric Blue
* Neon Green
* Gold
* Coral Red

### **Visual Enhancements**

* Color-coded claim statuses
* Responsive across all screen sizes (Bootstrap 5)
* Modern cards, badges, tables, and dashboards

---

# 👨‍🏫 **Lecturer Features**

### ✓ *Simple, Automated Claim Submission*

* Official hourly rates are **locked & system-controlled**
* Total claim value auto-calculated (`Hours × Rate`)
* Upload formats: **PDF, DOCX, XLSX**
* File size: **≤ 5MB**, validated server-side
* Full claim history with real-time statuses

---

# 🧑‍🏫 **Coordinator & Manager Features**

* Review all pending claims
* Approve or reject with notes
* View attached documents
* Transparent workflow showing all claim details

---

# 🧾 **HR & Finance Features**

### **Lecturer Management**

* View all lecturers
* Edit contact details
* Set/update official hourly rates

### **Reporting & Invoicing**

* Generate filtered payment reports
* Export data as **CSV**
* Produce HTML invoices for any claim
* Mark claims as **Paid** (final workflow state)

---

# 📊 **Dashboards & Workflow**

Each role sees a custom dashboard with:

* Pending actions
* Financial summaries
* Quick links
* Status indicators and logs

---

# 🛡️ **Validation & Error Handling**

* Required-field checks
* Numeric range validation
* File validation rules
* Consistent success (green) / error (red) alerts
* Graceful fallback pages for edge cases

---

# 📦 **Project Structure**

```
ClaimCommander/
│
├── Controllers/
├── Services/
├── Models/
├── Views/
│   ├── Lecturer/
│   ├── Coordinator/
│   ├── Manager/
│   └── HR/
│
├── wwwroot/
│   ├── css/
│   └── uploads/
│
└── Program.cs (DI + Routing + Auth)
```

---

# 🗺️ **Roadmap**

* ✔ Add database migration option (SQL/SQLite)
* ✔ Add JWT authentication mode
* ⬜ Add email notifications
* ⬜ Add bulk approval and payment runs
* ⬜ Add Power BI export

---

# 🤝 **Contributing**

Pull requests are welcome—especially UI refinements, performance improvements, and feature extensions.

---

# 📜 **License**

MIT License.
