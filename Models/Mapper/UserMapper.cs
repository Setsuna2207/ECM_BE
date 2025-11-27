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
                userID = user.Id,
                UserName = user.UserName,
                Email = user.Email
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
