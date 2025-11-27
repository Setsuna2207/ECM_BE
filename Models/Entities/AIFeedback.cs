using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECM_BE.Models.Entities;

[Table("AIFeedback")]
public partial class AIFeedback
{
    [Column("FeedbackID")]
    public int FeedbackID { get; set; }

    [Column("ResultID")]
    public int ResultID { get; set; }

    [Column("WeakSkill")]
    public string? WeakSkill { get; set; }

    [Column("RcmCourses")]
    public string? RcmCourses { get; set; }

    [Column("CreatedAt")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey(nameof(ResultID))]
    [InverseProperty(nameof(TestResult.AIFeedback))]
    public virtual TestResult TestResult { get; set; } = null!;
}
