using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECM_BE.Models.Entities;

[Table("Courses")]
public partial class Course
{
    [Column("CourseID")]
    public int CourseID { get; set; }
    [Column("Title")]
    public string? Title { get; set; }
    [Column("Description")]
    public string Description { get; set; } = null!;
    [Column("ThumbnailUrl")]
    public string ThumbnailUrl { get; set; } = null!;
    [Column("CreatedAt")]
    public DateTime CreatedAt { get; set; }

    [InverseProperty("Course")]
    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    [InverseProperty("Course")]
    public virtual ICollection<Following> Followings { get; set; } = new List<Following>();
    [InverseProperty("Course")]
    public virtual ICollection<History> Histories { get; set; } = new List<History>();
    [InverseProperty("Course")]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

}
