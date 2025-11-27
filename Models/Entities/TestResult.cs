using System;
using System.Collections.Generic;
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
    [Column("GrammarScore")]
    public float GrammarScore { get; set; }
    [Column("VocabularyScore")]
    public float VocabularyScore { get; set; }
    [Column("ListeningScore")]
    public float ListeningScore { get; set; }
    [Column("ReadingScore")]
    public float ReadingScore { get; set; }
    [Column("WritingScore")]
    public float WritingScore { get; set; }
    [Column("OverallScore")]
    public float OverallScore { get; set; }
    [Column("LevelDetected")]
    public string? LevelDetected { get; set; }
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
