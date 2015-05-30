CREATE PROCEDURE [dbo].[InsertGenre]
	@Name nvarchar(120),
	@GenreId INT OUTPUT
AS
BEGIN
	
	INSERT INTO [dbo].[Genre] (Name) VALUES(@Name)

	SET @GenreId = SCOPE_IDENTITY()

END