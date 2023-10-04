using System.Data;
using Dapper;
using DotNetApi.Data;
using DotNetApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        public PostController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [AllowAnonymous]
        [HttpGet("Posts/{postId?}/{userId?}/{searchParam?}")]
        public IEnumerable<Post> GetPosts(int postId = 0, int userId = 0, string? searchParam = "NONE")
        {
            string sql = @"EXEC TutorialAppSchema.spPosts_Get ";
            string parameters = "";

            DynamicParameters sqlParameters = new();

            if (postId != 0)
            {
                parameters += ", @PostId=@PostIdParam";
                sqlParameters.Add("@PostIdParam", postId, DbType.Int32);
            }

            if (userId != 0)
            {
                parameters += ", @UserId=@UserIdParam";
                sqlParameters.Add("@UserIdParam", userId, DbType.Int32);
            }

            if (searchParam != "NONE")
            {
                parameters += ", @SearchParam=@SearchParamParam";
                sqlParameters.Add("@SearchParamParam", userId, DbType.String);

            }

            if (parameters.Length > 0) sql += parameters[1..]; //remove first comma
            return _dapper.LoadDataWithParams<Post>(sql, sqlParameters);
        }
        // [HttpGet("Posts/{postId}")]
        // public Post GetSinglePost(int postId)
        // {
        //     string sql = @" SELECT 
        //         [PostId],
        //         [UserId],
        //         [PostTitle],
        //         [PostContent],
        //         [PostCreated],
        //         [PostUpdated] 
        //         FROM [TutorialAppSchema].[Posts]
        //         WHERE PostId = '" + postId + "'";
        //     return _dapper.LoadDataSingle<Post>(sql);
        // }

        // [HttpGet("Posts/PostByUser/{userId}")]
        // public IEnumerable<Post> GetPostsByUser(int userId)
        // {
        //     string sql = @" SELECT 
        //         [PostId],
        //         [UserId],
        //         [PostTitle],
        //         [PostContent],
        //         [PostCreated],
        //         [PostUpdated] 
        //         FROM [TutorialAppSchema].[Posts]
        //         WHERE UserId = '" + userId + "'";
        //     return _dapper.LoadData<Post>(sql);
        // }

        [HttpGet("MyPosts")]
        public IEnumerable<Post> GetMyPosts()
        {
            DynamicParameters sqlParameters = new();
            sqlParameters.Add("@UserIdParam", this.User.FindFirst("userId")?.Value, DbType.Int32);

            string sql = @" EXEC TutorialAppSchema.spPosts_Get @UserId=@UserIdParam";

            return _dapper.LoadDataWithParams<Post>(sql, sqlParameters);
        }

        // @UserId = " +
        // @PostTitle = " +
        // @PostContent = " +
        // @PostId = " +

        [HttpPut("Upsert")]
        public IActionResult EditPost(Post postToAdd)
        {
            string sql = @" EXEC TutorialAppSchema.spPosts_Upsert ";
            DynamicParameters sqlParameters = new();

            string body = "@UserId = @UserIdParam," +
                            "@PostTitle = @PostTitleParam," +
                            "@PostContent = @PostContentParam";

            sqlParameters.Add("@UserIdParam", this.User.FindFirst("userId")?.Value, DbType.Int32);
            sqlParameters.Add("@PostTitleParam", postToAdd.PostTitle, DbType.String);
            sqlParameters.Add("@PostContentParam", postToAdd.PostContent, DbType.String);

            string errMessage;
            if (postToAdd.PostId > 0)
            {
                body += ", @PostId=@PostIdParam";
                sqlParameters.Add("@PostIdParam", postToAdd.PostId, DbType.Int32);
                errMessage = "Failed to update post";
            }
            else
            {
                errMessage = "Failed to create new post";
            }

            sql += body;

            if (_dapper.ExecuteSqlWithParams(sql, sqlParameters))
            {
                return Ok();
            }

            throw new Exception(errMessage);
        }

        // [HttpPut("Posts")]
        // public IActionResult Edit(PostToEditDto postToEdit)
        // {
        //     string sql = @" UPDATE TutorialAppSchema.Posts 
        //                         SET 
        //                         PostContent = '" + postToEdit.PostContent + "'," +
        //                         "PostTitle = '" + postToEdit.PostTitle + "'," +
        //                         "PostUpdated = GETDATE()" +
        //                         "WHERE PostId = " + postToEdit.PostId +
        //                         "AND UserId = " + this.User.FindFirst("userId")?.Value;

        //     if (_dapper.ExecuteSql(sql))
        //     {
        //         return Ok();
        //     }

        //     throw new Exception("Failed to create new post");
        // }

        [HttpDelete("Post/{postId}")]
        public IActionResult DeletePost(int postId)
        {

            DynamicParameters sqlParameters = new();
            sqlParameters.Add("@UserIdParam", this.User.FindFirst("userId")?.Value, DbType.Int32);

            string sql = "EXEC TutorialAppSchema.spPost_Delete @PostId=@PostIdParam ";
            sql += ", @UserId= @UserIdParam";

            if (_dapper.ExecuteSqlWithParams(sql, sqlParameters))
            {
                return Ok();
            }

            throw new Exception("Failed to delete post");
        }

        // [HttpGet("PostBySearch/{SearchParam}")]
        // public IEnumerable<Post> PostBySearch(string SearchParam)
        // {
        //     string sql = @" SELECT * FROM [TutorialAppSchema].[Posts]
        //                     WHERE PostTitle LIKE '%" + SearchParam + " %'" +
        //                     "OR PostContent LIKE '%" + SearchParam + " %'";
        //     return _dapper.LoadData<Post>(sql);
        // }
    }
}