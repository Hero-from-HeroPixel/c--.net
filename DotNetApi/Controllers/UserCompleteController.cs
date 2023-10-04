using System.Data;
using Dapper;
using DotNetApi.Data;
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
        string stringParameters = "";

        DynamicParameters sqlParameters = new();

        if (userId != 0)
        {
            stringParameters += ", @UserId=@UserIdParam";
            sqlParameters.Add("@UserIdParam", userId, DbType.Int32);
        }

        if (isActive)
        {
            stringParameters += ", @Active=@ActiveParm";
            sqlParameters.Add("@ActiveParm", isActive, DbType.Boolean);
        }

        if (stringParameters.Length > 0) sql += stringParameters[1..];

        IEnumerable<UserComplete> users = _dapper.LoadDataWithParams<UserComplete>(sql, sqlParameters);
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
        string sql = @" EXEC TutorialAppSchema.spUser_Upsert
                        @FirstName=@FirstNameParam , 
                        @LastName=@LastNameParam , 
                        @Email=@EmailParam ,
                        @Gender=@GenderParam ,
                        @Active=@ActiveParam ,
                        @JobTitle=@JobTitleParam,
                        @JobDepartment=@DepartmentParam ,
                        @Salary=@SalaryParam ,
                        @UserId=@UserIdParam ";

        DynamicParameters sqlParameters = new();

        sqlParameters.Add("@FirstNameParam", user.FirstName, DbType.String);
        sqlParameters.Add("@LastNameParam", user.LastName, DbType.String);
        sqlParameters.Add("@EmailParam", user.Email, DbType.String);
        sqlParameters.Add("@GenderParam", user.Gender, DbType.String);
        sqlParameters.Add("@ActiveParam", user.Active, DbType.Boolean);
        sqlParameters.Add("@JobTitleParam", user.JobTitle, DbType.String);
        sqlParameters.Add("@DepartmentParam", user.Department, DbType.String);
        sqlParameters.Add("@SalaryParam", user.Salary, DbType.Decimal);
        sqlParameters.Add("@UserIdParam", user.UserId, DbType.Int32);

        //To Do [{param1,type} , {param2,type} , {param3,type} , {param4,type} , etc.] for loop, and assign each param with format ("@" + param + "Param",object value = null param , DbType type)

        //Pros, Can map js object into params easier.


        if (_dapper.ExecuteSqlWithParams(sql, sqlParameters))
        {
            return Ok();
        }

        throw new Exception("Failed to Add User");
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        string sql = @"EXEC TutorialAppSchema.spUser_Delete @UserId=@UserIdParam";

        DynamicParameters sqlParameters = new();
        sqlParameters.Add("@UserIdParam", userId, DbType.Int32);

        if (_dapper.ExecuteSqlWithParams(sql, sqlParameters))
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
