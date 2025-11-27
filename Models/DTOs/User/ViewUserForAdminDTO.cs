namespace ECM_BE.Models.DTOs.User
{
    public class ViewUserForAdminDTO
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Roles { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Avatar { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
