using DotNetApi.Data;
using DotNetApi.Dto;
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

        [HttpGet("Posts")]
        public IEnumerable<Post> GetPosts()
        {
            string sql = @" SELECT 
                [PostId],
                [UserId],
                [PostTitle],
                [PostContent],
                [PostCreated],
                [PostUpdated] 
                FROM [TutorialAppSchema].[Posts]";
            return _dapper.LoadData<Post>(sql);
        }
        [HttpGet("Posts/{postId}")]
        public Post GetSinglePost(int postId)
        {
            string sql = @" SELECT 
                [PostId],
                [UserId],
                [PostTitle],
                [PostContent],
                [PostCreated],
                [PostUpdated] 
                FROM [TutorialAppSchema].[Posts]
                WHERE PostId = '" + postId + "'";
            return _dapper.LoadDataSingle<Post>(sql);
        }

        [HttpGet("Posts/PostByUser/{userId}")]
        public IEnumerable<Post> GetPostsByUser(int userId)
        {
            string sql = @" SELECT 
                [PostId],
                [UserId],
                [PostTitle],
                [PostContent],
                [PostCreated],
                [PostUpdated] 
                FROM [TutorialAppSchema].[Posts]
                WHERE UserId = '" + userId + "'";
            return _dapper.LoadData<Post>(sql);
        }

        [HttpGet("MyPosts")]
        public IEnumerable<Post> GetMyPosts(int userId)
        {
            string sql = @" SELECT 
                [PostId],
                [UserId],
                [PostTitle],
                [PostContent],
                [PostCreated],
                [PostUpdated] 
                FROM [TutorialAppSchema].[Posts]
                WHERE PostId =" + this.User.FindFirst("userId")?.Value;


            return _dapper.LoadData<Post>(sql);
        }

        [HttpPost("Posts")]
        public IActionResult AddPost(PostToAddDto postToAdd)
        {
            string sql = @" INSERT INTO TutorialAppSchema.Posts (
                                    [UserId],
                                    [PostTitle],
                                    [PostContent],
                                    [PostCreated],
                                    [PostUpdated] 
                                    ) VALUES (" +
                                this.User.FindFirst("userId")?.Value +
                                ",'" + postToAdd.PostTitle + "'" +
                                ",'" + postToAdd.PostContent + "'" +
                                ", GETDATE()" +
                                ", GETDATE()" +
                                ")";

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to create new post");
        }

        [HttpPut("Posts")]
        public IActionResult Edit(PostToEditDto postToEdit)
        {
            string sql = @" UPDATE TutorialAppSchema.Posts 
                                SET 
                                PostContent = '" + postToEdit.PostContent + "'," +
                                "PostTitle = '" + postToEdit.PostTitle + "'," +
                                "PostUpdated = GETDATE()" +
                                "WHERE PostId = " + postToEdit.PostId +
                                "AND UserId = " + this.User.FindFirst("userId")?.Value;

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to create new post");
        }

        [HttpDelete("Post/{postId}")]
        public IActionResult DeletePost(int postId)
        {

            string sql = @"DELETE TutorialAppSchema.Posts 
                        WHERE PostId = " + postId.ToString() +
                        "AND UserId = " + this.User.FindFirst("userId")?.Value;

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to delete post");
        }

        [HttpGet("PostBySearch/{SearchParam}")]
        public IEnumerable<Post> PostBySearch(string SearchParam)
        {
            string sql = @" SELECT * FROM [TutorialAppSchema].[Posts]
                            WHERE PostTitle LIKE '%" + SearchParam + " %'" +
                            "OR PostContent LIKE '%" + SearchParam + " %'";
            return _dapper.LoadData<Post>(sql);
        }
    }
}