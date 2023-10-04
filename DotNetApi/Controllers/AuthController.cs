using System.Data;
using AutoMapper;
using Dapper;
using DotnetAPI.Dto;
using DotnetAPI.Helpers;
using DotNetApi;
using DotNetApi.Data;
using DotNetApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{

    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly DataContextDapper _dapper;
        private readonly AuthHelper _authHelper;
        private readonly ReusableSql _reusableSql;
        private readonly IMapper _mapper;


        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _authHelper = new AuthHelper(config);
            _reusableSql = new(config);
            _mapper = new Mapper(new MapperConfiguration((cfg) =>
            {
                cfg.CreateMap<UserRegisterDto, UserComplete>();
            }));
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register(UserRegisterDto user)
        {
            if (user.Password == user.PasswordConfirm)
            {
                string sqlCheckUserDuplicate = @"SELECT 
                                    [Email] 
                                    FROM TutorialAppSchema.Auth
                                    WHERE Email = '" + user.Email + "'";
                IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserDuplicate);

                if (!existingUsers.Any())
                {

                    UserLoginDto userForSetPassword = new()
                    {
                        Email = user.Email,
                        Password = user.Password
                    };

                    if (_authHelper.SetPassword(userForSetPassword))
                    {
                        UserComplete userComplete = _mapper.Map<UserComplete>(user);
                        userComplete.Active = true;
                        // string sqlAddUser = @" EXEC TutorialAppSchema.spUser_Upsert " +
                        // "@FirstName='" + user.FirstName + "'," +
                        // "@LastName='" + user.LastName + "'," +
                        // "@Email='" + user.Email + "'," +
                        // "@Gender='" + user.Gender + "'," +
                        // "@JobTitle='" + user.JobTitle + "'," +
                        // "@JobDepartment='" + user.Department + "'," +
                        // "@Salary=" + user.Salary + "," +
                        // "@Active=1";

                        if (_reusableSql.UpsertUser(userComplete))
                        {
                            return Ok();
                        }
                        throw new Exception("Failed to add user details");
                    }
                    throw new Exception("Failed to register user");

                }
                throw new Exception("User already exists");

            }
            throw new Exception("Passwords do not match");
        }

        [HttpPut("ResetPassword")]
        public IActionResult ResetPassword(UserLoginDto userForResetPassword)
        {
            if (_authHelper.SetPassword(userForResetPassword))
            {
                return Ok();
            }

            throw new Exception("Failed to update password");
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login(UserLoginDto user)
        {

            string sqlForHashAndSalt = @"EXEC TutorialAppSchema.spLoginConfirmation_Get " +
                                        "@Email = @EmailParam";

            DynamicParameters sqlParameters = new();

            // SqlParameter userEmailParameter = new("@EmailParam", SqlDbType.NVarChar)
            // {
            //     Value = user.Email
            // };
            sqlParameters.Add("@EmailParam", user.Email, DbType.String);


            string userIdSql = @"
                    SELECT UserId FROM TutorialAppSchema.Users WHERE Email = '" + user.Email + "'";

            int userId;
            try
            {
                userId = _dapper.LoadDataSingle<int>(userIdSql);
            }
            catch (System.Exception)
            {
                return StatusCode(401, "Password or Email does not match");
            }

            UserForLoginConfirmationDto userConfirmation = _dapper.
            LoadDataSingleWithParams<UserForLoginConfirmationDto>(sqlForHashAndSalt, sqlParameters);

            byte[] passwordHash = _authHelper.GetPasswordHash(user.Password, userConfirmation.PasswordSalt);

            for (int i = 0; i < passwordHash.Length; i++)
            {
                if (passwordHash[i] != userConfirmation.PasswordHash[i])
                {
                    return StatusCode(401, "Password or Email does not match");
                }
            }

            return Ok(new Dictionary<string, string> {
                {
                    "token", _authHelper.CreateToken(userId)
                }
            });
        }

        [HttpGet("RefreshToken")]
        public string RefreshToken()
        {
            string sqlGetUserId = "SELECT UserId FROM TutorialAppSchema.Users WHERE userId = '" + User.FindFirst("userId")?.Value + "'";

            int userId = _dapper.LoadDataSingle<int>(sqlGetUserId);

            return _authHelper.CreateToken(userId);
        }

    }
}