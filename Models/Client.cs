using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.AspNetCore.Identity;

namespace Practice_App.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string AccountsD { get; set; } 
        public string CS01D { get; set; }
        public string IdentityVerificationD { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public int VatNum { get; set; }
        public string Quaters { get; set; }
        public string VatReturnDue { get; set; }

        // Identity Relationship
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
    }
}