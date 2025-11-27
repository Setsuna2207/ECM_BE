using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ECM_BE.Models.Entities;

[Table("Categories")]
public partial class Category
{
    [Column("CategoryID")]
    public int CategoryID { get; set; }

    [Column("Name")]
    public string Name { get; set; } = null!;

    [Column("Description")]
    public string? Description { get; set; }

    [InverseProperty("Categories")]
    public virtual ICollection<UserGoal> UserGoals { get; set; } = new List<UserGoal>();
    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

}
