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

    // Lưu lựa chọn hoặc câu trả lời của người dùng (JSON, text, v.v.)
    [Column("UserAnswers")]
    public string? UserAnswers { get; set; }

    // Điểm hoặc % đạt được (ví dụ: 8.5 / 10, hoặc 85%)
    [Column("Score")]
    public float? Score { get; set; }

    // Tổng số câu hỏi (để tính % nếu cần)
    [Column("TotalQuestions")]
    public int? TotalQuestions { get; set; }

    // Thời điểm nộp bài
    [Column("SubmittedAt")]
    public DateTime? SubmittedAt { get; set; }

    // === Quan hệ ===
    [ForeignKey("QuizID")]
    [InverseProperty("QuizResults")]
    public virtual Quiz Quiz { get; set; } = null!;

    [ForeignKey("userID")]
    [InverseProperty("QuizResults")]
    public virtual User User { get; set; } = null!;
}
