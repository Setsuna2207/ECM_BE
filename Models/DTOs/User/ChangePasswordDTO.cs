namespace ECM_BE.Models.DTOs.User
{
    public class ChangePasswordDTO
    {
        public string UserName { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
