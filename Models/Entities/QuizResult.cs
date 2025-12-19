using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECM_BE.Models.Entities;

[Table("QuizResults")]
public partial class QuizResult
{
    [Key]
    [Column("ResultID")]
    public int ResultID { get; set; }

    [Column("QuizID")]
    public int QuizID { get; set; }

    [Column("userID")]
    [Required]
    public string userID { get; set; } = null!;

    [Column("UserAnswers")]
    public string? UserAnswers { get; set; }

    [Column("Score")]
    public float? Score { get; set; }

    [Column("TotalQuestions")]
    public int? TotalQuestions { get; set; }

    [Column("SubmittedAt")]
    public DateTime? SubmittedAt { get; set; }

    [ForeignKey("QuizID")]
    [InverseProperty("QuizResults")]
    public virtual Quiz Quiz { get; set; } = null!;

    [ForeignKey("userID")]
    [InverseProperty("QuizResults")]
    public virtual User User { get; set; } = null!;
}
