using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECM_BE.Models.Entities;

[Table("PlacementTests")]
public partial class PlacementTest
{
    [Column("TestID")]
    public int TestID { get; set; }

    [Column("Title")]
    public string Title { get; set; } = null!;

    [Column("Description")]
    public string? Description { get; set; }

    [Column("Duration")]
    public int Duration { get; set; }

    [Column("TotalQuestions")]
    public int TotalQuestions { get; set; }

    [Column("QuestionFileURL")]
    public string QuestionFileURL { get; set; } = null!;

    [Column("MediaURL")]
    public string MediaURL { get; set; } = null!;

    [InverseProperty("PlacementTest")]
    public virtual ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();
}