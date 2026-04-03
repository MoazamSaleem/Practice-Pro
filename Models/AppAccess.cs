using System;

namespace Practice_Pro.Models
{
    public static class AppAccess
    {
        public const string PrimaryAdminEmail = "info@premiumaccountants.co.uk";

        public static bool IsPrimaryAdmin(string email)
        {
            return !string.IsNullOrWhiteSpace(email) &&
                   string.Equals(email, PrimaryAdminEmail, StringComparison.OrdinalIgnoreCase);
        }
    }
}
