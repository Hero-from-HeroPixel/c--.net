using System.Data;
using Dapper;
using DotNetApi.Data;
using DotNetApi.Models;

namespace DotnetAPI.Helpers
{

    public class ReusableSql
    {

        private readonly DataContextDapper _dapper;
        public ReusableSql(IConfiguration config)
        {
            _dapper = new(config);
        }

        public bool UpsertUser(UserComplete user)
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


            return _dapper.ExecuteSqlWithParams(sql, sqlParameters);

        }
    }
}