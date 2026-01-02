using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECM_BE.Models.Entities;

[Table("AIRcms")]
public partial class AIRcm
{
    [Key]
    [Column("RcmID")]
    public int RcmID { get; set; }

    [Column("userID")]
    public string userID { get; set; } = null!;

    [Column("RcmCourses")]
    public string RcmCourses { get; set; } = null!; // JSON: [{"courseId": 6, "reason": "..."}]

    [Column("CreatedAt")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("userID")]
    [InverseProperty("AIRcms")]
    public virtual User User { get; set; } = null!;
}
