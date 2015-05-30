# Simple Dependency Free Data Access Code Generator

## How to generate

    Generator.SimpleDataAccess.exe connectionstring outputfile namespace classname

    Generator.SimpleDataAccess.exe "Data Source=POWERPC\SQLEXPRESS;Initial Catalog=Chinook;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False" "..\\..\\..\\Generator.TestConsole\\ChinookDatabase.cs" "Generator.SimpleDataAccess.Samples" "ChinookDatabase"

## How to use generated code

    String defaultConnectionString = ConfigurationManager.ConnectionStrings["Database"].ConnectionString;
    using (ChinookDatabase database = new ChinookDatabase(defaultConnectionString))
    {
        database.BeginTransaction();
                
        List<Album> albums = database.SelectAlbum();
                
        Artist artist = new Artist();
        artist.Name = "Heaven Street Seven";

        database.InsertArtist(artist);

        Album album = new Album();
        album.ArtistId = artist.ArtistId;
        album.Title = "Gesztenyefák alatt";

        database.InsertAlbum(album);

        album.Title = "Felkeltem a Reggelt";
        database.UpdateAlbum(album);

        database.DeleteAlbumByAlbumId(album.AlbumId);

        int genreCount = database.SelectGenreCount();
        if (genreCount > 0)
        {
            int genreIdentity;
            database.ExecuteInsertGenre("Favorite", out genreIdentity);
        }


        Artist artist2 = new Artist();
        artist2.Name = "Kis Pál és a borz";

        database.UpsertArtist(artist2);

        artist2.Name = "Kispál és a Borz";

        database.UpsertArtist(artist2);

        database.RollbackTransaction();
    }

