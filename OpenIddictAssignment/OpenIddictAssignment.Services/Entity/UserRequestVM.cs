namespace OpenIddictAssignment.Services.Entity
{
    public class UserRequestVM
    {
        public string grant_type { get; set; } = string.Empty;
        public string username { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public string? ipaddress { get; set; }
        public string device { get; set; } = string.Empty;
        public string? browser { get; set; }
     
        public override string ToString()
        {
            return $"grant_type:{grant_type}, username:{username}, password:{password}, ipaddress: {ipaddress}, device: {device}, browser: {browser}";
        }
    }
}
