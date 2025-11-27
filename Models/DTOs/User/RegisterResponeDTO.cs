namespace ECM_BE.Models.DTOs.User
{
    public class RegisterResponeDTO
    {
        public NewUserDTO NewUser { get; set; }
        public string EmailConfirmationMessage { get; set; }
    }
}
