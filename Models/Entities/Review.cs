using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ECM_BE.Models.Entities;

[Table("Reviews")]
public partial class Review
{
    [Column("userID")]
    public string userID { get; set; } = null!;

    [Column("CourseID")]
    public int CourseID { get; set; }

    [Column("ReviewScore")]
    public int ReviewScore { get; set; }

    [Column("ReviewContent")]
    public string? ReviewContent { get; set; }

    [Column("CreatedAt")]
    public DateTime? CreatedAt { get; set; }

    [ForeignKey("userID")]
    [InverseProperty("Reviews")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("CourseID")]
    [InverseProperty("Reviews")]
    public virtual Course Course { get; set; } = null!;
}
