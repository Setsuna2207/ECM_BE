using ECM_BE.Data;
using ECM_BE.Models.DTOs.User;
using ECM_BE.Models.Entities;
using ECM_BE.Models.Mapper;
using ECM_BE.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECM_BE.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<User> _signInManager;
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        public UserService(UserManager<User> userManager, ITokenService tokenService, SignInManager<User> signInManager, AppDbContext context, IEmailService emailService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _context = context;
            _emailService = emailService;
        }
        public async Task<IActionResult> RegisterAsync(RegisterDTO registerDTO)
        {
            try
            {
                var user = new User
                {
                    UserName = registerDTO.UserName,
                    Email = registerDTO.Email,
                    CreatedAt = DateTime.UtcNow,
                };
                var createUser = await _userManager.CreateAsync(user, registerDTO.Password);

                if (createUser.Succeeded)
                {
                    var userRole = await _userManager.AddToRoleAsync(user, "User");
                    if (userRole.Succeeded)
                    {
                        // Gửi email xác thực tài khoản chứ k login luôn
                        var token = await _tokenService.createToken(user);
                        var roles = await _userManager.GetRolesAsync(user);
                        var role = roles.FirstOrDefault();

                        var newUserDTO = UserMapper.ToNewUserDTOFromUser(user);
                        newUserDTO.Roles = role;
                        newUserDTO.Token = await _tokenService.createToken(user);
                        return new OkObjectResult(newUserDTO);
                    }
                    else
                    {
                        var result = await _userManager.DeleteAsync(user);
                        return new BadRequestObjectResult(userRole.Errors);
                    }
                }
                else
                {
                    return new BadRequestObjectResult(createUser.Errors);
                }
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex) { StatusCode = 500 };
            }
        }

        public async Task<IActionResult> AddUserAsync(AdminAddUserDTO adminAddUserDTO)
        {
            try
            {
                var user = adminAddUserDTO.ToUserFromAdminAddUserDTO();
                user.CreatedAt = DateTime.UtcNow;
                var createUser = await _userManager.CreateAsync(user, adminAddUserDTO.Password);
                if (createUser.Succeeded)
                {
                    if (adminAddUserDTO.Role == "Admin")
                    {
                        var userRole = await _userManager.AddToRoleAsync(user, adminAddUserDTO.Role);
                        if (userRole.Succeeded)
                        {
                            return new OkObjectResult("User added successfully");
                        }
                        else
                        {
                            var result = await _userManager.DeleteAsync(user);
                            return new BadRequestObjectResult(userRole.Errors);
                        }
                    }
                    else
                    {
                        var userRole = await _userManager.AddToRoleAsync(user, "User");
                        if (userRole.Succeeded)
                        {
                            return new OkObjectResult("User added successfully");
                        }
                        else
                        {
                            var result = await _userManager.DeleteAsync(user);
                            return new BadRequestObjectResult(userRole.Errors);
                        }
                    }
                }
                else
                {
                    return new BadRequestObjectResult(createUser.Errors);
                }
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex) { StatusCode = 500 };
            }
        }
        public async Task<IActionResult> LoginAsync(LoginDTO loginDTO)
        {
            try
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == loginDTO.UserName.ToLower());
                if (user == null)
                {
                    return new UnauthorizedObjectResult("UserName or Password is incorrect!");
                }

                //if (!user.EmailConfirmed)
                //{
                //    return new UnauthorizedObjectResult("Email not confirmed");
                //}

                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);
                if (!result.Succeeded)
                {
                    return new UnauthorizedObjectResult("UserName or Password is incorrect!");
                }

                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault();

                var newUserDTO = UserMapper.ToNewUserDTOFromUser(user);
                newUserDTO.Roles = role;
                newUserDTO.Token = await _tokenService.createToken(user);

                return new OkObjectResult(newUserDTO);
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex) { StatusCode = 500 };
            }
        }

        public async Task<IActionResult> GetAllUsersAsync()
        {
            try
            {
                var users = await _userManager.Users.ToListAsync();
                var userDtos = new List<ViewUserForAdminDTO>();
                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var role = roles.FirstOrDefault();
                    var userForAdminDTO = UserMapper.ToViewUserForAdminDTOFromUser(user);
                    userForAdminDTO.Roles = role;
                    userDtos.Add(userForAdminDTO);
                }
                return new OkObjectResult(userDtos);
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex) { StatusCode = 500 };
            }
        }

        public async Task<IActionResult> GetUserAsync(string userName)
        {
            try
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == userName);
                if (user == null)
                {
                    return new UnauthorizedObjectResult("User not found");
                }
                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault();
                var userForAdminDTO = UserMapper.ToViewUserForAdminDTOFromUser(user);
                userForAdminDTO.Roles = role;
                return new OkObjectResult(userForAdminDTO);
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex) { StatusCode = 500 };
            }
        }

        public async Task<IActionResult> UpdateUserAsync(string userName, UserChangeUserDTO userChangeUser)
        {
            try
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == userName);
                if (user == null)
                {
                    return new UnauthorizedObjectResult("User not found");
                }

                user.UserUpdateUserFromDTO(userChangeUser);

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return new OkObjectResult("User updated successfully");
                }
                else
                {
                    return new BadRequestObjectResult(result.Errors);
                }
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex) { StatusCode = 500 };
            }
        }

        public async Task<IActionResult> AdminUpdateUserAsync(UserForAdminDTO userForAdminDTO)
        {
            try
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == userForAdminDTO.UserName);
                if (user == null)
                {
                    return new UnauthorizedObjectResult("User not found");
                }

                user.UpdateUserFromDTO(userForAdminDTO);

                var roles = await _userManager.GetRolesAsync(user);
                var currentRole = roles.FirstOrDefault();
                if (currentRole != userForAdminDTO.Roles)
                {
                    var removeRoleResult = await _userManager.RemoveFromRoleAsync(user, currentRole);
                    if (!removeRoleResult.Succeeded)
                    {
                        return new ObjectResult(removeRoleResult.Errors) { StatusCode = 500 };
                    }

                    var addRoleResult = await _userManager.AddToRoleAsync(user, userForAdminDTO.Roles);
                    if (!addRoleResult.Succeeded)
                    {
                        return new ObjectResult(addRoleResult.Errors) { StatusCode = 500 };
                    }
                }

                if (!string.IsNullOrEmpty(userForAdminDTO.Password))
                {
                    var removePasswordResult = await _userManager.RemovePasswordAsync(user);
                    if (!removePasswordResult.Succeeded)
                    {
                        return new BadRequestObjectResult(removePasswordResult.Errors);
                    }

                    var addPasswordResult = await _userManager.AddPasswordAsync(user, userForAdminDTO.Password);
                    if (!addPasswordResult.Succeeded)
                    {
                        return new BadRequestObjectResult(addPasswordResult.Errors);
                    }
                }

                var updateResult = await _userManager.UpdateAsync(user);
                if (updateResult.Succeeded)
                {
                    return new OkObjectResult("User updated successfully");
                }
                else
                {
                    return new BadRequestObjectResult(updateResult.Errors);
                }
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex) { StatusCode = 500 };
            }
        }

        public async Task<IActionResult> DeleteUserAsync(string userName)
        {
            try
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == userName);
                if (user == null)
                {
                    return new UnauthorizedObjectResult("User not found");
                }
                var review = await _context.Reviews.Where(r => r.userID == user.Id).ToListAsync();
                foreach (var r in review)
                {
                    _context.Reviews.Remove(r);
                }
                var userTokens = await _context.UserTokens.Where(ut => ut.UserId == user.Id).ToListAsync();
                foreach (var token in userTokens)
                {
                    _context.UserTokens.Remove(token);
                }
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return new OkObjectResult("User deleted successfully");
                }
                else
                {
                    return new BadRequestObjectResult(result.Errors);
                }
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex) { StatusCode = 500 };
            }
        }

        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordDTO changePasswordDTO)
        {
            try
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == changePasswordDTO.UserName);
                if (user == null)
                {
                    return new UnauthorizedObjectResult("User not found");
                }
                if (user.EmailConfirmed == false)
                {
                    return new UnauthorizedObjectResult("Email not confirmed");
                }
                if (user.Email == null)
                {
                    return new UnauthorizedObjectResult("User dont have an email!");
                }
                var result = await _userManager.ChangePasswordAsync(user, changePasswordDTO.CurrentPassword, changePasswordDTO.NewPassword);
                if (result.Succeeded)
                {
                    await _emailService.SendEmailAsync(user.Email, "Mật khẩu của bạn đã được cập nhật – ELearning",
                        $"<div style='font-family: Time New Roman; font-size: 18px; color: black;'>Xin chào {user.UserName},  " +
                        "<br><br>Mật khẩu của bạn đã được thay đổi thành công. Nếu bạn đã thực hiện thay đổi này, không cần làm gì thêm.  " +
                        "<br><br>Nếu bạn không thực hiện, vui lòng liên hệ ngay với chúng tôi tại trantuyen.cfx@gmail.com để bảo vệ tài khoản của bạn.  " +
                        "<br><br>Trân trọng,  <br>ELearning." +
                        "</div>");
                    return new OkObjectResult("Password changed successfully");
                }
                else return new BadRequestObjectResult(result.Errors);
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex) { StatusCode = 500 };
            }
        }

        public async Task<IActionResult> SendForgotPasswordEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new UnauthorizedObjectResult("Email not found");
            }
            if (user.EmailConfirmed == false)
            {
                return new UnauthorizedObjectResult("Email not confirmed");
            }
            try
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var forgotPasswordLink = "http://localhost:5173/reset-password?token=" + token + "&email=" + user.Email;
                await _emailService.SendEmailAsync(user.Email, "Đặt lại mật khẩu của bạn – ELearning",
                     $"<div style='font-family: Time New Roman; font-size: 18px; color: black;'>Xin chào {user.UserName},  " +
                    "<br><br>Vui lòng đặt lại mật khẩu của bạn bằng cách nhấp vào đường dẫn dưới đây:" +
                    $"<br><br><a href='{forgotPasswordLink}'>Đổi lại mật khẩu</a>" +
                    "<br><br>Nếu bạn không thực hiện, xin hãy bỏ qua email này." +
                    "<br><br>Trân trọng,  <br>ELearning." +
                    "</div>");
                return new OkObjectResult("Email sent successfully!" + token);
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex) { StatusCode = 500 };
            }
        }

        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(resetPasswordDTO.Email);
                if (user == null)
                {
                    return new UnauthorizedObjectResult("Email not found");
                }
                var result = await _userManager.ResetPasswordAsync(user, resetPasswordDTO.Token, resetPasswordDTO.NewPassword);
                if (result.Succeeded)
                {
                    return new OkObjectResult("Password reset successfully");
                }
                else return new BadRequestObjectResult(result.Errors);
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex) { StatusCode = 500 };
            }
        }

        public async Task<IActionResult> SendEmailConfirmedAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new BadRequestObjectResult("User not found!");
            }
            try
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = "http://localhost:5173/emailConfirmed?token=" + token + "&email=" + user.Email;
                await _emailService.SendEmailAsync(user.Email, "Xác nhận email của bạn – ELearning",
                    $"<div style='font-family: Time New Roman; font-size: 18px; color: black;'>Xin chào {user.UserName},  " +
                    "<br><br>Cảm ơn bạn đã đăng ký tài khoản tại ELearning. Vui lòng xác nhận email của bạn bằng cách nhấp vào đường dẫn dưới đây:" +
                    $"<br><br><a href='{confirmationLink}'>Xác nhận email</a>" +
                    "<br><br>Nếu bạn không thực hiện, xin hãy bỏ qua email này." +
                    "<br><br>Trân trọng,  <br>ELearning." +
                    token +
                    "</div>");
                return new OkObjectResult("Confirmed email sent successfully! " + token);
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex) { StatusCode = 500 };
            }
        }

        public async Task<IActionResult> ConfirmEmailAsync(string email, string token)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return new BadRequestObjectResult("Email not found!");
                }
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return new OkObjectResult("Email confirmed successfully");
                }
                else return new BadRequestObjectResult(result.Errors);
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex) { StatusCode = 500 };
            }
        }

        public async Task<IActionResult> UpdateAvatarAsync(string userName, string url)
        {
            try
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == userName);
                if (user == null)
                {
                    return new UnauthorizedObjectResult("User not found");
                }
                user.Avatar = url;
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return new OkObjectResult("Avatar updated successfully");
                }
                else
                {
                    return new BadRequestObjectResult(result.Errors);
                }
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex) { StatusCode = 500 };
            }
        }
    }
}
