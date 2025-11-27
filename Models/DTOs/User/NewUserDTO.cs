namespace ECM_BE.Models.DTOs.User
{
    public class NewUserDTO
    {
        public string userID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string Roles { get; set; }
    }
}
