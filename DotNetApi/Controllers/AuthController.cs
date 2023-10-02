using System.Data;
using System.Security.Cryptography;
using DotnetAPI.Dto;
using DotnetAPI.Helpers;
using DotNetApi;
using DotNetApi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace DotnetAPI.Controllers
{

    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly DataContextDapper _dapper;

        private readonly AuthHelper _authHelper;

        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);

            _authHelper = new AuthHelper(config);
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
                Console.WriteLine(sqlCheckUserDuplicate);
                IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserDuplicate);

                if (!existingUsers.Any())
                {
                    byte[] passwordSalt = new byte[128 / 8];
                    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                    {
                        rng.GetNonZeroBytes(passwordSalt);
                    }

                    byte[] passwordHash = _authHelper.GetPasswordHash(user.Password, passwordSalt);

                    string sqlAddAuth = @"INSERT INTO TutorialAppSchema.Auth (
                                        [Email],
                                        [PasswordHash],
                                        [PasswordSalt]    
                                        ) VALUES ("
                                            + "'" + user.Email + "',"
                                            + "@PasswordHash, @PasswordSalt)";

                    List<SqlParameter> sqlParameters = new();

                    SqlParameter passwordSaltParameter = new("@PasswordSalt", SqlDbType.VarBinary)
                    {
                        Value = passwordSalt
                    };

                    SqlParameter passwordHashParameter = new("@PasswordHash", SqlDbType.VarBinary)
                    {
                        Value = passwordHash
                    };

                    sqlParameters.Add(passwordSaltParameter);
                    sqlParameters.Add(passwordHashParameter);

                    if (_dapper.ExecuteSqlWithParams(sqlAddAuth, sqlParameters))
                    {

                        string sqlAddUser = @"
                                    INSERT INTO TutorialAppSchema.Users(
                                        [FirstName],
                                        [LastName],
                                        [Email],
                                        [Gender]
                                    ) VALUES (" +
                                    "'" + user.FirstName +
                                    "', '" + user.LastName +
                                    "', '" + user.Email +
                                    "', '" + user.Gender +
                                "')";
                        if (_dapper.ExecuteSql(sqlAddUser))
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

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login(UserLoginDto user)
        {

            string sqlForHashAndSalt = @"SELECT 
                                    [Email],
                                    [PasswordHash],
                                    [PasswordSalt]  
                                    FROM TutorialAppSchema.Auth WHERE Email = '"
                                    + user.Email + "'";

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
            LoadDataSingle<UserForLoginConfirmationDto>(sqlForHashAndSalt);

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