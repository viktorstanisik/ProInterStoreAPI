using Microsoft.AspNetCore.Mvc;
using ProInterStore_Domain.DTOModels;
using ProInterStore_Service.Interfaces;
using ProInterStore_Service.Models;
using ProInterStore_Service.Services;
using Serilog;

namespace ProInterStore_Api.Controllers
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

        [HttpPost("register-user")]
        public async Task<IActionResult> RegisterUser([FromBody] UserDto userEntity)
        {
            try
            {
                int userId = await _userService.Create(userEntity);

                return Ok(userId);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

        [HttpPost("login-user")]
        public async Task<IActionResult> LoginUser([FromBody] LoginModel loginModel)
        {
            try
            {
                var jwt = await _userService.Login(loginModel);

                return Ok(jwt);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

        [TypeFilter(typeof(JWTInterceptorAuthFilter))]
        [HttpPost("logout")]
        public async Task<bool> Logout()
        {
            try
            {
                return await _userService.LogoutUser();

            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }
    }
}
