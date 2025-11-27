using ECM_BE.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Quizzes")]
public partial class Quiz
{
    [Key]
    [Column("QuizID")]
    public int QuizID { get; set; }

    [Column("LessonID")]
    public int LessonID { get; set; }

    // File hoặc nội dung văn bản chứa nhiều câu hỏi
    [Column("QuestionFileUrl")]
    public string? QuestionFileUrl { get; set; }  // ví dụ file JSON, text, docx...

    // Nếu là bài nghe
    [Column("MediaUrl")]
    public string? MediaUrl { get; set; }         // link audio hoặc video

    // (Tùy chọn) Mô tả bài quiz
    [Column("Description")]
    public string? Description { get; set; }

    [ForeignKey("LessonID")]
    [InverseProperty("Quizzes")]
    public virtual Lesson Lesson { get; set; } = null!;

    [InverseProperty("Quiz")]
    public virtual ICollection<QuizResult> QuizResults { get; set; } = new List<QuizResult>();
}
