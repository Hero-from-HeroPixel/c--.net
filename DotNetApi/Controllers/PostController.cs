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

            if (postId != 0)
            {
                parameters += ", @PostId=" + postId.ToString();
            }

            if (userId != 0)
            {
                parameters += ", @UserId=" + userId.ToString();
            }

            if (searchParam != "NONE")
            {
                parameters += ", @SearchParam='" + searchParam + "'";
            }

            if (parameters.Length > 0) sql += parameters[1..]; //remove first comma
            return _dapper.LoadData<Post>(sql);
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
            string sql = @" EXEC TutorialAppSchema.spPosts_Get @UserId=" + this.User.FindFirst("userId")?.Value;


            return _dapper.LoadData<Post>(sql);
        }

        // @UserId = " +
        // @PostTitle = " +
        // @PostContent = " +
        // @PostId = " +

        [HttpPut("Upsert")]
        public IActionResult EditPost(Post postToAdd)
        {
            string sql = @" EXEC TutorialAppSchema.spPosts_Upsert ";
            string body = "@UserId = " + this.User.FindFirst("userId")?.Value + "," +
                            "@PostTitle = '" + postToAdd.PostTitle + "'," +
                            "@PostContent = '" + postToAdd.PostContent + "'";

            string errMessage;
            if (postToAdd.PostId > 0)
            {
                body += ", @PostId=" + postToAdd.PostId;
                errMessage = "Failed to update post";
            }
            else
            {
                errMessage = "Failed to create new post";
            }

            sql += body;

            if (_dapper.ExecuteSql(sql))
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
            string sql = "EXEC TutorialAppSchema.spPost_Delete @PostId= " + postId;
            sql += ", @UserId= " + this.User.FindFirst("userId")?.Value;

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            Console.WriteLine(sql);
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