using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECM_BE.Models.Entities;

[Table("TestResults")]
public partial class TestResult
{
    [Column("ResultID")]
    public int ResultID { get; set; }

    [Column("TestID")]
    public int TestID { get; set; }

    [Column("userID")]
    public string userID { get; set; } = null!;

    [Column("UserAnswers")]
    public string UserAnswers { get; set; } = null!; // Store JSON: {"1": 0, "2": 1, "3": 2, ...}

    [Column("CorrectAnswers")]
    public int CorrectAnswers { get; set; }

    [Column("IncorrectAnswers")]
    public int IncorrectAnswers { get; set; }

    [Column("SkippedAnswers")]
    public int SkippedAnswers { get; set; }

    [Column("OverallScore")]
    public float OverallScore { get; set; } // Percentage or total points

    [Column("SectionScores")]
    public string? SectionScores { get; set; } // Store JSON: {"listening": 80, "reading": 75, "grammar": 90, ...}

    [Column("LevelDetected")]
    public string? LevelDetected { get; set; } // e.g., "Beginner", "Intermediate", "Advanced"

    [Column("TimeSpent")]
    public int? TimeSpent { get; set; }

    [Column("CreatedAt")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("userID")]
    [InverseProperty("TestResults")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("TestID")]
    [InverseProperty("TestResults")]
    public virtual PlacementTest PlacementTest { get; set; } = null!;

    [InverseProperty(nameof(AIFeedback.TestResult))]
    public virtual AIFeedback? AIFeedback { get; set; }
}