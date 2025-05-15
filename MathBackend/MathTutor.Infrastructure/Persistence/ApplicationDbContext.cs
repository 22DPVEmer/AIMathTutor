using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MathTutor.Core.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using MathTutor.Core.Enums;

namespace MathTutor.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<SchoolClass> SchoolClasses { get; set; } = null!;
    public DbSet<MathTopic> MathTopics { get; set; } = null!;
    public DbSet<MathProblem> MathProblems { get; set; } = null!;
    public DbSet<MathProblemAttempt> MathProblemAttempts { get; set; } = null!;
    public DbSet<StudentProgress> StudentProgress { get; set; } = null!;
    public DbSet<UserMathProblem> UserMathProblems { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure relationships and constraints
        builder.Entity<ApplicationUser>()
            .HasMany(u => u.Progress)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ApplicationUser>()
            .HasMany(u => u.ProblemAttempts)
            .WithOne(a => a.User)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ApplicationUser>()
            .HasMany(u => u.UserMathProblems)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<SchoolClass>()
            .HasMany(c => c.Topics)
            .WithOne(t => t.SchoolClass)
            .HasForeignKey(t => t.SchoolClassId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<MathTopic>()
            .HasMany(t => t.Problems)
            .WithOne(p => p.Topic)
            .HasForeignKey(p => p.TopicId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<MathTopic>()
            .HasMany(t => t.StudentProgress)
            .WithOne(p => p.Topic)
            .HasForeignKey(p => p.TopicId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure parent-child relationship for MathTopic
        builder.Entity<MathTopic>()
            .HasOne(t => t.ParentTopic)
            .WithMany(t => t.Subtopics)
            .HasForeignKey(t => t.ParentTopicId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<MathProblem>()
            .HasMany(p => p.Attempts)
            .WithOne(a => a.Problem)
            .HasForeignKey(a => a.ProblemId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure relationship between MathProblem and ApplicationUser (Author)
        builder.Entity<MathProblem>()
            .HasOne(p => p.Author)
            .WithMany()
            .HasForeignKey(p => p.AuthorId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        // Seed data for school classes (grades 7-12)
        builder.Entity<SchoolClass>().HasData(
            new SchoolClass { Id = 7, Name = "Grade 7", Description = "7th grade mathematics course" },
            new SchoolClass { Id = 8, Name = "Grade 8", Description = "8th grade mathematics course" },
            new SchoolClass { Id = 9, Name = "Grade 9", Description = "9th grade mathematics course" },
            new SchoolClass { Id = 10, Name = "Grade 10", Description = "10th grade mathematics course" },
            new SchoolClass { Id = 11, Name = "Grade 11", Description = "11th grade mathematics course" },
            new SchoolClass { Id = 12, Name = "Grade 12", Description = "12th grade mathematics course" }
        );

        // Seed topics for the classes
        // Parent topics
        builder.Entity<MathTopic>().HasData(
            // Vectors and Motion (parent topic)
            new MathTopic {
                Id = 1,
                Name = "Vectors and Motion",
                Description = "Study of vectors and their applications in motion",
                SchoolClassId = 8,
                ParentTopicId = null,
                GradeLevel = 8
            },

            // Combinatorics and Probability I (parent topic)
            new MathTopic {
                Id = 20,
                Name = "Combinatorics and Probability I",
                Description = "Introduction to combinatorics and probability theory",
                SchoolClassId = 8,
                ParentTopicId = null,
                GradeLevel = 8
            },

            // Statistics I (parent topic)
            new MathTopic {
                Id = 30,
                Name = "Statistics I",
                Description = "Introduction to statistical concepts and data analysis",
                SchoolClassId = 9,
                ParentTopicId = null,
                GradeLevel = 9
            },

            // Fractional Functions and Algebraic Fractions (parent topic)
            new MathTopic {
                Id = 40,
                Name = "Fractional Functions and Algebraic Fractions",
                Description = "Study of fractional functions and algebraic expressions",
                SchoolClassId = 9,
                ParentTopicId = null,
                GradeLevel = 9
            },

            // Sine and Cosine Functions (parent topic)
            new MathTopic {
                Id = 60,
                Name = "Sine and Cosine Functions",
                Description = "Study of trigonometric functions and their properties",
                SchoolClassId = 10,
                ParentTopicId = null,
                GradeLevel = 10
            },

            // Power with Rational Exponent, Geometric Progression (parent topic)
            new MathTopic {
                Id = 70,
                Name = "Power with Rational Exponent, Geometric Progression",
                Description = "Study of powers with rational exponents and geometric progressions",
                SchoolClassId = 10,
                ParentTopicId = null,
                GradeLevel = 10
            },

            // Exponential Function (parent topic)
            new MathTopic {
                Id = 80,
                Name = "Exponential Function",
                Description = "Study of exponential functions and their applications",
                SchoolClassId = 11,
                ParentTopicId = null,
                GradeLevel = 11
            },

            // Lines and Planes in Space. Polyhedra (parent topic)
            new MathTopic {
                Id = 90,
                Name = "Lines and Planes in Space. Polyhedra",
                Description = "Study of 3D geometry, lines, planes, and polyhedra",
                SchoolClassId = 11,
                ParentTopicId = null,
                GradeLevel = 11
            },

            // Rotational Bodies (parent topic)
            new MathTopic {
                Id = 100,
                Name = "Rotational Bodies",
                Description = "Study of bodies formed by rotation",
                SchoolClassId = 12,
                ParentTopicId = null,
                GradeLevel = 12
            }
        );

        // Child topics for Vectors and Motion
        builder.Entity<MathTopic>().HasData(
            new MathTopic {
                Id = 2,
                Name = "Vector and its Magnitude. Vector Placement",
                Description = "Understanding vectors, their magnitude, and placement in space",
                SchoolClassId = 8,
                ParentTopicId = 1,
                GradeLevel = 8
            },

            new MathTopic {
                Id = 3,
                Name = "Vector Addition Laws",
                Description = "Laws of vector addition",
                SchoolClassId = 8,
                ParentTopicId = 1,
                GradeLevel = 8
            }
        );

        // Add standard algebra topics as top-level topics (no parent)
        builder.Entity<MathTopic>().HasData(
            new MathTopic {
                Id = 110,
                Name = "Linear Equations",
                Description = "Solving equations in the form ax + b = c",
                SchoolClassId = 7,
                ParentTopicId = null,
                GradeLevel = 7
            },

            new MathTopic {
                Id = 111,
                Name = "Quadratic Equations",
                Description = "Solving equations in the form ax² + bx + c = 0",
                SchoolClassId = 8,
                ParentTopicId = null,
                GradeLevel = 8
            }
        );

        // Seed roles
        builder.Entity<IdentityRole>().HasData(
            new IdentityRole
            {
                Id = "1",
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            },
            new IdentityRole
            {
                Id = "2",
                Name = "Student",
                NormalizedName = "STUDENT",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            },
            new IdentityRole
            {
                Id = "3",
                Name = "Teacher",
                NormalizedName = "TEACHER",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            }
        );

        // Seed users - using PasswordHasher directly as requested
        var hasher = new PasswordHasher<ApplicationUser>();
        var adminUser = new ApplicationUser
        {
            Id = "1",
            UserName = "admin@example.com",
            NormalizedUserName = "ADMIN@EXAMPLE.COM",
            Email = "admin@example.com",
            NormalizedEmail = "ADMIN@EXAMPLE.COM",
            EmailConfirmed = true,
            FirstName = "Admin",
            LastName = "User",
            CreatedAt = DateTime.UtcNow,
            IsVerified = true,
            SecurityStamp = Guid.NewGuid().ToString()
        };
        adminUser.PasswordHash = hasher.HashPassword(adminUser, "Parole123!");
        builder.Entity<ApplicationUser>().HasData(adminUser);

        var studentUser = new ApplicationUser
        {
            Id = "2",
            UserName = "student@example.com",
            NormalizedUserName = "STUDENT@EXAMPLE.COM",
            Email = "student@example.com",
            NormalizedEmail = "STUDENT@EXAMPLE.COM",
            EmailConfirmed = true,
            FirstName = "Student",
            LastName = "User",
            CreatedAt = DateTime.UtcNow,
            IsVerified = true,
            SecurityStamp = Guid.NewGuid().ToString()
        };
        studentUser.PasswordHash = hasher.HashPassword(studentUser, "Parole123!");
        builder.Entity<ApplicationUser>().HasData(studentUser);

        var teacherUser = new ApplicationUser
        {
            Id = "3",
            UserName = "teacher@example.com",
            NormalizedUserName = "TEACHER@EXAMPLE.COM",
            Email = "teacher@example.com",
            NormalizedEmail = "TEACHER@EXAMPLE.COM",
            EmailConfirmed = true,
            FirstName = "Teacher",
            LastName = "User",
            CreatedAt = DateTime.UtcNow,
            IsVerified = true,
            SecurityStamp = Guid.NewGuid().ToString()
        };
        teacherUser.PasswordHash = hasher.HashPassword(teacherUser, "Parole123!");
        builder.Entity<ApplicationUser>().HasData(teacherUser);

        // Seed user roles
        builder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string> { RoleId = "1", UserId = "1" }, // Admin role for admin user
            new IdentityUserRole<string> { RoleId = "2", UserId = "2" }, // Student role for student user
            new IdentityUserRole<string> { RoleId = "3", UserId = "3" }  // Teacher role for teacher user
        );

        // Seed MathProblems
        builder.Entity<MathProblem>().HasData(
            // Vector and its Magnitude (Topic ID: 2)
            new MathProblem {
                Id = 1001,
                Name = "Vector Magnitude Calculation",
                TopicId = 2,
                Statement = "Calculate the magnitude of vector v⃗ = (3,4)",
                Solution = "|v⃗| = 5",
                Explanation = "Using the Pythagorean theorem: |v⃗| = √(3² + 4²) = √(9 + 16) = √25 = 5",
                Difficulty = DifficultyLevel.Easy,
                PointValue = 1,
                AuthorId = "3"
            },
            new MathProblem {
                Id = 1002,
                Name = "3D Vector Magnitude",
                TopicId = 2,
                Statement = "Find the magnitude of vector a⃗ = (2,2,1)",
                Solution = "|a⃗| = 3",
                Explanation = "Using the 3D magnitude formula: |a⃗| = √(2² + 2² + 1²) = √(4 + 4 + 1) = √9 = 3",
                Difficulty = DifficultyLevel.Medium,
                PointValue = 2,
                AuthorId = "3"
            },
            new MathProblem {
                Id = 1003,
                Name = "Vector Components",
                TopicId = 2,
                Statement = "A vector has magnitude 5 and makes a 30° angle with the x-axis. Find its x and y components.",
                Solution = "x ≈ 4.33, y = 2.5",
                Explanation = "Using trigonometry: x = 5cos(30°), y = 5sin(30°)",
                Difficulty = DifficultyLevel.Medium,
                PointValue = 2,
                AuthorId = "3"
            },

            // Additional Vector Magnitude problems (Topic ID: 2)
            new MathProblem {
                Id = 1004,
                Name = "Unit Vector Calculation",
                TopicId = 2,
                Statement = "Find the unit vector in the direction of v⃗ = (6,8)",
                Solution = "u⃗ = (0.6,0.8)",
                Explanation = "First find |v⃗| = √(6² + 8²) = 10, then divide components by magnitude: (6/10, 8/10)",
                Difficulty = DifficultyLevel.Medium,
                PointValue = 2,
                AuthorId = "3"
            },
            new MathProblem {
                Id = 1005,
                Name = "Vector Direction Angle",
                TopicId = 2,
                Statement = "Find the angle that vector v⃗ = (4,-4) makes with the positive x-axis.",
                Solution = "-45°",
                Explanation = "Use arctan(y/x) = arctan(-4/4) = -45°",
                Difficulty = DifficultyLevel.Medium,
                PointValue = 2,
                AuthorId = "3"
            },
            new MathProblem {
                Id = 1006,
                Name = "3D Vector Components",
                TopicId = 2,
                Statement = "A vector has magnitude 6 and makes angles of 60° with x-axis and 60° with y-axis. Find its z-component.",
                Solution = "z ≈ 3.46",
                Explanation = "Using direction cosines: cos²α + cos²β + cos²γ = 1, cos(60°) = 0.5, z = 6cos(γ) ≈ 3.46",
                Difficulty = DifficultyLevel.Hard,
                PointValue = 3,
                AuthorId = "3"
            },
            new MathProblem {
                Id = 1007,
                Name = "Vector Decomposition",
                TopicId = 2,
                Statement = "Decompose vector v⃗ with magnitude 10 into components along 30° and 120° from x-axis.",
                Solution = "v₁ ≈ 8.66, v₂ ≈ 5",
                Explanation = "Using vector decomposition formulas and the given angles",
                Difficulty = DifficultyLevel.Hard,
                PointValue = 3,
                AuthorId = "3"
            },
            new MathProblem {
                Id = 1008,
                Name = "Equal Vectors",
                TopicId = 2,
                Statement = "When are two vectors equal if they start at different points?",
                Solution = "When they have same magnitude and direction",
                Explanation = "Vectors are equal when they have the same magnitude and direction, regardless of their position",
                Difficulty = DifficultyLevel.Easy,
                PointValue = 1,
                AuthorId = "3"
            },
            new MathProblem {
                Id = 1009,
                Name = "Vector Position",
                TopicId = 2,
                Statement = "A vector starts at point A(1,2) and ends at B(4,6). Find its components and magnitude.",
                Solution = "Components: (3,4), Magnitude: 5",
                Explanation = "Components: B - A = (4-1,6-2) = (3,4). Magnitude: √(3² + 4²) = 5",
                Difficulty = DifficultyLevel.Medium,
                PointValue = 2,
                AuthorId = "3"
            },
            new MathProblem {
                Id = 1010,
                Name = "Parallel Vectors",
                TopicId = 2,
                Statement = "Are vectors a⃗ = (6,8) and b⃗ = (3,4) parallel?",
                Solution = "Yes",
                Explanation = "Vectors are parallel if one is a scalar multiple of the other. Here b⃗ = ½a⃗",
                Difficulty = DifficultyLevel.Medium,
                PointValue = 2,
                AuthorId = "3"
            },

            // Vector Addition Laws (Topic ID: 3)
            new MathProblem {
                Id = 2001,
                Name = "Vector Addition",
                TopicId = 3,
                Statement = "Add vectors p⃗ = (2,3) and q⃗ = (-1,4)",
                Solution = "p⃗ + q⃗ = (1,7)",
                Explanation = "Add corresponding components: (2+(-1), 3+4) = (1,7)",
                Difficulty = DifficultyLevel.Easy,
                PointValue = 1,
                AuthorId = "3"
            },
            new MathProblem {
                Id = 2002,
                Name = "Multiple Vector Addition",
                TopicId = 3,
                Statement = "Add three vectors: a⃗ = (1,1), b⃗ = (2,-1), and c⃗ = (-1,3)",
                Solution = "a⃗ + b⃗ + c⃗ = (2,3)",
                Explanation = "Add all x components and all y components: (1+2+(-1), 1+(-1)+3) = (2,3)",
                Difficulty = DifficultyLevel.Medium,
                PointValue = 2,
                AuthorId = "3"
            },

            // Additional Vector Addition problems (Topic ID: 3)
            new MathProblem {
                Id = 2003,
                Name = "Vector Subtraction",
                TopicId = 3,
                Statement = "Subtract vector b⃗ = (3,4) from a⃗ = (5,7)",
                Solution = "a⃗ - b⃗ = (2,3)",
                Explanation = "Subtract corresponding components: (5-3,7-4) = (2,3)",
                Difficulty = DifficultyLevel.Easy,
                PointValue = 1,
                AuthorId = "3"
            },
            new MathProblem {
                Id = 2004,
                Name = "Resultant Vector",
                TopicId = 3,
                Statement = "Find the magnitude of the resultant vector when a⃗ = (3,0) and b⃗ = (0,4) are added.",
                Solution = "|r⃗| = 5",
                Explanation = "Using Pythagorean theorem on the resultant: √(3² + 4²) = 5",
                Difficulty = DifficultyLevel.Medium,
                PointValue = 2,
                AuthorId = "3"
            },
            new MathProblem {
                Id = 2005,
                Name = "Vector Triangle",
                TopicId = 3,
                Statement = "Three vectors form a triangle. If two vectors are a⃗ = (2,3) and b⃗ = (4,1), find the third vector c⃗.",
                Solution = "c⃗ = (-6,-4)",
                Explanation = "In a vector triangle, a⃗ + b⃗ + c⃗ = 0, so c⃗ = -(a⃗ + b⃗) = -(2+4,3+1) = (-6,-4)",
                Difficulty = DifficultyLevel.Medium,
                PointValue = 2,
                AuthorId = "3"
            },
            new MathProblem {
                Id = 2006,
                Name = "Parallel Vector Addition",
                TopicId = 3,
                Statement = "Add three parallel vectors: 2î, 3î, and -4î",
                Solution = "î",
                Explanation = "Add the scalar components: 2 + 3 + (-4) = 1, so result is 1î",
                Difficulty = DifficultyLevel.Easy,
                PointValue = 1,
                AuthorId = "3"
            },
            new MathProblem {
                Id = 2007,
                Name = "Force Vectors",
                TopicId = 3,
                Statement = "Two forces of 3N and 4N act at right angles. Find their resultant.",
                Solution = "5N",
                Explanation = "Using Pythagorean theorem: √(3² + 4²) = 5N",
                Difficulty = DifficultyLevel.Medium,
                PointValue = 2,
                AuthorId = "3"
            },
            new MathProblem {
                Id = 2008,
                Name = "Zero Vector",
                TopicId = 3,
                Statement = "What vector should be added to a⃗ = (2,3) to get zero vector?",
                Solution = "(-2,-3)",
                Explanation = "The negative of the vector: -(2,3) = (-2,-3)",
                Difficulty = DifficultyLevel.Easy,
                PointValue = 1,
                AuthorId = "3"
            },
            new MathProblem {
                Id = 2009,
                Name = "Vector Chain",
                TopicId = 3,
                Statement = "A person walks 3m east, 4m north, then 6m west. Find the resultant displacement.",
                Solution = "5m at 37° south of west",
                Explanation = "Final position: (-3,4). Magnitude = 5m, angle = arctan(4/3) from west direction",
                Difficulty = DifficultyLevel.Hard,
                PointValue = 3,
                AuthorId = "3"
            },
            new MathProblem {
                Id = 2010,
                Name = "Equilibrium Vectors",
                TopicId = 3,
                Statement = "Three forces act on a point: 2N at 0°, 2N at 120°, and 2N at 240°. Are they in equilibrium?",
                Solution = "Yes",
                Explanation = "Sum of vectors = 0 due to symmetric 120° angles and equal magnitudes",
                Difficulty = DifficultyLevel.Hard,
                PointValue = 3,
                AuthorId = "3"
            },

            // Linear Equations (Topic ID: 110) - Additional problems to existing ones
            new MathProblem {
                Id = 7001,
                Name = "Linear Equation with Parentheses",
                TopicId = 110,
                Statement = "Solve: 3(x - 2) = 15",
                Solution = "x = 7",
                Explanation = "First distribute: 3x - 6 = 15. Add 6 to both sides: 3x = 21. Divide by 3: x = 7",
                Difficulty = DifficultyLevel.Medium,
                PointValue = 2,
                AuthorId = "3"
            },
            new MathProblem {
                Id = 7002,
                Name = "Linear Equation with Variables on Both Sides",
                TopicId = 110,
                Statement = "Solve: 4x + 3 = 2x + 11",
                Solution = "x = 4",
                Explanation = "Subtract 2x from both sides: 2x + 3 = 11. Subtract 3: 2x = 8. Divide by 2: x = 4",
                Difficulty = DifficultyLevel.Medium,
                PointValue = 2,
                AuthorId = "3"
            },

            // Additional Linear Equations problems (Topic ID: 110)
            new MathProblem {
                Id = 7003,
                Name = "Linear Equation with Fractions",
                TopicId = 110,
                Statement = "Solve: x/2 + x/3 = 5",
                Solution = "x = 6",
                Explanation = "Find common denominator: (3x + 2x)/6 = 5, 5x/6 = 5, x = 6",
                Difficulty = DifficultyLevel.Medium,
                PointValue = 2,
                AuthorId = "3"
            },
            new MathProblem {
                Id = 7004,
                Name = "Word Problem - Age",
                TopicId = 110,
                Statement = "In 5 years, John will be twice as old as he was 7 years ago. How old is he now?",
                Solution = "19 years old",
                Explanation = "Let x be current age. x + 5 = 2(x - 7), solve for x",
                Difficulty = DifficultyLevel.Medium,
                PointValue = 2,
                AuthorId = "3"
            },
            new MathProblem {
                Id = 7005,
                Name = "Linear Inequalities",
                TopicId = 110,
                Statement = "Solve: 2x + 1 < 7",
                Solution = "x < 3",
                Explanation = "Subtract 1 from both sides: 2x < 6, divide by 2: x < 3",
                Difficulty = DifficultyLevel.Easy,
                PointValue = 1,
                AuthorId = "3"
            },
            new MathProblem {
                Id = 7006,
                Name = "Distance Problem",
                TopicId = 110,
                Statement = "A car travels 120 km at constant speed. If it takes 2 hours, find the speed.",
                Solution = "60 km/h",
                Explanation = "Use distance = speed × time: 120 = x × 2, x = 60",
                Difficulty = DifficultyLevel.Easy,
                PointValue = 1,
                AuthorId = "3"
            },
            new MathProblem {
                Id = 7007,
                Name = "Mixture Problem",
                TopicId = 110,
                Statement = "How much 80% solution should be mixed with 50% solution to get 200ml of 60% solution?",
                Solution = "66.67ml of 80% solution",
                Explanation = "Use mixture equation: 0.8x + 0.5(200-x) = 0.6(200)",
                Difficulty = DifficultyLevel.Hard,
                PointValue = 3,
                AuthorId = "3"
            },
            new MathProblem {
                Id = 7008,
                Name = "Linear System",
                TopicId = 110,
                Statement = "Solve: x + y = 5, x - y = 1",
                Solution = "x = 3, y = 2",
                Explanation = "Add equations: 2x = 6, x = 3. Substitute to find y = 2",
                Difficulty = DifficultyLevel.Medium,
                PointValue = 2,
                AuthorId = "3"
            },
            new MathProblem {
                Id = 7009,
                Name = "Investment Problem",
                TopicId = 110,
                Statement = "Find investment at 5% to earn $1000 annual interest.",
                Solution = "$20,000",
                Explanation = "Use I = Pr: 1000 = x(0.05), x = 20000",
                Difficulty = DifficultyLevel.Medium,
                PointValue = 2,
                AuthorId = "3"
            },
            new MathProblem {
                Id = 7010,
                Name = "Motion Problem",
                TopicId = 110,
                Statement = "A train travels 300km. Going is 2h faster than return due to 20km/h speed difference. Find speeds.",
                Solution = "60km/h and 40km/h",
                Explanation = "Let x be slower speed: 300/x = 300/(x+20) + 2",
                Difficulty = DifficultyLevel.Hard,
                PointValue = 3,
                AuthorId = "3"
            },

            // Quadratic Equations (Topic ID: 111) - Additional problems to existing ones
            new MathProblem {
                Id = 8001,
                Name = "Complex Quadratic Equation",
                TopicId = 111,
                Statement = "Solve: 3x² - 12x + 12 = 0",
                Solution = "x = 2 or x = 2",
                Explanation = "This is a perfect square trinomial: 3(x² - 4x + 4) = 0, 3(x - 2)² = 0, x = 2 (double root)",
                Difficulty = DifficultyLevel.Hard,
                PointValue = 3,
                AuthorId = "3"
            },
            new MathProblem {
                Id = 8002,
                Name = "Quadratic with Complex Roots",
                TopicId = 111,
                Statement = "Solve: x² + 1 = 0",
                Solution = "x = i or x = -i",
                Explanation = "This equation has no real solutions. The solutions are the complex numbers i and -i",
                Difficulty = DifficultyLevel.Hard,
                PointValue = 3,
                AuthorId = "3"
            },

            // Additional Quadratic Equations problems (Topic ID: 111)
            new MathProblem {
                Id = 8003,
                Name = "Factoring Quadratic",
                TopicId = 111,
                Statement = "Solve by factoring: x² - 5x + 6 = 0",
                Solution = "x = 2 or x = 3",
                Explanation = "Factor as (x-2)(x-3) = 0, use zero product property",
                Difficulty = DifficultyLevel.Medium,
                PointValue = 2,
                AuthorId = "3"
            },
            new MathProblem {
                Id = 8004,
                Name = "Completing Square",
                TopicId = 111,
                Statement = "Solve by completing square: x² + 6x + 5 = 0",
                Solution = "x = -1 or x = -5",
                Explanation = "(x² + 6x + 9) - 9 + 5 = 0, (x + 3)² = 4",
                Difficulty = DifficultyLevel.Medium,
                PointValue = 2,
                AuthorId = "3"
            },
            new MathProblem {
                Id = 8005,
                Name = "Area Problem",
                TopicId = 111,
                Statement = "Rectangle's length is 2 more than width. Area is 48. Find dimensions.",
                Solution = "8 by 6",
                Explanation = "Let w be width: w(w+2) = 48, w² + 2w - 48 = 0",
                Difficulty = DifficultyLevel.Medium,
                PointValue = 2,
                AuthorId = "3"
            },
            new MathProblem {
                Id = 8006,
                Name = "Quadratic Inequalities",
                TopicId = 111,
                Statement = "Solve: x² - x - 6 > 0",
                Solution = "x < -2 or x > 3",
                Explanation = "Factor: (x+2)(x-3) > 0, test intervals",
                Difficulty = DifficultyLevel.Hard,
                PointValue = 3,
                AuthorId = "3"
            },
            new MathProblem {
                Id = 8007,
                Name = "Projectile Motion",
                TopicId = 111,
                Statement = "Ball thrown up at 20m/s. When does it reach 15m? (g=10m/s²)",
                Solution = "t = 1 or t = 3",
                Explanation = "-5t² + 20t = 15, solve quadratic",
                Difficulty = DifficultyLevel.Hard,
                PointValue = 3,
                AuthorId = "3"
            },
            new MathProblem {
                Id = 8008,
                Name = "Sum and Product",
                TopicId = 111,
                Statement = "Find two numbers whose sum is 7 and product is 12.",
                Solution = "x = 3 or x = 4",
                Explanation = "x + y = 7, xy = 12, substitute: x² - 7x + 12 = 0",
                Difficulty = DifficultyLevel.Medium,
                PointValue = 2,
                AuthorId = "3"
            },
            new MathProblem {
                Id = 8009,
                Name = "Quadratic System",
                TopicId = 111,
                Statement = "Solve: x² + y = 4, x + y = 2",
                Solution = "x = 0, y = 2 or x = 2, y = 0",
                Explanation = "Substitute y = 2-x: x² + 2-x = 4",
                Difficulty = DifficultyLevel.Hard,
                PointValue = 3,
                AuthorId = "3"
            },
            new MathProblem {
                Id = 8010,
                Name = "Complex Roots Pattern",
                TopicId = 111,
                Statement = "For what values of k are roots of x² + kx + 1 = 0 complex?",
                Solution = "|k| < 2",
                Explanation = "Using discriminant: k² - 4 < 0",
                Difficulty = DifficultyLevel.Hard,
                PointValue = 3,
                AuthorId = "3"
            }
        );
    }
}