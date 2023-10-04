USE DotNetCourseDatabase
GO

CREATE OR ALTER PROCEDURE TutorialAppSchema.spUser_Upsert
	@FirstName NVARCHAR (50),
	@LastName NVARCHAR (50),
	@Email NVARCHAR (50),
	@Gender NVARCHAR (50),
	@JobTitle NVARCHAR (50),
	@JobDepartment NVARCHAR (50),
	@Salary DECIMAL(18,4),
	@Active BIT = 1,
    @UserId INT = NULL

AS
BEGIN
	SELECT * FROM TutorialAppSchema.UserSalary
	SELECT * FROM TutorialAppSchema.UserJobInfo


	IF NOT EXISTS (SELECT * FROM TutorialAppSchema.Users WHERE Users.UserId = @UserId)
		BEGIN
			IF NOT EXISTS (SELECT * FROM TutorialAppSchema.Users WHERE Users.Email = @Email)
			BEGIN
				
				DECLARE @OutputUserId INT

				INSERT INTO TutorialAppSchema.Users(
				[FirstName],
				[LastName],
				[Email],
				[Gender],
				[Active]
				) VALUES (
					@FirstName,
					@LastName,
					@Email,
					@Gender,
					@Active
				)
				SET @OutputUserId = @@IDENTITY

				INSERT INTO TutorialAppSchema.UserSalary(
					UserId,
					Salary
				) VALUES (
					@OutputUserId,
					@Salary
				)

				INSERT INTO TutorialAppSchema.UserJobInfo(
					UserId,
					Department,
					JobTitle
				) VALUES (
					@OutputUserId,
					@JobDepartment,
					@JobTitle
				)

			END
		END
	ELSE
		BEGIN 
			UPDATE TutorialAppSchema.Users 
				SET 
				FirstName = @FirstName,
				LastName = @LastName,
				Email = @Email,
				Gender = @Gender,
				Active = @Active
			WHERE UserId = @UserId

			UPDATE TutorialAppSchema.UserSalary 
				SET 
					Salary = @Salary
				WHERE UserId = @UserId

			UPDATE TutorialAppSchema.UserJobInfo 
				SET 
					Department = @JobDepartment,
					JobTitle = @JobTitle
				WHERE UserId = @UserId
		END	
END