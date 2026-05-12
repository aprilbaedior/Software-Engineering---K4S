# Krumpin 4 Success — Youth Program Management Platform

<img width="1536" height="1024" alt="k4slogo_mini_nobackground" src="https://github.com/user-attachments/assets/3e92ad60-5009-4c6d-a5cb-b78458790988" />


A full-stack web application built for **Krumpin 4 Success, Inc.**, a nonprofit organization focused on decreasing risk-taking behaviors and youth recidivism by providing creative, academic, and career-focused programming for youth ages 13–24.

This platform digitalizes the organization's program management workflows — replacing manual processes with a centralized system for youth enrollment, service scheduling, attendance tracking, certification, and staff management across multiple user roles.

Built as a continuous software engineering project across four development sprints (CS395SI).

---

## My Contributions

This was a 3-person team project developed over a full semester. I served as **team leader** for the final submission and was the primary contributor across product management, UI/UX, and the entire Manager role implementation.

### Project Management & Leadership
- Served as team leader for the final sprint and overall product delivery
- Authored the project introduction and platform vision documentation
- Led sprint planning, backlog grooming, and task coordination across all milestones
- Produced all submission documents across Milestone 1, 2, 3, and the Final Report
- Created and maintained user role definitions, story cards, and student user requirements documentation

### UI/UX Design & Overhaul
- Redesigned the platform UI across multiple sprints — overhauling visual consistency, layout, and navigation
- Redesigned the K4S logo (multiple versions including light/dark variants)
- Defined UI patterns for role-based navigation ensuring each role only sees relevant links and tabs
- Co-developed user experience flows for student, manager, and instructor personas
- Fixed frontend UI bugs across identity pages and role-based routing

### Manager Role — Full Implementation
Owned and built the entire Manager branch of the application, including:
- **All Students / All Instructors** — list views with application detail access
- **Student Application Approval** — review, approve, and deny student applications with status updates
- **Instructor Application Approval** — review and approve instructor registration requests
- **Instructor Assignments** — assign instructors to specific service sections
- **Section Management** — create and manage multi-day sections per service with capacity limits
- **Certificate Workflow** — designed and implemented certificate issuance based on student attendance records and disciplinary eligibility; added student-facing certificate view and download
- **Strike System** — manager review and approval of instructor-filed strikes; enforcement of five-strike disciplinary policy

### Database Architecture
- Co-designed the full relational database schema across all entities: Profile, Students, Services, Sections, Schedule, Session, Attendance, Instructor, Manager, Certificate, Strike
- Identified and fixed critical database bugs — ensuring role assignments were persisted correctly in the DB (not just reflected on the frontend), and that correct tables were being queried per role

### Bug Fixes & Stability
- Fixed major role assignment bug: user roles were being set on the frontend only, not written to the database — identified root cause and resolved
- Fixed incorrect table references across multiple Manager pages
- Resolved multi-day section creation bug for services with complex schedules

---

## Platform Overview

The system provides a centralized space for:
- Youth participants (students) to create accounts, apply to the program, register for services, and track attendance and certifications
- Managers to oversee student enrollment, approve applications, manage services and instructors, and issue certificates
- Instructors to view assigned sections, record attendance, and file disciplinary strikes
- Admins to manage all user roles, view system analytics, and maintain platform integrity

---

## User Roles

| Role | Access Summary |
|---|---|
| **Admin** | Full system access — user/role management, attendance analytics, system reports, emergency contacts |
| **Manager** | Student & instructor approvals, section management, certificate issuance, strike review |
| **Instructor** | View assigned sections, record attendance, file strikes, view student emergency contacts |
| **Student** | Apply to program, register for services, view schedule, track attendance, download certificates |

---

## Tech Stack

| Layer | Technology |
|---|---|
| Web Framework | ASP.NET (Razor Pages) |
| Language | C# |
| Database | Azure SQL |
| ORM | Entity Framework Core |
| Authentication | ASP.NET Identity (role-based) |
| Version Control | Azure DevOps (migrated to GitHub) |
| Development | Visual Studio / VS Code |

---

## Database Schema

### Core Tables

**Profile** — `(PK) Email`, Name, Password, PhoneNum, Address, License fields, Demographics, Emergency contacts (×3), ApplicationStatus

**Students** — `(PK)(FK Profile.Email) Email`, Name, DateOfBirth

