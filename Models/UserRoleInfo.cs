using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Practice_Pro.Models
{
    public class UserRoleInfo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } // Foreign key from AspNetUsers

        [Required]
        public string RoleName { get; set; } // e.g. "Admin", "User", etc.

        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public IdentityUser User { get; set; } // ✅ linked to IdentityUser, not ApplicationUser
    }
}
