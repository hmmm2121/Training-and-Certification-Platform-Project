# Training & Certification Platform

This solution implements a web-based training and certification system with three projects:

- **TrainingAndCertificationAPI** – ASP.NET Core Web API with Entity Framework Core and JWT authentication.
- **TrainingAndCertificationPlatform** – ASP.NET Core MVC web app for trainees, instructors, and the training coordinator.
- **TrainingAndCertificationReports** – ASP.NET Core MVC reporting application for the Training Coordinator only.

The project is based on the “Training & Certification Platform” brief from IT8118: Advanced Programming.

---

## Features

### API (TrainingAndCertificationAPI)

- JWT-based authentication via `/api/auth/login`.
- Public certification verification endpoint:
  - `GET /api/public/certifications/verify`
- Reporting endpoints (Training Coordinator role only):
  - `GET /api/reports/enrollment-stats`
  - `GET /api/reports/instructor-workload`
  - `GET /api/reports/certification-rates`
  - `GET /api/reports/revenue`
  - `GET /api/reports/course-popularity`
  - `GET /api/reports/room-utilization`
  - `GET /api/reports/trainee-payments`

### Main MVC App (TrainingAndCertificationPlatform)

- Login and role-based access using ASP.NET Identity.
- Course catalog, sessions, enrollments, and certification tracking.
- Public certification lookup page consuming the API.
- Uses EF Core for database access with the shared `TrainAndCertContext`.

### Reporting App (TrainingAndCertificationReports)

- Separate MVC application that **only** talks to the API via `HttpClient`.
- Logs in to the API using a Training Coordinator account and stores the JWT in session.
- Read-only dashboard with:
  - Enrollment statistics by course and subject
  - Instructor workload
  - Certification completion rates
  - Revenue summary (collected vs outstanding, overdue count)
  - Course popularity
  - Room utilization
  - Trainee payment overview
- Dark/light theme toggle with a simple admin-style UI.

---

## Database

The system uses SQL Server (LocalDB in development).

To create and seed the database:

1. Run `Create-Table-sql.sql` to create the `TrainingAndCertificationPlatform` database and tables.
2. Run `Seed-data.sql` to insert sample users, courses, sessions, enrollments, payments, and notifications.

Make sure the connection string in `appsettings.json` points to the same instance you used for the scripts (e.g. `(localdb)\MSSQLLocalDB`).

---

## Running the solution

1. Open `TrainingAndCertificationPlatform.sln` in Visual Studio.
2. Restore NuGet packages and build the solution.
3. Set **multiple startup projects**:
   - `TrainingAndCertificationAPI` – Start
   - `TrainingAndCertificationPlatform` – Start
   - `TrainingAndCertificationReports` – Start (optional, for reports)
4. Press F5 or Ctrl+F5.

The default setup runs:

- API on `https://localhost:7159`
- Main MVC app on another localhost port
- Reporting app on another localhost port

Update the `ApiSettings:BaseUrl` in the MVC and Reporting projects if ports change.

---

## Reporting App Login

The reporting app expects a Training Coordinator account that exists in ASP.NET Identity (not just in the `Users` table).

The default seeded Training Coordinator is:

- **Email:** `fatima.nasser@gmail.com`
- **Password:** `Admin123`

Log in with this account to access the reporting dashboard.

---

## Notes

- The reporting application has no direct access to the database and does not reference the API’s DbContext.
- All reporting data is retrieved through the Web API using authenticated HTTP requests.

---
# Author
-Made by Khalid Alateya 202300458
- The reporting app is read-only and intended for internal use by the Training Coordinator.
