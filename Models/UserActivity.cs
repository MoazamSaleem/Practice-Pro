using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Practice_Pro.Models
{
    public class UserActivity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } 
        public IdentityUser User { get; set; }

        public string CurrentPage { get; set; }

        public DateTime LastLogin { get; set; }

        public int LoginCountThisMonth { get; set; }

        public DateTime? LastActivityTime { get; set; } 
    }
}
