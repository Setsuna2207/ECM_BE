namespace ECM_BE.Models.DTOs.User
{
    public class AdminAddUserDTO
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
        public string? FullName { get; set; }
        public string? Avatar { get; set; }
    }
}
