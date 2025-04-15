using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MathTutor.Core.Entities;
using Microsoft.AspNetCore.Identity;
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
            new MathCategory { Id = 4, Name = "Statistics", Description = "Understand data analysis and probability" }
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
    }
} 