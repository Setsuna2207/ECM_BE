using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECM_BE.Models.Entities;

[Table("PlacementTests")]
public partial class PlacementTest
{
    [Column("TestID")]
    public int TestID { get; set; }

    [Column("TestType")]
    public string TestType { get; set; } = null!;

    [Column("Title")]
    public string? Title { get; set; } 

    [Column("Description")]
    public string Description { get; set; } = null!;

    [Column("SkillsIncluded")]
    public string SkillsIncluded { get; set; } = null!;

    [Column("CreatedAt")]
    public DateTime? CreatedAt { get; set; }

    [InverseProperty("PlacementTest")]
    public virtual ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();
}