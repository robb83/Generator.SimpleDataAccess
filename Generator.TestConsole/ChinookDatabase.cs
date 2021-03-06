using System;
using System.Collections.Generic;

namespace Generator.SimpleDataAccess.Samples
{
    #region Models

    public partial class Album
    {
        public System.Int32 AlbumId { get; set; }
        public System.String Title { get; set; }
        public System.Int32 ArtistId { get; set; }
    }

    public partial class Artist
    {
        public System.Int32 ArtistId { get; set; }
        public System.String Name { get; set; }
    }

    public partial class Customer
    {
        public System.Int32 CustomerId { get; set; }
        public System.String FirstName { get; set; }
        public System.String LastName { get; set; }
        public System.String Company { get; set; }
        public System.String Address { get; set; }
        public System.String City { get; set; }
        public System.String State { get; set; }
        public System.String Country { get; set; }
        public System.String PostalCode { get; set; }
        public System.String Phone { get; set; }
        public System.String Fax { get; set; }
        public System.String Email { get; set; }
        public System.Nullable<System.Int32> SupportRepId { get; set; }
        public System.String FullName { get; set; }
        public System.String Comment { get; set; }
        public System.String FullDetail { get; set; }
    }

    public partial class Employee
    {
        public System.Int32 EmployeeId { get; set; }
        public System.String LastName { get; set; }
        public System.String FirstName { get; set; }
        public System.String Title { get; set; }
        public System.Nullable<System.Int32> ReportsTo { get; set; }
        public System.Nullable<System.DateTime> BirthDate { get; set; }
        public System.Nullable<System.DateTime> HireDate { get; set; }
        public System.String Address { get; set; }
        public System.String City { get; set; }
        public System.String State { get; set; }
        public System.String Country { get; set; }
        public System.String PostalCode { get; set; }
        public System.String Phone { get; set; }
        public System.String Fax { get; set; }
        public System.String Email { get; set; }
    }

    public partial class Genre
    {
        public System.Int32 GenreId { get; set; }
        public System.String Name { get; set; }
    }

    public partial class Invoice
    {
        public System.Int32 InvoiceId { get; set; }
        public System.Int32 CustomerId { get; set; }
        public System.DateTime InvoiceDate { get; set; }
        public System.String BillingAddress { get; set; }
        public System.String BillingCity { get; set; }
        public System.String BillingState { get; set; }
        public System.String BillingCountry { get; set; }
        public System.String BillingPostalCode { get; set; }
        public System.Decimal Total { get; set; }
    }

    public partial class InvoiceLine
    {
        public System.Int32 InvoiceLineId { get; set; }
        public System.Int32 InvoiceId { get; set; }
        public System.Int32 TrackId { get; set; }
        public System.Decimal UnitPrice { get; set; }
        public System.Int32 Quantity { get; set; }
    }

    public partial class MediaType
    {
        public System.Int32 MediaTypeId { get; set; }
        public System.String Name { get; set; }
    }

    public partial class Playlist
    {
        public System.Int32 PlaylistId { get; set; }
        public System.String Name { get; set; }
    }

    public partial class PlaylistTrack
    {
        public System.Int32 PlaylistId { get; set; }
        public System.Int32 TrackId { get; set; }
    }

    public partial class Track
    {
        public System.Int32 TrackId { get; set; }
        public System.String Name { get; set; }
        public System.Nullable<System.Int32> AlbumId { get; set; }
        public System.Int32 MediaTypeId { get; set; }
        public System.Nullable<System.Int32> GenreId { get; set; }
        public System.String Composer { get; set; }
        public System.Int32 Milliseconds { get; set; }
        public System.Nullable<System.Int32> Bytes { get; set; }
        public System.Decimal UnitPrice { get; set; }
    }

    #endregion

    public partial class ChinookDatabase : IDisposable
    {

        private System.Data.SqlClient.SqlConnection connection;
        private System.Data.SqlClient.SqlTransaction transaction;
        private int transactionCounter;
        private String connectionString;
        private bool externalResource;

        public ChinookDatabase(String connectionString)
        {
            this.externalResource = false;
            this.connectionString = connectionString;
        }

