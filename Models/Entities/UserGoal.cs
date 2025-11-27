using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECM_BE.Models.Entities;

[Table("UserGoals")]
public partial class UserGoal
{
    [Column("UserGoalID")]
    public int UserGoalID { get; set; }

    [Column("userID")]
    public string userID { get; set; } = null!;

    [Column("CategoryID")]
    public int CategoryID { get; set; }

    [Column("UpdatedAt")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("userID")]
    [InverseProperty("UserGoals")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("CategoryID")]
    [InverseProperty("UserGoals")]
    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
}
