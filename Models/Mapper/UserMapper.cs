using ECM_BE.Models.DTOs.User;
using ECM_BE.Models.Entities;

namespace ECM_BE.Models.Mapper
{
    public static class UserMapper
    {
        public static ViewUserForAdminDTO ToViewUserForAdminDTOFromUser(this User user)
        {
            return new ViewUserForAdminDTO()
            {
                UserID = user.Id,  // Added UserID
                UserName = user.UserName,
                FullName = user.FullName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                Avatar = user.Avatar,
                CreatedAt = user.CreatedAt
            };
        }

        public static NewUserDTO ToNewUserDTOFromUser(this User user)
        {
            return new NewUserDTO()
            {
                UserID = user.Id,  // Changed to PascalCase to match DTO
                UserName = user.UserName,
                FullName = user.FullName,
                Email = user.Email,
                Avatar = user.Avatar
            };
        }

        public static User ToUserFromAdminAddUserDTO(this AdminAddUserDTO adminAddUserDTO)
        {
            return new User()
            {
                UserName = adminAddUserDTO.UserName,
                Email = adminAddUserDTO.Email,
                FullName = adminAddUserDTO.FullName,
                Avatar = adminAddUserDTO.Avatar,
            };
        }

        public static void UpdateUserFromDTO(this User user, UserForAdminDTO userForAdminDTO)
        {
            user.FullName = userForAdminDTO.FullName;
            user.Avatar = userForAdminDTO.Avatar;
            user.Email = userForAdminDTO.Email;
            user.EmailConfirmed = userForAdminDTO.EmailConfirmed;
        }

        public static void UserUpdateUserFromDTO(this User user, UserChangeUserDTO userChangeUser)
        {
            user.FullName = userChangeUser.FullName;
            user.Avatar = userChangeUser.Avatar;
        }
    }
}
