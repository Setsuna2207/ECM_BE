using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECM_BE.Models.Entities;

[Table("Users")]
public partial class User : IdentityUser
    {
        [Column("FullName")]
        public string? FullName { get; set; }

        [Column("Avatar")]
        public string? Avatar { get; set; }

        [Column("CreatedAt")]
        public DateTime? CreatedAt { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<UserGoal> UserGoals { get; set; } = new List<UserGoal>();

        [InverseProperty("User")]
        public virtual ICollection<Following> Followings { get; set; } = new List<Following>();

        [InverseProperty("User")]
        public virtual ICollection<History> Histories { get; set; } = new List<History>();

        [InverseProperty("User")]
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

        [InverseProperty("User")]
        public virtual ICollection<QuizResult> QuizResults { get; set; } = new List<QuizResult>();

        [InverseProperty("User")]
        public virtual ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();
    }

