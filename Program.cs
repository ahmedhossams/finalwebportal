using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Identity; // 1. Added for Identity
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

// 3. ADD IDENTITY CORE CONFIGURATION
builder.Services.AddIdentity<User, IdentityRole>()
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
        policy.WithOrigins("http://localhost:3000")
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

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
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

builder.Services.AddAuthorization();

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

    if (!db.Users.Any() && !db.Students.Any() && !db.Courses.Any() && !db.Classrooms.Any() && !db.Attendances.Any() && !db.Assignments.Any())
    {
        var instructorUser1 = new User
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "instructor1",
            NormalizedUserName = "INSTRUCTOR1",
            Email = "instructor1@example.com",
            NormalizedEmail = "INSTRUCTOR1@EXAMPLE.COM",
            EmailConfirmed = true,
            Name = "Dr. tamer bsiony    "
        };

        var instructorUser2 = new User
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "instructor2",
            NormalizedUserName = "INSTRUCTOR2",
            Email = "instructor2@example.com",
            NormalizedEmail = "INSTRUCTOR2@EXAMPLE.COM",
            EmailConfirmed = true,
            Name = "Dr. ahemed shihatta"
        };

        var instructorUser3 = new User
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "instructor3",
            NormalizedUserName = "INSTRUCTOR3",
            Email = "instructor3@example.com",
            NormalizedEmail = "INSTRUCTOR3@EXAMPLE.COM",
            EmailConfirmed = true,
            Name = "Prof. waled elshazly"
        };

        var instructorUser4 = new User
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "instructor4",
            NormalizedUserName = "INSTRUCTOR4",
            Email = "instructor4@example.com",
            NormalizedEmail = "INSTRUCTOR4@EXAMPLE.COM",
            EmailConfirmed = true,
            Name = "Dr. 3lewa"
        };

        var instructorUser5 = new User
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "instructor5",
            NormalizedUserName = "INSTRUCTOR5",
            Email = "instructor5@example.com",
            NormalizedEmail = "INSTRUCTOR5@EXAMPLE.COM",
            EmailConfirmed = true,
            Name = "Prof. karim hamoda"
        };

        var instructorUser6 = new User
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "instructor6",
            NormalizedUserName = "INSTRUCTOR6",
            Email = "instructor6@example.com",
            NormalizedEmail = "INSTRUCTOR6@EXAMPLE.COM",
            EmailConfirmed = true,
            Name = "Dr. shihatta"
        };

        var studentUser1 = new User
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "student1",
            NormalizedUserName = "STUDENT1",
            Email = "student1@example.com",
            NormalizedEmail = "STUDENT1@EXAMPLE.COM",
            EmailConfirmed = true,
            Name = "tamer ahmed"
        };

        var studentUser2 = new User
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "student2",
            NormalizedUserName = "STUDENT2",
            Email = "student2@example.com",
            NormalizedEmail = "STUDENT2@EXAMPLE.COM",
            EmailConfirmed = true,
            Name = "youssef ahmed  "
        };

        var studentUser3 = new User
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "student3",
            NormalizedUserName = "STUDENT3",
            Email = "student3@example.com",
            NormalizedEmail = "STUDENT3@EXAMPLE.COM",
            EmailConfirmed = true,
            Name = "ahmed hossam"
        };

        var studentUser4 = new User
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "student4",
            NormalizedUserName = "STUDENT4",
            Email = "student4@example.com",
            NormalizedEmail = "STUDENT4@EXAMPLE.COM",
            EmailConfirmed = true,
            Name = "noura Al-Farsi"
        };

        var studentUser5 = new User
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "student5",
            NormalizedUserName = "STUDENT5",
            Email = "student5@example.com",
            NormalizedEmail = "STUDENT5@EXAMPLE.COM",
            EmailConfirmed = true,
            Name = "karim osama"
        };

        var studentUser6 = new User
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "student6",
            NormalizedUserName = "STUDENT6",
            Email = "student6@example.com",
            NormalizedEmail = "STUDENT6@EXAMPLE.COM",
            EmailConfirmed = true,
            Name = "Leila Hassan"
        };

        db.Users.AddRange(
            instructorUser1,
            instructorUser2,
            instructorUser3,
            instructorUser4,
            instructorUser5,
            instructorUser6,
            studentUser1,
            studentUser2,
            studentUser3,
            studentUser4,
            studentUser5,
            studentUser6
        );
        db.SaveChanges();

        var instructors = new[]
        {
            new Instructor { UserId = instructorUser1.Id },
            new Instructor { UserId = instructorUser2.Id },
            new Instructor { UserId = instructorUser3.Id },
            new Instructor { UserId = instructorUser4.Id },
            new Instructor { UserId = instructorUser5.Id },
            new Instructor { UserId = instructorUser6.Id },
        };

        var students = new[]
        {
            new Student { UserId = studentUser1.Id },
            new Student { UserId = studentUser2.Id },
            new Student { UserId = studentUser3.Id },
            new Student { UserId = studentUser4.Id },
            new Student { UserId = studentUser5.Id },
            new Student { UserId = studentUser6.Id },
        };

        db.Instructors.AddRange(instructors);
        db.Students.AddRange(students);
        db.SaveChanges();

        var instructor = instructors[0];
        var student = students[0];

        var course = new Course
        {
            Name = "Introduction to Computer Science",
            InstructorId = instructor.Id
        };
     
        var classroom = new Classroom
        {
            Name = "Room 101",
            Location = "Main Building",
            Capacity = 30,
            InstructorId = instructor.Id
        };

        db.Courses.Add(course);
        db.Classrooms.Add(classroom);
        db.SaveChanges();

        var attendance = new Attendance
        {
            StudentId = student.Id,
            CourseId = course.Id,
            IsPresent = true,
            Date = DateTime.Today
        };

        var assignment = new Assignment
        {
            CourseId = course.Id,
            Title = "Week 1 Homework",
            Description = "Complete the introductory programming exercises.",
            DueDate = DateTime.Today.AddDays(7)
        };

        db.Attendances.Add(attendance);
        db.Assignments.Add(assignment);
        db.SaveChanges();
    }
}

app.MapControllers();

app.Run();