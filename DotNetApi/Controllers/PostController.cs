using DotNetApi.Data;
using DotNetApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("controller")]
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
        public IEnumerable<Post> GetSinglePost(int postId)
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
            return _dapper.LoadData<Post>(sql);
        }

        [HttpGet("Posts/{userId}")]
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
                WHERE PostId = '" + userId + "'";
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
                WHERE PostId =" + User.FindFirst("userId")?.Value;
            return _dapper.LoadData<Post>(sql);
        }
    }
}