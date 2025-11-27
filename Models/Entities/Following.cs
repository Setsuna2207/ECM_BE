using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECM_BE.Models.Entities;

[Table("Following")]
public partial class Following
{
    [Column("FollowingID")]
    public int FollowingID { get; set; }

    [Column("userID")]
    public string userID { get; set; } = null!;

    [Column("CourseID")]
    public int CourseID { get; set; }

    [Column("FollowedAt")]
    public DateTime FollowedAt { get; set; }

    [ForeignKey("userID")]
    [InverseProperty("Followings")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("CourseID")]
    [InverseProperty("Followings")]
    public virtual Course Course { get; set; } = null!;
}
