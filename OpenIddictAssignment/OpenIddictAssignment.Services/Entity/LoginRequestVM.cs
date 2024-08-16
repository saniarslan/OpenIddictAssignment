using System.ComponentModel.DataAnnotations;

namespace OpenIddictAssignment.Services.Entity
{
    public class LoginRequestVM
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Ipaddress { get; set; } = string.Empty;

        public string Device { get; set; } = string.Empty;

        public string Browser { get; set; } = string.Empty;
        public override string ToString()
        {
            return $" Username:{Username}, password:{Password}, ipaddress:{Ipaddress}, device:{Device}, browser:{Browser}";
        }
    }
    public class LoginResponseVM 
    {
        public bool IsAuthenticated { get; set; }
        public string Username { get; set; } = string.Empty;
        public bool UserFound { get; set; }

        // Success Response 
        public Guid UserId { get; set; }
        public Dictionary<string, List<string>> UserClaims { get; set; }
        public Dictionary<string, object> UserAttributes { get; set; }
        public bool IsLocked { get; set; }
        public bool Active { get; set; }

        public override string ToString()
        {
            return $" IsAuthenticated:{IsAuthenticated}, Username:{Username},  UserFound:{UserFound}, UserId:{UserId}, IsLocked:{IsLocked}, Active:{Active}";
        }
    }
}
