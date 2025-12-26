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

    [Column("QuestionFileUrl")]
    public string? QuestionFileUrl { get; set; }

    [Column("MediaUrl")]
    public string? MediaUrl { get; set; } 

    [Column("Description")]
    public string? Description { get; set; }

    // Store actual questions as JSON
    [Column("Questions", TypeName = "nvarchar(max)")]
    public string? Questions { get; set; }

    [ForeignKey("LessonID")]
    [InverseProperty("Quizzes")]
    public virtual Lesson Lesson { get; set; } = null!;

    [InverseProperty("Quiz")]
    public virtual ICollection<QuizResult> QuizResults { get; set; } = new List<QuizResult>();
}
