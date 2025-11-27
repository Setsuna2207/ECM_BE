using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECM_BE.Models.Entities;

[Table("Lessons")]
public partial class Lesson
{
    [Column("LessonID")]
    public int LessonID { get; set; }

    [Column("CourseID")]
    public int CourseID { get; set; }

    [Column("Title")]
    public string? Title { get; set; } 

    [Column("VideoUrl")]
    public string VideoUrl { get; set; } = null!;

    [Column("DocumentUrl")]
    public string DocumentUrl { get; set; } = null!;

    [Column("OrderIndex")]
    public int? OrderIndex { get; set; }

    [ForeignKey("CourseID")]
    [InverseProperty("Lessons")]
    public virtual Course Course { get; set; } = null!;

    [InverseProperty("Lesson")]
    public virtual ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
}
