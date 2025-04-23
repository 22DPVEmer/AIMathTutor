using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MathTutor.Core.Entities;
using Microsoft.AspNetCore.Identity;
using MathTutor.Core.Enums;
using System;

namespace MathTutor.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<MathCategory> MathCategories { get; set; } = null!;
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

        builder.Entity<MathCategory>()
            .HasMany(c => c.Topics)
            .WithOne(t => t.Category)
            .HasForeignKey(t => t.CategoryId)
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

        // Seed data for math categories
        builder.Entity<MathCategory>().HasData(
            new MathCategory { Id = 1, Name = "Algebra", Description = "Master algebraic expressions and equations" },
            new MathCategory { Id = 2, Name = "Geometry", Description = "Explore shapes and spatial relationships" },
            new MathCategory { Id = 3, Name = "Calculus", Description = "Learn about limits, derivatives, and integrals" },
            new MathCategory { Id = 4, Name = "Statistics", Description = "Understand data analysis and probability" },
            new MathCategory { Id = 5, Name = "Vectors and Physics", Description = "Study vectors and their applications in physics" },
            new MathCategory { Id = 6, Name = "Probability and Statistics", Description = "Learn combinatorics, probability and statistical analysis" },
            new MathCategory { Id = 7, Name = "Trigonometry", Description = "Study of triangles and trigonometric functions" }
        );

        // Seed topics for the categories
        // Parent topics
        builder.Entity<MathTopic>().HasData(
            // Vectors and Motion (parent topic)
            new MathTopic { 
                Id = 1, 
                Name = "Vectors and Motion", 
                Description = "Study of vectors and their applications in motion", 
                CategoryId = 5, 
                ParentTopicId = null, 
                GradeLevel = 8 
            },
            
            // Combinatorics and Probability I (parent topic)
            new MathTopic { 
                Id = 20, 
                Name = "Combinatorics and Probability I", 
                Description = "Introduction to combinatorics and probability theory", 
                CategoryId = 6, 
                ParentTopicId = null, 
                GradeLevel = 8 
            },
            
            // Statistics I (parent topic)
            new MathTopic { 
                Id = 30, 
                Name = "Statistics I", 
                Description = "Introduction to statistical concepts and data analysis", 
                CategoryId = 6, 
                ParentTopicId = null, 
                GradeLevel = 5 
            },
            
            // Fractional Functions and Algebraic Fractions (parent topic)
            new MathTopic { 
                Id = 40, 
                Name = "Fractional Functions and Algebraic Fractions", 
                Description = "Study of fractional functions and algebraic expressions", 
                CategoryId = 1, 
                ParentTopicId = null, 
                GradeLevel = 9 
            },
            
            // Sine and Cosine Functions (parent topic)
            new MathTopic { 
                Id = 60, 
                Name = "Sine and Cosine Functions", 
                Description = "Study of trigonometric functions and their properties", 
                CategoryId = 7, 
                ParentTopicId = null, 
                GradeLevel = 10 
            },
            
            // Power with Rational Exponent, Geometric Progression (parent topic)
            new MathTopic { 
                Id = 70, 
                Name = "Power with Rational Exponent, Geometric Progression", 
                Description = "Study of powers with rational exponents and geometric progressions", 
                CategoryId = 1, 
                ParentTopicId = null, 
                GradeLevel = 10 
            },
            
            // Exponential Function (parent topic)
            new MathTopic { 
                Id = 80, 
                Name = "Exponential Function", 
                Description = "Study of exponential functions and their applications", 
                CategoryId = 1, 
                ParentTopicId = null, 
                GradeLevel = 11 
            },
            
            // Lines and Planes in Space. Polyhedra (parent topic)
            new MathTopic { 
                Id = 90, 
                Name = "Lines and Planes in Space. Polyhedra", 
                Description = "Study of 3D geometry, lines, planes and polyhedra", 
                CategoryId = 2, 
                ParentTopicId = null, 
                GradeLevel = 11 
            },
            
            // Rotational Bodies (parent topic)
            new MathTopic { 
                Id = 100, 
                Name = "Rotational Bodies", 
                Description = "Study of bodies formed by rotation", 
                CategoryId = 2, 
                ParentTopicId = null, 
                GradeLevel = 12 
            }
        );

        // Child topics for Vectors and Motion
        builder.Entity<MathTopic>().HasData(
            new MathTopic { 
                Id = 2, 
                Name = "Vector and its Magnitude. Vector Placement", 
                Description = "Understanding vectors, their magnitude and placement in space", 
                CategoryId = 5, 
                ParentTopicId = 1, 
                GradeLevel = 9 
            },
            
            new MathTopic { 
                Id = 3, 
                Name = "Vector Addition Laws", 
                Description = "Laws governing the addition of vectors", 
                CategoryId = 5, 
                ParentTopicId = 1, 
                GradeLevel = 9 
            },
            
            new MathTopic { 
                Id = 4, 
                Name = "Vector Representation", 
                Description = "Different ways to represent vectors", 
                CategoryId = 5, 
                ParentTopicId = 1, 
                GradeLevel = 9 
            },
            
            new MathTopic { 
                Id = 5, 
                Name = "Vector Projection on an Axis", 
                Description = "Projecting vectors onto coordinate axes", 
                CategoryId = 5, 
                ParentTopicId = 1, 
                GradeLevel = 9 
            },
            
            new MathTopic { 
                Id = 6, 
                Name = "Distance Between Two Points", 
                Description = "Calculating distance in vector spaces", 
                CategoryId = 5, 
                ParentTopicId = 1, 
                GradeLevel = 9 
            },
            
            new MathTopic { 
                Id = 7, 
                Name = "Vectors in Coordinate Form on a Plane", 
                Description = "Working with vectors in 2D coordinate systems", 
                CategoryId = 5, 
                ParentTopicId = 1, 
                GradeLevel = 10 
            },
            
            new MathTopic { 
                Id = 8, 
                Name = "Vectors in Space", 
                Description = "Working with vectors in 3D space", 
                CategoryId = 5, 
                ParentTopicId = 1, 
                GradeLevel = 10 
            },
            
            new MathTopic { 
                Id = 9, 
                Name = "Line Equation", 
                Description = "Equations of lines in vector form", 
                CategoryId = 5, 
                ParentTopicId = 1, 
                GradeLevel = 10 
            },
            
            new MathTopic { 
                Id = 10, 
                Name = "Linear Function. Function and Argument Increment", 
                Description = "Understanding linear functions and increments", 
                CategoryId = 5, 
                ParentTopicId = 1, 
                GradeLevel = 10 
            },
            
            new MathTopic { 
                Id = 11, 
                Name = "Line Equation", 
                Description = "Different forms of line equations", 
                CategoryId = 5, 
                ParentTopicId = 1, 
                GradeLevel = 7 
            },
            
            new MathTopic { 
                Id = 12, 
                Name = "Parallel and Perpendicular Lines", 
                Description = "Properties of parallel and perpendicular lines", 
                CategoryId = 5, 
                ParentTopicId = 1, 
                GradeLevel = 8 
            },
            
            new MathTopic { 
                Id = 13, 
                Name = "Equation with 2 Variables. Circle Equation", 
                Description = "Working with equations in two variables and circle equations", 
                CategoryId = 5, 
                ParentTopicId = 1, 
                GradeLevel = 10 
            },
            
            new MathTopic { 
                Id = 14, 
                Name = "Inequality with 2 Variables", 
                Description = "Solving and graphing inequalities with two variables", 
                CategoryId = 5, 
                ParentTopicId = 1, 
                GradeLevel = 10 
            }
        );

        // Child topics for Combinatorics and Probability I
        builder.Entity<MathTopic>().HasData(
            new MathTopic { 
                Id = 21, 
                Name = "Sets. Operations with Sets", 
                Description = "Understanding sets and operations on sets", 
                CategoryId = 6, 
                ParentTopicId = 20, 
                GradeLevel = 9 
            },
            
            new MathTopic { 
                Id = 22, 
                Name = "Introduction to Combinatorics", 
                Description = "Basic principles of combinatorics", 
                CategoryId = 6, 
                ParentTopicId = 20, 
                GradeLevel = 9 
            },
            
            new MathTopic { 
                Id = 23, 
                Name = "Combinatorics I", 
                Description = "Counting principles, permutations and combinations", 
                CategoryId = 6, 
                ParentTopicId = 20, 
                GradeLevel = 9 
            },
            
            new MathTopic { 
                Id = 24, 
                Name = "Elements of Probability Theory", 
                Description = "Basic concepts of probability theory", 
                CategoryId = 6, 
                ParentTopicId = 20, 
                GradeLevel = 6 
            },
            
            new MathTopic { 
                Id = 25, 
                Name = "Sum Probability. Conditional Probability", 
                Description = "Advanced probability concepts", 
                CategoryId = 6, 
                ParentTopicId = 20, 
                GradeLevel = 10 
            }
        );

        // Child topics for Statistics I
        builder.Entity<MathTopic>().HasData(
            new MathTopic { 
                Id = 31, 
                Name = "Population, Sample and Data. Mean Values", 
                Description = "Understanding statistical populations, samples and central tendency", 
                CategoryId = 6, 
                ParentTopicId = 30, 
                GradeLevel = 9 
            },
            
            new MathTopic { 
                Id = 32, 
                Name = "Measures of Dispersion, Graphical Representation of Data", 
                Description = "Understanding variability in data and data visualization", 
                CategoryId = 6, 
                ParentTopicId = 30, 
                GradeLevel = 3 
            }
        );

        // Child topics for Fractional Functions and Algebraic Fractions
        builder.Entity<MathTopic>().HasData(
            new MathTopic { 
                Id = 41, 
                Name = "Rational Algebraic Expressions", 
                Description = "Working with rational expressions in algebra", 
                CategoryId = 1, 
                ParentTopicId = 40, 
                GradeLevel = 9 
            },
            
            new MathTopic { 
                Id = 42, 
                Name = "Algebraic Equations", 
                Description = "Solving various types of algebraic equations", 
                CategoryId = 1, 
                ParentTopicId = 40, 
                GradeLevel = 9 
            },
            
            new MathTopic { 
                Id = 43, 
                Name = "Algebraic Fractions. Domain of Definition", 
                Description = "Understanding algebraic fractions and their domains", 
                CategoryId = 1, 
                ParentTopicId = 40, 
                GradeLevel = 10 
            },
            
            new MathTopic { 
                Id = 44, 
                Name = "Simplification and Expansion of Algebraic Fractions", 
                Description = "Techniques for simplifying and expanding algebraic fractions", 
                CategoryId = 1, 
                ParentTopicId = 40, 
                GradeLevel = 10 
            },
            
            new MathTopic { 
                Id = 45, 
                Name = "Identity. Sign Change Rule", 
                Description = "Understanding identities and sign changes in algebra", 
                CategoryId = 1, 
                ParentTopicId = 40, 
                GradeLevel = 10 
            },
            
            new MathTopic { 
                Id = 46, 
                Name = "Multiplication, Division, Exponentiation of Algebraic Fractions", 
                Description = "Operations with algebraic fractions", 
                CategoryId = 1, 
                ParentTopicId = 40, 
                GradeLevel = 10 
            },
            
            new MathTopic { 
                Id = 47, 
                Name = "Addition and Subtraction of Algebraic Fractions", 
                Description = "Techniques for adding and subtracting algebraic fractions", 
                CategoryId = 1, 
                ParentTopicId = 40, 
                GradeLevel = 10 
            },
            
            new MathTopic { 
                Id = 48, 
                Name = "Fractional Function", 
                Description = "Understanding and working with fractional functions", 
                CategoryId = 1, 
                ParentTopicId = 40, 
                GradeLevel = 10 
            },
            
            new MathTopic { 
                Id = 49, 
                Name = "Fractional Equations and Inequalities", 
                Description = "Solving equations and inequalities involving fractions", 
                CategoryId = 1, 
                ParentTopicId = 40, 
                GradeLevel = 10 
            },
            
            new MathTopic { 
                Id = 50, 
                Name = "Fractional Equations", 
                Description = "Methods for solving equations with fractions", 
                CategoryId = 1, 
                ParentTopicId = 40, 
                GradeLevel = 10 
            },
            
            new MathTopic { 
                Id = 51, 
                Name = "Fractional Equations in Word Problems", 
                Description = "Applying fractional equations to solve word problems", 
                CategoryId = 1, 
                ParentTopicId = 40, 
                GradeLevel = 10 
            },
            
            new MathTopic { 
                Id = 52, 
                Name = "Review of Linear and Quadratic Inequality Solving", 
                Description = "Revisiting methods for solving linear and quadratic inequalities", 
                CategoryId = 1, 
                ParentTopicId = 40, 
                GradeLevel = 10 
            },
            
            new MathTopic { 
                Id = 53, 
                Name = "Fractional Inequalities. Interval Method", 
                Description = "Solving inequalities with fractions using intervals", 
                CategoryId = 1, 
                ParentTopicId = 40, 
                GradeLevel = 11 
            }
        );

        // Child topics for Sine and Cosine Functions
        builder.Entity<MathTopic>().HasData(
            new MathTopic { 
                Id = 61, 
                Name = "Relationships in a Right Triangle. Review", 
                Description = "Reviewing trigonometric relationships in right triangles", 
                CategoryId = 7, 
                ParentTopicId = 60, 
                GradeLevel = 10 
            },
            
            new MathTopic { 
                Id = 62, 
                Name = "Sine and Cosine of a Rotation Angle", 
                Description = "Understanding sine and cosine for any angle", 
                CategoryId = 7, 
                ParentTopicId = 60, 
                GradeLevel = 10 
            },
            
            new MathTopic { 
                Id = 63, 
                Name = "Sine and Cosine Theorems", 
                Description = "Applications of sine and cosine theorems in triangles", 
                CategoryId = 7, 
                ParentTopicId = 60, 
                GradeLevel = 11 
            },
            
            new MathTopic { 
                Id = 64, 
                Name = "Trigonometric Functions and Their Properties", 
                Description = "Study of trigonometric functions and their key properties", 
                CategoryId = 7, 
                ParentTopicId = 60, 
                GradeLevel = 11 
            }
        );

        // Child topics for Trigonometric Expressions and Equations
        builder.Entity<MathTopic>().HasData(
            new MathTopic { 
                Id = 65, 
                Name = "Trigonometric Expressions and Equations", 
                Description = "Working with trigonometric expressions and solving equations", 
                CategoryId = 7, 
                ParentTopicId = 60, 
                GradeLevel = 11 
            },
            
            new MathTopic { 
                Id = 66, 
                Name = "Trigonometric Expressions and Basic Identity", 
                Description = "Understanding fundamental trigonometric identities", 
                CategoryId = 7, 
                ParentTopicId = 65, 
                GradeLevel = 11 
            },
            
            new MathTopic { 
                Id = 67, 
                Name = "Sum of Arguments and Double Argument Formulas", 
                Description = "Working with complex trigonometric formulas", 
                CategoryId = 7, 
                ParentTopicId = 65, 
                GradeLevel = 11 
            },
            
            new MathTopic { 
                Id = 68, 
                Name = "Basic Trigonometric Equations", 
                Description = "Solving fundamental trigonometric equations", 
                CategoryId = 7, 
                ParentTopicId = 65, 
                GradeLevel = 11 
            },
            
            new MathTopic { 
                Id = 69, 
                Name = "Factorization and Substitution Method", 
                Description = "Advanced techniques for solving trigonometric equations", 
                CategoryId = 7, 
                ParentTopicId = 65, 
                GradeLevel = 11 
            }
        );

        // Child topics for Power with Rational Exponent, Geometric Progression
        builder.Entity<MathTopic>().HasData(
            new MathTopic { 
                Id = 71, 
                Name = "Nth Degree Root", 
                Description = "Understanding and calculating nth roots", 
                CategoryId = 1, 
                ParentTopicId = 70, 
                GradeLevel = 10 
            },
            
            new MathTopic { 
                Id = 72, 
                Name = "Power with Rational Exponent", 
                Description = "Working with powers that have rational exponents", 
                CategoryId = 1, 
                ParentTopicId = 70, 
                GradeLevel = 10 
            },
            
            new MathTopic { 
                Id = 73, 
                Name = "Sequences", 
                Description = "Understanding and working with numeric sequences", 
                CategoryId = 1, 
                ParentTopicId = 70, 
                GradeLevel = 10 
            },
            
            new MathTopic { 
                Id = 74, 
                Name = "Geometric Progression", 
                Description = "Properties and applications of geometric progressions", 
                CategoryId = 1, 
                ParentTopicId = 70, 
                GradeLevel = 10 
            }
        );

        // Child topics for Exponential Function
        builder.Entity<MathTopic>().HasData(
            new MathTopic { 
                Id = 81, 
                Name = "Exponential Function", 
                Description = "Understanding exponential functions and their graphs", 
                CategoryId = 1, 
                ParentTopicId = 80, 
                GradeLevel = 11 
            },
            
            new MathTopic { 
                Id = 82, 
                Name = "Logarithm of a Number", 
                Description = "Understanding logarithms and their properties", 
                CategoryId = 1, 
                ParentTopicId = 80, 
                GradeLevel = 11 
            },
            
            new MathTopic { 
                Id = 83, 
                Name = "Exponential Processes", 
                Description = "Applications of exponential functions in real-world processes", 
                CategoryId = 1, 
                ParentTopicId = 80, 
                GradeLevel = 11 
            },
            
            new MathTopic { 
                Id = 84, 
                Name = "Power Properties. Review", 
                Description = "Reviewing properties of powers", 
                CategoryId = 1, 
                ParentTopicId = 80, 
                GradeLevel = 11 
            },
            
            new MathTopic { 
                Id = 85, 
                Name = "Basic Equations. Review", 
                Description = "Reviewing methods for solving basic equations", 
                CategoryId = 1, 
                ParentTopicId = 80, 
                GradeLevel = 11 
            },
            
            new MathTopic { 
                Id = 86, 
                Name = "Exponential Equations", 
                Description = "Techniques for solving exponential equations", 
                CategoryId = 1, 
                ParentTopicId = 80, 
                GradeLevel = 11 
            },
            
            new MathTopic { 
                Id = 87, 
                Name = "Basic Inequalities. Review", 
                Description = "Reviewing methods for solving basic inequalities", 
                CategoryId = 1, 
                ParentTopicId = 80, 
                GradeLevel = 11 
            },
            
            new MathTopic { 
                Id = 88, 
                Name = "Exponential Inequalities", 
                Description = "Techniques for solving exponential inequalities", 
                CategoryId = 1, 
                ParentTopicId = 80, 
                GradeLevel = 11 
            }
        );

        // Child topics for Lines and Planes in Space. Polyhedra
        builder.Entity<MathTopic>().HasData(
            new MathTopic { 
                Id = 91, 
                Name = "Right Triangle Calculation. Review", 
                Description = "Reviewing methods for solving right triangles", 
                CategoryId = 2, 
                ParentTopicId = 90, 
                GradeLevel = 11 
            },
            
            new MathTopic { 
                Id = 92, 
                Name = "Lines and Planes in Space", 
                Description = "Understanding geometric relationships in 3D space", 
                CategoryId = 2, 
                ParentTopicId = 90, 
                GradeLevel = 11 
            },
            
            new MathTopic { 
                Id = 93, 
                Name = "Polyhedron Diagonals and Section with a Plane", 
                Description = "Understanding polyhedron geometry and cross-sections", 
                CategoryId = 2, 
                ParentTopicId = 90, 
                GradeLevel = 11 
            },
            
            new MathTopic { 
                Id = 94, 
                Name = "Prism Surface and Volume", 
                Description = "Calculating surface area and volume of prisms", 
                CategoryId = 2, 
                ParentTopicId = 90, 
                GradeLevel = 11 
            },
            
            new MathTopic { 
                Id = 95, 
                Name = "Regular Triangular Pyramid", 
                Description = "Properties and calculations for regular triangular pyramids", 
                CategoryId = 2, 
                ParentTopicId = 90, 
                GradeLevel = 11 
            },
            
            new MathTopic { 
                Id = 96, 
                Name = "Regular Quadrangular and Hexagonal Pyramid", 
                Description = "Properties and calculations for regular quadrangular and hexagonal pyramids", 
                CategoryId = 2, 
                ParentTopicId = 90, 
                GradeLevel = 11 
            },
            
            new MathTopic { 
                Id = 97, 
                Name = "Irregular Pyramid", 
                Description = "Properties and calculations for irregular pyramids", 
                CategoryId = 2, 
                ParentTopicId = 90, 
                GradeLevel = 11 
            }
        );

        // Child topics for Rotational Bodies
        builder.Entity<MathTopic>().HasData(
            new MathTopic { 
                Id = 101, 
                Name = "Cylinder", 
                Description = "Properties and calculations for cylinders", 
                CategoryId = 2, 
                ParentTopicId = 100, 
                GradeLevel = 12 
            },
            
            new MathTopic { 
                Id = 102, 
                Name = "Cone", 
                Description = "Properties and calculations for cones", 
                CategoryId = 2, 
                ParentTopicId = 100, 
                GradeLevel = 12 
            },
            
            new MathTopic { 
                Id = 103, 
                Name = "Sphere", 
                Description = "Properties and calculations for spheres", 
                CategoryId = 2, 
                ParentTopicId = 100, 
                GradeLevel = 12 
            },
            
            new MathTopic { 
                Id = 104, 
                Name = "Cylinder and Prism Geometric Combinations", 
                Description = "Working with geometric combinations of cylinders and prisms", 
                CategoryId = 2, 
                ParentTopicId = 100, 
                GradeLevel = 12 
            },
            
            new MathTopic { 
                Id = 105, 
                Name = "Sphere and Prism Geometric Combinations", 
                Description = "Working with geometric combinations of spheres and prisms", 
                CategoryId = 2, 
                ParentTopicId = 100, 
                GradeLevel = 12 
            }
        );

        // Add standard algebra topics as top-level topics (no parent)
        builder.Entity<MathTopic>().HasData(
            new MathTopic { 
                Id = 110, 
                Name = "Linear Equations", 
                Description = "Solve equations in the form ax + b = c", 
                CategoryId = 1, 
                ParentTopicId = null, 
                GradeLevel = 7 
            },
            
            new MathTopic { 
                Id = 111, 
                Name = "Quadratic Equations", 
                Description = "Solve equations in the form ax² + bx + c = 0", 
                CategoryId = 1, 
                ParentTopicId = null, 
                GradeLevel = 8 
            },
            
            new MathTopic { 
                Id = 112, 
                Name = "Systems of Equations", 
                Description = "Solve multiple equations with multiple variables", 
                CategoryId = 1, 
                ParentTopicId = null, 
                GradeLevel = 9 
            },
            
            new MathTopic { 
                Id = 113, 
                Name = "Inequalities", 
                Description = "Solve and graph inequalities", 
                CategoryId = 1, 
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
            // Linear Equations (Topic ID: 110)
            new MathProblem { 
                Id = 1, 
                TopicId = 110, 
                Statement = "Solve for x: 2x + 3 = 11", 
                Solution = "x = 4", 
                Explanation = "To solve this equation, subtract 3 from both sides: 2x = 8. Then divide both sides by 2: x = 4.", 
                Difficulty = MathTutor.Core.Enums.DifficultyLevel.Easy,
                PointValue = 1
            },
            new MathProblem { 
                Id = 2, 
                TopicId = 110, 
                Statement = "Solve for y: 3y - 7 = 14", 
                Solution = "y = 7", 
                Explanation = "Add 7 to both sides: 3y = 21. Then divide both sides by 3: y = 7.", 
                Difficulty = MathTutor.Core.Enums.DifficultyLevel.Easy,
                PointValue = 1
            },
            new MathProblem { 
                Id = 3, 
                TopicId = 110, 
                Statement = "Solve for x: 5x - 8 = 2x + 13", 
                Solution = "x = 7", 
                Explanation = "Subtract 2x from both sides: 3x - 8 = 13. Add 8 to both sides: 3x = 21. Divide both sides by 3: x = 7.", 
                Difficulty = MathTutor.Core.Enums.DifficultyLevel.Medium,
                PointValue = 2
            },
            
            // Quadratic Equations (Topic ID: 111)
            new MathProblem { 
                Id = 4, 
                TopicId = 111, 
                Statement = "Solve for x: x² - 9 = 0", 
                Solution = "x = 3 or x = -3", 
                Explanation = "This is a difference of squares: x² - 9 = (x+3)(x-3) = 0. Therefore x = 3 or x = -3.", 
                Difficulty = MathTutor.Core.Enums.DifficultyLevel.Easy,
                PointValue = 1
            },
            new MathProblem { 
                Id = 5, 
                TopicId = 111, 
                Statement = "Solve for x: x² + 6x + 9 = 0", 
                Solution = "x = -3", 
                Explanation = "This is a perfect square trinomial: x² + 6x + 9 = (x+3)² = 0. Therefore x = -3.", 
                Difficulty = MathTutor.Core.Enums.DifficultyLevel.Medium,
                PointValue = 2
            },
            new MathProblem { 
                Id = 6, 
                TopicId = 111, 
                Statement = "Solve for x: 2x² - 5x - 3 = 0", 
                Solution = "x = 3 or x = -1/2", 
                Explanation = "Use the quadratic formula: x = [-b ± √(b² - 4ac)]/2a with a=2, b=-5, c=-3. This gives x = 3 or x = -1/2.", 
                Difficulty = MathTutor.Core.Enums.DifficultyLevel.Hard,
                PointValue = 3
            },
            
            // Systems of Equations (Topic ID: 112)
            new MathProblem { 
                Id = 7, 
                TopicId = 112, 
                Statement = "Solve the system: x + y = 7, x - y = 3", 
                Solution = "x = 5, y = 2", 
                Explanation = "Add the equations: 2x = 10, so x = 5. Substitute back: 5 + y = 7, so y = 2.", 
                Difficulty = MathTutor.Core.Enums.DifficultyLevel.Medium,
                PointValue = 2
            },
            new MathProblem { 
                Id = 8, 
                TopicId = 112, 
                Statement = "Solve the system: 2x + 3y = 12, 4x - y = 5", 
                Solution = "x = 2, y = 2.67", 
                Explanation = "Multiply the second equation by 3: 12x - 3y = 15. Add to first equation: 14x = 27, so x = 27/14 = 1.93. Substitute back to find y.", 
                Difficulty = MathTutor.Core.Enums.DifficultyLevel.Hard,
                PointValue = 3
            },
            
            // Inequalities (Topic ID: 113)
            new MathProblem { 
                Id = 9, 
                TopicId = 113, 
                Statement = "Solve for x: 2x - 5 > 7", 
                Solution = "x > 6", 
                Explanation = "Add 5 to both sides: 2x > 12. Divide both sides by 2: x > 6.", 
                Difficulty = MathTutor.Core.Enums.DifficultyLevel.Easy,
                PointValue = 1
            },
            new MathProblem { 
                Id = 10, 
                TopicId = 113, 
                Statement = "Solve for x: -3x + 2 ≤ -7", 
                Solution = "x ≥ 3", 
                Explanation = "Subtract 2 from both sides: -3x ≤ -9. Divide both sides by -3 (and reverse the inequality): x ≥ 3.", 
                Difficulty = MathTutor.Core.Enums.DifficultyLevel.Medium,
                PointValue = 2
            },
            
            // Vectors (Topic ID: 1 - Vectors and Motion)
            new MathProblem { 
                Id = 11, 
                TopicId = 1, 
                Statement = "Find the magnitude of vector v = (3, 4).", 
                Solution = "5", 
                Explanation = "Use the Pythagorean theorem: |v| = √(3² + 4²) = √(9 + 16) = √25 = 5.", 
                Difficulty = MathTutor.Core.Enums.DifficultyLevel.Easy,
                PointValue = 1
            },
            new MathProblem { 
                Id = 12, 
                TopicId = 2, 
                Statement = "Find the unit vector in the direction of v = (6, 8).", 
                Solution = "(0.6, 0.8)", 
                Explanation = "First find |v| = √(6² + 8²) = √(36 + 64) = √100 = 10. Then v/|v| = (6/10, 8/10) = (0.6, 0.8).", 
                Difficulty = MathTutor.Core.Enums.DifficultyLevel.Medium,
                PointValue = 2
            },
            
            // Combinatorics (Topic ID: 20 - Combinatorics and Probability I)
            new MathProblem { 
                Id = 13, 
                TopicId = 20, 
                Statement = "How many ways can you arrange the letters in the word 'MATH'?", 
                Solution = "24", 
                Explanation = "This is a permutation of 4 distinct objects, which is 4! = 4 × 3 × 2 × 1 = 24.", 
                Difficulty = MathTutor.Core.Enums.DifficultyLevel.Easy,
                PointValue = 1
            },
            new MathProblem { 
                Id = 14, 
                TopicId = 23, 
                Statement = "How many 3-digit numbers can be formed using digits 1 to 5 if no repetition is allowed?", 
                Solution = "60", 
                Explanation = "For the first position, we have 5 choices. For the second, 4 choices. For the third, 3 choices. Total: 5 × 4 × 3 = 60.", 
                Difficulty = MathTutor.Core.Enums.DifficultyLevel.Medium,
                PointValue = 2
            },
            
            // Statistics (Topic ID: 30 - Statistics I)
            new MathProblem { 
                Id = 15, 
                TopicId = 30, 
                Statement = "Find the mean of the data set: 4, 7, 9, 3, 8, 5.", 
                Solution = "6", 
                Explanation = "Add all values and divide by the number of items: (4 + 7 + 9 + 3 + 8 + 5) ÷ 6 = 36 ÷ 6 = 6.", 
                Difficulty = MathTutor.Core.Enums.DifficultyLevel.Easy,
                PointValue = 1
            },
            new MathProblem { 
                Id = 16, 
                TopicId = 31, 
                Statement = "Find the standard deviation of the data set: 2, 4, 6, 8, 10.", 
                Solution = "3.16", 
                Explanation = "First find the mean: (2 + 4 + 6 + 8 + 10) ÷ 5 = 30 ÷ 5 = 6. Then find the variance: [(2-6)² + (4-6)² + (6-6)² + (8-6)² + (10-6)²] ÷ 5 = [16 + 4 + 0 + 4 + 16] ÷ 5 = 40 ÷ 5 = 8. Standard deviation = √8 ≈ 2.83.", 
                Difficulty = MathTutor.Core.Enums.DifficultyLevel.Hard,
                PointValue = 3
            },
            
            // Algebraic Fractions (Topic ID: 40)
            new MathProblem { 
                Id = 17, 
                TopicId = 40, 
                Statement = "Simplify: (x² - 4) ÷ (x - 2)", 
                Solution = "x + 2", 
                Explanation = "Factor the numerator: (x² - 4) ÷ (x - 2) = [(x - 2)(x + 2)] ÷ (x - 2) = x + 2.", 
                Difficulty = MathTutor.Core.Enums.DifficultyLevel.Medium,
                PointValue = 2
            },
            new MathProblem { 
                Id = 18, 
                TopicId = 49, 
                Statement = "Solve for x: (x + 1)/(x - 2) = 3", 
                Solution = "x = 7", 
                Explanation = "Cross multiply: x + 1 = 3(x - 2). Expand: x + 1 = 3x - 6. Subtract 3x from both sides: -2x + 1 = -6. Subtract 1 from both sides: -2x = -7. Divide by -2: x = 7/2.", 
                Difficulty = MathTutor.Core.Enums.DifficultyLevel.Hard,
                PointValue = 3
            },
            
            // Trigonometry (Topic ID: 60 - Sine and Cosine Functions)
            new MathProblem { 
                Id = 19, 
                TopicId = 60, 
                Statement = "Find the value of sin(30°).", 
                Solution = "0.5", 
                Explanation = "sin(30°) = 1/2 = 0.5", 
                Difficulty = MathTutor.Core.Enums.DifficultyLevel.Easy,
                PointValue = 1
            },
            new MathProblem { 
                Id = 20, 
                TopicId = 64, 
                Statement = "Solve for x in the equation: sin(x) = cos(x).", 
                Solution = "x = π/4 + nπ", 
                Explanation = "When sin(x) = cos(x), then sin(x)/cos(x) = 1, so tan(x) = 1. This occurs when x = π/4 + nπ, where n is an integer.", 
                Difficulty = MathTutor.Core.Enums.DifficultyLevel.Hard,
                PointValue = 3
            },
            
            // Exponents (Topic ID: 70)
            new MathProblem { 
                Id = 21, 
                TopicId = 70, 
                Statement = "Simplify: (x^3)^2 × x^4", 
                Solution = "x^10", 
                Explanation = "(x^3)^2 = x^6, so x^6 × x^4 = x^10.", 
                Difficulty = MathTutor.Core.Enums.DifficultyLevel.Medium,
                PointValue = 2
            },
            new MathProblem { 
                Id = 22, 
                TopicId = 72, 
                Statement = "Convert to a single expression with a rational exponent: ∛(x^5)", 
                Solution = "x^(5/3)", 
                Explanation = "∛(x^5) = (x^5)^(1/3) = x^(5/3).", 
                Difficulty = MathTutor.Core.Enums.DifficultyLevel.Medium,
                PointValue = 2
            },
            
            // Exponential Functions (Topic ID: 80)
            new MathProblem { 
                Id = 23, 
                TopicId = 80, 
                Statement = "Solve for x: 2^x = 8", 
                Solution = "x = 3", 
                Explanation = "We can rewrite 8 as 2^3. So, 2^x = 2^3, which means x = 3.", 
                Difficulty = MathTutor.Core.Enums.DifficultyLevel.Easy,
                PointValue = 1
            },
            new MathProblem { 
                Id = 24, 
                TopicId = 82, 
                Statement = "Simplify: log₄(64)", 
                Solution = "3", 
                Explanation = "log₄(64) = log₄(4^3) = 3", 
                Difficulty = MathTutor.Core.Enums.DifficultyLevel.Medium,
                PointValue = 2
            },
            
            // Geometry (Topic ID: 90 - Lines and Planes in Space)
            new MathProblem { 
                Id = 25, 
                TopicId = 90, 
                Statement = "Find the distance between points (1, 2, 3) and (4, 6, 8).", 
                Solution = "√50 ≈ 7.07", 
                Explanation = "Use the 3D distance formula: d = √[(4-1)² + (6-2)² + (8-3)²] = √[9 + 16 + 25] = √50 ≈ 7.07.", 
                Difficulty = MathTutor.Core.Enums.DifficultyLevel.Medium,
                PointValue = 2
            },
            new MathProblem { 
                Id = 26, 
                TopicId = 94, 
                Statement = "Find the volume of a rectangular prism with dimensions 3 cm × 4 cm × 5 cm.", 
                Solution = "60 cm³", 
                Explanation = "Volume = length × width × height = 3 cm × 4 cm × 5 cm = 60 cm³.", 
                Difficulty = MathTutor.Core.Enums.DifficultyLevel.Easy,
                PointValue = 1
            },
            
            // Rotational Bodies (Topic ID: 100)
            new MathProblem { 
                Id = 27, 
                TopicId = 100, 
                Statement = "Find the volume of a sphere with radius 3 cm.", 
                Solution = "113.1 cm³", 
                Explanation = "Volume of a sphere = (4/3) × π × r³ = (4/3) × π × 3³ = (4/3) × π × 27 ≈ 113.1 cm³.", 
                Difficulty = MathTutor.Core.Enums.DifficultyLevel.Medium,
                PointValue = 2
            },
            new MathProblem { 
                Id = 28, 
                TopicId = 102, 
                Statement = "Find the volume of a cone with radius 4 cm and height 9 cm.", 
                Solution = "150.8 cm³", 
                Explanation = "Volume of a cone = (1/3) × π × r² × h = (1/3) × π × 4² × 9 = (1/3) × π × 16 × 9 = 48π ≈ 150.8 cm³.", 
                Difficulty = MathTutor.Core.Enums.DifficultyLevel.Medium,
                PointValue = 2
            },
            
            // Additional algebra problems
            new MathProblem { 
                Id = 29, 
                TopicId = 110, 
                Statement = "Solve for x: 7x - 2 = 4x + 10", 
                Solution = "x = 4", 
                Explanation = "Subtract 4x from both sides: 3x - 2 = 10. Add 2 to both sides: 3x = 12. Divide by 3: x = 4.", 
                Difficulty = MathTutor.Core.Enums.DifficultyLevel.Medium,
                PointValue = 2
            },
            new MathProblem { 
                Id = 30, 
                TopicId = 111, 
                Statement = "Find the solutions to x² - 7x + 12 = 0.", 
                Solution = "x = 3 or x = 4", 
                Explanation = "Factor the expression: x² - 7x + 12 = (x - 3)(x - 4) = 0. Therefore, x = 3 or x = 4.", 
                Difficulty = MathTutor.Core.Enums.DifficultyLevel.Medium,
                PointValue = 2
            }
        );
    }
} 