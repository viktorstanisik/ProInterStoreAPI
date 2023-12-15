using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ProInterStore_DataAccess.Interfaces;
using ProInterStore_Domain.DTOModels;
using ProInterStore_Domain.EntityModels;
using ProInterStore_Service.Interfaces;
using ProInterStore_Service.Models;
using ProInterStore_Shared.AppConstans;
using ProInterStore_Shared.Enums;
using ProInterStore_Shared.HelperMethods;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace ProInterStore_Service.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly ILoggedUsersInfoRepository _loggedUsersInfoRepository;


        public UserService(IUserRepository userRepository, ITokenService tokenService, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IMapper mapper, ILoggedUsersInfoRepository loggedUsersInfoRepository)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _loggedUsersInfoRepository = loggedUsersInfoRepository;
        }

        public async Task<int> Create(UserDto userDtoModel)
        {
            await RequiredField(userDtoModel);

            userDtoModel.Password = Methods.GenerateSha512Hash(userDtoModel.Password);

            var user = await _userRepository.CreateUser(_mapper.Map<User>(userDtoModel));

            return user.Id;
        }

        public int GetLoggedUserIdFromHttpContext()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Convert.ToInt32(userIdClaim);
        }

        public async Task<bool> LogoutUser()
        {
            var userId = GetLoggedUserIdFromHttpContext();

            var validateUserToken = await GetUserByUserIdFromLoggedUserInfo(userId);

            if (validateUserToken.LastLogin != null && validateUserToken.LoginStatus != (int)LoginStatus.LoggedOut)
            {
                var loggedUserInfo = await _loggedUsersInfoRepository.LogoutUser(userId);

                return loggedUserInfo != null;
            }

            return false;


        }

        public async Task<JwtResponseModel> Login(LoginModel model)
        {
            User user = await _userRepository.GetUser(model.Email, Methods.GenerateSha512Hash(model.Password));

            if (user is null) throw new Exception(ErrorMessages.InvalidEmailOrPassword);

            var token = _tokenService.GenerateJwtToken(model, user.Id);

            JwtResponseModel jwtToken = new() { Jwt = token };

            await _loggedUsersInfoRepository.CreateLoggedUserRecord(user.Id);

            return jwtToken;
        }

        public async Task<LoggedUserInfo> GetUserByUserIdFromLoggedUserInfo(int userId)
        {
            return await _loggedUsersInfoRepository.GetLoggedUserInfo(userId);
        }

        public async Task<bool> RequiredField(UserDto model)
        {
            model.TrimStringProperties();

            Regex emailRegex = new Regex(_configuration["RegexValidation:EmailRegex"]);
            Regex passwordRegex = new Regex(_configuration["RegexValidation:PasswordRegex"]);
            Regex phoneRegex = new Regex(_configuration["RegexValidation:PhoneRegex"]);

            if (model.Password != model.ConfirmPassword) throw new Exception(ErrorMessages.InvalidEmailOrPassword);

            User currentUser = await _userRepository.GetUserByEmail(model.Email);

            if (currentUser != null) throw new Exception(ErrorMessages.UserAlreadyExist);

            if (!emailRegex.IsMatch(model.Email) 
                || !passwordRegex.IsMatch(model.Password))
                throw new Exception(ErrorMessages.InvalidEmailOrPassword);

            if(!phoneRegex.IsMatch(model.PhoneNumber)) throw new InvalidDataException(ErrorMessages.InvalidPhoneNumberFormat);


            return await Task.FromResult(true);
        }
    }
}
