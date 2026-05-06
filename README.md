SmartAttendance: Project Documentation
1. Overview
SmartAttendance is a comprehensive attendance management system featuring a modern web-based frontend and a robust ASP.NET Core backend API. Designed for educational institutions, it provides an intuitive solution for managing student attendance, courses, assignments, and classroom operations with role-based access control.
2. Tech Stack
Category
Technologies
 
Frontend
React 18, Vite, React Router, Context API, Axios
Backend
ASP.NET Core 6+, C#, Entity Framework Core
Security
JWT Authentication, Role-Based Access Control (RBAC)
Database
SQL Server / SQLite

3. Project Structure
SmartAttendance/
├── frontend/                 # React frontend application
│   ├── src/
│   │   ├── components/       # Reusable UI components
│   │   ├── pages/            # View components (Dashboard, Login, etc.)
│   │   ├── context/          # React Context (Auth & State)
│   │   └── styles/           # Global CSS and Variables
└── SmartAttendanceback/      # ASP.NET Core backend
    ├── Controllers/          # API Endpoints
    ├── Models/               # Data Entities
    ├── Data/                 # DB Context & Migrations
    └── Program.cs            # App Entry & Middleware


4. Key Features
Secure Authentication: JWT token-based login with persistent sessions.
Real-time Dashboard: Statistical overviews of attendance and course activity.
Attendance Tracking: Automated and manual logging for students and instructors.
Course Management: Create, update, and manage course enrollment and materials.
Responsive UI: Optimized for desktop, tablet, and mobile viewing.
5. API Endpoints
Endpoint
Method
Description
 
/api/auth/login
POST
User authentication
/api/attendance
GET/POST
Manage attendance records
/api/courses
GET/POST
Manage course data

6. Installation & Setup
Backend Setup
cd SmartAttendanceback
dotnet restore
dotnet ef database update
dotnet run


Frontend Setup
cd frontend
npm install
npm run dev
