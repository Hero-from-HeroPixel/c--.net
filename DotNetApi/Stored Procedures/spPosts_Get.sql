USE DotNetCourseDatabase
GO


-- EXECUTE TutorialAppSchema.spPosts_Get @PostId = 2, @UserId = 1003 
-- EXECUTE TutorialAppSchema.spPosts_Get @UserId = 1003, @SearchParam = 'second' 
-- EXECUTE TutorialAppSchema.spPosts_Get @PostId = 2
-- EXECUTE TutorialAppSchema.spPosts_Get
CREATE OR ALTER PROCEDURE TutorialAppSchema.spPosts_Get
    @UserId INT = NULL ,
    @SearchParam NVARCHAR(MAX) = NULL,
    @PostId INT = NULL
AS
BEGIN
    SELECT  [Posts].[PostId],
            [Posts].[UserId],
            [Posts].[PostTitle],
            [Posts].[PostContent],
            [Posts].[PostCreated],
            [Posts].[PostUpdated] 
    FROM TutorialAppSchema.Posts AS Posts
        WHERE Posts.UserId = ISNULL(@UserId, Posts.UserId)
         AND Posts.PostId = ISNULL(@PostId, Posts.PostId)
         AND (@SearchParam IS NULL 
            OR Posts.PostContent LIKE '%' + @SearchParam + '%' 
            OR Posts.PostTitle LIKE '%' + @SearchParam + '%'
            )
END