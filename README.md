# Simple Dependency Free Data Access Code Generator

## How to generate

    Generator.SimpleDataAccess.exe connectionstring outputfile namespace classname

    Generator.SimpleDataAccess.exe "Data Source=POWERPC\SQLEXPRESS;Initial Catalog=Chinook;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False" "..\\..\\..\\Generator.TestConsole\\ChinookDatabase.cs" "Generator.SimpleDataAccess.Samples" "ChinookDatabase"

## How to use generated code

    String defaultConnectionString = ConfigurationManager.ConnectionStrings["Database"].ConnectionString;
    using (ChinookDatabase database = new ChinookDatabase(defaultConnectionString))
    {
        database.BeginTransaction();

        // stored procedure
        {
            int genreIdentity;
            database.ExecuteInsertGenre("Favorite", out genreIdentity);
        }

        // select all
        List<Artist> artists = database.SelectArtist();

        // select insert, upsert, update, delete
        if (artists.Where(a => "Heaven Street Seven".Equals(a.Name, StringComparison.InvariantCultureIgnoreCase)).Any() == false)
        {
            Artist artist1 = new Artist();
            artist1.Name = "Heaven Street Seven";

            database.InsertArtist(artist1);

            Album album1 = new Album();
            album1.ArtistId = artist1.ArtistId;
            album1.Title = "Tick Tock No Fear";

            database.InsertAlbum(album1);

            Album album2 = new Album();
            album2.ArtistId = artist1.ArtistId;
            album2.Title = "Goal";

            database.InsertAlbum(album2);

            Album album3 = new Album();
            album3.ArtistId = artist1.ArtistId;
            album3.Title = "Budapest Dolls";

            database.InsertAlbum(album3);

            Album album4 = new Album();
            album4.ArtistId = artist1.ArtistId;
            album4.Title = "Cukor";

            database.UpsertAlbum(album4);

            Album album5 = new Album();
            album5.ArtistId = artist1.ArtistId;
            album5.Title = "Kisfilmek a nagyvilágból - 2002";

            database.InsertAlbum(album5);

            album5.Title = "Kisfilmek a nagyvilágból";

            database.UpdateAlbum(album5);

            database.DeleteAlbumByAlbumId(album5.AlbumId);
        }
                
        // computed columns
        Customer customer = new Customer();
        customer.Address = "Alma u. 1";
        customer.City = "Budapest";
        customer.Company = null;
        customer.Country = "Magyarország";
        customer.Email = "robb83@gmail.com";
        customer.Fax = null;
        customer.FirstName = "Róbert";
        customer.LastName = "Kovács";
        customer.Phone = null;
        customer.PostalCode = "1188";
        customer.State = null;
        customer.SupportRepId = null;

        database.InsertCustomer(customer);

        int customerID = customer.CustomerId;
        String fullName = customer.FullName;
        String fullDetail = customer.FullDetail;

        customer.LastName = "Oláh";

        database.UpdateCustomer(customer);
                
        // pageing

        int artitsCount = database.SelectArtistCount();
        int pageSize = 10, pageIndex = 0;
        while(true)
        {
            List<Artist> page = database.SelectArtistPaged(pageIndex * pageSize + 1, pageIndex * pageSize + pageSize - 1);

            if (page.Count == 0)
            {
                break;
            }

            ++pageIndex;
        }

        database.RollbackTransaction();
    }

## Example of Generated code

    public partial class Album
    {
        public System.Int32 AlbumId { get; set; }
        public System.String Title { get; set; }
        public System.Int32 ArtistId { get; set; }
    }

    #region Upsert, Insert, Update, Delete, Select, Mapping - dbo.Album

    public static Album ReadAlbum(System.Data.SqlClient.SqlDataReader reader)
    {
        Album entity = new Album();
        entity.AlbumId = reader.GetInt32(0);
        entity.Title = reader.GetString(1);
        entity.ArtistId = reader.GetInt32(2);
        return entity;
    }

    public void UpsertAlbum(Album entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException("entity");
        }

        using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("MERGE [dbo].[Album] AS T USING (SELECT @AlbumId, @Title, @ArtistId) AS S (AlbumId, [Title], [ArtistId]) ON (S.[AlbumId] = T.[AlbumId]) WHEN MATCHED THEN UPDATE SET [Title] = S.[Title], [ArtistId] = S.[ArtistId] WHEN NOT MATCHED THEN INSERT ([Title], [ArtistId]) VALUES (S.[Title], S.[ArtistId]) OUTPUT inserted.[AlbumId];"))
        {
            try
            {
                PopConnection(command);
                System.Data.SqlClient.SqlParameter pAlbumId = command.Parameters.Add("@AlbumId", System.Data.SqlDbType.Int);
                pAlbumId.Value = entity.AlbumId;

                System.Data.SqlClient.SqlParameter pTitle = command.Parameters.Add("@Title", System.Data.SqlDbType.NVarChar, 320);
                pTitle.Value = entity.Title;

                System.Data.SqlClient.SqlParameter pArtistId = command.Parameters.Add("@ArtistId", System.Data.SqlDbType.Int);
                pArtistId.Value = entity.ArtistId;

                using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        entity.AlbumId = reader.GetInt32(0);
                    }
                    else
                    {
                        throw new InvalidOperationException("Upsert failed.");
                    }
                }
            }
            finally
            {
                PushConnection(command);
            }
        }
    }

    public void InsertAlbum(Album entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException("entity");
        }

        using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("INSERT INTO [dbo].[Album] ([Title], [ArtistId]) VALUES (@Title, @ArtistId); SET @AlbumId = SCOPE_IDENTITY();"))
        {
            try
            {
                PopConnection(command);
                System.Data.SqlClient.SqlParameter pAlbumId = command.Parameters.Add("@AlbumId", System.Data.SqlDbType.Int);
                pAlbumId.Direction = System.Data.ParameterDirection.Output;

                System.Data.SqlClient.SqlParameter pTitle = command.Parameters.Add("@Title", System.Data.SqlDbType.NVarChar, 320);
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

        using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("UPDATE [dbo].[Album] SET [Title] = @Title, [ArtistId] = @ArtistId WHERE [AlbumId] = @AlbumId;"))
        {
            try
            {
                PopConnection(command);
                System.Data.SqlClient.SqlParameter pAlbumId = command.Parameters.Add("@AlbumId", System.Data.SqlDbType.Int);
                pAlbumId.Value = entity.AlbumId;

                System.Data.SqlClient.SqlParameter pTitle = command.Parameters.Add("@Title", System.Data.SqlDbType.NVarChar, 320);
                pTitle.Value = entity.Title;

                System.Data.SqlClient.SqlParameter pArtistId = command.Parameters.Add("@ArtistId", System.Data.SqlDbType.Int);
                pArtistId.Value = entity.ArtistId;

                if (command.ExecuteNonQuery() > 0)
                {
                }
                else
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

    public int SelectAlbumCount()
    {
        using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT Count(*) FROM [dbo].[Album]"))
        {
            try
            {
                PopConnection(command);

                using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetInt32(0);
                    }
                    else
                    {
                        throw new InvalidOperationException("Select count failed.");
                    }
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