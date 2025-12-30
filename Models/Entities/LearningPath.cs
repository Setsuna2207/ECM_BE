using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECM_BE.Models.Entities;

[Table("LearningPaths")]
public partial class LearningPath
{
    [Column("LearningPathID")]
    public int LearningPathID { get; set; }

    [Column("userID")]
    public string userID { get; set; } = null!;

    [Column("UserGoalID")]
    public int UserGoalID { get; set; }

    [Column("InitialTestID")]
    public int? InitialTestID { get; set; }

    [Column("InitialResultID")]
    public int? InitialResultID { get; set; }

    [Column("RecommendedCourses")]
    public string? RecommendedCourses { get; set; } // JSON: [1, 2, 3, ...]

    [Column("CompletedCourses")]
    public string? CompletedCourses { get; set; } // JSON: [1, 2, ...]

    [Column("FinalTestID")]
    public int? FinalTestID { get; set; }

    [Column("FinalResultID")]
    public int? FinalResultID { get; set; }

    [Column("Status")]
    [MaxLength(50)]
    public string Status { get; set; } = "GoalSet"; // GoalSet, TestRecommended, TestCompleted, CoursesRecommended, InProgress, ReadyForFinal, Completed

    [Column("CreatedAt")]
    public DateTime CreatedAt { get; set; }

    [Column("UpdatedAt")]
    public DateTime? UpdatedAt { get; set; }

    [Column("CompletedAt")]
    public DateTime? CompletedAt { get; set; }

    [ForeignKey("userID")]
    [InverseProperty("LearningPaths")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("UserGoalID")]
    [InverseProperty("LearningPaths")]
    public virtual UserGoal UserGoal { get; set; } = null!;

    [ForeignKey("InitialTestID")]
    public virtual PlacementTest? InitialTest { get; set; }

    [ForeignKey("FinalTestID")]
    public virtual PlacementTest? FinalTest { get; set; }

    [ForeignKey("InitialResultID")]
    public virtual TestResult? InitialResult { get; set; }

    [ForeignKey("FinalResultID")]
    public virtual TestResult? FinalResult { get; set; }
}