**Services** — `(PK) ServiceID`, ServiceName, ServiceFrequency, ServiceDescription, ServiceHours

**Sections** — `(PK) SectionID`, `(FK) ServiceID`, ServiceName, Schedule (days/times), StartDate, EndDate, Status

**Schedule** — `(PK) ScheduleID`, `(FK) StudentEmail`, `(FK) ServiceID`, `(FK) SectionID`, WeekDay, Start/EndTime, Status

**Session** — `(PK) SessionID`, `(FK) ServiceID`, `(FK) SectionID`, MeetingDate, Start/EndTime, Status

**Attendance** — `(PK) Attendance_ID`, `(FK) Email`, `(FK) ServiceID`, `(FK) SectionID`, `(FK) ScheduleID`, CurrentDate, AttendanceStatus

**Instructor** — `(PK)(FK Profile.Email) Email`, HireDate, Active

**Manager** — `(PK)(FK Profile.Email) Email`

**Certificate** — `(PK) CertificateID`, `(FK) StudentEmail`, `(FK) ManagerID`, `(FK) ServiceID`, IssueDate, CertificateStatus

**Strike** — `(PK) StrikeID`, `(FK) InstructorEmail`, `(FK) ManagerEmail`, Reason, DateIssued, Status

**SchedulingForm** — `(PK) RequestID`, `(FK) ServiceID`, `(FK) Email`, Schedule preferences, Status

---

## Key Features

**Student Journey**
- Account creation with email confirmation
- Student application (personal, employment, emergency contact info)
- Application status tracking
- Service browsing and section enrollment
- Attendance summaries and weekly schedule view
- Certificate viewing and download upon program completion

**Manager Workflows**
- Review and approve/deny student and instructor applications
- Assign instructors to service sections
- Create and manage services and multi-day sections with capacity limits
- Issue completion certificates based on attendance and disciplinary standing
- Review and approve/deny instructor-filed strikes

**Instructor Tools**
- View only assigned sections
- Record attendance for enrolled students
- File disciplinary strikes with rule violation, date, and notes
- View student emergency contact information

**Admin Tools**
- User and role management across all accounts
- Attendance analytics and system reporting
- Emergency contact oversight
- Password reset and account deletion

---

## Local Setup

### Prerequisites
- Visual Studio 2022 or VS Code with C# extension
- .NET 8 SDK
- Azure SQL database (or SQL Server locally)

### Steps

```bash
git clone https://github.com/aprilbaedior/Software-Engineering---K4S.git
cd Software-Engineering---K4S
```

Configure your connection string in `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "your-azure-sql-or-local-connection-string"
}
```

Run the application:
```bash
dotnet restore
dotnet run
```

### Test Credentials

| Role | Email | Password |
|---|---|---|
| Admin | admin@gmail.com | Admin@123 |
| Manager | manager@gmail.com | Password123! |
| Instructor | londonteacher@gmail.com | Password123! |
| Student (approved) | studenttest04@gmail.com | Password123! |
| General (unapproved) | generaluser@gmail.com | Password123! |

---

## Key Pages

| Route | Description |
|---|---|
| `/Registration/FacultyApplication` | Instructor/Manager application submission |
| `/Scheduling/Registration/Status` | Application status check |
| `/Admin/Users/Edit` | Admin role management |
| `/Managers/FacultyApplications` | Manager: approve instructor applications |
| `/Managers/Students/Applications` | Manager: approve student applications |
| `/Managers/InstructorAssignments` | Manager: assign instructors to sections |
| `/Managers/Certificates/Issue` | Manager: issue certificates |
| `/Students/MyCertificates` | Student: view and download certificates |
| `/AttendanceForAdmin/SelectSection` | Instructor: record attendance by section |
| `/Instructors/DisciplinaryHistory` | Instructor: file/view strikes per section |

---

## Security Notes

Known vulnerabilities identified via GitHub Copilot security scan (documented in Final Report):
- Hardcoded database credentials — recommend migrating to Azure Key Vault or environment variables
- Missing CSRF protection on select AJAX handlers
- Rate limiting not implemented on search endpoints
- Missing security headers (CSP, X-Frame-Options)

Fixes implemented during final sprint:
- Corrected cookie security settings (HttpOnly, Secure, SameSite)
- Added role-based authorization for certificate issuance
- Implemented manager-section permission validation before certificate issue
- Added input validation and duplicate prevention on key forms
