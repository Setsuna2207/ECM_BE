using ECM_BE.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECM_BE.Data;

public partial class AppDbContext : IdentityDbContext<User>
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<UserGoal> UserGoals { get; set; }
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Course> Courses { get; set; }
    public virtual DbSet<Lesson> Lessons { get; set; }
    public virtual DbSet<Following> Followings { get; set; }
    public virtual DbSet<History> Histories { get; set; }
    public virtual DbSet<Review> Reviews { get; set; }
    public virtual DbSet<Quiz> Quizzes { get; set; }
    public virtual DbSet<QuizResult> QuizResults { get; set; }
    public virtual DbSet<PlacementTest> PlacementTests { get; set; }
    public virtual DbSet<TestResult> TestResults { get; set; }
    public virtual DbSet<AIFeedback> AIFeedbacks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Role Seeding
        var roles = new List<IdentityRole>
        {
            new IdentityRole { Id = "Admin", Name = "Admin", NormalizedName = "ADMIN" },
            new IdentityRole { Id = "User", Name = "User", NormalizedName = "USER" },
        };
        modelBuilder.Entity<IdentityRole>().HasData(roles);

        // Category Configuration
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Categories");
            entity.HasKey(e => e.CategoryID);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Description)
                .HasColumnType("nvarchar(max)");
        });

        // Course Configuration
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseID);

            // Many-to-Many: Course <-> Category
            entity.HasMany(c => c.Categories)
                .WithMany(cat => cat.Courses)
                .UsingEntity<Dictionary<string, object>>(
                    "CoursesCategories",
                    j => j
                        .HasOne<Category>()
                        .WithMany()
                        .HasForeignKey("CategoryID")
                        .HasConstraintName("FK_CoursesCategories_Categories")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<Course>()
                        .WithMany()
                        .HasForeignKey("CourseID")
                        .HasConstraintName("FK_CoursesCategories_Courses")
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("CourseID", "CategoryID");
                        j.ToTable("CoursesCategories");
                    });
        });

        // Lesson Configuration
        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.HasKey(e => e.LessonID);

            // 1-n: Course -> Lessons
            entity.HasOne(e => e.Course)
                .WithMany(c => c.Lessons)
                .HasForeignKey(e => e.CourseID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Lessons_Courses");

            // 1-n: Lesson -> Quizzes
            entity.HasMany(e => e.Quizzes)
                .WithOne(q => q.Lesson)
                .HasForeignKey(q => q.LessonID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Quizzes_Lessons");
        });

        // Quiz Configuration
        modelBuilder.Entity<Quiz>(entity =>
        {
            entity.HasKey(e => e.QuizID);
            entity.Property(e => e.QuestionFileUrl)
                .HasColumnType("nvarchar(max)");
            entity.Property(e => e.MediaUrl)
                .HasColumnType("nvarchar(max)");
            entity.Property(e => e.Description)
                .HasColumnType("nvarchar(max)");

            // n-1: Quiz -> Lesson
            entity.HasOne(e => e.Lesson)
                .WithMany(l => l.Quizzes)
                .HasForeignKey(e => e.LessonID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Quizzes_Lessons");
        });

        // QuizResult Configuration
        modelBuilder.Entity<QuizResult>(entity =>
        {
            entity.HasKey(e => e.ResultID);
            entity.Property(e => e.UserAnswers)
                .HasColumnType("nvarchar(max)");
            entity.Property(e => e.Score)
                .HasColumnType("float");
            entity.Property(e => e.SubmittedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()");

            // n-1: QuizResult -> Quiz
            entity.HasOne(e => e.Quiz)
                .WithMany(q => q.QuizResults)
                .HasForeignKey(e => e.QuizID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_QuizResults_Quizzes");

            // n-1: QuizResult -> User
            entity.HasOne(e => e.User)
                .WithMany(u => u.QuizResults)
                .HasForeignKey(e => e.userID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_QuizResults_Users");
        });

        // Following Configuration
        modelBuilder.Entity<Following>(entity =>
        {
            entity.HasKey(e => e.FollowingID);
            entity.Property(e => e.FollowedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()");

            // n-1: Following -> User
            entity.HasOne(d => d.User)
                .WithMany(p => p.Followings)
                .HasForeignKey(d => d.userID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Followings_Users");

            // n-1: Following -> Course
            entity.HasOne(d => d.Course)
                .WithMany(p => p.Followings)
                .HasForeignKey(d => d.CourseID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Followings_Courses");
        });

        // History Configuration
        modelBuilder.Entity<History>(entity =>
        {
            entity.HasKey(e => e.HistoryID);
            entity.Property(e => e.Progress)
                .HasColumnType("float")
                .HasDefaultValue(0f);
            entity.Property(e => e.LastAccessed)
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()");

            // n-1: History -> User
            entity.HasOne(d => d.User)
                .WithMany(p => p.Histories)
                .HasForeignKey(d => d.userID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Histories_Users");

            // n-1: History -> Course
            entity.HasOne(d => d.Course)
                .WithMany(p => p.Histories)
                .HasForeignKey(d => d.CourseID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Histories_Courses");
        });

        // Review Configuration
        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => new { e.userID, e.CourseID });
            entity.Property(e => e.ReviewScore)
                .HasColumnType("int");
            entity.Property(e => e.ReviewContent)
                .HasColumnType("nvarchar(max)");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()");

            // n-1: Review -> User
            entity.HasOne(d => d.User)
                .WithMany(p => p.Reviews)
                .HasForeignKey(d => d.userID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Reviews_Users");

            // n-1: Review -> Course
            entity.HasOne(d => d.Course)
                .WithMany(p => p.Reviews)
                .HasForeignKey(d => d.CourseID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Reviews_Courses");
        });

        // PlacementTest Configuration
        modelBuilder.Entity<PlacementTest>(entity =>
        {
            entity.HasKey(e => e.TestID);
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.Description)
                .HasColumnType("nvarchar(max)");
            entity.Property(e => e.Duration)
                .HasColumnName("Duration");
            entity.Property(e => e.TotalQuestions)
                .HasColumnName("TotalQuestions");
            entity.Property(e => e.QuestionFileURL)
                .IsRequired()
                .HasColumnType("nvarchar(max)");
            entity.Property(e => e.MediaURL)
                .IsRequired()
                .HasColumnType("nvarchar(max)");
        });

        // TestResult Configuration
        modelBuilder.Entity<TestResult>(entity =>
        {
            entity.HasKey(e => e.ResultID);
            entity.Property(e => e.UserAnswers)
                .IsRequired()
                .HasColumnType("nvarchar(max)");
            entity.Property(e => e.SectionScores)
                .HasColumnType("nvarchar(max)");
            entity.Property(e => e.LevelDetected)
                .HasMaxLength(20);
            entity.Property(e => e.OverallScore)
                .HasColumnType("float");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()");

            // n-1: TestResult -> PlacementTest
            entity.HasOne(e => e.PlacementTest)
                .WithMany(pt => pt.TestResults)
                .HasForeignKey(e => e.TestID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_TestResults_PlacementTests");

            // n-1: TestResult -> User
            entity.HasOne(e => e.User)
                .WithMany(u => u.TestResults)
                .HasForeignKey(e => e.userID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_TestResults_Users");

            // 1-1: TestResult <-> AIFeedback
            entity.HasOne(tr => tr.AIFeedback)
                .WithOne(af => af.TestResult)
                .HasForeignKey<AIFeedback>(af => af.ResultID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_AIFeedbacks_TestResults");
        });

        // AIFeedback Configuration
        modelBuilder.Entity<AIFeedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackID);
            entity.Property(e => e.WeakSkill)
                .HasMaxLength(50);
            entity.Property(e => e.RcmCourses)
                .HasColumnType("nvarchar(max)");
            entity.Property(e => e.FeedbackSummary)
                .HasColumnType("nvarchar(max)");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()");

            // 1-1: AIFeedback <-> TestResult
            entity.HasOne(e => e.TestResult)
                .WithOne(tr => tr.AIFeedback)
                .HasForeignKey<AIFeedback>(e => e.ResultID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_AIFeedbacks_TestResults");
        });

        // UserGoal Configuration
        modelBuilder.Entity<UserGoal>(entity =>
        {
            entity.HasKey(e => e.UserGoalID);
            entity.Property(e => e.Content)
                .IsRequired()
                .HasColumnType("nvarchar(max)");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()");

            // n-1: UserGoal -> User
            entity.HasOne(e => e.User)
                .WithMany(u => u.UserGoals)
                .HasForeignKey(e => e.userID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_UserGoals_Users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

