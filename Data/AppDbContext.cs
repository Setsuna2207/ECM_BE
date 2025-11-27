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
    public virtual DbSet<User> User { get; set; }
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

        // phân quyền
        List<IdentityRole> roles = new List<IdentityRole>
        {
            new IdentityRole { Id = "Admin", Name = "Admin",  NormalizedName = "ADMIN" },
            new IdentityRole { Id = "User", Name = "User",  NormalizedName = "USER" },
        };
        modelBuilder.Entity<IdentityRole>().HasData(roles);

        // Category
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Categories");
            entity.HasKey(e => e.CategoryID).HasName("PK_Categories");
            entity.Property(e => e.CategoryID).HasColumnName("CategoryID");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("Name");

            entity.Property(e => e.Description)
                .HasColumnType("nvarchar(max)")
                .HasColumnName("Description");
        });

        // Course
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseID).HasName("PK_Courses");
            entity.HasMany(c => c.Categories)
                .WithMany(cat => cat.Courses)
                .UsingEntity<Dictionary<string, object>>("CoursesCategories",
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

        // Following
        modelBuilder.Entity<Following>(entity =>
        {
            entity.HasKey(e => e.FollowingID).HasName("PK_Followings");
            entity.HasOne(d => d.User)
                .WithMany(p => p.Followings)
                .HasForeignKey(d => d.userID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Followings_Users");
            entity.HasOne(d => d.Course)
                .WithMany(p => p.Followings)
                .HasForeignKey(d => d.CourseID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Followings_Courses");
        });


        // History
        modelBuilder.Entity<History>(entity =>
        {
            entity.HasKey(e => e.HistoryID).HasName("PK_Histories");
            entity.HasOne(d => d.User)
                .WithMany(p => p.Histories)
                .HasForeignKey(d => d.userID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Histories_Users");
            entity.HasOne(d => d.Course)
                .WithMany(p => p.Histories)
                .HasForeignKey(d => d.CourseID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Histories_Courses");
        });

        // Lesson 
        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.HasKey(e => e.LessonID).HasName("PK_Lessons");

            // 1–n: Course - Lessons
            entity.HasOne(e => e.Course)
                .WithMany(c => c.Lessons)
                .HasForeignKey(e => e.CourseID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Lessons_Courses");

            // 1–n: Lesson - Quizzes
            entity.HasMany(e => e.Quizzes)
                .WithOne(q => q.Lesson)
                .HasForeignKey(q => q.LessonID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Quizzes_Lessons");
        });

        // Review
        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => new { e.userID, e.CourseID }).HasName("PK_Reviews");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasOne(d => d.Course).WithMany(p => p.Reviews)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Reviews_Users");
            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reviews_Courses");
        });

        // Quiz
        modelBuilder.Entity<Quiz>(entity =>
        {
            entity.HasKey(e => e.QuizID).HasName("PK_Quizzes");

            entity.Property(e => e.QuizID).HasColumnName("QuizID");
            entity.Property(e => e.LessonID).HasColumnName("LessonID");

            // Thông tin quiz
            entity.Property(e => e.QuestionFileUrl)
                .HasColumnType("nvarchar(max)")
                .HasColumnName("QuestionFileUrl");

            entity.Property(e => e.MediaUrl)
                .HasColumnType("nvarchar(max)")
                .HasColumnName("MediaUrl");

            entity.Property(e => e.Description)
                .HasColumnType("nvarchar(max)")
                .HasColumnName("Description");

            // n-1: Quiz - Lesson
            entity.HasOne(e => e.Lesson)
                .WithMany(l => l.Quizzes)
                .HasForeignKey(e => e.LessonID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Quizzes_Lessons");
        });

        // QuizResult (đã bỏ SelectedOptionID, IsCorrect)
        modelBuilder.Entity<QuizResult>(entity =>
        {
            entity.HasKey(e => e.ResultID).HasName("PK_QuizResults");

            entity.Property(e => e.ResultID).HasColumnName("ResultID");
            entity.Property(e => e.QuizID).HasColumnName("QuizID");
            entity.Property(e => e.userID).HasColumnName("userID");

            // Dữ liệu lưu kết quả user
            entity.Property(e => e.UserAnswers)
                .HasColumnType("nvarchar(max)")
                .HasColumnName("UserAnswers");

            entity.Property(e => e.Score)
                .HasColumnType("float")
                .HasColumnName("Score");

            entity.Property(e => e.TotalQuestions)
                .HasColumnName("TotalQuestions");

            entity.Property(e => e.SubmittedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()")
                .HasColumnName("SubmittedAt");

            // 1-n: QuizResult - Quiz
            entity.HasOne(e => e.Quiz)
                .WithMany(q => q.QuizResults)
                .HasForeignKey(e => e.QuizID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_QuizResults_Quizzes");

            // 1-n: QuizResult - User
            entity.HasOne(e => e.User)
                .WithMany(u => u.QuizResults)
                .HasForeignKey(e => e.userID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_QuizResults_Users");
        });

        // PlacementTest
        modelBuilder.Entity<PlacementTest>(entity =>
        {
            entity.HasKey(e => e.TestID).HasName("PK_PlacementTests");
            entity.Property(e => e.TestID).HasColumnName("TestID");

            entity.Property(e => e.TestType)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("TestType");

            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("Title");

            entity.Property(e => e.Description)
                .HasColumnType("nvarchar(max)")
                .HasColumnName("Description");

            entity.Property(e => e.SkillsIncluded)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("SkillsIncluded");

            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()")
                .HasColumnName("CreatedAt");

            entity.HasMany(e => e.TestResults)
                .WithOne(tr => tr.PlacementTest)
                .HasForeignKey(tr => tr.TestID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_TestResults_PlacementTests");
        });


        // TestResult
        modelBuilder.Entity<TestResult>(entity =>
        {
            entity.HasKey(e => e.ResultID).HasName("PK_TestResults");
            entity.Property(e => e.ResultID).HasColumnName("ResultID");
            entity.Property(e => e.TestID).HasColumnName("TestID");
            entity.Property(e => e.userID).HasColumnName("userID");

            entity.Property(e => e.GrammarScore).HasColumnName("GrammarScore");
            entity.Property(e => e.VocabularyScore).HasColumnName("VocabularyScore");
            entity.Property(e => e.ListeningScore).HasColumnName("ListeningScore");
            entity.Property(e => e.ReadingScore).HasColumnName("ReadingScore");
            entity.Property(e => e.WritingScore).HasColumnName("WritingScore");
            entity.Property(e => e.LevelDetected)
                .HasMaxLength(20)
                .HasColumnName("LevelDetected");

            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()");

            // n-1: TestResult - PlacementTest
            entity.HasOne(e => e.PlacementTest)
                .WithMany(pt => pt.TestResults)
                .HasForeignKey(e => e.TestID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_TestResults_PlacementTests");

            // n-1: TestResult - User
            entity.HasOne(e => e.User)
                .WithMany(u => u.TestResults)
                .HasForeignKey(e => e.userID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_TestResults_Users");

            // 1-1: TestResult - AIFeedback
            entity.HasOne(tr => tr.AIFeedback)
                .WithOne(af => af.TestResult)
                .HasForeignKey<AIFeedback>(af => af.ResultID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_AIFeedbacks_TestResults");
        });

        // AIFeedback
        modelBuilder.Entity<AIFeedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackID).HasName("PK_AIFeedbacks");
            entity.Property(e => e.FeedbackID).HasColumnName("FeedbackID");
            entity.Property(e => e.ResultID).HasColumnName("ResultID");

            entity.Property(e => e.WeakSkill)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("WeakSkill");

            entity.Property(e => e.RcmCourses)
                .IsRequired()
                .HasColumnType("nvarchar(max)")
                .HasColumnName("RcmCourses");

            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()")
                .HasColumnName("CreatedAt");

            entity.HasOne(e => e.TestResult)
                .WithOne(tr => tr.AIFeedback)
                .HasForeignKey<AIFeedback>(e => e.ResultID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_AIFeedbacks_TestResults");
        });

        // UserGoal
        modelBuilder.Entity<UserGoal>(entity =>
        {
            entity.HasKey(e => e.UserGoalID).HasName("PK_UserGoals");
            entity.Property(e => e.UserGoalID).HasColumnName("UserGoalID");
            entity.Property(e => e.userID).HasColumnName("userID");
            entity.Property(e => e.CategoryID).HasColumnName("CategoryID");

            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()");

            // 1-n: User - UserGoal
            entity.HasOne(e => e.User)
                .WithMany(u => u.UserGoals)
                .HasForeignKey(e => e.userID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_UserGoals_Users");

            // n-n: UserGoal – Category
            entity.HasMany(ug => ug.Categories)
                .WithMany(c => c.UserGoals)
                .UsingEntity<Dictionary<string, object>>("UserGoalsCategories",
                    j => j
                        .HasOne<Category>()
                        .WithMany()
                        .HasForeignKey("CategoryID")
                        .HasConstraintName("FK_UserGoalsCategories_Categories")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<UserGoal>()
                        .WithMany()
                        .HasForeignKey("UserGoalID")
                        .HasConstraintName("FK_UserGoalsCategories_UserGoals")
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("UserGoalID", "CategoryID");
                        j.ToTable("UserGoalsCategories");
                    });
        });

    }
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

}

