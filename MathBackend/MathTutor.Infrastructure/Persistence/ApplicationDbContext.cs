using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MathTutor.Core.Entities;

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
    }
} 