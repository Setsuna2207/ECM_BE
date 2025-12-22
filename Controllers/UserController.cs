using ECM_BE.Extensions;
using ECM_BE.Models.DTOs.User;
using ECM_BE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECM_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await _userService.RegisterAsync(registerDTO);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await _userService.LoginAsync(loginDTO);
        }

        [HttpGet]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> GetAllUsers()
        {
            return await _userService.GetAllUsersAsync();
        }

        [HttpGet("{userName}")]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> GetUser(string userName)
        {
            return await _userService.GetUserAsync(userName);
        }

        [HttpPut("{userName}")]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> UpdateUser(string userName, [FromBody] UserChangeUserDTO userChangeUser)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserName = User.GetUsername();
            if (currentUserName != userName)
                return Unauthorized("Bạn chỉ có thể cập nhật thông tin của chính mình");

            return await _userService.UpdateUserAsync(userName, userChangeUser);
        }

        [HttpPost("add")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> AddUser([FromBody] AdminAddUserDTO adminAddUserDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await _userService.AddUserAsync(adminAddUserDTO);
        }

        [HttpPut("admin/update")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> AdminUpdateUser([FromBody] UserForAdminDTO userForAdminDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await _userService.AdminUpdateUserAsync(userForAdminDTO);
        }

        [HttpDelete("{userName}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> DeleteUser(string userName)
        {
            return await _userService.DeleteUserAsync(userName);
        }

        [HttpPost("change-password")]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserName = User.GetUsername();
            if (currentUserName != changePasswordDTO.UserName)
                return Unauthorized("Bạn chỉ có thể thay đổi mật khẩu của chính mình");

            return await _userService.ChangePasswordAsync(changePasswordDTO);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest("Email không được để trống");

            return await _userService.SendForgotPasswordEmail(email);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await _userService.ResetPasswordAsync(resetPasswordDTO);
        }

        [HttpPost("send-email-confirmation")]
        public async Task<IActionResult> SendEmailConfirmation([FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest("Email không được để trống");

            return await _userService.SendEmailConfirmedAsync(email);
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
                return BadRequest("Email và token không được để trống");

            return await _userService.ConfirmEmailAsync(email, token);
        }

        [HttpPut("{userName}/avatar")]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> UpdateAvatar(string userName, [FromBody] string url)
        {
            var currentUserName = User.GetUsername();
            if (currentUserName != userName)
                return Unauthorized("Bạn chỉ có thể cập nhật avatar của chính mình");

            return await _userService.UpdateAvatarAsync(userName, url);
        }
    }
}