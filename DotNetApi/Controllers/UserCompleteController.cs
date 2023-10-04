using DotNetApi.Data;
using DotNetApi.Dto;
using DotNetApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserCompleteController : ControllerBase
{
    readonly DataContextDapper _dapper;
    public UserCompleteController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }

    [HttpGet("TestConnection")]
    public DateTime TestConnection()
    {
        return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
    }

    // public IEnumerable<User> GetUsers()

    [HttpGet("GetUsers/{userId}/{isActive}")]
    /************** FizzBuzz pattern */
    public IEnumerable<UserComplete> GetUsers(bool isActive, int userId = 0)
    {
        string sql = "EXEC TutorialAppSchema.spUsers_Get";
        string parameters = "";

        if (userId != 0)
        {
            parameters += ", @UserId=" + userId.ToString();
        }

        if (isActive)
        {
            parameters += ", @Active=" + isActive.ToString();
        }

        if (parameters.Length > 0) sql += parameters[1..];

        IEnumerable<UserComplete> users = _dapper.LoadData<UserComplete>(sql);
        return users;
    }

    // [HttpGet("GetSingleUser/{userId}")]
    // // public IEnumerable<User> GetUsers()
    // public UserComplete GetSingleUser(int userId)
    // {
    //     string sql = "EXEC TutorialAppSchema.spUsers_Get" + "@UserId=" + userId.ToString();
    //     UserComplete user = _dapper.LoadDataSingle<UserComplete>(sql);
    //     return user;
    // }

    // [HttpPut("EditUser")]
    // public IActionResult EditUser(User user)
    // {
    //     string sql = @"
    //     UPDATE TutorialAppSchema.Users
    //         SET [FirstName] = '" + user.FirstName +
    //             "', [LastName] = '" + user.LastName +
    //             "', [Email] = '" + user.Email +
    //             "', [Gender] = '" + user.Gender +
    //             "', [Active] = '" + user.Active +
    //         "' WHERE UserId = " + user.UserId;

    //     Console.WriteLine(sql);

    //     if (_dapper.ExecuteSql(sql))
    //     {
    //         return Ok();
    //     }

    //     throw new Exception("Failed to Update User");
    // }


    // TutorialAppSchema.spUser_Upsert
    // @FirstName=
    // @LastName=
    // @Email=
    // @Gender=
    // @Active=
    // @JobTitle=
    // @JobDepartment=
    // @Salary=
    // @UserId=

    [HttpPost("UpsertUser")]
    public IActionResult UpsertUser(UserComplete user)
    {
        string sql = @" EXEC TutorialAppSchema.spUser_Upsert " +
                        "@FirstName=" + user.FirstName + "," +
                        "@LastName=" + user.LastName + "," +
                        "@Email=" + user.Email + "," +
                        "@Gender=" + user.Gender + "," +
                        "@Active=" + user.Active + "," +
                        "@JobTitle=" + user.JobTitle + "," +
                        "@JobDepartment=" + user.Department + "," +
                        "@Salary=" + user.Salary + "," +
                        "@UserId=" + user.UserId.ToString();


        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to Add User");
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        string sql = @"EXEC TutorialAppSchema.spUser_Delete @UserId=" + userId.ToString();

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to Delete User");
    }

    // [HttpGet("UserSalary/{userId}")]
    // public IEnumerable<UserSalary> GetUserSalary(int userId)
    // {
    //     return _dapper.LoadData<UserSalary>(@"
    //         SELECT UserSalary.UserId
    //                 , UserSalary.Salary
    //         FROM  TutorialAppSchema.UserSalary
    //             WHERE UserId = " + userId.ToString());
    // }

    // [HttpPost("UserSalary")]
    // public IActionResult PostUserSalary(UserSalary userSalaryForInsert)
    // {
    //     string sql = @"
    //         INSERT INTO TutorialAppSchema.UserSalary (
    //             UserId,
    //             Salary
    //         ) VALUES (" + userSalaryForInsert.UserId.ToString()
    //             + ", " + userSalaryForInsert.Salary
    //             + ")";

    //     if (_dapper.ExecuteSqlWithRows(sql) > 0)
    //     {
    //         return Ok(userSalaryForInsert);
    //     }
    //     throw new Exception("Adding User Salary failed on save");
    // }

    // [HttpPut("UserSalary")]
    // public IActionResult PutUserSalary(UserSalary userSalaryForUpdate)
    // {
    //     string sql = "UPDATE TutorialAppSchema.UserSalary SET Salary="
    //         + userSalaryForUpdate.Salary
    //         + " WHERE UserId=" + userSalaryForUpdate.UserId.ToString();

    //     if (_dapper.ExecuteSql(sql))
    //     {
    //         return Ok(userSalaryForUpdate);
    //     }
    //     throw new Exception("Updating User Salary failed on save");
    // }

    // [HttpDelete("UserSalary/{userId}")]
    // public IActionResult DeleteUserSalary(int userId)
    // {
    //     string sql = "DELETE FROM TutorialAppSchema.UserSalary WHERE UserId=" + userId.ToString();

    //     if (_dapper.ExecuteSql(sql))
    //     {
    //         return Ok();
    //     }
    //     throw new Exception("Deleting User Salary failed on save");
    // }

    // [HttpGet("UserJobInfo/{userId}")]
    // public IEnumerable<UserJobInfo> GetUserJobInfo(int userId)
    // {
    //     return _dapper.LoadData<UserJobInfo>(@"
    //         SELECT  UserJobInfo.UserId
    //                 , UserJobInfo.JobTitle
    //                 , UserJobInfo.Department
    //         FROM  TutorialAppSchema.UserJobInfo
    //             WHERE UserId = " + userId.ToString());
    // }

    // [HttpPost("UserJobInfo")]
    // public IActionResult PostUserJobInfo(UserJobInfo userJobInfoForInsert)
    // {
    //     string sql = @"
    //         INSERT INTO TutorialAppSchema.UserJobInfo (
    //             UserId,
    //             Department,
    //             JobTitle
    //         ) VALUES (" + userJobInfoForInsert.UserId
    //             + ", '" + userJobInfoForInsert.Department
    //             + "', '" + userJobInfoForInsert.JobTitle
    //             + "')";

    //     if (_dapper.ExecuteSql(sql))
    //     {
    //         return Ok(userJobInfoForInsert);
    //     }
    //     throw new Exception("Adding User Job Info failed on save");
    // }

    // [HttpPut("UserJobInfo")]
    // public IActionResult PutUserJobInfo(UserJobInfo userJobInfoForUpdate)
    // {
    //     string sql = "UPDATE TutorialAppSchema.UserJobInfo SET Department='"
    //         + userJobInfoForUpdate.Department
    //         + "', JobTitle='"
    //         + userJobInfoForUpdate.JobTitle
    //         + "' WHERE UserId=" + userJobInfoForUpdate.UserId.ToString();

    //     if (_dapper.ExecuteSql(sql))
    //     {
    //         return Ok(userJobInfoForUpdate);
    //     }
    //     throw new Exception("Updating User Job Info failed on save");
    // }

    // [HttpDelete("UserJobInfo/{userId}")]
    // public IActionResult DeleteUserJobInfo(int userId)
    // {
    //     string sql = "DELETE FROM TutorialAppSchema.UserJobInfo  WHERE UserId=" + userId;

    //     if (_dapper.ExecuteSql(sql))
    //     {
    //         return Ok();
    //     }
    //     throw new Exception("Deleting User Job Info failed on save");
    // }

    // [HttpDelete("UserJobInfo/{userId}")]
    // public IActionResult DeleteUserJobInfo(int userId)
    // {
    //     string sql = @"
    //         DELETE FROM TutorialAppSchema.UserJobInfo 
    //             WHERE UserId = " + userId.ToString();


    //     if (_dapper.ExecuteSql(sql))
    //     {
    //         return Ok();
    //     }

    //     throw new Exception("Failed to Delete User");
    // }
}
