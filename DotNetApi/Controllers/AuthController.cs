using System.Data;
using System.Security.Cryptography;
using System.Text;
using DotnetAPI.Dto;
using DotNetApi;
using DotNetApi.Data;
using DotNetApi.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace DotnetAPI.Controllers
{
    public class AuthController : ControllerBase
    {

        private readonly DataContextDapper _dapper;
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _config = config;
        }

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

                    byte[] passwordHash = GetPasswordHash(user.Password, passwordSalt);

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

                        return Ok();
                    }
                    throw new Exception("Failed to add user");

                }
                throw new Exception("User already exists");

            }
            throw new Exception("Passwords do not match");
        }

        [HttpPost("login")]
        public IActionResult Login(UserLoginDto user)
        {

            string sqlForHashAndSalt = @"SELECT 
                                    [Email],
                                    [PasswordHash],
                                    [PasswordSalt]  
                                    FROM TutorialAppSchema.Auth WHERE Email = '"
                                    + user.Email + "'";
            UserForLoginConfirmationDto userConfirmation = _dapper.
            LoadDataSingle<UserForLoginConfirmationDto>(sqlForHashAndSalt);

            byte[] passwordHash = GetPasswordHash(user.Password, userConfirmation.PasswordSalt);

            for (int i = 0; i < passwordHash.Length; i++)
            {
                if (passwordHash[i] != userConfirmation.PasswordHash[i])
                {
                    return StatusCode(401, "Password or Email does not match");
                }
            }

            return Ok();
        }

        private byte[] GetPasswordHash(string password, byte[] passwordSalt)
        {
            string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey").Value + Convert.ToBase64String(passwordSalt);

            return KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8
            );
        }
    }
}