## Example of Generated code

    public partial class Album
    {
        public System.Int32 AlbumId { get; set; }
        public System.String Title { get; set; }
        public System.Int32 ArtistId { get; set; }
    }

    #region Insert, Update, Delete, Select, Mapping - dbo.Album

    public static Album ReadAlbum(System.Data.SqlClient.SqlDataReader reader)
    {
        Album entity = new Album();
        entity.AlbumId = reader.GetInt32(0);
        entity.Title = reader.GetString(1);
        entity.ArtistId = reader.GetInt32(2);
        return entity;
    }

    public void InsertAlbum(Album entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException("entity");
        }

        using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("INSERT INTO [dbo].[Album] ([Title], [ArtistId]) VALUES (@Title, @ArtistId); SET @AlbumId = SCOPE_IDENTITY(); "))
        {
            try
            {
                PopConnection(command);
                System.Data.SqlClient.SqlParameter pAlbumId = command.Parameters.Add("@AlbumId", System.Data.SqlDbType.Int);
                pAlbumId.Direction = System.Data.ParameterDirection.Output;

                System.Data.SqlClient.SqlParameter pTitle = command.Parameters.Add("@Title", System.Data.SqlDbType.NVarChar);
                pTitle.Value = entity.Title;

                System.Data.SqlClient.SqlParameter pArtistId = command.Parameters.Add("@ArtistId", System.Data.SqlDbType.Int);
                pArtistId.Value = entity.ArtistId;

                if (command.ExecuteNonQuery() > 0)
                {
                    entity.AlbumId = (System.Int32)pAlbumId.Value;
                }
                else
                {
                    throw new InvalidOperationException("Insert failed.");
                }
            }
            finally
            {
                PushConnection(command);
            }
        }
    }

    public void UpdateAlbum(Album entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException("entity");
        }

        using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("UPDATE [dbo].[Album] SET [Title] = @Title, [ArtistId] = @ArtistId WHERE [AlbumId] = @AlbumId"))
        {
            try
            {
                PopConnection(command);
                System.Data.SqlClient.SqlParameter pAlbumId = command.Parameters.Add("@AlbumId", System.Data.SqlDbType.Int);
                pAlbumId.Value = entity.AlbumId;

                System.Data.SqlClient.SqlParameter pTitle = command.Parameters.Add("@Title", System.Data.SqlDbType.NVarChar);
                pTitle.Value = entity.Title;

                System.Data.SqlClient.SqlParameter pArtistId = command.Parameters.Add("@ArtistId", System.Data.SqlDbType.Int);
                pArtistId.Value = entity.ArtistId;

                if (command.ExecuteNonQuery() <= 0)
                {
                    throw new InvalidOperationException("Update failed.");
                }
            }
            finally
            {
                PushConnection(command);
            }
        }
    }

    public List<Album> SelectAlbum()
    {
        using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [AlbumId], [Title], [ArtistId] FROM [dbo].[Album]"))
        {
            try
            {
                PopConnection(command);
                using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                {
                    List<Album> result = new List<Album>();
                    while (reader.Read())
                    {
                        result.Add(ReadAlbum(reader));
                    }
                    return result;
                }
            }
            finally
            {
                PushConnection(command);
            }
        }
    }

    public List<Album> SelectAlbumByArtistId(System.Int32 artistId)
    {
        using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [AlbumId], [Title], [ArtistId] FROM [dbo].[Album] WHERE [ArtistId] = @ArtistId"))
        {
            try
            {
                PopConnection(command);
                System.Data.SqlClient.SqlParameter pArtistId = command.Parameters.Add("@ArtistId", System.Data.SqlDbType.Int);
                pArtistId.Value = artistId;

                using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                {
                    List<Album> result = new List<Album>();
                    while (reader.Read())
                    {
                        result.Add(ReadAlbum(reader));
                    }
                    return result;
                }
            }
            finally
            {
                PushConnection(command);
            }
        }
    }

    public void DeleteAlbumByArtistId(System.Int32 artistId)
    {
        using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("DELETE FROM [dbo].[Album] WHERE [ArtistId] = @ArtistId"))
        {
            try
            {
                PopConnection(command);
                System.Data.SqlClient.SqlParameter pArtistId = command.Parameters.Add("@ArtistId", System.Data.SqlDbType.Int);
                pArtistId.Value = artistId;

                command.ExecuteNonQuery();
            }
            finally
            {
                PushConnection(command);
            }
        }
    }

    public Album SelectAlbumByAlbumId(System.Int32 albumId)
    {
        using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [AlbumId], [Title], [ArtistId] FROM [dbo].[Album] WHERE [AlbumId] = @AlbumId"))
        {
            try
            {
                PopConnection(command);
                System.Data.SqlClient.SqlParameter pAlbumId = command.Parameters.Add("@AlbumId", System.Data.SqlDbType.Int);
                pAlbumId.Value = albumId;

                using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadAlbum(reader);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            finally
            {
                PushConnection(command);
            }
        }
    }

    public void DeleteAlbumByAlbumId(System.Int32 albumId)
    {
        using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("DELETE FROM [dbo].[Album] WHERE [AlbumId] = @AlbumId"))
        {
            try
            {
                PopConnection(command);
                System.Data.SqlClient.SqlParameter pAlbumId = command.Parameters.Add("@AlbumId", System.Data.SqlDbType.Int);
                pAlbumId.Value = albumId;

                command.ExecuteNonQuery();
            }
            finally
            {
                PushConnection(command);
            }
        }
    }

    #endregion

    #region Stored Procedures

    public void ExecuteInsertGenre(System.String name, out System.Int32 genreId)
    {
        BeginTransaction();

        try
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("[dbo].[InsertGenre]"))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;

                try
                {
                    PopConnection(command);

                    System.Data.SqlClient.SqlParameter pName = command.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar);
                    pName.Value = name;

                    System.Data.SqlClient.SqlParameter pGenreId = command.Parameters.Add("@GenreId", System.Data.SqlDbType.Int);
                    pGenreId.Direction = System.Data.ParameterDirection.Output;

                    command.ExecuteNonQuery();

                    genreId = (System.Int32)pGenreId.Value;
                }
                finally
                {
                    PushConnection(command);
                }
            }

            CommitTransaction();
        }
        catch
        {
            RollbackTransaction();
            throw;
        }
    }
    #endregion

## Sample database: 

<https://chinookdatabase.codeplex.com/>

## More
<http://robb83.github.io>