        public ChinookDatabase(System.Data.SqlClient.SqlConnection connection, System.Data.SqlClient.SqlTransaction transaction)
        {
            this.externalResource = true;
            this.connection = connection;
            this.transaction = transaction;
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

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("MERGE [dbo].[Album] AS T USING (SELECT @AlbumId, @Title, @ArtistId) AS S ([AlbumId], [Title], [ArtistId]) ON (S.[AlbumId] = T.[AlbumId]) WHEN MATCHED THEN UPDATE SET [Title] = S.[Title], [ArtistId] = S.[ArtistId] WHEN NOT MATCHED THEN INSERT ([Title], [ArtistId]) VALUES (S.[Title], S.[ArtistId]) OUTPUT inserted.[AlbumId];"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @Title
                    System.Data.SqlClient.SqlParameter pTitle = command.Parameters.Add("@Title", System.Data.SqlDbType.NVarChar, 320);
                    pTitle.Value = entity.Title;

                    // Parameter settings: @ArtistId
                    System.Data.SqlClient.SqlParameter pArtistId = command.Parameters.Add("@ArtistId", System.Data.SqlDbType.Int);
                    pArtistId.Value = entity.ArtistId;

                    // Parameter settings: @AlbumId
                    System.Data.SqlClient.SqlParameter pAlbumId = command.Parameters.Add("@AlbumId", System.Data.SqlDbType.Int);
                    pAlbumId.Value = entity.AlbumId;

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

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("INSERT INTO [dbo].[Album] ([Title], [ArtistId])  OUTPUT inserted.[AlbumId] VALUES (@Title, @ArtistId);"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @Title
                    System.Data.SqlClient.SqlParameter pTitle = command.Parameters.Add("@Title", System.Data.SqlDbType.NVarChar, 320);
                    pTitle.Value = entity.Title;

                    // Parameter settings: @ArtistId
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
                            throw new InvalidOperationException("Insert failed.");
                        }
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

                    // Parameter settings: @AlbumId
                    System.Data.SqlClient.SqlParameter pAlbumId = command.Parameters.Add("@AlbumId", System.Data.SqlDbType.Int);
                    pAlbumId.Value = entity.AlbumId;

                    // Parameter settings: @Title
                    System.Data.SqlClient.SqlParameter pTitle = command.Parameters.Add("@Title", System.Data.SqlDbType.NVarChar, 320);
                    pTitle.Value = entity.Title;

                    // Parameter settings: @ArtistId
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

        public List<Album> SelectAlbumPaged(int firstIndex, int lastIndex)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT * FROM ( SELECT [AlbumId], [Title], [ArtistId], ROW_NUMBER() OVER(ORDER BY [AlbumId]) AS _ROW_NUMBER FROM [dbo].[Album] ) AS T0 WHERE _ROW_NUMBER BETWEEN @__FirstIndex AND @__LastIndex ORDER BY [AlbumId]"))
            {
                try
                {
                    PopConnection(command);

                    System.Data.SqlClient.SqlParameter pFirstIndex = command.Parameters.Add("@__FirstIndex", System.Data.SqlDbType.Int);
                    pFirstIndex.Value = firstIndex;

                    System.Data.SqlClient.SqlParameter pLastIndex = command.Parameters.Add("@__LastIndex", System.Data.SqlDbType.Int);
                    pLastIndex.Value = lastIndex;

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

                    // Parameter settings: @ArtistId
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

                    // Parameter settings: @ArtistId
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

                    // Parameter settings: @AlbumId
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

                    // Parameter settings: @AlbumId
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

        #region Upsert, Insert, Update, Delete, Select, Mapping - dbo.Artist

        public static Artist ReadArtist(System.Data.SqlClient.SqlDataReader reader)
        {
            Artist entity = new Artist();
            entity.ArtistId = reader.GetInt32(0);
            entity.Name = reader.GetString(1);
            return entity;
        }

        public void UpsertArtist(Artist entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("MERGE [dbo].[Artist] AS T USING (SELECT @ArtistId, @Name) AS S ([ArtistId], [Name]) ON (S.[ArtistId] = T.[ArtistId]) WHEN MATCHED THEN UPDATE SET [Name] = S.[Name] WHEN NOT MATCHED THEN INSERT ([Name]) VALUES (S.[Name]) OUTPUT inserted.[ArtistId];"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @Name
                    System.Data.SqlClient.SqlParameter pName = command.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar, 240);
                    if (entity.Name == null)
                    {
                        pName.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pName.Value = entity.Name;
                    }

                    // Parameter settings: @ArtistId
                    System.Data.SqlClient.SqlParameter pArtistId = command.Parameters.Add("@ArtistId", System.Data.SqlDbType.Int);
                    pArtistId.Value = entity.ArtistId;

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            entity.ArtistId = reader.GetInt32(0);
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

        public void InsertArtist(Artist entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("INSERT INTO [dbo].[Artist] ([Name])  OUTPUT inserted.[ArtistId] VALUES (@Name);"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @Name
                    System.Data.SqlClient.SqlParameter pName = command.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar, 240);
                    if (entity.Name == null)
                    {
                        pName.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pName.Value = entity.Name;
                    }

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            entity.ArtistId = reader.GetInt32(0);
                        }
                        else
                        {
                            throw new InvalidOperationException("Insert failed.");
                        }
                    }
                }
                finally
                {
                    PushConnection(command);
                }
            }
        }

        public void UpdateArtist(Artist entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("UPDATE [dbo].[Artist] SET [Name] = @Name WHERE [ArtistId] = @ArtistId;"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @ArtistId
                    System.Data.SqlClient.SqlParameter pArtistId = command.Parameters.Add("@ArtistId", System.Data.SqlDbType.Int);
                    pArtistId.Value = entity.ArtistId;

                    // Parameter settings: @Name
                    System.Data.SqlClient.SqlParameter pName = command.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar, 240);
                    if (entity.Name == null)
                    {
                        pName.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pName.Value = entity.Name;
                    }

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

        public List<Artist> SelectArtist()
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [ArtistId], [Name] FROM [dbo].[Artist]"))
            {
                try
                {
                    PopConnection(command);

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        List<Artist> result = new List<Artist>();
                        while (reader.Read())
                        {
                            result.Add(ReadArtist(reader));
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

        public int SelectArtistCount()
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT Count(*) FROM [dbo].[Artist]"))
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

        public List<Artist> SelectArtistPaged(int firstIndex, int lastIndex)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT * FROM ( SELECT [ArtistId], [Name], ROW_NUMBER() OVER(ORDER BY [ArtistId]) AS _ROW_NUMBER FROM [dbo].[Artist] ) AS T0 WHERE _ROW_NUMBER BETWEEN @__FirstIndex AND @__LastIndex ORDER BY [ArtistId]"))
            {
                try
                {
                    PopConnection(command);

                    System.Data.SqlClient.SqlParameter pFirstIndex = command.Parameters.Add("@__FirstIndex", System.Data.SqlDbType.Int);
                    pFirstIndex.Value = firstIndex;

                    System.Data.SqlClient.SqlParameter pLastIndex = command.Parameters.Add("@__LastIndex", System.Data.SqlDbType.Int);
                    pLastIndex.Value = lastIndex;

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        List<Artist> result = new List<Artist>();
                        while (reader.Read())
                        {
                            result.Add(ReadArtist(reader));
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

        public Artist SelectArtistByArtistId(System.Int32 artistId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [ArtistId], [Name] FROM [dbo].[Artist] WHERE [ArtistId] = @ArtistId"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @ArtistId
                    System.Data.SqlClient.SqlParameter pArtistId = command.Parameters.Add("@ArtistId", System.Data.SqlDbType.Int);
                    pArtistId.Value = artistId;

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return ReadArtist(reader);
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

        public void DeleteArtistByArtistId(System.Int32 artistId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("DELETE FROM [dbo].[Artist] WHERE [ArtistId] = @ArtistId"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @ArtistId
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

        #endregion

        #region Upsert, Insert, Update, Delete, Select, Mapping - dbo.Customer

        public static Customer ReadCustomer(System.Data.SqlClient.SqlDataReader reader)
        {
            Customer entity = new Customer();
            entity.CustomerId = reader.GetInt32(0);
            entity.FirstName = reader.GetString(1);
            entity.LastName = reader.GetString(2);
            entity.Company = reader.GetString(3);
            entity.Address = reader.GetString(4);
            entity.City = reader.GetString(5);
            entity.State = reader.GetString(6);
            entity.Country = reader.GetString(7);
            entity.PostalCode = reader.GetString(8);
            entity.Phone = reader.GetString(9);
            entity.Fax = reader.GetString(10);
            entity.Email = reader.GetString(11);
            if (reader.IsDBNull(12))
            {
                entity.SupportRepId = null;
            }
            else
            {
                entity.SupportRepId = reader.GetInt32(12);
            }
            entity.FullName = reader.GetString(13);
            entity.Comment = reader.GetString(14);
            entity.FullDetail = reader.GetString(15);
            return entity;
        }

        public void UpsertCustomer(Customer entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("MERGE [dbo].[Customer] AS T USING (SELECT @CustomerId, @FirstName, @LastName, @Company, @Address, @City, @State, @Country, @PostalCode, @Phone, @Fax, @Email, @SupportRepId, @Comment) AS S ([CustomerId], [FirstName], [LastName], [Company], [Address], [City], [State], [Country], [PostalCode], [Phone], [Fax], [Email], [SupportRepId], [Comment]) ON (S.[CustomerId] = T.[CustomerId]) WHEN MATCHED THEN UPDATE SET [FirstName] = S.[FirstName], [LastName] = S.[LastName], [Company] = S.[Company], [Address] = S.[Address], [City] = S.[City], [State] = S.[State], [Country] = S.[Country], [PostalCode] = S.[PostalCode], [Phone] = S.[Phone], [Fax] = S.[Fax], [Email] = S.[Email], [SupportRepId] = S.[SupportRepId], [Comment] = S.[Comment] WHEN NOT MATCHED THEN INSERT ([FirstName], [LastName], [Company], [Address], [City], [State], [Country], [PostalCode], [Phone], [Fax], [Email], [SupportRepId], [Comment]) VALUES (S.[FirstName], S.[LastName], S.[Company], S.[Address], S.[City], S.[State], S.[Country], S.[PostalCode], S.[Phone], S.[Fax], S.[Email], S.[SupportRepId], S.[Comment]) OUTPUT inserted.[CustomerId], inserted.[FullName], inserted.[FullDetail];"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @FirstName
                    System.Data.SqlClient.SqlParameter pFirstName = command.Parameters.Add("@FirstName", System.Data.SqlDbType.NVarChar, 80);
                    pFirstName.Value = entity.FirstName;

                    // Parameter settings: @LastName
                    System.Data.SqlClient.SqlParameter pLastName = command.Parameters.Add("@LastName", System.Data.SqlDbType.NVarChar, 40);
                    pLastName.Value = entity.LastName;

                    // Parameter settings: @Company
                    System.Data.SqlClient.SqlParameter pCompany = command.Parameters.Add("@Company", System.Data.SqlDbType.NVarChar, 160);
                    if (entity.Company == null)
                    {
                        pCompany.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pCompany.Value = entity.Company;
                    }

                    // Parameter settings: @Address
                    System.Data.SqlClient.SqlParameter pAddress = command.Parameters.Add("@Address", System.Data.SqlDbType.NVarChar, 140);
                    if (entity.Address == null)
                    {
                        pAddress.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pAddress.Value = entity.Address;
                    }

                    // Parameter settings: @City
                    System.Data.SqlClient.SqlParameter pCity = command.Parameters.Add("@City", System.Data.SqlDbType.NVarChar, 80);
                    if (entity.City == null)
                    {
                        pCity.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pCity.Value = entity.City;
                    }

                    // Parameter settings: @State
                    System.Data.SqlClient.SqlParameter pState = command.Parameters.Add("@State", System.Data.SqlDbType.NVarChar, 80);
                    if (entity.State == null)
                    {
                        pState.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pState.Value = entity.State;
                    }

                    // Parameter settings: @Country
                    System.Data.SqlClient.SqlParameter pCountry = command.Parameters.Add("@Country", System.Data.SqlDbType.NVarChar, 80);
                    if (entity.Country == null)
                    {
                        pCountry.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pCountry.Value = entity.Country;
                    }

                    // Parameter settings: @PostalCode
                    System.Data.SqlClient.SqlParameter pPostalCode = command.Parameters.Add("@PostalCode", System.Data.SqlDbType.NVarChar, 20);
                    if (entity.PostalCode == null)
                    {
                        pPostalCode.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pPostalCode.Value = entity.PostalCode;
                    }

                    // Parameter settings: @Phone
                    System.Data.SqlClient.SqlParameter pPhone = command.Parameters.Add("@Phone", System.Data.SqlDbType.NVarChar, 48);
                    if (entity.Phone == null)
                    {
                        pPhone.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pPhone.Value = entity.Phone;
                    }

                    // Parameter settings: @Fax
                    System.Data.SqlClient.SqlParameter pFax = command.Parameters.Add("@Fax", System.Data.SqlDbType.NVarChar, 48);
                    if (entity.Fax == null)
                    {
                        pFax.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pFax.Value = entity.Fax;
                    }

                    // Parameter settings: @Email
                    System.Data.SqlClient.SqlParameter pEmail = command.Parameters.Add("@Email", System.Data.SqlDbType.NVarChar, 120);
                    pEmail.Value = entity.Email;

                    // Parameter settings: @SupportRepId
                    System.Data.SqlClient.SqlParameter pSupportRepId = command.Parameters.Add("@SupportRepId", System.Data.SqlDbType.Int);
                    if (entity.SupportRepId == null)
                    {
                        pSupportRepId.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pSupportRepId.Value = entity.SupportRepId;
                    }

                    // Parameter settings: @Comment
                    System.Data.SqlClient.SqlParameter pComment = command.Parameters.Add("@Comment", System.Data.SqlDbType.NVarChar, -1);
                    if (entity.Comment == null)
                    {
                        pComment.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pComment.Value = entity.Comment;
                    }

                    // Parameter settings: @CustomerId
                    System.Data.SqlClient.SqlParameter pCustomerId = command.Parameters.Add("@CustomerId", System.Data.SqlDbType.Int);
                    pCustomerId.Value = entity.CustomerId;

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            entity.CustomerId = reader.GetInt32(0);
                            entity.FullName = reader.GetString(1);
                            entity.FullDetail = reader.GetString(2);
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

        public void InsertCustomer(Customer entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("INSERT INTO [dbo].[Customer] ([FirstName], [LastName], [Company], [Address], [City], [State], [Country], [PostalCode], [Phone], [Fax], [Email], [SupportRepId], [Comment])  OUTPUT inserted.[CustomerId], inserted.[FullName], inserted.[FullDetail] VALUES (@FirstName, @LastName, @Company, @Address, @City, @State, @Country, @PostalCode, @Phone, @Fax, @Email, @SupportRepId, @Comment);"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @FirstName
                    System.Data.SqlClient.SqlParameter pFirstName = command.Parameters.Add("@FirstName", System.Data.SqlDbType.NVarChar, 80);
                    pFirstName.Value = entity.FirstName;

                    // Parameter settings: @LastName
                    System.Data.SqlClient.SqlParameter pLastName = command.Parameters.Add("@LastName", System.Data.SqlDbType.NVarChar, 40);
                    pLastName.Value = entity.LastName;

                    // Parameter settings: @Company
                    System.Data.SqlClient.SqlParameter pCompany = command.Parameters.Add("@Company", System.Data.SqlDbType.NVarChar, 160);
                    if (entity.Company == null)
                    {
                        pCompany.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pCompany.Value = entity.Company;
                    }

                    // Parameter settings: @Address
                    System.Data.SqlClient.SqlParameter pAddress = command.Parameters.Add("@Address", System.Data.SqlDbType.NVarChar, 140);
                    if (entity.Address == null)
                    {
                        pAddress.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pAddress.Value = entity.Address;
                    }

                    // Parameter settings: @City
                    System.Data.SqlClient.SqlParameter pCity = command.Parameters.Add("@City", System.Data.SqlDbType.NVarChar, 80);
                    if (entity.City == null)
                    {
                        pCity.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pCity.Value = entity.City;
                    }

                    // Parameter settings: @State
                    System.Data.SqlClient.SqlParameter pState = command.Parameters.Add("@State", System.Data.SqlDbType.NVarChar, 80);
                    if (entity.State == null)
                    {
                        pState.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pState.Value = entity.State;
                    }

                    // Parameter settings: @Country
                    System.Data.SqlClient.SqlParameter pCountry = command.Parameters.Add("@Country", System.Data.SqlDbType.NVarChar, 80);
                    if (entity.Country == null)
                    {
                        pCountry.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pCountry.Value = entity.Country;
                    }

                    // Parameter settings: @PostalCode
                    System.Data.SqlClient.SqlParameter pPostalCode = command.Parameters.Add("@PostalCode", System.Data.SqlDbType.NVarChar, 20);
                    if (entity.PostalCode == null)
                    {
                        pPostalCode.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pPostalCode.Value = entity.PostalCode;
                    }

                    // Parameter settings: @Phone
                    System.Data.SqlClient.SqlParameter pPhone = command.Parameters.Add("@Phone", System.Data.SqlDbType.NVarChar, 48);
                    if (entity.Phone == null)
                    {
                        pPhone.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pPhone.Value = entity.Phone;
                    }

                    // Parameter settings: @Fax
                    System.Data.SqlClient.SqlParameter pFax = command.Parameters.Add("@Fax", System.Data.SqlDbType.NVarChar, 48);
                    if (entity.Fax == null)
                    {
                        pFax.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pFax.Value = entity.Fax;
                    }

                    // Parameter settings: @Email
                    System.Data.SqlClient.SqlParameter pEmail = command.Parameters.Add("@Email", System.Data.SqlDbType.NVarChar, 120);
                    pEmail.Value = entity.Email;

                    // Parameter settings: @SupportRepId
                    System.Data.SqlClient.SqlParameter pSupportRepId = command.Parameters.Add("@SupportRepId", System.Data.SqlDbType.Int);
                    if (entity.SupportRepId == null)
                    {
                        pSupportRepId.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pSupportRepId.Value = entity.SupportRepId;
                    }

                    // Parameter settings: @Comment
                    System.Data.SqlClient.SqlParameter pComment = command.Parameters.Add("@Comment", System.Data.SqlDbType.NVarChar, -1);
                    if (entity.Comment == null)
                    {
                        pComment.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pComment.Value = entity.Comment;
                    }

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            entity.CustomerId = reader.GetInt32(0);
                            entity.FullName = reader.GetString(1);
                            entity.FullDetail = reader.GetString(2);
                        }
                        else
                        {
                            throw new InvalidOperationException("Insert failed.");
                        }
                    }
                }
                finally
                {
                    PushConnection(command);
                }
            }
        }

        public void UpdateCustomer(Customer entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("UPDATE [dbo].[Customer] SET [FirstName] = @FirstName, [LastName] = @LastName, [Company] = @Company, [Address] = @Address, [City] = @City, [State] = @State, [Country] = @Country, [PostalCode] = @PostalCode, [Phone] = @Phone, [Fax] = @Fax, [Email] = @Email, [SupportRepId] = @SupportRepId, [Comment] = @Comment OUTPUT inserted.[FullName], inserted.[FullDetail] WHERE [CustomerId] = @CustomerId;"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @CustomerId
                    System.Data.SqlClient.SqlParameter pCustomerId = command.Parameters.Add("@CustomerId", System.Data.SqlDbType.Int);
                    pCustomerId.Value = entity.CustomerId;

                    // Parameter settings: @FirstName
                    System.Data.SqlClient.SqlParameter pFirstName = command.Parameters.Add("@FirstName", System.Data.SqlDbType.NVarChar, 80);
                    pFirstName.Value = entity.FirstName;

                    // Parameter settings: @LastName
                    System.Data.SqlClient.SqlParameter pLastName = command.Parameters.Add("@LastName", System.Data.SqlDbType.NVarChar, 40);
                    pLastName.Value = entity.LastName;

                    // Parameter settings: @Company
                    System.Data.SqlClient.SqlParameter pCompany = command.Parameters.Add("@Company", System.Data.SqlDbType.NVarChar, 160);
                    if (entity.Company == null)
                    {
                        pCompany.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pCompany.Value = entity.Company;
                    }

                    // Parameter settings: @Address
                    System.Data.SqlClient.SqlParameter pAddress = command.Parameters.Add("@Address", System.Data.SqlDbType.NVarChar, 140);
                    if (entity.Address == null)
                    {
                        pAddress.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pAddress.Value = entity.Address;
                    }

                    // Parameter settings: @City
                    System.Data.SqlClient.SqlParameter pCity = command.Parameters.Add("@City", System.Data.SqlDbType.NVarChar, 80);
                    if (entity.City == null)
                    {
                        pCity.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pCity.Value = entity.City;
                    }

                    // Parameter settings: @State
                    System.Data.SqlClient.SqlParameter pState = command.Parameters.Add("@State", System.Data.SqlDbType.NVarChar, 80);
                    if (entity.State == null)
                    {
                        pState.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pState.Value = entity.State;
                    }

                    // Parameter settings: @Country
                    System.Data.SqlClient.SqlParameter pCountry = command.Parameters.Add("@Country", System.Data.SqlDbType.NVarChar, 80);
                    if (entity.Country == null)
                    {
                        pCountry.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pCountry.Value = entity.Country;
                    }

                    // Parameter settings: @PostalCode
                    System.Data.SqlClient.SqlParameter pPostalCode = command.Parameters.Add("@PostalCode", System.Data.SqlDbType.NVarChar, 20);
                    if (entity.PostalCode == null)
                    {
                        pPostalCode.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pPostalCode.Value = entity.PostalCode;
                    }

                    // Parameter settings: @Phone
                    System.Data.SqlClient.SqlParameter pPhone = command.Parameters.Add("@Phone", System.Data.SqlDbType.NVarChar, 48);
                    if (entity.Phone == null)
                    {
                        pPhone.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pPhone.Value = entity.Phone;
                    }

                    // Parameter settings: @Fax
                    System.Data.SqlClient.SqlParameter pFax = command.Parameters.Add("@Fax", System.Data.SqlDbType.NVarChar, 48);
                    if (entity.Fax == null)
                    {
                        pFax.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pFax.Value = entity.Fax;
                    }

                    // Parameter settings: @Email
                    System.Data.SqlClient.SqlParameter pEmail = command.Parameters.Add("@Email", System.Data.SqlDbType.NVarChar, 120);
                    pEmail.Value = entity.Email;

                    // Parameter settings: @SupportRepId
                    System.Data.SqlClient.SqlParameter pSupportRepId = command.Parameters.Add("@SupportRepId", System.Data.SqlDbType.Int);
                    if (entity.SupportRepId == null)
                    {
                        pSupportRepId.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pSupportRepId.Value = entity.SupportRepId;
                    }

                    // Parameter settings: @Comment
                    System.Data.SqlClient.SqlParameter pComment = command.Parameters.Add("@Comment", System.Data.SqlDbType.NVarChar, -1);
                    if (entity.Comment == null)
                    {
                        pComment.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pComment.Value = entity.Comment;
                    }

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            entity.FullName = reader.GetString(0);
                            entity.FullDetail = reader.GetString(1);
                        }
                        else
                        {
                            throw new InvalidOperationException("Insert failed.");
                        }
                    }
                }
                finally
                {
                    PushConnection(command);
                }
            }
        }

        public List<Customer> SelectCustomer()
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [CustomerId], [FirstName], [LastName], [Company], [Address], [City], [State], [Country], [PostalCode], [Phone], [Fax], [Email], [SupportRepId], [FullName], [Comment], [FullDetail] FROM [dbo].[Customer]"))
            {
                try
                {
                    PopConnection(command);

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        List<Customer> result = new List<Customer>();
                        while (reader.Read())
                        {
                            result.Add(ReadCustomer(reader));
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

        public int SelectCustomerCount()
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT Count(*) FROM [dbo].[Customer]"))
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

        public List<Customer> SelectCustomerPaged(int firstIndex, int lastIndex)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT * FROM ( SELECT [CustomerId], [FirstName], [LastName], [Company], [Address], [City], [State], [Country], [PostalCode], [Phone], [Fax], [Email], [SupportRepId], [FullName], [Comment], [FullDetail], ROW_NUMBER() OVER(ORDER BY [CustomerId]) AS _ROW_NUMBER FROM [dbo].[Customer] ) AS T0 WHERE _ROW_NUMBER BETWEEN @__FirstIndex AND @__LastIndex ORDER BY [CustomerId]"))
            {
                try
                {
                    PopConnection(command);

                    System.Data.SqlClient.SqlParameter pFirstIndex = command.Parameters.Add("@__FirstIndex", System.Data.SqlDbType.Int);
                    pFirstIndex.Value = firstIndex;

                    System.Data.SqlClient.SqlParameter pLastIndex = command.Parameters.Add("@__LastIndex", System.Data.SqlDbType.Int);
                    pLastIndex.Value = lastIndex;

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        List<Customer> result = new List<Customer>();
                        while (reader.Read())
                        {
                            result.Add(ReadCustomer(reader));
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

        public List<Customer> SelectCustomerBySupportRepId(System.Nullable<System.Int32> supportRepId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [CustomerId], [FirstName], [LastName], [Company], [Address], [City], [State], [Country], [PostalCode], [Phone], [Fax], [Email], [SupportRepId], [FullName], [Comment], [FullDetail] FROM [dbo].[Customer] WHERE [SupportRepId] = @SupportRepId"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @SupportRepId
                    System.Data.SqlClient.SqlParameter pSupportRepId = command.Parameters.Add("@SupportRepId", System.Data.SqlDbType.Int);
                    if (supportRepId == null)
                    {
                        pSupportRepId.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pSupportRepId.Value = supportRepId;
                    }

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        List<Customer> result = new List<Customer>();
                        while (reader.Read())
                        {
                            result.Add(ReadCustomer(reader));
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

        public void DeleteCustomerBySupportRepId(System.Nullable<System.Int32> supportRepId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("DELETE FROM [dbo].[Customer] WHERE [SupportRepId] = @SupportRepId"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @SupportRepId
                    System.Data.SqlClient.SqlParameter pSupportRepId = command.Parameters.Add("@SupportRepId", System.Data.SqlDbType.Int);
                    if (supportRepId == null)
                    {
                        pSupportRepId.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pSupportRepId.Value = supportRepId;
                    }

                    command.ExecuteNonQuery();
                }
                finally
                {
                    PushConnection(command);
                }
            }
        }

        public Customer SelectCustomerByCustomerId(System.Int32 customerId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [CustomerId], [FirstName], [LastName], [Company], [Address], [City], [State], [Country], [PostalCode], [Phone], [Fax], [Email], [SupportRepId], [FullName], [Comment], [FullDetail] FROM [dbo].[Customer] WHERE [CustomerId] = @CustomerId"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @CustomerId
                    System.Data.SqlClient.SqlParameter pCustomerId = command.Parameters.Add("@CustomerId", System.Data.SqlDbType.Int);
                    pCustomerId.Value = customerId;

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return ReadCustomer(reader);
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

        public void DeleteCustomerByCustomerId(System.Int32 customerId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("DELETE FROM [dbo].[Customer] WHERE [CustomerId] = @CustomerId"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @CustomerId
                    System.Data.SqlClient.SqlParameter pCustomerId = command.Parameters.Add("@CustomerId", System.Data.SqlDbType.Int);
                    pCustomerId.Value = customerId;

                    command.ExecuteNonQuery();
                }
                finally
                {
                    PushConnection(command);
                }
            }
        }

        #endregion

        #region Upsert, Insert, Update, Delete, Select, Mapping - dbo.Employee

        public static Employee ReadEmployee(System.Data.SqlClient.SqlDataReader reader)
        {
            Employee entity = new Employee();
            entity.EmployeeId = reader.GetInt32(0);
            entity.LastName = reader.GetString(1);
            entity.FirstName = reader.GetString(2);
            entity.Title = reader.GetString(3);
            if (reader.IsDBNull(4))
            {
                entity.ReportsTo = null;
            }
            else
            {
                entity.ReportsTo = reader.GetInt32(4);
            }
            if (reader.IsDBNull(5))
            {
                entity.BirthDate = null;
            }
            else
            {
                entity.BirthDate = reader.GetDateTime(5);
            }
            if (reader.IsDBNull(6))
            {
                entity.HireDate = null;
            }
            else
            {
                entity.HireDate = reader.GetDateTime(6);
            }
            entity.Address = reader.GetString(7);
            entity.City = reader.GetString(8);
            entity.State = reader.GetString(9);
            entity.Country = reader.GetString(10);
            entity.PostalCode = reader.GetString(11);
            entity.Phone = reader.GetString(12);
            entity.Fax = reader.GetString(13);
            entity.Email = reader.GetString(14);
            return entity;
        }

        public void UpsertEmployee(Employee entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("MERGE [dbo].[Employee] AS T USING (SELECT @EmployeeId, @LastName, @FirstName, @Title, @ReportsTo, @BirthDate, @HireDate, @Address, @City, @State, @Country, @PostalCode, @Phone, @Fax, @Email) AS S ([EmployeeId], [LastName], [FirstName], [Title], [ReportsTo], [BirthDate], [HireDate], [Address], [City], [State], [Country], [PostalCode], [Phone], [Fax], [Email]) ON (S.[EmployeeId] = T.[EmployeeId]) WHEN MATCHED THEN UPDATE SET [LastName] = S.[LastName], [FirstName] = S.[FirstName], [Title] = S.[Title], [ReportsTo] = S.[ReportsTo], [BirthDate] = S.[BirthDate], [HireDate] = S.[HireDate], [Address] = S.[Address], [City] = S.[City], [State] = S.[State], [Country] = S.[Country], [PostalCode] = S.[PostalCode], [Phone] = S.[Phone], [Fax] = S.[Fax], [Email] = S.[Email] WHEN NOT MATCHED THEN INSERT ([LastName], [FirstName], [Title], [ReportsTo], [BirthDate], [HireDate], [Address], [City], [State], [Country], [PostalCode], [Phone], [Fax], [Email]) VALUES (S.[LastName], S.[FirstName], S.[Title], S.[ReportsTo], S.[BirthDate], S.[HireDate], S.[Address], S.[City], S.[State], S.[Country], S.[PostalCode], S.[Phone], S.[Fax], S.[Email]) OUTPUT inserted.[EmployeeId];"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @LastName
                    System.Data.SqlClient.SqlParameter pLastName = command.Parameters.Add("@LastName", System.Data.SqlDbType.NVarChar, 40);
                    pLastName.Value = entity.LastName;

                    // Parameter settings: @FirstName
                    System.Data.SqlClient.SqlParameter pFirstName = command.Parameters.Add("@FirstName", System.Data.SqlDbType.NVarChar, 40);
                    pFirstName.Value = entity.FirstName;

                    // Parameter settings: @Title
                    System.Data.SqlClient.SqlParameter pTitle = command.Parameters.Add("@Title", System.Data.SqlDbType.NVarChar, 60);
                    if (entity.Title == null)
                    {
                        pTitle.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pTitle.Value = entity.Title;
                    }

                    // Parameter settings: @ReportsTo
                    System.Data.SqlClient.SqlParameter pReportsTo = command.Parameters.Add("@ReportsTo", System.Data.SqlDbType.Int);
                    if (entity.ReportsTo == null)
                    {
                        pReportsTo.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pReportsTo.Value = entity.ReportsTo;
                    }

                    // Parameter settings: @BirthDate
                    System.Data.SqlClient.SqlParameter pBirthDate = command.Parameters.Add("@BirthDate", System.Data.SqlDbType.DateTime);
                    if (entity.BirthDate == null)
                    {
                        pBirthDate.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pBirthDate.Value = entity.BirthDate;
                    }

                    // Parameter settings: @HireDate
                    System.Data.SqlClient.SqlParameter pHireDate = command.Parameters.Add("@HireDate", System.Data.SqlDbType.DateTime);
                    if (entity.HireDate == null)
                    {
                        pHireDate.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pHireDate.Value = entity.HireDate;
                    }

                    // Parameter settings: @Address
                    System.Data.SqlClient.SqlParameter pAddress = command.Parameters.Add("@Address", System.Data.SqlDbType.NVarChar, 140);
                    if (entity.Address == null)
                    {
                        pAddress.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pAddress.Value = entity.Address;
                    }

                    // Parameter settings: @City
                    System.Data.SqlClient.SqlParameter pCity = command.Parameters.Add("@City", System.Data.SqlDbType.NVarChar, 80);
                    if (entity.City == null)
                    {
                        pCity.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pCity.Value = entity.City;
                    }

                    // Parameter settings: @State
                    System.Data.SqlClient.SqlParameter pState = command.Parameters.Add("@State", System.Data.SqlDbType.NVarChar, 80);
                    if (entity.State == null)
                    {
                        pState.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pState.Value = entity.State;
                    }

                    // Parameter settings: @Country
                    System.Data.SqlClient.SqlParameter pCountry = command.Parameters.Add("@Country", System.Data.SqlDbType.NVarChar, 80);
                    if (entity.Country == null)
                    {
                        pCountry.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pCountry.Value = entity.Country;
                    }

                    // Parameter settings: @PostalCode
                    System.Data.SqlClient.SqlParameter pPostalCode = command.Parameters.Add("@PostalCode", System.Data.SqlDbType.NVarChar, 20);
                    if (entity.PostalCode == null)
                    {
                        pPostalCode.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pPostalCode.Value = entity.PostalCode;
                    }

                    // Parameter settings: @Phone
                    System.Data.SqlClient.SqlParameter pPhone = command.Parameters.Add("@Phone", System.Data.SqlDbType.NVarChar, 48);
                    if (entity.Phone == null)
                    {
                        pPhone.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pPhone.Value = entity.Phone;
                    }

                    // Parameter settings: @Fax
                    System.Data.SqlClient.SqlParameter pFax = command.Parameters.Add("@Fax", System.Data.SqlDbType.NVarChar, 48);
                    if (entity.Fax == null)
                    {
                        pFax.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pFax.Value = entity.Fax;
                    }

                    // Parameter settings: @Email
                    System.Data.SqlClient.SqlParameter pEmail = command.Parameters.Add("@Email", System.Data.SqlDbType.NVarChar, 120);
                    if (entity.Email == null)
                    {
                        pEmail.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pEmail.Value = entity.Email;
                    }

                    // Parameter settings: @EmployeeId
                    System.Data.SqlClient.SqlParameter pEmployeeId = command.Parameters.Add("@EmployeeId", System.Data.SqlDbType.Int);
                    pEmployeeId.Value = entity.EmployeeId;

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            entity.EmployeeId = reader.GetInt32(0);
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

        public void InsertEmployee(Employee entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("INSERT INTO [dbo].[Employee] ([LastName], [FirstName], [Title], [ReportsTo], [BirthDate], [HireDate], [Address], [City], [State], [Country], [PostalCode], [Phone], [Fax], [Email])  OUTPUT inserted.[EmployeeId] VALUES (@LastName, @FirstName, @Title, @ReportsTo, @BirthDate, @HireDate, @Address, @City, @State, @Country, @PostalCode, @Phone, @Fax, @Email);"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @LastName
                    System.Data.SqlClient.SqlParameter pLastName = command.Parameters.Add("@LastName", System.Data.SqlDbType.NVarChar, 40);
                    pLastName.Value = entity.LastName;

                    // Parameter settings: @FirstName
                    System.Data.SqlClient.SqlParameter pFirstName = command.Parameters.Add("@FirstName", System.Data.SqlDbType.NVarChar, 40);
                    pFirstName.Value = entity.FirstName;

                    // Parameter settings: @Title
                    System.Data.SqlClient.SqlParameter pTitle = command.Parameters.Add("@Title", System.Data.SqlDbType.NVarChar, 60);
                    if (entity.Title == null)
                    {
                        pTitle.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pTitle.Value = entity.Title;
                    }

                    // Parameter settings: @ReportsTo
                    System.Data.SqlClient.SqlParameter pReportsTo = command.Parameters.Add("@ReportsTo", System.Data.SqlDbType.Int);
                    if (entity.ReportsTo == null)
                    {
                        pReportsTo.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pReportsTo.Value = entity.ReportsTo;
                    }

                    // Parameter settings: @BirthDate
                    System.Data.SqlClient.SqlParameter pBirthDate = command.Parameters.Add("@BirthDate", System.Data.SqlDbType.DateTime);
                    if (entity.BirthDate == null)
                    {
                        pBirthDate.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pBirthDate.Value = entity.BirthDate;
                    }

                    // Parameter settings: @HireDate
                    System.Data.SqlClient.SqlParameter pHireDate = command.Parameters.Add("@HireDate", System.Data.SqlDbType.DateTime);
                    if (entity.HireDate == null)
                    {
                        pHireDate.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pHireDate.Value = entity.HireDate;
                    }

                    // Parameter settings: @Address
                    System.Data.SqlClient.SqlParameter pAddress = command.Parameters.Add("@Address", System.Data.SqlDbType.NVarChar, 140);
                    if (entity.Address == null)
                    {
                        pAddress.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pAddress.Value = entity.Address;
                    }

                    // Parameter settings: @City
                    System.Data.SqlClient.SqlParameter pCity = command.Parameters.Add("@City", System.Data.SqlDbType.NVarChar, 80);
                    if (entity.City == null)
                    {
                        pCity.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pCity.Value = entity.City;
                    }

                    // Parameter settings: @State
                    System.Data.SqlClient.SqlParameter pState = command.Parameters.Add("@State", System.Data.SqlDbType.NVarChar, 80);
                    if (entity.State == null)
                    {
                        pState.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pState.Value = entity.State;
                    }

                    // Parameter settings: @Country
                    System.Data.SqlClient.SqlParameter pCountry = command.Parameters.Add("@Country", System.Data.SqlDbType.NVarChar, 80);
                    if (entity.Country == null)
                    {
                        pCountry.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pCountry.Value = entity.Country;
                    }

                    // Parameter settings: @PostalCode
                    System.Data.SqlClient.SqlParameter pPostalCode = command.Parameters.Add("@PostalCode", System.Data.SqlDbType.NVarChar, 20);
                    if (entity.PostalCode == null)
                    {
                        pPostalCode.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pPostalCode.Value = entity.PostalCode;
                    }

                    // Parameter settings: @Phone
                    System.Data.SqlClient.SqlParameter pPhone = command.Parameters.Add("@Phone", System.Data.SqlDbType.NVarChar, 48);
                    if (entity.Phone == null)
                    {
                        pPhone.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pPhone.Value = entity.Phone;
                    }

                    // Parameter settings: @Fax
                    System.Data.SqlClient.SqlParameter pFax = command.Parameters.Add("@Fax", System.Data.SqlDbType.NVarChar, 48);
                    if (entity.Fax == null)
                    {
                        pFax.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pFax.Value = entity.Fax;
                    }

                    // Parameter settings: @Email
                    System.Data.SqlClient.SqlParameter pEmail = command.Parameters.Add("@Email", System.Data.SqlDbType.NVarChar, 120);
                    if (entity.Email == null)
                    {
                        pEmail.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pEmail.Value = entity.Email;
                    }

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            entity.EmployeeId = reader.GetInt32(0);
                        }
                        else
                        {
                            throw new InvalidOperationException("Insert failed.");
                        }
                    }
                }
                finally
                {
                    PushConnection(command);
                }
            }
        }

        public void UpdateEmployee(Employee entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("UPDATE [dbo].[Employee] SET [LastName] = @LastName, [FirstName] = @FirstName, [Title] = @Title, [ReportsTo] = @ReportsTo, [BirthDate] = @BirthDate, [HireDate] = @HireDate, [Address] = @Address, [City] = @City, [State] = @State, [Country] = @Country, [PostalCode] = @PostalCode, [Phone] = @Phone, [Fax] = @Fax, [Email] = @Email WHERE [EmployeeId] = @EmployeeId;"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @EmployeeId
                    System.Data.SqlClient.SqlParameter pEmployeeId = command.Parameters.Add("@EmployeeId", System.Data.SqlDbType.Int);
                    pEmployeeId.Value = entity.EmployeeId;

                    // Parameter settings: @LastName
                    System.Data.SqlClient.SqlParameter pLastName = command.Parameters.Add("@LastName", System.Data.SqlDbType.NVarChar, 40);
                    pLastName.Value = entity.LastName;

                    // Parameter settings: @FirstName
                    System.Data.SqlClient.SqlParameter pFirstName = command.Parameters.Add("@FirstName", System.Data.SqlDbType.NVarChar, 40);
                    pFirstName.Value = entity.FirstName;

                    // Parameter settings: @Title
                    System.Data.SqlClient.SqlParameter pTitle = command.Parameters.Add("@Title", System.Data.SqlDbType.NVarChar, 60);
                    if (entity.Title == null)
                    {
                        pTitle.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pTitle.Value = entity.Title;
                    }

                    // Parameter settings: @ReportsTo
                    System.Data.SqlClient.SqlParameter pReportsTo = command.Parameters.Add("@ReportsTo", System.Data.SqlDbType.Int);
                    if (entity.ReportsTo == null)
                    {
                        pReportsTo.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pReportsTo.Value = entity.ReportsTo;
                    }

                    // Parameter settings: @BirthDate
                    System.Data.SqlClient.SqlParameter pBirthDate = command.Parameters.Add("@BirthDate", System.Data.SqlDbType.DateTime);
                    if (entity.BirthDate == null)
                    {
                        pBirthDate.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pBirthDate.Value = entity.BirthDate;
                    }

                    // Parameter settings: @HireDate
                    System.Data.SqlClient.SqlParameter pHireDate = command.Parameters.Add("@HireDate", System.Data.SqlDbType.DateTime);
                    if (entity.HireDate == null)
                    {
                        pHireDate.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pHireDate.Value = entity.HireDate;
                    }

                    // Parameter settings: @Address
                    System.Data.SqlClient.SqlParameter pAddress = command.Parameters.Add("@Address", System.Data.SqlDbType.NVarChar, 140);
                    if (entity.Address == null)
                    {
                        pAddress.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pAddress.Value = entity.Address;
                    }

                    // Parameter settings: @City
                    System.Data.SqlClient.SqlParameter pCity = command.Parameters.Add("@City", System.Data.SqlDbType.NVarChar, 80);
                    if (entity.City == null)
                    {
                        pCity.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pCity.Value = entity.City;
                    }

                    // Parameter settings: @State
                    System.Data.SqlClient.SqlParameter pState = command.Parameters.Add("@State", System.Data.SqlDbType.NVarChar, 80);
                    if (entity.State == null)
                    {
                        pState.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pState.Value = entity.State;
                    }

                    // Parameter settings: @Country
                    System.Data.SqlClient.SqlParameter pCountry = command.Parameters.Add("@Country", System.Data.SqlDbType.NVarChar, 80);
                    if (entity.Country == null)
                    {
                        pCountry.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pCountry.Value = entity.Country;
                    }

                    // Parameter settings: @PostalCode
                    System.Data.SqlClient.SqlParameter pPostalCode = command.Parameters.Add("@PostalCode", System.Data.SqlDbType.NVarChar, 20);
                    if (entity.PostalCode == null)
                    {
                        pPostalCode.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pPostalCode.Value = entity.PostalCode;
                    }

                    // Parameter settings: @Phone
                    System.Data.SqlClient.SqlParameter pPhone = command.Parameters.Add("@Phone", System.Data.SqlDbType.NVarChar, 48);
                    if (entity.Phone == null)
                    {
                        pPhone.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pPhone.Value = entity.Phone;
                    }

                    // Parameter settings: @Fax
                    System.Data.SqlClient.SqlParameter pFax = command.Parameters.Add("@Fax", System.Data.SqlDbType.NVarChar, 48);
                    if (entity.Fax == null)
                    {
                        pFax.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pFax.Value = entity.Fax;
                    }

                    // Parameter settings: @Email
                    System.Data.SqlClient.SqlParameter pEmail = command.Parameters.Add("@Email", System.Data.SqlDbType.NVarChar, 120);
                    if (entity.Email == null)
                    {
                        pEmail.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pEmail.Value = entity.Email;
                    }

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

        public List<Employee> SelectEmployee()
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [EmployeeId], [LastName], [FirstName], [Title], [ReportsTo], [BirthDate], [HireDate], [Address], [City], [State], [Country], [PostalCode], [Phone], [Fax], [Email] FROM [dbo].[Employee]"))
            {
                try
                {
                    PopConnection(command);

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        List<Employee> result = new List<Employee>();
                        while (reader.Read())
                        {
                            result.Add(ReadEmployee(reader));
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

        public int SelectEmployeeCount()
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT Count(*) FROM [dbo].[Employee]"))
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

        public List<Employee> SelectEmployeePaged(int firstIndex, int lastIndex)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT * FROM ( SELECT [EmployeeId], [LastName], [FirstName], [Title], [ReportsTo], [BirthDate], [HireDate], [Address], [City], [State], [Country], [PostalCode], [Phone], [Fax], [Email], ROW_NUMBER() OVER(ORDER BY [EmployeeId]) AS _ROW_NUMBER FROM [dbo].[Employee] ) AS T0 WHERE _ROW_NUMBER BETWEEN @__FirstIndex AND @__LastIndex ORDER BY [EmployeeId]"))
            {
                try
                {
                    PopConnection(command);

                    System.Data.SqlClient.SqlParameter pFirstIndex = command.Parameters.Add("@__FirstIndex", System.Data.SqlDbType.Int);
                    pFirstIndex.Value = firstIndex;

                    System.Data.SqlClient.SqlParameter pLastIndex = command.Parameters.Add("@__LastIndex", System.Data.SqlDbType.Int);
                    pLastIndex.Value = lastIndex;

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        List<Employee> result = new List<Employee>();
                        while (reader.Read())
                        {
                            result.Add(ReadEmployee(reader));
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

        public List<Employee> SelectEmployeeByReportsTo(System.Nullable<System.Int32> reportsTo)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [EmployeeId], [LastName], [FirstName], [Title], [ReportsTo], [BirthDate], [HireDate], [Address], [City], [State], [Country], [PostalCode], [Phone], [Fax], [Email] FROM [dbo].[Employee] WHERE [ReportsTo] = @ReportsTo"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @ReportsTo
                    System.Data.SqlClient.SqlParameter pReportsTo = command.Parameters.Add("@ReportsTo", System.Data.SqlDbType.Int);
                    if (reportsTo == null)
                    {
                        pReportsTo.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pReportsTo.Value = reportsTo;
                    }

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        List<Employee> result = new List<Employee>();
                        while (reader.Read())
                        {
                            result.Add(ReadEmployee(reader));
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

        public void DeleteEmployeeByReportsTo(System.Nullable<System.Int32> reportsTo)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("DELETE FROM [dbo].[Employee] WHERE [ReportsTo] = @ReportsTo"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @ReportsTo
                    System.Data.SqlClient.SqlParameter pReportsTo = command.Parameters.Add("@ReportsTo", System.Data.SqlDbType.Int);
                    if (reportsTo == null)
                    {
                        pReportsTo.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pReportsTo.Value = reportsTo;
                    }

                    command.ExecuteNonQuery();
                }
                finally
                {
                    PushConnection(command);
                }
            }
        }

        public Employee SelectEmployeeByEmployeeId(System.Int32 employeeId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [EmployeeId], [LastName], [FirstName], [Title], [ReportsTo], [BirthDate], [HireDate], [Address], [City], [State], [Country], [PostalCode], [Phone], [Fax], [Email] FROM [dbo].[Employee] WHERE [EmployeeId] = @EmployeeId"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @EmployeeId
                    System.Data.SqlClient.SqlParameter pEmployeeId = command.Parameters.Add("@EmployeeId", System.Data.SqlDbType.Int);
                    pEmployeeId.Value = employeeId;

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return ReadEmployee(reader);
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

        public void DeleteEmployeeByEmployeeId(System.Int32 employeeId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("DELETE FROM [dbo].[Employee] WHERE [EmployeeId] = @EmployeeId"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @EmployeeId
                    System.Data.SqlClient.SqlParameter pEmployeeId = command.Parameters.Add("@EmployeeId", System.Data.SqlDbType.Int);
                    pEmployeeId.Value = employeeId;

                    command.ExecuteNonQuery();
                }
                finally
                {
                    PushConnection(command);
                }
            }
        }

        #endregion

        #region Upsert, Insert, Update, Delete, Select, Mapping - dbo.Genre

        public static Genre ReadGenre(System.Data.SqlClient.SqlDataReader reader)
        {
            Genre entity = new Genre();
            entity.GenreId = reader.GetInt32(0);
            entity.Name = reader.GetString(1);
            return entity;
        }

        public void UpsertGenre(Genre entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("MERGE [dbo].[Genre] AS T USING (SELECT @GenreId, @Name) AS S ([GenreId], [Name]) ON (S.[GenreId] = T.[GenreId]) WHEN MATCHED THEN UPDATE SET [Name] = S.[Name] WHEN NOT MATCHED THEN INSERT ([Name]) VALUES (S.[Name]) OUTPUT inserted.[GenreId];"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @Name
                    System.Data.SqlClient.SqlParameter pName = command.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar, 240);
                    if (entity.Name == null)
                    {
                        pName.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pName.Value = entity.Name;
                    }

                    // Parameter settings: @GenreId
                    System.Data.SqlClient.SqlParameter pGenreId = command.Parameters.Add("@GenreId", System.Data.SqlDbType.Int);
                    pGenreId.Value = entity.GenreId;

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            entity.GenreId = reader.GetInt32(0);
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

        public void InsertGenre(Genre entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("INSERT INTO [dbo].[Genre] ([Name])  OUTPUT inserted.[GenreId] VALUES (@Name);"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @Name
                    System.Data.SqlClient.SqlParameter pName = command.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar, 240);
                    if (entity.Name == null)
                    {
                        pName.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pName.Value = entity.Name;
                    }

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            entity.GenreId = reader.GetInt32(0);
                        }
                        else
                        {
                            throw new InvalidOperationException("Insert failed.");
                        }
                    }
                }
                finally
                {
                    PushConnection(command);
                }
            }
        }

        public void UpdateGenre(Genre entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("UPDATE [dbo].[Genre] SET [Name] = @Name WHERE [GenreId] = @GenreId;"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @GenreId
                    System.Data.SqlClient.SqlParameter pGenreId = command.Parameters.Add("@GenreId", System.Data.SqlDbType.Int);
                    pGenreId.Value = entity.GenreId;

                    // Parameter settings: @Name
                    System.Data.SqlClient.SqlParameter pName = command.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar, 240);
                    if (entity.Name == null)
                    {
                        pName.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pName.Value = entity.Name;
                    }

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

        public List<Genre> SelectGenre()
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [GenreId], [Name] FROM [dbo].[Genre]"))
            {
                try
                {
                    PopConnection(command);

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        List<Genre> result = new List<Genre>();
                        while (reader.Read())
                        {
                            result.Add(ReadGenre(reader));
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

        public int SelectGenreCount()
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT Count(*) FROM [dbo].[Genre]"))
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

        public List<Genre> SelectGenrePaged(int firstIndex, int lastIndex)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT * FROM ( SELECT [GenreId], [Name], ROW_NUMBER() OVER(ORDER BY [GenreId]) AS _ROW_NUMBER FROM [dbo].[Genre] ) AS T0 WHERE _ROW_NUMBER BETWEEN @__FirstIndex AND @__LastIndex ORDER BY [GenreId]"))
            {
                try
                {
                    PopConnection(command);

                    System.Data.SqlClient.SqlParameter pFirstIndex = command.Parameters.Add("@__FirstIndex", System.Data.SqlDbType.Int);
                    pFirstIndex.Value = firstIndex;

                    System.Data.SqlClient.SqlParameter pLastIndex = command.Parameters.Add("@__LastIndex", System.Data.SqlDbType.Int);
                    pLastIndex.Value = lastIndex;

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        List<Genre> result = new List<Genre>();
                        while (reader.Read())
                        {
                            result.Add(ReadGenre(reader));
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

        public Genre SelectGenreByGenreId(System.Int32 genreId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [GenreId], [Name] FROM [dbo].[Genre] WHERE [GenreId] = @GenreId"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @GenreId
                    System.Data.SqlClient.SqlParameter pGenreId = command.Parameters.Add("@GenreId", System.Data.SqlDbType.Int);
                    pGenreId.Value = genreId;

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return ReadGenre(reader);
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

        public void DeleteGenreByGenreId(System.Int32 genreId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("DELETE FROM [dbo].[Genre] WHERE [GenreId] = @GenreId"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @GenreId
                    System.Data.SqlClient.SqlParameter pGenreId = command.Parameters.Add("@GenreId", System.Data.SqlDbType.Int);
                    pGenreId.Value = genreId;

                    command.ExecuteNonQuery();
                }
                finally
                {
                    PushConnection(command);
                }
            }
        }

        #endregion

        #region Upsert, Insert, Update, Delete, Select, Mapping - dbo.Invoice

        public static Invoice ReadInvoice(System.Data.SqlClient.SqlDataReader reader)
        {
            Invoice entity = new Invoice();
            entity.InvoiceId = reader.GetInt32(0);
            entity.CustomerId = reader.GetInt32(1);
            entity.InvoiceDate = reader.GetDateTime(2);
            entity.BillingAddress = reader.GetString(3);
            entity.BillingCity = reader.GetString(4);
            entity.BillingState = reader.GetString(5);
            entity.BillingCountry = reader.GetString(6);
            entity.BillingPostalCode = reader.GetString(7);
            entity.Total = reader.GetDecimal(8);
            return entity;
        }

        public void UpsertInvoice(Invoice entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("MERGE [dbo].[Invoice] AS T USING (SELECT @InvoiceId, @CustomerId, @InvoiceDate, @BillingAddress, @BillingCity, @BillingState, @BillingCountry, @BillingPostalCode, @Total) AS S ([InvoiceId], [CustomerId], [InvoiceDate], [BillingAddress], [BillingCity], [BillingState], [BillingCountry], [BillingPostalCode], [Total]) ON (S.[InvoiceId] = T.[InvoiceId]) WHEN MATCHED THEN UPDATE SET [CustomerId] = S.[CustomerId], [InvoiceDate] = S.[InvoiceDate], [BillingAddress] = S.[BillingAddress], [BillingCity] = S.[BillingCity], [BillingState] = S.[BillingState], [BillingCountry] = S.[BillingCountry], [BillingPostalCode] = S.[BillingPostalCode], [Total] = S.[Total] WHEN NOT MATCHED THEN INSERT ([CustomerId], [InvoiceDate], [BillingAddress], [BillingCity], [BillingState], [BillingCountry], [BillingPostalCode], [Total]) VALUES (S.[CustomerId], S.[InvoiceDate], S.[BillingAddress], S.[BillingCity], S.[BillingState], S.[BillingCountry], S.[BillingPostalCode], S.[Total]) OUTPUT inserted.[InvoiceId];"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @CustomerId
                    System.Data.SqlClient.SqlParameter pCustomerId = command.Parameters.Add("@CustomerId", System.Data.SqlDbType.Int);
                    pCustomerId.Value = entity.CustomerId;

                    // Parameter settings: @InvoiceDate
                    System.Data.SqlClient.SqlParameter pInvoiceDate = command.Parameters.Add("@InvoiceDate", System.Data.SqlDbType.DateTime);
                    pInvoiceDate.Value = entity.InvoiceDate;

                    // Parameter settings: @BillingAddress
                    System.Data.SqlClient.SqlParameter pBillingAddress = command.Parameters.Add("@BillingAddress", System.Data.SqlDbType.NVarChar, 140);
                    if (entity.BillingAddress == null)
                    {
                        pBillingAddress.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pBillingAddress.Value = entity.BillingAddress;
                    }

                    // Parameter settings: @BillingCity
                    System.Data.SqlClient.SqlParameter pBillingCity = command.Parameters.Add("@BillingCity", System.Data.SqlDbType.NVarChar, 80);
                    if (entity.BillingCity == null)
                    {
                        pBillingCity.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pBillingCity.Value = entity.BillingCity;
                    }

                    // Parameter settings: @BillingState
                    System.Data.SqlClient.SqlParameter pBillingState = command.Parameters.Add("@BillingState", System.Data.SqlDbType.NVarChar, 80);
                    if (entity.BillingState == null)
                    {
                        pBillingState.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pBillingState.Value = entity.BillingState;
                    }

                    // Parameter settings: @BillingCountry
                    System.Data.SqlClient.SqlParameter pBillingCountry = command.Parameters.Add("@BillingCountry", System.Data.SqlDbType.NVarChar, 80);
                    if (entity.BillingCountry == null)
                    {
                        pBillingCountry.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pBillingCountry.Value = entity.BillingCountry;
                    }

                    // Parameter settings: @BillingPostalCode
                    System.Data.SqlClient.SqlParameter pBillingPostalCode = command.Parameters.Add("@BillingPostalCode", System.Data.SqlDbType.NVarChar, 20);
                    if (entity.BillingPostalCode == null)
                    {
                        pBillingPostalCode.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pBillingPostalCode.Value = entity.BillingPostalCode;
                    }

                    // Parameter settings: @Total
                    System.Data.SqlClient.SqlParameter pTotal = command.Parameters.Add("@Total", System.Data.SqlDbType.Decimal);
                    pTotal.Value = entity.Total;

                    // Parameter settings: @InvoiceId
                    System.Data.SqlClient.SqlParameter pInvoiceId = command.Parameters.Add("@InvoiceId", System.Data.SqlDbType.Int);
                    pInvoiceId.Value = entity.InvoiceId;

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            entity.InvoiceId = reader.GetInt32(0);
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

        public void InsertInvoice(Invoice entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("INSERT INTO [dbo].[Invoice] ([CustomerId], [InvoiceDate], [BillingAddress], [BillingCity], [BillingState], [BillingCountry], [BillingPostalCode], [Total])  OUTPUT inserted.[InvoiceId] VALUES (@CustomerId, @InvoiceDate, @BillingAddress, @BillingCity, @BillingState, @BillingCountry, @BillingPostalCode, @Total);"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @CustomerId
                    System.Data.SqlClient.SqlParameter pCustomerId = command.Parameters.Add("@CustomerId", System.Data.SqlDbType.Int);
                    pCustomerId.Value = entity.CustomerId;

                    // Parameter settings: @InvoiceDate
                    System.Data.SqlClient.SqlParameter pInvoiceDate = command.Parameters.Add("@InvoiceDate", System.Data.SqlDbType.DateTime);
                    pInvoiceDate.Value = entity.InvoiceDate;

                    // Parameter settings: @BillingAddress
                    System.Data.SqlClient.SqlParameter pBillingAddress = command.Parameters.Add("@BillingAddress", System.Data.SqlDbType.NVarChar, 140);
                    if (entity.BillingAddress == null)
                    {
                        pBillingAddress.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pBillingAddress.Value = entity.BillingAddress;
                    }

                    // Parameter settings: @BillingCity
                    System.Data.SqlClient.SqlParameter pBillingCity = command.Parameters.Add("@BillingCity", System.Data.SqlDbType.NVarChar, 80);
                    if (entity.BillingCity == null)
                    {
                        pBillingCity.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pBillingCity.Value = entity.BillingCity;
                    }

                    // Parameter settings: @BillingState
                    System.Data.SqlClient.SqlParameter pBillingState = command.Parameters.Add("@BillingState", System.Data.SqlDbType.NVarChar, 80);
                    if (entity.BillingState == null)
                    {
                        pBillingState.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pBillingState.Value = entity.BillingState;
                    }

                    // Parameter settings: @BillingCountry
                    System.Data.SqlClient.SqlParameter pBillingCountry = command.Parameters.Add("@BillingCountry", System.Data.SqlDbType.NVarChar, 80);
                    if (entity.BillingCountry == null)
                    {
                        pBillingCountry.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pBillingCountry.Value = entity.BillingCountry;
                    }

                    // Parameter settings: @BillingPostalCode
                    System.Data.SqlClient.SqlParameter pBillingPostalCode = command.Parameters.Add("@BillingPostalCode", System.Data.SqlDbType.NVarChar, 20);
                    if (entity.BillingPostalCode == null)
                    {
                        pBillingPostalCode.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pBillingPostalCode.Value = entity.BillingPostalCode;
                    }

                    // Parameter settings: @Total
                    System.Data.SqlClient.SqlParameter pTotal = command.Parameters.Add("@Total", System.Data.SqlDbType.Decimal);
                    pTotal.Value = entity.Total;

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            entity.InvoiceId = reader.GetInt32(0);
                        }
                        else
                        {
                            throw new InvalidOperationException("Insert failed.");
                        }
                    }
                }
                finally
                {
                    PushConnection(command);
                }
            }
        }

        public void UpdateInvoice(Invoice entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("UPDATE [dbo].[Invoice] SET [CustomerId] = @CustomerId, [InvoiceDate] = @InvoiceDate, [BillingAddress] = @BillingAddress, [BillingCity] = @BillingCity, [BillingState] = @BillingState, [BillingCountry] = @BillingCountry, [BillingPostalCode] = @BillingPostalCode, [Total] = @Total WHERE [InvoiceId] = @InvoiceId;"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @InvoiceId
                    System.Data.SqlClient.SqlParameter pInvoiceId = command.Parameters.Add("@InvoiceId", System.Data.SqlDbType.Int);
                    pInvoiceId.Value = entity.InvoiceId;

                    // Parameter settings: @CustomerId
                    System.Data.SqlClient.SqlParameter pCustomerId = command.Parameters.Add("@CustomerId", System.Data.SqlDbType.Int);
                    pCustomerId.Value = entity.CustomerId;

                    // Parameter settings: @InvoiceDate
                    System.Data.SqlClient.SqlParameter pInvoiceDate = command.Parameters.Add("@InvoiceDate", System.Data.SqlDbType.DateTime);
                    pInvoiceDate.Value = entity.InvoiceDate;

                    // Parameter settings: @BillingAddress
                    System.Data.SqlClient.SqlParameter pBillingAddress = command.Parameters.Add("@BillingAddress", System.Data.SqlDbType.NVarChar, 140);
                    if (entity.BillingAddress == null)
                    {
                        pBillingAddress.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pBillingAddress.Value = entity.BillingAddress;
                    }

                    // Parameter settings: @BillingCity
                    System.Data.SqlClient.SqlParameter pBillingCity = command.Parameters.Add("@BillingCity", System.Data.SqlDbType.NVarChar, 80);
                    if (entity.BillingCity == null)
                    {
                        pBillingCity.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pBillingCity.Value = entity.BillingCity;
                    }

                    // Parameter settings: @BillingState
                    System.Data.SqlClient.SqlParameter pBillingState = command.Parameters.Add("@BillingState", System.Data.SqlDbType.NVarChar, 80);
                    if (entity.BillingState == null)
                    {
                        pBillingState.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pBillingState.Value = entity.BillingState;
                    }

                    // Parameter settings: @BillingCountry
                    System.Data.SqlClient.SqlParameter pBillingCountry = command.Parameters.Add("@BillingCountry", System.Data.SqlDbType.NVarChar, 80);
                    if (entity.BillingCountry == null)
                    {
                        pBillingCountry.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pBillingCountry.Value = entity.BillingCountry;
                    }

                    // Parameter settings: @BillingPostalCode
                    System.Data.SqlClient.SqlParameter pBillingPostalCode = command.Parameters.Add("@BillingPostalCode", System.Data.SqlDbType.NVarChar, 20);
                    if (entity.BillingPostalCode == null)
                    {
                        pBillingPostalCode.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pBillingPostalCode.Value = entity.BillingPostalCode;
                    }

                    // Parameter settings: @Total
                    System.Data.SqlClient.SqlParameter pTotal = command.Parameters.Add("@Total", System.Data.SqlDbType.Decimal);
                    pTotal.Value = entity.Total;

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

        public List<Invoice> SelectInvoice()
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [InvoiceId], [CustomerId], [InvoiceDate], [BillingAddress], [BillingCity], [BillingState], [BillingCountry], [BillingPostalCode], [Total] FROM [dbo].[Invoice]"))
            {
                try
                {
                    PopConnection(command);

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        List<Invoice> result = new List<Invoice>();
                        while (reader.Read())
                        {
                            result.Add(ReadInvoice(reader));
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

        public int SelectInvoiceCount()
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT Count(*) FROM [dbo].[Invoice]"))
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

        public List<Invoice> SelectInvoicePaged(int firstIndex, int lastIndex)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT * FROM ( SELECT [InvoiceId], [CustomerId], [InvoiceDate], [BillingAddress], [BillingCity], [BillingState], [BillingCountry], [BillingPostalCode], [Total], ROW_NUMBER() OVER(ORDER BY [InvoiceId]) AS _ROW_NUMBER FROM [dbo].[Invoice] ) AS T0 WHERE _ROW_NUMBER BETWEEN @__FirstIndex AND @__LastIndex ORDER BY [InvoiceId]"))
            {
                try
                {
                    PopConnection(command);

                    System.Data.SqlClient.SqlParameter pFirstIndex = command.Parameters.Add("@__FirstIndex", System.Data.SqlDbType.Int);
                    pFirstIndex.Value = firstIndex;

                    System.Data.SqlClient.SqlParameter pLastIndex = command.Parameters.Add("@__LastIndex", System.Data.SqlDbType.Int);
                    pLastIndex.Value = lastIndex;

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        List<Invoice> result = new List<Invoice>();
                        while (reader.Read())
                        {
                            result.Add(ReadInvoice(reader));
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

        public List<Invoice> SelectInvoiceByCustomerId(System.Int32 customerId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [InvoiceId], [CustomerId], [InvoiceDate], [BillingAddress], [BillingCity], [BillingState], [BillingCountry], [BillingPostalCode], [Total] FROM [dbo].[Invoice] WHERE [CustomerId] = @CustomerId"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @CustomerId
                    System.Data.SqlClient.SqlParameter pCustomerId = command.Parameters.Add("@CustomerId", System.Data.SqlDbType.Int);
                    pCustomerId.Value = customerId;

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        List<Invoice> result = new List<Invoice>();
                        while (reader.Read())
                        {
                            result.Add(ReadInvoice(reader));
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

        public void DeleteInvoiceByCustomerId(System.Int32 customerId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("DELETE FROM [dbo].[Invoice] WHERE [CustomerId] = @CustomerId"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @CustomerId
                    System.Data.SqlClient.SqlParameter pCustomerId = command.Parameters.Add("@CustomerId", System.Data.SqlDbType.Int);
                    pCustomerId.Value = customerId;

                    command.ExecuteNonQuery();
                }
                finally
                {
                    PushConnection(command);
                }
            }
        }

        public Invoice SelectInvoiceByInvoiceId(System.Int32 invoiceId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [InvoiceId], [CustomerId], [InvoiceDate], [BillingAddress], [BillingCity], [BillingState], [BillingCountry], [BillingPostalCode], [Total] FROM [dbo].[Invoice] WHERE [InvoiceId] = @InvoiceId"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @InvoiceId
                    System.Data.SqlClient.SqlParameter pInvoiceId = command.Parameters.Add("@InvoiceId", System.Data.SqlDbType.Int);
                    pInvoiceId.Value = invoiceId;

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return ReadInvoice(reader);
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

        public void DeleteInvoiceByInvoiceId(System.Int32 invoiceId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("DELETE FROM [dbo].[Invoice] WHERE [InvoiceId] = @InvoiceId"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @InvoiceId
                    System.Data.SqlClient.SqlParameter pInvoiceId = command.Parameters.Add("@InvoiceId", System.Data.SqlDbType.Int);
                    pInvoiceId.Value = invoiceId;

                    command.ExecuteNonQuery();
                }
                finally
                {
                    PushConnection(command);
                }
            }
        }

        #endregion

        #region Upsert, Insert, Update, Delete, Select, Mapping - dbo.InvoiceLine

        public static InvoiceLine ReadInvoiceLine(System.Data.SqlClient.SqlDataReader reader)
        {
            InvoiceLine entity = new InvoiceLine();
            entity.InvoiceLineId = reader.GetInt32(0);
            entity.InvoiceId = reader.GetInt32(1);
            entity.TrackId = reader.GetInt32(2);
            entity.UnitPrice = reader.GetDecimal(3);
            entity.Quantity = reader.GetInt32(4);
            return entity;
        }

        public void UpsertInvoiceLine(InvoiceLine entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("MERGE [dbo].[InvoiceLine] AS T USING (SELECT @InvoiceLineId, @InvoiceId, @TrackId, @UnitPrice, @Quantity) AS S ([InvoiceLineId], [InvoiceId], [TrackId], [UnitPrice], [Quantity]) ON (S.[InvoiceLineId] = T.[InvoiceLineId]) WHEN MATCHED THEN UPDATE SET [InvoiceId] = S.[InvoiceId], [TrackId] = S.[TrackId], [UnitPrice] = S.[UnitPrice], [Quantity] = S.[Quantity] WHEN NOT MATCHED THEN INSERT ([InvoiceId], [TrackId], [UnitPrice], [Quantity]) VALUES (S.[InvoiceId], S.[TrackId], S.[UnitPrice], S.[Quantity]) OUTPUT inserted.[InvoiceLineId];"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @InvoiceId
                    System.Data.SqlClient.SqlParameter pInvoiceId = command.Parameters.Add("@InvoiceId", System.Data.SqlDbType.Int);
                    pInvoiceId.Value = entity.InvoiceId;

                    // Parameter settings: @TrackId
                    System.Data.SqlClient.SqlParameter pTrackId = command.Parameters.Add("@TrackId", System.Data.SqlDbType.Int);
                    pTrackId.Value = entity.TrackId;

                    // Parameter settings: @UnitPrice
                    System.Data.SqlClient.SqlParameter pUnitPrice = command.Parameters.Add("@UnitPrice", System.Data.SqlDbType.Decimal);
                    pUnitPrice.Value = entity.UnitPrice;

                    // Parameter settings: @Quantity
                    System.Data.SqlClient.SqlParameter pQuantity = command.Parameters.Add("@Quantity", System.Data.SqlDbType.Int);
                    pQuantity.Value = entity.Quantity;

                    // Parameter settings: @InvoiceLineId
                    System.Data.SqlClient.SqlParameter pInvoiceLineId = command.Parameters.Add("@InvoiceLineId", System.Data.SqlDbType.Int);
                    pInvoiceLineId.Value = entity.InvoiceLineId;

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            entity.InvoiceLineId = reader.GetInt32(0);
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

        public void InsertInvoiceLine(InvoiceLine entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("INSERT INTO [dbo].[InvoiceLine] ([InvoiceId], [TrackId], [UnitPrice], [Quantity])  OUTPUT inserted.[InvoiceLineId] VALUES (@InvoiceId, @TrackId, @UnitPrice, @Quantity);"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @InvoiceId
                    System.Data.SqlClient.SqlParameter pInvoiceId = command.Parameters.Add("@InvoiceId", System.Data.SqlDbType.Int);
                    pInvoiceId.Value = entity.InvoiceId;

                    // Parameter settings: @TrackId
                    System.Data.SqlClient.SqlParameter pTrackId = command.Parameters.Add("@TrackId", System.Data.SqlDbType.Int);
                    pTrackId.Value = entity.TrackId;

                    // Parameter settings: @UnitPrice
                    System.Data.SqlClient.SqlParameter pUnitPrice = command.Parameters.Add("@UnitPrice", System.Data.SqlDbType.Decimal);
                    pUnitPrice.Value = entity.UnitPrice;

                    // Parameter settings: @Quantity
                    System.Data.SqlClient.SqlParameter pQuantity = command.Parameters.Add("@Quantity", System.Data.SqlDbType.Int);
                    pQuantity.Value = entity.Quantity;

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            entity.InvoiceLineId = reader.GetInt32(0);
                        }
                        else
                        {
                            throw new InvalidOperationException("Insert failed.");
                        }
                    }
                }
                finally
                {
                    PushConnection(command);
                }
            }
        }

        public void UpdateInvoiceLine(InvoiceLine entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("UPDATE [dbo].[InvoiceLine] SET [InvoiceId] = @InvoiceId, [TrackId] = @TrackId, [UnitPrice] = @UnitPrice, [Quantity] = @Quantity WHERE [InvoiceLineId] = @InvoiceLineId;"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @InvoiceLineId
                    System.Data.SqlClient.SqlParameter pInvoiceLineId = command.Parameters.Add("@InvoiceLineId", System.Data.SqlDbType.Int);
                    pInvoiceLineId.Value = entity.InvoiceLineId;

                    // Parameter settings: @InvoiceId
                    System.Data.SqlClient.SqlParameter pInvoiceId = command.Parameters.Add("@InvoiceId", System.Data.SqlDbType.Int);
                    pInvoiceId.Value = entity.InvoiceId;

                    // Parameter settings: @TrackId
                    System.Data.SqlClient.SqlParameter pTrackId = command.Parameters.Add("@TrackId", System.Data.SqlDbType.Int);
                    pTrackId.Value = entity.TrackId;

                    // Parameter settings: @UnitPrice
                    System.Data.SqlClient.SqlParameter pUnitPrice = command.Parameters.Add("@UnitPrice", System.Data.SqlDbType.Decimal);
                    pUnitPrice.Value = entity.UnitPrice;

                    // Parameter settings: @Quantity
                    System.Data.SqlClient.SqlParameter pQuantity = command.Parameters.Add("@Quantity", System.Data.SqlDbType.Int);
                    pQuantity.Value = entity.Quantity;

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

        public List<InvoiceLine> SelectInvoiceLine()
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [InvoiceLineId], [InvoiceId], [TrackId], [UnitPrice], [Quantity] FROM [dbo].[InvoiceLine]"))
            {
                try
                {
                    PopConnection(command);

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        List<InvoiceLine> result = new List<InvoiceLine>();
                        while (reader.Read())
                        {
                            result.Add(ReadInvoiceLine(reader));
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

        public int SelectInvoiceLineCount()
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT Count(*) FROM [dbo].[InvoiceLine]"))
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

        public List<InvoiceLine> SelectInvoiceLinePaged(int firstIndex, int lastIndex)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT * FROM ( SELECT [InvoiceLineId], [InvoiceId], [TrackId], [UnitPrice], [Quantity], ROW_NUMBER() OVER(ORDER BY [InvoiceLineId]) AS _ROW_NUMBER FROM [dbo].[InvoiceLine] ) AS T0 WHERE _ROW_NUMBER BETWEEN @__FirstIndex AND @__LastIndex ORDER BY [InvoiceLineId]"))
            {
                try
                {
                    PopConnection(command);

                    System.Data.SqlClient.SqlParameter pFirstIndex = command.Parameters.Add("@__FirstIndex", System.Data.SqlDbType.Int);
                    pFirstIndex.Value = firstIndex;

                    System.Data.SqlClient.SqlParameter pLastIndex = command.Parameters.Add("@__LastIndex", System.Data.SqlDbType.Int);
                    pLastIndex.Value = lastIndex;

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        List<InvoiceLine> result = new List<InvoiceLine>();
                        while (reader.Read())
                        {
                            result.Add(ReadInvoiceLine(reader));
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

        public List<InvoiceLine> SelectInvoiceLineByInvoiceId(System.Int32 invoiceId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [InvoiceLineId], [InvoiceId], [TrackId], [UnitPrice], [Quantity] FROM [dbo].[InvoiceLine] WHERE [InvoiceId] = @InvoiceId"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @InvoiceId
                    System.Data.SqlClient.SqlParameter pInvoiceId = command.Parameters.Add("@InvoiceId", System.Data.SqlDbType.Int);
                    pInvoiceId.Value = invoiceId;

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        List<InvoiceLine> result = new List<InvoiceLine>();
                        while (reader.Read())
                        {
                            result.Add(ReadInvoiceLine(reader));
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

        public void DeleteInvoiceLineByInvoiceId(System.Int32 invoiceId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("DELETE FROM [dbo].[InvoiceLine] WHERE [InvoiceId] = @InvoiceId"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @InvoiceId
                    System.Data.SqlClient.SqlParameter pInvoiceId = command.Parameters.Add("@InvoiceId", System.Data.SqlDbType.Int);
                    pInvoiceId.Value = invoiceId;

                    command.ExecuteNonQuery();
                }
                finally
                {
                    PushConnection(command);
                }
            }
        }

        public List<InvoiceLine> SelectInvoiceLineByTrackId(System.Int32 trackId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [InvoiceLineId], [InvoiceId], [TrackId], [UnitPrice], [Quantity] FROM [dbo].[InvoiceLine] WHERE [TrackId] = @TrackId"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @TrackId
                    System.Data.SqlClient.SqlParameter pTrackId = command.Parameters.Add("@TrackId", System.Data.SqlDbType.Int);
                    pTrackId.Value = trackId;

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        List<InvoiceLine> result = new List<InvoiceLine>();
                        while (reader.Read())
                        {
                            result.Add(ReadInvoiceLine(reader));
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

        public void DeleteInvoiceLineByTrackId(System.Int32 trackId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("DELETE FROM [dbo].[InvoiceLine] WHERE [TrackId] = @TrackId"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @TrackId
                    System.Data.SqlClient.SqlParameter pTrackId = command.Parameters.Add("@TrackId", System.Data.SqlDbType.Int);
                    pTrackId.Value = trackId;

                    command.ExecuteNonQuery();
                }
                finally
                {
                    PushConnection(command);
                }
            }
        }

        public InvoiceLine SelectInvoiceLineByInvoiceLineId(System.Int32 invoiceLineId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [InvoiceLineId], [InvoiceId], [TrackId], [UnitPrice], [Quantity] FROM [dbo].[InvoiceLine] WHERE [InvoiceLineId] = @InvoiceLineId"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @InvoiceLineId
                    System.Data.SqlClient.SqlParameter pInvoiceLineId = command.Parameters.Add("@InvoiceLineId", System.Data.SqlDbType.Int);
                    pInvoiceLineId.Value = invoiceLineId;

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return ReadInvoiceLine(reader);
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

        public void DeleteInvoiceLineByInvoiceLineId(System.Int32 invoiceLineId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("DELETE FROM [dbo].[InvoiceLine] WHERE [InvoiceLineId] = @InvoiceLineId"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @InvoiceLineId
                    System.Data.SqlClient.SqlParameter pInvoiceLineId = command.Parameters.Add("@InvoiceLineId", System.Data.SqlDbType.Int);
                    pInvoiceLineId.Value = invoiceLineId;

                    command.ExecuteNonQuery();
                }
                finally
                {
                    PushConnection(command);
                }
            }
        }

        #endregion

        #region Upsert, Insert, Update, Delete, Select, Mapping - dbo.MediaType

        public static MediaType ReadMediaType(System.Data.SqlClient.SqlDataReader reader)
        {
            MediaType entity = new MediaType();
            entity.MediaTypeId = reader.GetInt32(0);
            entity.Name = reader.GetString(1);
            return entity;
        }

        public void UpsertMediaType(MediaType entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("MERGE [dbo].[MediaType] AS T USING (SELECT @MediaTypeId, @Name) AS S ([MediaTypeId], [Name]) ON (S.[MediaTypeId] = T.[MediaTypeId]) WHEN MATCHED THEN UPDATE SET [Name] = S.[Name] WHEN NOT MATCHED THEN INSERT ([Name]) VALUES (S.[Name]) OUTPUT inserted.[MediaTypeId];"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @Name
                    System.Data.SqlClient.SqlParameter pName = command.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar, 240);
                    if (entity.Name == null)
                    {
                        pName.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pName.Value = entity.Name;
                    }

                    // Parameter settings: @MediaTypeId
                    System.Data.SqlClient.SqlParameter pMediaTypeId = command.Parameters.Add("@MediaTypeId", System.Data.SqlDbType.Int);
                    pMediaTypeId.Value = entity.MediaTypeId;

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            entity.MediaTypeId = reader.GetInt32(0);
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

        public void InsertMediaType(MediaType entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("INSERT INTO [dbo].[MediaType] ([Name])  OUTPUT inserted.[MediaTypeId] VALUES (@Name);"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @Name
                    System.Data.SqlClient.SqlParameter pName = command.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar, 240);
                    if (entity.Name == null)
                    {
                        pName.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pName.Value = entity.Name;
                    }

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            entity.MediaTypeId = reader.GetInt32(0);
                        }
                        else
                        {
                            throw new InvalidOperationException("Insert failed.");
                        }
                    }
                }
                finally
                {
                    PushConnection(command);
                }
            }
        }

        public void UpdateMediaType(MediaType entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("UPDATE [dbo].[MediaType] SET [Name] = @Name WHERE [MediaTypeId] = @MediaTypeId;"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @MediaTypeId
                    System.Data.SqlClient.SqlParameter pMediaTypeId = command.Parameters.Add("@MediaTypeId", System.Data.SqlDbType.Int);
                    pMediaTypeId.Value = entity.MediaTypeId;

                    // Parameter settings: @Name
                    System.Data.SqlClient.SqlParameter pName = command.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar, 240);
                    if (entity.Name == null)
                    {
                        pName.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pName.Value = entity.Name;
                    }

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

        public List<MediaType> SelectMediaType()
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [MediaTypeId], [Name] FROM [dbo].[MediaType]"))
            {
                try
                {
                    PopConnection(command);

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        List<MediaType> result = new List<MediaType>();
                        while (reader.Read())
                        {
                            result.Add(ReadMediaType(reader));
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

        public int SelectMediaTypeCount()
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT Count(*) FROM [dbo].[MediaType]"))
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

        public List<MediaType> SelectMediaTypePaged(int firstIndex, int lastIndex)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT * FROM ( SELECT [MediaTypeId], [Name], ROW_NUMBER() OVER(ORDER BY [MediaTypeId]) AS _ROW_NUMBER FROM [dbo].[MediaType] ) AS T0 WHERE _ROW_NUMBER BETWEEN @__FirstIndex AND @__LastIndex ORDER BY [MediaTypeId]"))
            {
                try
                {
                    PopConnection(command);

                    System.Data.SqlClient.SqlParameter pFirstIndex = command.Parameters.Add("@__FirstIndex", System.Data.SqlDbType.Int);
                    pFirstIndex.Value = firstIndex;

                    System.Data.SqlClient.SqlParameter pLastIndex = command.Parameters.Add("@__LastIndex", System.Data.SqlDbType.Int);
                    pLastIndex.Value = lastIndex;

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        List<MediaType> result = new List<MediaType>();
                        while (reader.Read())
                        {
                            result.Add(ReadMediaType(reader));
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

        public MediaType SelectMediaTypeByMediaTypeId(System.Int32 mediaTypeId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [MediaTypeId], [Name] FROM [dbo].[MediaType] WHERE [MediaTypeId] = @MediaTypeId"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @MediaTypeId
                    System.Data.SqlClient.SqlParameter pMediaTypeId = command.Parameters.Add("@MediaTypeId", System.Data.SqlDbType.Int);
                    pMediaTypeId.Value = mediaTypeId;

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return ReadMediaType(reader);
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

        public void DeleteMediaTypeByMediaTypeId(System.Int32 mediaTypeId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("DELETE FROM [dbo].[MediaType] WHERE [MediaTypeId] = @MediaTypeId"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @MediaTypeId
                    System.Data.SqlClient.SqlParameter pMediaTypeId = command.Parameters.Add("@MediaTypeId", System.Data.SqlDbType.Int);
                    pMediaTypeId.Value = mediaTypeId;

                    command.ExecuteNonQuery();
                }
                finally
                {
                    PushConnection(command);
                }
            }
        }

        #endregion

        #region Upsert, Insert, Update, Delete, Select, Mapping - dbo.Playlist

        public static Playlist ReadPlaylist(System.Data.SqlClient.SqlDataReader reader)
        {
            Playlist entity = new Playlist();
            entity.PlaylistId = reader.GetInt32(0);
            entity.Name = reader.GetString(1);
            return entity;
        }

        public void UpsertPlaylist(Playlist entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("MERGE [dbo].[Playlist] AS T USING (SELECT @PlaylistId, @Name) AS S ([PlaylistId], [Name]) ON (S.[PlaylistId] = T.[PlaylistId]) WHEN MATCHED THEN UPDATE SET [Name] = S.[Name] WHEN NOT MATCHED THEN INSERT ([Name]) VALUES (S.[Name]) OUTPUT inserted.[PlaylistId];"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @Name
                    System.Data.SqlClient.SqlParameter pName = command.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar, 240);
                    if (entity.Name == null)
                    {
                        pName.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pName.Value = entity.Name;
                    }

                    // Parameter settings: @PlaylistId
                    System.Data.SqlClient.SqlParameter pPlaylistId = command.Parameters.Add("@PlaylistId", System.Data.SqlDbType.Int);
                    pPlaylistId.Value = entity.PlaylistId;

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            entity.PlaylistId = reader.GetInt32(0);
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

        public void InsertPlaylist(Playlist entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("INSERT INTO [dbo].[Playlist] ([Name])  OUTPUT inserted.[PlaylistId] VALUES (@Name);"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @Name
                    System.Data.SqlClient.SqlParameter pName = command.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar, 240);
                    if (entity.Name == null)
                    {
                        pName.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pName.Value = entity.Name;
                    }

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            entity.PlaylistId = reader.GetInt32(0);
                        }
                        else
                        {
                            throw new InvalidOperationException("Insert failed.");
                        }
                    }
                }
                finally
                {
                    PushConnection(command);
                }
            }
        }

        public void UpdatePlaylist(Playlist entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("UPDATE [dbo].[Playlist] SET [Name] = @Name WHERE [PlaylistId] = @PlaylistId;"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @PlaylistId
                    System.Data.SqlClient.SqlParameter pPlaylistId = command.Parameters.Add("@PlaylistId", System.Data.SqlDbType.Int);
                    pPlaylistId.Value = entity.PlaylistId;

                    // Parameter settings: @Name
                    System.Data.SqlClient.SqlParameter pName = command.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar, 240);
                    if (entity.Name == null)
                    {
                        pName.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pName.Value = entity.Name;
                    }

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

        public List<Playlist> SelectPlaylist()
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [PlaylistId], [Name] FROM [dbo].[Playlist]"))
            {
                try
                {
                    PopConnection(command);

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        List<Playlist> result = new List<Playlist>();
                        while (reader.Read())
                        {
                            result.Add(ReadPlaylist(reader));
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

        public int SelectPlaylistCount()
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT Count(*) FROM [dbo].[Playlist]"))
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

        public List<Playlist> SelectPlaylistPaged(int firstIndex, int lastIndex)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT * FROM ( SELECT [PlaylistId], [Name], ROW_NUMBER() OVER(ORDER BY [PlaylistId]) AS _ROW_NUMBER FROM [dbo].[Playlist] ) AS T0 WHERE _ROW_NUMBER BETWEEN @__FirstIndex AND @__LastIndex ORDER BY [PlaylistId]"))
            {
                try
                {
                    PopConnection(command);

                    System.Data.SqlClient.SqlParameter pFirstIndex = command.Parameters.Add("@__FirstIndex", System.Data.SqlDbType.Int);
                    pFirstIndex.Value = firstIndex;

                    System.Data.SqlClient.SqlParameter pLastIndex = command.Parameters.Add("@__LastIndex", System.Data.SqlDbType.Int);
                    pLastIndex.Value = lastIndex;

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        List<Playlist> result = new List<Playlist>();
                        while (reader.Read())
                        {
                            result.Add(ReadPlaylist(reader));
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

        public Playlist SelectPlaylistByPlaylistId(System.Int32 playlistId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [PlaylistId], [Name] FROM [dbo].[Playlist] WHERE [PlaylistId] = @PlaylistId"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @PlaylistId
                    System.Data.SqlClient.SqlParameter pPlaylistId = command.Parameters.Add("@PlaylistId", System.Data.SqlDbType.Int);
                    pPlaylistId.Value = playlistId;

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return ReadPlaylist(reader);
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

        public void DeletePlaylistByPlaylistId(System.Int32 playlistId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("DELETE FROM [dbo].[Playlist] WHERE [PlaylistId] = @PlaylistId"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @PlaylistId
                    System.Data.SqlClient.SqlParameter pPlaylistId = command.Parameters.Add("@PlaylistId", System.Data.SqlDbType.Int);
                    pPlaylistId.Value = playlistId;

                    command.ExecuteNonQuery();
                }
                finally
                {
                    PushConnection(command);
                }
            }
        }

        #endregion

        #region Upsert, Insert, Update, Delete, Select, Mapping - dbo.PlaylistTrack

        public static PlaylistTrack ReadPlaylistTrack(System.Data.SqlClient.SqlDataReader reader)
        {
            PlaylistTrack entity = new PlaylistTrack();
            entity.PlaylistId = reader.GetInt32(0);
            entity.TrackId = reader.GetInt32(1);
            return entity;
        }


        public void InsertPlaylistTrack(PlaylistTrack entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("INSERT INTO [dbo].[PlaylistTrack] ([PlaylistId], [TrackId])  VALUES (@PlaylistId, @TrackId);"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @PlaylistId
                    System.Data.SqlClient.SqlParameter pPlaylistId = command.Parameters.Add("@PlaylistId", System.Data.SqlDbType.Int);
                    pPlaylistId.Value = entity.PlaylistId;

                    // Parameter settings: @TrackId
                    System.Data.SqlClient.SqlParameter pTrackId = command.Parameters.Add("@TrackId", System.Data.SqlDbType.Int);
                    pTrackId.Value = entity.TrackId;

                    if (command.ExecuteNonQuery() <= 0)
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


        public List<PlaylistTrack> SelectPlaylistTrack()
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [PlaylistId], [TrackId] FROM [dbo].[PlaylistTrack]"))
            {
                try
                {
                    PopConnection(command);

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        List<PlaylistTrack> result = new List<PlaylistTrack>();
                        while (reader.Read())
                        {
                            result.Add(ReadPlaylistTrack(reader));
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

        public int SelectPlaylistTrackCount()
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT Count(*) FROM [dbo].[PlaylistTrack]"))
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

        public List<PlaylistTrack> SelectPlaylistTrackPaged(int firstIndex, int lastIndex)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT * FROM ( SELECT [PlaylistId], [TrackId], ROW_NUMBER() OVER(ORDER BY [PlaylistId], [TrackId]) AS _ROW_NUMBER FROM [dbo].[PlaylistTrack] ) AS T0 WHERE _ROW_NUMBER BETWEEN @__FirstIndex AND @__LastIndex ORDER BY [PlaylistId], [TrackId]"))
            {
                try
                {
                    PopConnection(command);

                    System.Data.SqlClient.SqlParameter pFirstIndex = command.Parameters.Add("@__FirstIndex", System.Data.SqlDbType.Int);
                    pFirstIndex.Value = firstIndex;

                    System.Data.SqlClient.SqlParameter pLastIndex = command.Parameters.Add("@__LastIndex", System.Data.SqlDbType.Int);
                    pLastIndex.Value = lastIndex;

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        List<PlaylistTrack> result = new List<PlaylistTrack>();
                        while (reader.Read())
                        {
                            result.Add(ReadPlaylistTrack(reader));
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

        public List<PlaylistTrack> SelectPlaylistTrackByTrackId(System.Int32 trackId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [PlaylistId], [TrackId] FROM [dbo].[PlaylistTrack] WHERE [TrackId] = @TrackId"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @TrackId
                    System.Data.SqlClient.SqlParameter pTrackId = command.Parameters.Add("@TrackId", System.Data.SqlDbType.Int);
                    pTrackId.Value = trackId;

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        List<PlaylistTrack> result = new List<PlaylistTrack>();
                        while (reader.Read())
                        {
                            result.Add(ReadPlaylistTrack(reader));
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

        public void DeletePlaylistTrackByTrackId(System.Int32 trackId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("DELETE FROM [dbo].[PlaylistTrack] WHERE [TrackId] = @TrackId"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @TrackId
                    System.Data.SqlClient.SqlParameter pTrackId = command.Parameters.Add("@TrackId", System.Data.SqlDbType.Int);
                    pTrackId.Value = trackId;

                    command.ExecuteNonQuery();
                }
                finally
                {
                    PushConnection(command);
                }
            }
        }

        public PlaylistTrack SelectPlaylistTrackByPlaylistIdTrackId(System.Int32 playlistId, System.Int32 trackId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [PlaylistId], [TrackId] FROM [dbo].[PlaylistTrack] WHERE [PlaylistId] = @PlaylistId AND [TrackId] = @TrackId"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @PlaylistId
                    System.Data.SqlClient.SqlParameter pPlaylistId = command.Parameters.Add("@PlaylistId", System.Data.SqlDbType.Int);
                    pPlaylistId.Value = playlistId;

                    // Parameter settings: @TrackId
                    System.Data.SqlClient.SqlParameter pTrackId = command.Parameters.Add("@TrackId", System.Data.SqlDbType.Int);
                    pTrackId.Value = trackId;

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return ReadPlaylistTrack(reader);
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

        public void DeletePlaylistTrackByPlaylistIdTrackId(System.Int32 playlistId, System.Int32 trackId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("DELETE FROM [dbo].[PlaylistTrack] WHERE [PlaylistId] = @PlaylistId AND [TrackId] = @TrackId"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @PlaylistId
                    System.Data.SqlClient.SqlParameter pPlaylistId = command.Parameters.Add("@PlaylistId", System.Data.SqlDbType.Int);
                    pPlaylistId.Value = playlistId;

                    // Parameter settings: @TrackId
                    System.Data.SqlClient.SqlParameter pTrackId = command.Parameters.Add("@TrackId", System.Data.SqlDbType.Int);
                    pTrackId.Value = trackId;

                    command.ExecuteNonQuery();
                }
                finally
                {
                    PushConnection(command);
                }
            }
        }

        public List<PlaylistTrack> SelectPlaylistTrackByPlaylistId(System.Int32 playlistId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [PlaylistId], [TrackId] FROM [dbo].[PlaylistTrack] WHERE [PlaylistId] = @PlaylistId"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @PlaylistId
                    System.Data.SqlClient.SqlParameter pPlaylistId = command.Parameters.Add("@PlaylistId", System.Data.SqlDbType.Int);
                    pPlaylistId.Value = playlistId;

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        List<PlaylistTrack> result = new List<PlaylistTrack>();
                        while (reader.Read())
                        {
                            result.Add(ReadPlaylistTrack(reader));
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

        public void DeletePlaylistTrackByPlaylistId(System.Int32 playlistId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("DELETE FROM [dbo].[PlaylistTrack] WHERE [PlaylistId] = @PlaylistId"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @PlaylistId
                    System.Data.SqlClient.SqlParameter pPlaylistId = command.Parameters.Add("@PlaylistId", System.Data.SqlDbType.Int);
                    pPlaylistId.Value = playlistId;

                    command.ExecuteNonQuery();
                }
                finally
                {
                    PushConnection(command);
                }
            }
        }

        #endregion

        #region Upsert, Insert, Update, Delete, Select, Mapping - dbo.Track

        public static Track ReadTrack(System.Data.SqlClient.SqlDataReader reader)
        {
            Track entity = new Track();
            entity.TrackId = reader.GetInt32(0);
            entity.Name = reader.GetString(1);
            if (reader.IsDBNull(2))
            {
                entity.AlbumId = null;
            }
            else
            {
                entity.AlbumId = reader.GetInt32(2);
            }
            entity.MediaTypeId = reader.GetInt32(3);
            if (reader.IsDBNull(4))
            {
                entity.GenreId = null;
            }
            else
            {
                entity.GenreId = reader.GetInt32(4);
            }
            entity.Composer = reader.GetString(5);
            entity.Milliseconds = reader.GetInt32(6);
            if (reader.IsDBNull(7))
            {
                entity.Bytes = null;
            }
            else
            {
                entity.Bytes = reader.GetInt32(7);
            }
            entity.UnitPrice = reader.GetDecimal(8);
            return entity;
        }

        public void UpsertTrack(Track entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("MERGE [dbo].[Track] AS T USING (SELECT @TrackId, @Name, @AlbumId, @MediaTypeId, @GenreId, @Composer, @Milliseconds, @Bytes, @UnitPrice) AS S ([TrackId], [Name], [AlbumId], [MediaTypeId], [GenreId], [Composer], [Milliseconds], [Bytes], [UnitPrice]) ON (S.[TrackId] = T.[TrackId]) WHEN MATCHED THEN UPDATE SET [Name] = S.[Name], [AlbumId] = S.[AlbumId], [MediaTypeId] = S.[MediaTypeId], [GenreId] = S.[GenreId], [Composer] = S.[Composer], [Milliseconds] = S.[Milliseconds], [Bytes] = S.[Bytes], [UnitPrice] = S.[UnitPrice] WHEN NOT MATCHED THEN INSERT ([Name], [AlbumId], [MediaTypeId], [GenreId], [Composer], [Milliseconds], [Bytes], [UnitPrice]) VALUES (S.[Name], S.[AlbumId], S.[MediaTypeId], S.[GenreId], S.[Composer], S.[Milliseconds], S.[Bytes], S.[UnitPrice]) OUTPUT inserted.[TrackId];"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @Name
                    System.Data.SqlClient.SqlParameter pName = command.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar, 400);
                    pName.Value = entity.Name;

                    // Parameter settings: @AlbumId
                    System.Data.SqlClient.SqlParameter pAlbumId = command.Parameters.Add("@AlbumId", System.Data.SqlDbType.Int);
                    if (entity.AlbumId == null)
                    {
                        pAlbumId.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pAlbumId.Value = entity.AlbumId;
                    }

                    // Parameter settings: @MediaTypeId
                    System.Data.SqlClient.SqlParameter pMediaTypeId = command.Parameters.Add("@MediaTypeId", System.Data.SqlDbType.Int);
                    pMediaTypeId.Value = entity.MediaTypeId;

                    // Parameter settings: @GenreId
                    System.Data.SqlClient.SqlParameter pGenreId = command.Parameters.Add("@GenreId", System.Data.SqlDbType.Int);
                    if (entity.GenreId == null)
                    {
                        pGenreId.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pGenreId.Value = entity.GenreId;
                    }

                    // Parameter settings: @Composer
                    System.Data.SqlClient.SqlParameter pComposer = command.Parameters.Add("@Composer", System.Data.SqlDbType.NVarChar, 440);
                    if (entity.Composer == null)
                    {
                        pComposer.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pComposer.Value = entity.Composer;
                    }

                    // Parameter settings: @Milliseconds
                    System.Data.SqlClient.SqlParameter pMilliseconds = command.Parameters.Add("@Milliseconds", System.Data.SqlDbType.Int);
                    pMilliseconds.Value = entity.Milliseconds;

                    // Parameter settings: @Bytes
                    System.Data.SqlClient.SqlParameter pBytes = command.Parameters.Add("@Bytes", System.Data.SqlDbType.Int);
                    if (entity.Bytes == null)
                    {
                        pBytes.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pBytes.Value = entity.Bytes;
                    }

                    // Parameter settings: @UnitPrice
                    System.Data.SqlClient.SqlParameter pUnitPrice = command.Parameters.Add("@UnitPrice", System.Data.SqlDbType.Decimal);
                    pUnitPrice.Value = entity.UnitPrice;

                    // Parameter settings: @TrackId
                    System.Data.SqlClient.SqlParameter pTrackId = command.Parameters.Add("@TrackId", System.Data.SqlDbType.Int);
                    pTrackId.Value = entity.TrackId;

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            entity.TrackId = reader.GetInt32(0);
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

        public void InsertTrack(Track entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("INSERT INTO [dbo].[Track] ([Name], [AlbumId], [MediaTypeId], [GenreId], [Composer], [Milliseconds], [Bytes], [UnitPrice])  OUTPUT inserted.[TrackId] VALUES (@Name, @AlbumId, @MediaTypeId, @GenreId, @Composer, @Milliseconds, @Bytes, @UnitPrice);"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @Name
                    System.Data.SqlClient.SqlParameter pName = command.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar, 400);
                    pName.Value = entity.Name;

                    // Parameter settings: @AlbumId
                    System.Data.SqlClient.SqlParameter pAlbumId = command.Parameters.Add("@AlbumId", System.Data.SqlDbType.Int);
                    if (entity.AlbumId == null)
                    {
                        pAlbumId.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pAlbumId.Value = entity.AlbumId;
                    }

                    // Parameter settings: @MediaTypeId
                    System.Data.SqlClient.SqlParameter pMediaTypeId = command.Parameters.Add("@MediaTypeId", System.Data.SqlDbType.Int);
                    pMediaTypeId.Value = entity.MediaTypeId;

                    // Parameter settings: @GenreId
                    System.Data.SqlClient.SqlParameter pGenreId = command.Parameters.Add("@GenreId", System.Data.SqlDbType.Int);
                    if (entity.GenreId == null)
                    {
                        pGenreId.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pGenreId.Value = entity.GenreId;
                    }

                    // Parameter settings: @Composer
                    System.Data.SqlClient.SqlParameter pComposer = command.Parameters.Add("@Composer", System.Data.SqlDbType.NVarChar, 440);
                    if (entity.Composer == null)
                    {
                        pComposer.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pComposer.Value = entity.Composer;
                    }

                    // Parameter settings: @Milliseconds
                    System.Data.SqlClient.SqlParameter pMilliseconds = command.Parameters.Add("@Milliseconds", System.Data.SqlDbType.Int);
                    pMilliseconds.Value = entity.Milliseconds;

                    // Parameter settings: @Bytes
                    System.Data.SqlClient.SqlParameter pBytes = command.Parameters.Add("@Bytes", System.Data.SqlDbType.Int);
                    if (entity.Bytes == null)
                    {
                        pBytes.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pBytes.Value = entity.Bytes;
                    }

                    // Parameter settings: @UnitPrice
                    System.Data.SqlClient.SqlParameter pUnitPrice = command.Parameters.Add("@UnitPrice", System.Data.SqlDbType.Decimal);
                    pUnitPrice.Value = entity.UnitPrice;

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            entity.TrackId = reader.GetInt32(0);
                        }
                        else
                        {
                            throw new InvalidOperationException("Insert failed.");
                        }
                    }
                }
                finally
                {
                    PushConnection(command);
                }
            }
        }

        public void UpdateTrack(Track entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("UPDATE [dbo].[Track] SET [Name] = @Name, [AlbumId] = @AlbumId, [MediaTypeId] = @MediaTypeId, [GenreId] = @GenreId, [Composer] = @Composer, [Milliseconds] = @Milliseconds, [Bytes] = @Bytes, [UnitPrice] = @UnitPrice WHERE [TrackId] = @TrackId;"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @TrackId
                    System.Data.SqlClient.SqlParameter pTrackId = command.Parameters.Add("@TrackId", System.Data.SqlDbType.Int);
                    pTrackId.Value = entity.TrackId;

                    // Parameter settings: @Name
                    System.Data.SqlClient.SqlParameter pName = command.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar, 400);
                    pName.Value = entity.Name;

                    // Parameter settings: @AlbumId
                    System.Data.SqlClient.SqlParameter pAlbumId = command.Parameters.Add("@AlbumId", System.Data.SqlDbType.Int);
                    if (entity.AlbumId == null)
                    {
                        pAlbumId.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pAlbumId.Value = entity.AlbumId;
                    }

                    // Parameter settings: @MediaTypeId
                    System.Data.SqlClient.SqlParameter pMediaTypeId = command.Parameters.Add("@MediaTypeId", System.Data.SqlDbType.Int);
                    pMediaTypeId.Value = entity.MediaTypeId;

                    // Parameter settings: @GenreId
                    System.Data.SqlClient.SqlParameter pGenreId = command.Parameters.Add("@GenreId", System.Data.SqlDbType.Int);
                    if (entity.GenreId == null)
                    {
                        pGenreId.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pGenreId.Value = entity.GenreId;
                    }

                    // Parameter settings: @Composer
                    System.Data.SqlClient.SqlParameter pComposer = command.Parameters.Add("@Composer", System.Data.SqlDbType.NVarChar, 440);
                    if (entity.Composer == null)
                    {
                        pComposer.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pComposer.Value = entity.Composer;
                    }

                    // Parameter settings: @Milliseconds
                    System.Data.SqlClient.SqlParameter pMilliseconds = command.Parameters.Add("@Milliseconds", System.Data.SqlDbType.Int);
                    pMilliseconds.Value = entity.Milliseconds;

                    // Parameter settings: @Bytes
                    System.Data.SqlClient.SqlParameter pBytes = command.Parameters.Add("@Bytes", System.Data.SqlDbType.Int);
                    if (entity.Bytes == null)
                    {
                        pBytes.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pBytes.Value = entity.Bytes;
                    }

                    // Parameter settings: @UnitPrice
                    System.Data.SqlClient.SqlParameter pUnitPrice = command.Parameters.Add("@UnitPrice", System.Data.SqlDbType.Decimal);
                    pUnitPrice.Value = entity.UnitPrice;

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

        public List<Track> SelectTrack()
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [TrackId], [Name], [AlbumId], [MediaTypeId], [GenreId], [Composer], [Milliseconds], [Bytes], [UnitPrice] FROM [dbo].[Track]"))
            {
                try
                {
                    PopConnection(command);

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        List<Track> result = new List<Track>();
                        while (reader.Read())
                        {
                            result.Add(ReadTrack(reader));
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

        public int SelectTrackCount()
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT Count(*) FROM [dbo].[Track]"))
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

        public List<Track> SelectTrackPaged(int firstIndex, int lastIndex)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT * FROM ( SELECT [TrackId], [Name], [AlbumId], [MediaTypeId], [GenreId], [Composer], [Milliseconds], [Bytes], [UnitPrice], ROW_NUMBER() OVER(ORDER BY [TrackId]) AS _ROW_NUMBER FROM [dbo].[Track] ) AS T0 WHERE _ROW_NUMBER BETWEEN @__FirstIndex AND @__LastIndex ORDER BY [TrackId]"))
            {
                try
                {
                    PopConnection(command);

                    System.Data.SqlClient.SqlParameter pFirstIndex = command.Parameters.Add("@__FirstIndex", System.Data.SqlDbType.Int);
                    pFirstIndex.Value = firstIndex;

                    System.Data.SqlClient.SqlParameter pLastIndex = command.Parameters.Add("@__LastIndex", System.Data.SqlDbType.Int);
                    pLastIndex.Value = lastIndex;

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        List<Track> result = new List<Track>();
                        while (reader.Read())
                        {
                            result.Add(ReadTrack(reader));
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

        public List<Track> SelectTrackByAlbumId(System.Nullable<System.Int32> albumId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [TrackId], [Name], [AlbumId], [MediaTypeId], [GenreId], [Composer], [Milliseconds], [Bytes], [UnitPrice] FROM [dbo].[Track] WHERE [AlbumId] = @AlbumId"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @AlbumId
                    System.Data.SqlClient.SqlParameter pAlbumId = command.Parameters.Add("@AlbumId", System.Data.SqlDbType.Int);
                    if (albumId == null)
                    {
                        pAlbumId.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pAlbumId.Value = albumId;
                    }

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        List<Track> result = new List<Track>();
                        while (reader.Read())
                        {
                            result.Add(ReadTrack(reader));
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

        public void DeleteTrackByAlbumId(System.Nullable<System.Int32> albumId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("DELETE FROM [dbo].[Track] WHERE [AlbumId] = @AlbumId"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @AlbumId
                    System.Data.SqlClient.SqlParameter pAlbumId = command.Parameters.Add("@AlbumId", System.Data.SqlDbType.Int);
                    if (albumId == null)
                    {
                        pAlbumId.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pAlbumId.Value = albumId;
                    }

                    command.ExecuteNonQuery();
                }
                finally
                {
                    PushConnection(command);
                }
            }
        }

        public List<Track> SelectTrackByGenreId(System.Nullable<System.Int32> genreId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [TrackId], [Name], [AlbumId], [MediaTypeId], [GenreId], [Composer], [Milliseconds], [Bytes], [UnitPrice] FROM [dbo].[Track] WHERE [GenreId] = @GenreId"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @GenreId
                    System.Data.SqlClient.SqlParameter pGenreId = command.Parameters.Add("@GenreId", System.Data.SqlDbType.Int);
                    if (genreId == null)
                    {
                        pGenreId.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pGenreId.Value = genreId;
                    }

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        List<Track> result = new List<Track>();
                        while (reader.Read())
                        {
                            result.Add(ReadTrack(reader));
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

        public void DeleteTrackByGenreId(System.Nullable<System.Int32> genreId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("DELETE FROM [dbo].[Track] WHERE [GenreId] = @GenreId"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @GenreId
                    System.Data.SqlClient.SqlParameter pGenreId = command.Parameters.Add("@GenreId", System.Data.SqlDbType.Int);
                    if (genreId == null)
                    {
                        pGenreId.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pGenreId.Value = genreId;
                    }

                    command.ExecuteNonQuery();
                }
                finally
                {
                    PushConnection(command);
                }
            }
        }

        public List<Track> SelectTrackByMediaTypeId(System.Int32 mediaTypeId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [TrackId], [Name], [AlbumId], [MediaTypeId], [GenreId], [Composer], [Milliseconds], [Bytes], [UnitPrice] FROM [dbo].[Track] WHERE [MediaTypeId] = @MediaTypeId"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @MediaTypeId
                    System.Data.SqlClient.SqlParameter pMediaTypeId = command.Parameters.Add("@MediaTypeId", System.Data.SqlDbType.Int);
                    pMediaTypeId.Value = mediaTypeId;

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        List<Track> result = new List<Track>();
                        while (reader.Read())
                        {
                            result.Add(ReadTrack(reader));
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

        public void DeleteTrackByMediaTypeId(System.Int32 mediaTypeId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("DELETE FROM [dbo].[Track] WHERE [MediaTypeId] = @MediaTypeId"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @MediaTypeId
                    System.Data.SqlClient.SqlParameter pMediaTypeId = command.Parameters.Add("@MediaTypeId", System.Data.SqlDbType.Int);
                    pMediaTypeId.Value = mediaTypeId;

                    command.ExecuteNonQuery();
                }
                finally
                {
                    PushConnection(command);
                }
            }
        }

        public Track SelectTrackByTrackId(System.Int32 trackId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [TrackId], [Name], [AlbumId], [MediaTypeId], [GenreId], [Composer], [Milliseconds], [Bytes], [UnitPrice] FROM [dbo].[Track] WHERE [TrackId] = @TrackId"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @TrackId
                    System.Data.SqlClient.SqlParameter pTrackId = command.Parameters.Add("@TrackId", System.Data.SqlDbType.Int);
                    pTrackId.Value = trackId;

                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return ReadTrack(reader);
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

        public void DeleteTrackByTrackId(System.Int32 trackId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("DELETE FROM [dbo].[Track] WHERE [TrackId] = @TrackId"))
            {
                try
                {
                    PopConnection(command);

                    // Parameter settings: @TrackId
                    System.Data.SqlClient.SqlParameter pTrackId = command.Parameters.Add("@TrackId", System.Data.SqlDbType.Int);
                    pTrackId.Value = trackId;

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

                        System.Data.SqlClient.SqlParameter pName = command.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar, -1);
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

        
        private void PopConnection(System.Data.SqlClient.SqlCommand command)
        {
            if (this.connection != null)
            {
                command.Connection = this.connection;
                command.Transaction = this.transaction;
            }
            else
            {
                command.Connection = new System.Data.SqlClient.SqlConnection(this.connectionString);
                command.Connection.Open();
            }
        }
        
        private void PushConnection(System.Data.SqlClient.SqlCommand command)
        {
            System.Data.SqlClient.SqlConnection connection = command.Connection;
            System.Data.SqlClient.SqlTransaction transaction = command.Transaction;
        
            command.Connection = null;
            command.Transaction = null;
        
            if (connection != null && this.connection != connection)
            {
                connection.Close();
            }
        }
        
        public void BeginTransaction(System.Data.IsolationLevel isolationLevel = System.Data.IsolationLevel.Unspecified)
        {
            if (this.connection == null)
            {
                this.connection = new System.Data.SqlClient.SqlConnection(this.connectionString);
                this.connection.Open();
            }
                
            if (this.transaction == null)
            {
                this.transaction = this.connection.BeginTransaction(isolationLevel);
            }
            else
            {
                if (isolationLevel != System.Data.IsolationLevel.Unspecified && this.transaction.IsolationLevel != isolationLevel)
                {
                    throw new InvalidOperationException("Transaction isolation level mismatch.");
                }
            }
                
            ++this.transactionCounter;
        }
        
        public void CommitTransaction()
        {
            if (this.transaction == null || this.transactionCounter <= 0)
            {
                throw new InvalidOperationException("currentTransaction");
            }
        
            --this.transactionCounter;
        
            if (this.transactionCounter == 0)
            {
                this.transaction.Commit();
                this.transaction = null;
            }
        }
        
        public void RollbackTransaction()
        {
            if (this.transaction == null || this.transactionCounter <= 0)
            {
                throw new InvalidOperationException("currentTransaction");
            }
        
            this.transactionCounter = 0;
            this.transaction.Rollback();
            this.transaction = null;
        }
        
        public void Dispose()
        {
            if (this.externalResource)
            {
                return;
            }
        
            try
            {
                if (this.transaction != null)
                {
                    this.transaction.Rollback();
                    this.transaction = null;
                    this.transactionCounter = 0;
                }
            }
            finally
            {
                if (this.connection != null)
                {
                    this.connection.Close();
                    this.connection = null;
                }
            }
        }
        
    }
}
