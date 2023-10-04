USE DotNetCourseDatabase
GO

--EXEC TutorialAppSchema.spPost_Delete @PostId=1006, @UserId=2001
CREATE OR ALTER PROCEDURE TutorialAppSchema.spPost_Delete 
    @PostId INT,
    @UserId INT
AS 
BEGIN
    DELETE FROM TutorialAppSchema.Posts WHERE PostId = @PostId
        AND Posts.UserId = @UserId
END