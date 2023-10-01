using DotNetApi.Data;
using DotNetApi.Dto;
using DotNetApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotNetApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{

    // DataContextDapper _dapper;
    DataContextEF _entityFramework;
    public UserController(IConfiguration config)
    {
        // _dapper = new DataContextDapper(config);
        _entityFramework = new DataContextEF(config);
    }


    // [HttpGet("TestConnection")]
    // public DateTime TestConnection()
    // {
    //     return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
    // }

    [HttpGet("GetUsers")]
    // public IEnumerable<User> GetUsers()
    public IEnumerable<User> GetUsers()
    {
        IEnumerable<User> users = _entityFramework.Users.ToList<User>();
        // string sql = @"
        // SELECT 
        //     [UserId],
        //     [FirstName],
        //     [LastName],
        //     [Email],
        //     [Gender],
        //     [Active] 
        // FROM TutorialAppSchema.Users 
        // ";
        // IEnumerable<User> users = _dapper.LoadData<User>(sql);
        return users;

    }
    [HttpGet("GetSingleUser/{userId}")]
    public User GetSingleUser(int userId)
    {
        User? user = _entityFramework.Users.Where<User>(u => u.UserId == userId)
        .FirstOrDefault<User>();
        // string sql = @"
        // SELECT 
        //     [UserId],
        //     [FirstName],
        //     [LastName],
        //     [Email],
        //     [Gender],
        //     [Active] 
        // FROM TutorialAppSchema.Users 
        // WHERE UserId = " + userId.ToString()
        // ;
        // User user = _dapper.LoadDataSingle<User>(sql);
        if (user != null)
        {
            return user;
        }

        throw new Exception("No Users found");
    }

    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        // string sql = @"
        //     UPDATE TutorialAppSchema.Users 
        //         SET [FirstName] = '" + user.FirstName +
        //         "',[LastName]= '" + user.LastName +
        //         "',[Email]= '" + user.Email +
        //         "',[Gender]= '" + user.Gender +
        //         "',[Active]= '" + user.Active +
        //     "' WHERE UserId = " + user.UserId;
        // if (_dapper.ExecuteSql(sql))
        // {
        //     return Ok();
        // }

        User? userDb = _entityFramework.Users.Where<User>(u => u.UserId == user.UserId)
            .FirstOrDefault<User>();
        // string sql = @"
        // SELECT 
        //     [UserId],
        //     [FirstName],
        //     [LastName],
        //     [Email],
        //     [Gender],
        //     [Active] 
        // FROM TutorialAppSchema.Users 
        // WHERE UserId = " + userId.ToString()
        // ;
        // User user = _dapper.LoadDataSingle<User>(sql);
        if (userDb != null)
        {
            userDb.Active = user.Active;
            userDb.FirstName = user.FirstName;
            userDb.LastName = user.LastName;
            userDb.Gender = user.Gender;
            userDb.Email = user.Email;
            if (_entityFramework.SaveChanges() > 0)
            {

                return Ok();
            }
            throw new Exception("Failed to update user");
        }
        throw new Exception("Failed to get User");

    }

    [HttpPost("AddUser")]
    public IActionResult AddUser(UserDto user)
    {
        User userDb = new();
        userDb.Active = user.Active;
        userDb.FirstName = user.FirstName;
        userDb.LastName = user.LastName;
        userDb.Gender = user.Gender;
        userDb.Email = user.Email;
        _entityFramework.Add<User>(userDb);

        if (_entityFramework.SaveChanges() > 0)
        {

            return Ok();
        }
        throw new Exception("Failed to Add user");
        // string sql = @"
        //     INSERT INTO TutorialAppSchema.Users (
        //             [FirstName]," +
        //             "[LastName]," +
        //             "[Email]," +
        //             "[Gender]," +
        //             "[Active]" +
        //     ") VALUES (" +
        //         "'" + user.FirstName + "', " +
        //         "'" + user.LastName + "', " +
        //         "'" + user.Email + "', " +
        //         "'" + user.Gender + "', " +
        //         "'" + user.Active + "'" +
        //         ")";

        // Console.WriteLine(sql);
        // if (_dapper.ExecuteSql(sql))
        // {
        //     return Ok();
        // }
    }

    [HttpDelete()]
    public IActionResult DeleteUser(int userId)
    {
        User? userDb = _entityFramework.Users.Where<User>(u => u.UserId == userId)
        .FirstOrDefault<User>();

        if (userDb != null)
        {
            _entityFramework.Users.Remove(userDb);
            // string sql = "DELETE FROM TutorialAppSchema.Users WHERE UserId = " + userId.ToString();
            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }

            throw new Exception("Failed to delete user");
        }
        throw new Exception("No User found");
    }
}
