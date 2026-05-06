using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity; // 1. Added for Identity
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Linq;
using SmartAttendance.Data;
using SmartAttendance.Models;        // 2. Added to access the User model
using SmartAttendance.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseInMemoryDatabase("SmartAttendanceDB"));

// Add Identity with password configuration
builder.Services.AddIdentity<User, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// 4. Removed AuthService! 
builder.Services.AddScoped<StudentService>();
builder.Services.AddScoped<CourseService>();
builder.Services.AddScoped<AttendanceService>();
builder.Services.AddScoped<ClassroomService>();
builder.Services.AddScoped<AssignmentService>();
builder.Services.AddScoped<InstructorService>();

// Add CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://127.0.0.1:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// 5. SECURE JWT CONFIGURATION (Reading from appsettings.json)
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings.GetValue<string>("SecurityKey") ?? throw new InvalidOperationException("JWT SecurityKey is not configured.");
var issuer = jwtSettings.GetValue<string>("Issuer") ?? throw new InvalidOperationException("JWT Issuer is not configured.");
var audience = jwtSettings.GetValue<string>("Audience") ?? throw new InvalidOperationException("JWT Audience is not configured.");

// Add cookie-based authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "JWT_OR_COOKIE";
    options.DefaultChallengeScheme = "JWT_OR_COOKIE";
})
.AddPolicyScheme("JWT_OR_COOKIE", "JWT_OR_COOKIE", options =>
{
    options.ForwardDefaultSelector = context =>
    {
        string authorization = context.Request.Headers.Authorization.ToString();
        if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
            return JwtBearerDefaults.AuthenticationScheme;
        return CookieAuthenticationDefaults.AuthenticationScheme;
    };
})
.AddCookie(options =>
{
    options.Cookie.Name = "SmartAttendanceAuth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.None; // Set to Secure in production with HTTPS
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = 403;
        return Task.CompletedTask;
    };
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Enable CORS
app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

// Add root endpoint
app.MapGet("/", () => Results.Redirect("/swagger/index.html"));

// Seed sample data for the frontend dashboard
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

    var seeder = async () =>
    {
        // Create roles
        var roles = new[] { "Student", "Instructor" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        var instructorUsers = new[]
        {
            new User { UserName = "instructor1", Email = "instructor1@example.com", Name = "Dr. tamer bsiony", EmailConfirmed = true },
            new User { UserName = "instructor2", Email = "instructor2@example.com", Name = "Dr. ahemed shihatta", EmailConfirmed = true },
            new User { UserName = "instructor3", Email = "instructor3@example.com", Name = "Prof. waled elshazly", EmailConfirmed = true },
            new User { UserName = "instructor4", Email = "instructor4@example.com", Name = "Dr. 3lewa", EmailConfirmed = true },
            new User { UserName = "instructor5", Email = "instructor5@example.com", Name = "Prof. karim hamoda", EmailConfirmed = true },
            new User { UserName = "instructor6", Email = "instructor6@example.com", Name = "Dr. shihatta", EmailConfirmed = true },
        };

        var createdInstructors = new List<Instructor>();
        foreach (var user in instructorUsers)
        {
            var result = await userManager.CreateAsync(user, "Password123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Instructor");
                createdInstructors.Add(new Instructor { UserId = user.Id });
            }
        }

        var studentUsers = new[]
        {
            new User { UserName = "student1", Email = "student1@example.com", Name = "tamer ahmed", EmailConfirmed = true },
            new User { UserName = "student2", Email = "student2@example.com", Name = "youssef ahmed", EmailConfirmed = true },
            new User { UserName = "student3", Email = "student3@example.com", Name = "ahmed hossam", EmailConfirmed = true },
            new User { UserName = "student4", Email = "student4@example.com", Name = "noura Al-Farsi", EmailConfirmed = true },
            new User { UserName = "student5", Email = "student5@example.com", Name = "karim osama", EmailConfirmed = true },
            new User { UserName = "student6", Email = "student6@example.com", Name = "Leila Hassan", EmailConfirmed = true },
        };

        var createdStudents = new List<Student>();
        foreach (var user in studentUsers)
        {
            var result = await userManager.CreateAsync(user, "Password123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Student");
                createdStudents.Add(new Student { UserId = user.Id });
            }
        }

        db.Instructors.AddRange(createdInstructors);
        db.Students.AddRange(createdStudents);
        db.SaveChanges();

        var instructor1 = createdInstructors[0];
        var instructor2 = createdInstructors[1];
        var instructor3 = createdInstructors[2];
        var student = createdStudents.First();

        var courses = new List<Course>
        {
            new Course { Code = "CS101", Name = "Introduction to Computer Science", InstructorId = instructor1.Id },
            new Course { Code = "CS201", Name = "Data Structures and Algorithms", InstructorId = instructor1.Id },
            new Course { Code = "DB301", Name = "Database Management Systems", InstructorId = instructor2.Id },
            new Course { Code = "WEB401", Name = "Web Development", InstructorId = instructor2.Id },
            new Course { Code = "ML501", Name = "Machine Learning Fundamentals", InstructorId = instructor3.Id },
            new Course { Code = "NET601", Name = "Computer Networks", InstructorId = instructor3.Id },
        };

        db.Courses.AddRange(courses);
        db.SaveChanges();

        var classroom = new Classroom
        {
            Name = "Room 101",
            Location = "Main Building",
            Capacity = 30,
            InstructorId = instructor1.Id
        };

        db.Classrooms.Add(classroom);
        db.SaveChanges();

        var assignments = new List<Assignment>
        {
            new Assignment
            {
                CourseId = courses[0].Id,
                Title = "Week 1 Homework",
                Description = "Complete the introductory programming exercises.",
                DueDate = DateTime.Today.AddDays(7)
            },
            new Assignment
            {
                CourseId = courses[1].Id,
                Title = "Binary Search Tree Implementation",
                Description = "Implement a BST with insert, delete, and search operations.",
                DueDate = DateTime.Today.AddDays(14)
            },
            new Assignment
            {
                CourseId = courses[2].Id,
                Title = "SQL Query Assignment",
                Description = "Write complex SQL queries including joins, subqueries, and aggregations.",
                DueDate = DateTime.Today.AddDays(10)
            }
        };

        var attendances = new List<Attendance>();
        var random = new Random();
        foreach (var s in createdStudents)
        {
            foreach (var c in courses)
            {
                var isPresent = random.Next(0, 10) < 7;
                var daysAgo = random.Next(0, 30);
                attendances.Add(new Attendance
                {
                    StudentId = s.Id,
                    CourseId = c.Id,
                    IsPresent = isPresent,
                    Date = DateTime.Today.AddDays(-daysAgo)
                });
            }
        }

        db.Attendances.AddRange(attendances);
        db.Assignments.AddRange(assignments);
        db.SaveChanges();
    };
    
    if (!db.Users.Any() && !db.Students.Any() && !db.Courses.Any() && !db.Classrooms.Any() && !db.Attendances.Any() && !db.Assignments.Any())
    {
        seeder().Wait();
    }
}

app.MapControllers();

app.Run();