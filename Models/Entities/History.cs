using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECM_BE.Models.Entities;

[Table("History")]
public partial class History
{
    [Column("HistoryID")]
    public int HistoryID { get; set; }

    [Column("userID")]
    public string userID { get; set; } = null!;

    [Column("CourseID")]
    public int CourseID { get; set; }

    [Column("Progress")]
    public float Progress { get; set; }

    [Column("LastAccessed")]
    public DateTime LastAccessed { get; set; }

    [ForeignKey("userID")]
    [InverseProperty("Histories")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("CourseID")]
    [InverseProperty("Histories")]
    public virtual Course Course { get; set; } = null!;
}
