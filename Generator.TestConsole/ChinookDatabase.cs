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

    public class ChinookDatabase : IDisposable
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

        #region Insert, Update, Delete, Select, Mapping - dbo.Artist

        public static Artist ReadArtist(System.Data.SqlClient.SqlDataReader reader)
        {
            Artist entity = new Artist();
            entity.ArtistId = reader.GetInt32(0);
            entity.Name = reader.GetString(1);
            return entity;
        }

        public void InsertArtist(Artist entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("INSERT INTO [dbo].[Artist] ([Name]) VALUES (@Name); SET @ArtistId = SCOPE_IDENTITY(); "))
            {
                try
                {
                    PopConnection(command);
                    System.Data.SqlClient.SqlParameter pArtistId = command.Parameters.Add("@ArtistId", System.Data.SqlDbType.Int);
                    pArtistId.Direction = System.Data.ParameterDirection.Output;

                    System.Data.SqlClient.SqlParameter pName = command.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar);
                    if (entity.Name == null)
                    {
                        pName.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pName.Value = entity.Name;
                    }

                    if (command.ExecuteNonQuery() > 0)
                    {
                        entity.ArtistId = (System.Int32)pArtistId.Value;
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

        public void UpdateArtist(Artist entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("UPDATE [dbo].[Artist] SET [Name] = @Name WHERE [ArtistId] = @ArtistId"))
            {
                try
                {
                    PopConnection(command);
                    System.Data.SqlClient.SqlParameter pArtistId = command.Parameters.Add("@ArtistId", System.Data.SqlDbType.Int);
                    pArtistId.Value = entity.ArtistId;

                    System.Data.SqlClient.SqlParameter pName = command.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar);
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

        public Artist SelectArtistByArtistId(System.Int32 artistId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [ArtistId], [Name] FROM [dbo].[Artist] WHERE [ArtistId] = @ArtistId"))
            {
                try
                {
                    PopConnection(command);
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

        #region Insert, Update, Delete, Select, Mapping - dbo.Customer

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
            return entity;
        }

        public void InsertCustomer(Customer entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("INSERT INTO [dbo].[Customer] ([FirstName], [LastName], [Company], [Address], [City], [State], [Country], [PostalCode], [Phone], [Fax], [Email], [SupportRepId]) VALUES (@FirstName, @LastName, @Company, @Address, @City, @State, @Country, @PostalCode, @Phone, @Fax, @Email, @SupportRepId); SET @CustomerId = SCOPE_IDENTITY(); "))
            {
                try
                {
                    PopConnection(command);
                    System.Data.SqlClient.SqlParameter pCustomerId = command.Parameters.Add("@CustomerId", System.Data.SqlDbType.Int);
                    pCustomerId.Direction = System.Data.ParameterDirection.Output;

                    System.Data.SqlClient.SqlParameter pFirstName = command.Parameters.Add("@FirstName", System.Data.SqlDbType.NVarChar);
                    pFirstName.Value = entity.FirstName;

                    System.Data.SqlClient.SqlParameter pLastName = command.Parameters.Add("@LastName", System.Data.SqlDbType.NVarChar);
                    pLastName.Value = entity.LastName;

                    System.Data.SqlClient.SqlParameter pCompany = command.Parameters.Add("@Company", System.Data.SqlDbType.NVarChar);
                    if (entity.Company == null)
                    {
                        pCompany.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pCompany.Value = entity.Company;
                    }

                    System.Data.SqlClient.SqlParameter pAddress = command.Parameters.Add("@Address", System.Data.SqlDbType.NVarChar);
                    if (entity.Address == null)
                    {
                        pAddress.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pAddress.Value = entity.Address;
                    }

                    System.Data.SqlClient.SqlParameter pCity = command.Parameters.Add("@City", System.Data.SqlDbType.NVarChar);
                    if (entity.City == null)
                    {
                        pCity.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pCity.Value = entity.City;
                    }

                    System.Data.SqlClient.SqlParameter pState = command.Parameters.Add("@State", System.Data.SqlDbType.NVarChar);
                    if (entity.State == null)
                    {
                        pState.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pState.Value = entity.State;
                    }

                    System.Data.SqlClient.SqlParameter pCountry = command.Parameters.Add("@Country", System.Data.SqlDbType.NVarChar);
                    if (entity.Country == null)
                    {
                        pCountry.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pCountry.Value = entity.Country;
                    }

                    System.Data.SqlClient.SqlParameter pPostalCode = command.Parameters.Add("@PostalCode", System.Data.SqlDbType.NVarChar);
                    if (entity.PostalCode == null)
                    {
                        pPostalCode.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pPostalCode.Value = entity.PostalCode;
                    }

                    System.Data.SqlClient.SqlParameter pPhone = command.Parameters.Add("@Phone", System.Data.SqlDbType.NVarChar);
                    if (entity.Phone == null)
                    {
                        pPhone.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pPhone.Value = entity.Phone;
                    }

                    System.Data.SqlClient.SqlParameter pFax = command.Parameters.Add("@Fax", System.Data.SqlDbType.NVarChar);
                    if (entity.Fax == null)
                    {
                        pFax.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pFax.Value = entity.Fax;
                    }

                    System.Data.SqlClient.SqlParameter pEmail = command.Parameters.Add("@Email", System.Data.SqlDbType.NVarChar);
                    pEmail.Value = entity.Email;

                    System.Data.SqlClient.SqlParameter pSupportRepId = command.Parameters.Add("@SupportRepId", System.Data.SqlDbType.Int);
                    if (entity.SupportRepId == null)
                    {
                        pSupportRepId.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pSupportRepId.Value = entity.SupportRepId;
                    }

                    if (command.ExecuteNonQuery() > 0)
                    {
                        entity.CustomerId = (System.Int32)pCustomerId.Value;
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

        public void UpdateCustomer(Customer entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("UPDATE [dbo].[Customer] SET [FirstName] = @FirstName, [LastName] = @LastName, [Company] = @Company, [Address] = @Address, [City] = @City, [State] = @State, [Country] = @Country, [PostalCode] = @PostalCode, [Phone] = @Phone, [Fax] = @Fax, [Email] = @Email, [SupportRepId] = @SupportRepId WHERE [CustomerId] = @CustomerId"))
            {
                try
                {
                    PopConnection(command);
                    System.Data.SqlClient.SqlParameter pCustomerId = command.Parameters.Add("@CustomerId", System.Data.SqlDbType.Int);
                    pCustomerId.Value = entity.CustomerId;

                    System.Data.SqlClient.SqlParameter pFirstName = command.Parameters.Add("@FirstName", System.Data.SqlDbType.NVarChar);
                    pFirstName.Value = entity.FirstName;

                    System.Data.SqlClient.SqlParameter pLastName = command.Parameters.Add("@LastName", System.Data.SqlDbType.NVarChar);
                    pLastName.Value = entity.LastName;

                    System.Data.SqlClient.SqlParameter pCompany = command.Parameters.Add("@Company", System.Data.SqlDbType.NVarChar);
                    if (entity.Company == null)
                    {
                        pCompany.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pCompany.Value = entity.Company;
                    }

                    System.Data.SqlClient.SqlParameter pAddress = command.Parameters.Add("@Address", System.Data.SqlDbType.NVarChar);
                    if (entity.Address == null)
                    {
                        pAddress.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pAddress.Value = entity.Address;
                    }

                    System.Data.SqlClient.SqlParameter pCity = command.Parameters.Add("@City", System.Data.SqlDbType.NVarChar);
                    if (entity.City == null)
                    {
                        pCity.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pCity.Value = entity.City;
                    }

                    System.Data.SqlClient.SqlParameter pState = command.Parameters.Add("@State", System.Data.SqlDbType.NVarChar);
                    if (entity.State == null)
                    {
                        pState.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pState.Value = entity.State;
                    }

                    System.Data.SqlClient.SqlParameter pCountry = command.Parameters.Add("@Country", System.Data.SqlDbType.NVarChar);
                    if (entity.Country == null)
                    {
                        pCountry.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pCountry.Value = entity.Country;
                    }

                    System.Data.SqlClient.SqlParameter pPostalCode = command.Parameters.Add("@PostalCode", System.Data.SqlDbType.NVarChar);
                    if (entity.PostalCode == null)
                    {
                        pPostalCode.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pPostalCode.Value = entity.PostalCode;
                    }

                    System.Data.SqlClient.SqlParameter pPhone = command.Parameters.Add("@Phone", System.Data.SqlDbType.NVarChar);
                    if (entity.Phone == null)
                    {
                        pPhone.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pPhone.Value = entity.Phone;
                    }

                    System.Data.SqlClient.SqlParameter pFax = command.Parameters.Add("@Fax", System.Data.SqlDbType.NVarChar);
                    if (entity.Fax == null)
                    {
                        pFax.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pFax.Value = entity.Fax;
                    }

                    System.Data.SqlClient.SqlParameter pEmail = command.Parameters.Add("@Email", System.Data.SqlDbType.NVarChar);
                    pEmail.Value = entity.Email;

                    System.Data.SqlClient.SqlParameter pSupportRepId = command.Parameters.Add("@SupportRepId", System.Data.SqlDbType.Int);
                    if (entity.SupportRepId == null)
                    {
                        pSupportRepId.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pSupportRepId.Value = entity.SupportRepId;
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

        public List<Customer> SelectCustomer()
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [CustomerId], [FirstName], [LastName], [Company], [Address], [City], [State], [Country], [PostalCode], [Phone], [Fax], [Email], [SupportRepId] FROM [dbo].[Customer]"))
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

        public List<Customer> SelectCustomerBySupportRepId(System.Nullable<System.Int32> supportRepId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [CustomerId], [FirstName], [LastName], [Company], [Address], [City], [State], [Country], [PostalCode], [Phone], [Fax], [Email], [SupportRepId] FROM [dbo].[Customer] WHERE [SupportRepId] = @SupportRepId"))
            {
                try
                {
                    PopConnection(command);
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
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [CustomerId], [FirstName], [LastName], [Company], [Address], [City], [State], [Country], [PostalCode], [Phone], [Fax], [Email], [SupportRepId] FROM [dbo].[Customer] WHERE [CustomerId] = @CustomerId"))
            {
                try
                {
                    PopConnection(command);
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

        #region Insert, Update, Delete, Select, Mapping - dbo.Employee

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

        public void InsertEmployee(Employee entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("INSERT INTO [dbo].[Employee] ([LastName], [FirstName], [Title], [ReportsTo], [BirthDate], [HireDate], [Address], [City], [State], [Country], [PostalCode], [Phone], [Fax], [Email]) VALUES (@LastName, @FirstName, @Title, @ReportsTo, @BirthDate, @HireDate, @Address, @City, @State, @Country, @PostalCode, @Phone, @Fax, @Email); SET @EmployeeId = SCOPE_IDENTITY(); "))
            {
                try
                {
                    PopConnection(command);
                    System.Data.SqlClient.SqlParameter pEmployeeId = command.Parameters.Add("@EmployeeId", System.Data.SqlDbType.Int);
                    pEmployeeId.Direction = System.Data.ParameterDirection.Output;

                    System.Data.SqlClient.SqlParameter pLastName = command.Parameters.Add("@LastName", System.Data.SqlDbType.NVarChar);
                    pLastName.Value = entity.LastName;

                    System.Data.SqlClient.SqlParameter pFirstName = command.Parameters.Add("@FirstName", System.Data.SqlDbType.NVarChar);
                    pFirstName.Value = entity.FirstName;

                    System.Data.SqlClient.SqlParameter pTitle = command.Parameters.Add("@Title", System.Data.SqlDbType.NVarChar);
                    if (entity.Title == null)
                    {
                        pTitle.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pTitle.Value = entity.Title;
                    }

                    System.Data.SqlClient.SqlParameter pReportsTo = command.Parameters.Add("@ReportsTo", System.Data.SqlDbType.Int);
                    if (entity.ReportsTo == null)
                    {
                        pReportsTo.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pReportsTo.Value = entity.ReportsTo;
                    }

                    System.Data.SqlClient.SqlParameter pBirthDate = command.Parameters.Add("@BirthDate", System.Data.SqlDbType.DateTime);
                    if (entity.BirthDate == null)
                    {
                        pBirthDate.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pBirthDate.Value = entity.BirthDate;
                    }

                    System.Data.SqlClient.SqlParameter pHireDate = command.Parameters.Add("@HireDate", System.Data.SqlDbType.DateTime);
                    if (entity.HireDate == null)
                    {
                        pHireDate.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pHireDate.Value = entity.HireDate;
                    }

                    System.Data.SqlClient.SqlParameter pAddress = command.Parameters.Add("@Address", System.Data.SqlDbType.NVarChar);
                    if (entity.Address == null)
                    {
                        pAddress.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pAddress.Value = entity.Address;
                    }

                    System.Data.SqlClient.SqlParameter pCity = command.Parameters.Add("@City", System.Data.SqlDbType.NVarChar);
                    if (entity.City == null)
                    {
                        pCity.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pCity.Value = entity.City;
                    }

                    System.Data.SqlClient.SqlParameter pState = command.Parameters.Add("@State", System.Data.SqlDbType.NVarChar);
                    if (entity.State == null)
                    {
                        pState.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pState.Value = entity.State;
                    }

                    System.Data.SqlClient.SqlParameter pCountry = command.Parameters.Add("@Country", System.Data.SqlDbType.NVarChar);
                    if (entity.Country == null)
                    {
                        pCountry.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pCountry.Value = entity.Country;
                    }

                    System.Data.SqlClient.SqlParameter pPostalCode = command.Parameters.Add("@PostalCode", System.Data.SqlDbType.NVarChar);
                    if (entity.PostalCode == null)
                    {
                        pPostalCode.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pPostalCode.Value = entity.PostalCode;
                    }

                    System.Data.SqlClient.SqlParameter pPhone = command.Parameters.Add("@Phone", System.Data.SqlDbType.NVarChar);
                    if (entity.Phone == null)
                    {
                        pPhone.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pPhone.Value = entity.Phone;
                    }

                    System.Data.SqlClient.SqlParameter pFax = command.Parameters.Add("@Fax", System.Data.SqlDbType.NVarChar);
                    if (entity.Fax == null)
                    {
                        pFax.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pFax.Value = entity.Fax;
                    }

                    System.Data.SqlClient.SqlParameter pEmail = command.Parameters.Add("@Email", System.Data.SqlDbType.NVarChar);
                    if (entity.Email == null)
                    {
                        pEmail.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pEmail.Value = entity.Email;
                    }

                    if (command.ExecuteNonQuery() > 0)
                    {
                        entity.EmployeeId = (System.Int32)pEmployeeId.Value;
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

        public void UpdateEmployee(Employee entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("UPDATE [dbo].[Employee] SET [LastName] = @LastName, [FirstName] = @FirstName, [Title] = @Title, [ReportsTo] = @ReportsTo, [BirthDate] = @BirthDate, [HireDate] = @HireDate, [Address] = @Address, [City] = @City, [State] = @State, [Country] = @Country, [PostalCode] = @PostalCode, [Phone] = @Phone, [Fax] = @Fax, [Email] = @Email WHERE [EmployeeId] = @EmployeeId"))
            {
                try
                {
                    PopConnection(command);
                    System.Data.SqlClient.SqlParameter pEmployeeId = command.Parameters.Add("@EmployeeId", System.Data.SqlDbType.Int);
                    pEmployeeId.Value = entity.EmployeeId;

                    System.Data.SqlClient.SqlParameter pLastName = command.Parameters.Add("@LastName", System.Data.SqlDbType.NVarChar);
                    pLastName.Value = entity.LastName;

                    System.Data.SqlClient.SqlParameter pFirstName = command.Parameters.Add("@FirstName", System.Data.SqlDbType.NVarChar);
                    pFirstName.Value = entity.FirstName;

                    System.Data.SqlClient.SqlParameter pTitle = command.Parameters.Add("@Title", System.Data.SqlDbType.NVarChar);
                    if (entity.Title == null)
                    {
                        pTitle.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pTitle.Value = entity.Title;
                    }

                    System.Data.SqlClient.SqlParameter pReportsTo = command.Parameters.Add("@ReportsTo", System.Data.SqlDbType.Int);
                    if (entity.ReportsTo == null)
                    {
                        pReportsTo.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pReportsTo.Value = entity.ReportsTo;
                    }

                    System.Data.SqlClient.SqlParameter pBirthDate = command.Parameters.Add("@BirthDate", System.Data.SqlDbType.DateTime);
                    if (entity.BirthDate == null)
                    {
                        pBirthDate.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pBirthDate.Value = entity.BirthDate;
                    }

                    System.Data.SqlClient.SqlParameter pHireDate = command.Parameters.Add("@HireDate", System.Data.SqlDbType.DateTime);
                    if (entity.HireDate == null)
                    {
                        pHireDate.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pHireDate.Value = entity.HireDate;
                    }

                    System.Data.SqlClient.SqlParameter pAddress = command.Parameters.Add("@Address", System.Data.SqlDbType.NVarChar);
                    if (entity.Address == null)
                    {
                        pAddress.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pAddress.Value = entity.Address;
                    }

                    System.Data.SqlClient.SqlParameter pCity = command.Parameters.Add("@City", System.Data.SqlDbType.NVarChar);
                    if (entity.City == null)
                    {
                        pCity.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pCity.Value = entity.City;
                    }

                    System.Data.SqlClient.SqlParameter pState = command.Parameters.Add("@State", System.Data.SqlDbType.NVarChar);
                    if (entity.State == null)
                    {
                        pState.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pState.Value = entity.State;
                    }

                    System.Data.SqlClient.SqlParameter pCountry = command.Parameters.Add("@Country", System.Data.SqlDbType.NVarChar);
                    if (entity.Country == null)
                    {
                        pCountry.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pCountry.Value = entity.Country;
                    }

                    System.Data.SqlClient.SqlParameter pPostalCode = command.Parameters.Add("@PostalCode", System.Data.SqlDbType.NVarChar);
                    if (entity.PostalCode == null)
                    {
                        pPostalCode.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pPostalCode.Value = entity.PostalCode;
                    }

                    System.Data.SqlClient.SqlParameter pPhone = command.Parameters.Add("@Phone", System.Data.SqlDbType.NVarChar);
                    if (entity.Phone == null)
                    {
                        pPhone.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pPhone.Value = entity.Phone;
                    }

                    System.Data.SqlClient.SqlParameter pFax = command.Parameters.Add("@Fax", System.Data.SqlDbType.NVarChar);
                    if (entity.Fax == null)
                    {
                        pFax.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pFax.Value = entity.Fax;
                    }

                    System.Data.SqlClient.SqlParameter pEmail = command.Parameters.Add("@Email", System.Data.SqlDbType.NVarChar);
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

        public List<Employee> SelectEmployeeByReportsTo(System.Nullable<System.Int32> reportsTo)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [EmployeeId], [LastName], [FirstName], [Title], [ReportsTo], [BirthDate], [HireDate], [Address], [City], [State], [Country], [PostalCode], [Phone], [Fax], [Email] FROM [dbo].[Employee] WHERE [ReportsTo] = @ReportsTo"))
            {
                try
                {
                    PopConnection(command);
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

        #region Insert, Update, Delete, Select, Mapping - dbo.Genre

        public static Genre ReadGenre(System.Data.SqlClient.SqlDataReader reader)
        {
            Genre entity = new Genre();
            entity.GenreId = reader.GetInt32(0);
            entity.Name = reader.GetString(1);
            return entity;
        }

        public void InsertGenre(Genre entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("INSERT INTO [dbo].[Genre] ([Name]) VALUES (@Name); SET @GenreId = SCOPE_IDENTITY(); "))
            {
                try
                {
                    PopConnection(command);
                    System.Data.SqlClient.SqlParameter pGenreId = command.Parameters.Add("@GenreId", System.Data.SqlDbType.Int);
                    pGenreId.Direction = System.Data.ParameterDirection.Output;

                    System.Data.SqlClient.SqlParameter pName = command.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar);
                    if (entity.Name == null)
                    {
                        pName.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pName.Value = entity.Name;
                    }

                    if (command.ExecuteNonQuery() > 0)
                    {
                        entity.GenreId = (System.Int32)pGenreId.Value;
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

        public void UpdateGenre(Genre entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("UPDATE [dbo].[Genre] SET [Name] = @Name WHERE [GenreId] = @GenreId"))
            {
                try
                {
                    PopConnection(command);
                    System.Data.SqlClient.SqlParameter pGenreId = command.Parameters.Add("@GenreId", System.Data.SqlDbType.Int);
                    pGenreId.Value = entity.GenreId;

                    System.Data.SqlClient.SqlParameter pName = command.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar);
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

        public Genre SelectGenreByGenreId(System.Int32 genreId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [GenreId], [Name] FROM [dbo].[Genre] WHERE [GenreId] = @GenreId"))
            {
                try
                {
                    PopConnection(command);
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

        #region Insert, Update, Delete, Select, Mapping - dbo.Invoice

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

        public void InsertInvoice(Invoice entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("INSERT INTO [dbo].[Invoice] ([CustomerId], [InvoiceDate], [BillingAddress], [BillingCity], [BillingState], [BillingCountry], [BillingPostalCode], [Total]) VALUES (@CustomerId, @InvoiceDate, @BillingAddress, @BillingCity, @BillingState, @BillingCountry, @BillingPostalCode, @Total); SET @InvoiceId = SCOPE_IDENTITY(); "))
            {
                try
                {
                    PopConnection(command);
                    System.Data.SqlClient.SqlParameter pInvoiceId = command.Parameters.Add("@InvoiceId", System.Data.SqlDbType.Int);
                    pInvoiceId.Direction = System.Data.ParameterDirection.Output;

                    System.Data.SqlClient.SqlParameter pCustomerId = command.Parameters.Add("@CustomerId", System.Data.SqlDbType.Int);
                    pCustomerId.Value = entity.CustomerId;

                    System.Data.SqlClient.SqlParameter pInvoiceDate = command.Parameters.Add("@InvoiceDate", System.Data.SqlDbType.DateTime);
                    pInvoiceDate.Value = entity.InvoiceDate;

                    System.Data.SqlClient.SqlParameter pBillingAddress = command.Parameters.Add("@BillingAddress", System.Data.SqlDbType.NVarChar);
                    if (entity.BillingAddress == null)
                    {
                        pBillingAddress.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pBillingAddress.Value = entity.BillingAddress;
                    }

                    System.Data.SqlClient.SqlParameter pBillingCity = command.Parameters.Add("@BillingCity", System.Data.SqlDbType.NVarChar);
                    if (entity.BillingCity == null)
                    {
                        pBillingCity.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pBillingCity.Value = entity.BillingCity;
                    }

                    System.Data.SqlClient.SqlParameter pBillingState = command.Parameters.Add("@BillingState", System.Data.SqlDbType.NVarChar);
                    if (entity.BillingState == null)
                    {
                        pBillingState.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pBillingState.Value = entity.BillingState;
                    }

                    System.Data.SqlClient.SqlParameter pBillingCountry = command.Parameters.Add("@BillingCountry", System.Data.SqlDbType.NVarChar);
                    if (entity.BillingCountry == null)
                    {
                        pBillingCountry.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pBillingCountry.Value = entity.BillingCountry;
                    }

                    System.Data.SqlClient.SqlParameter pBillingPostalCode = command.Parameters.Add("@BillingPostalCode", System.Data.SqlDbType.NVarChar);
                    if (entity.BillingPostalCode == null)
                    {
                        pBillingPostalCode.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pBillingPostalCode.Value = entity.BillingPostalCode;
                    }

                    System.Data.SqlClient.SqlParameter pTotal = command.Parameters.Add("@Total", System.Data.SqlDbType.Decimal);
                    pTotal.Value = entity.Total;

                    if (command.ExecuteNonQuery() > 0)
                    {
                        entity.InvoiceId = (System.Int32)pInvoiceId.Value;
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

        public void UpdateInvoice(Invoice entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("UPDATE [dbo].[Invoice] SET [CustomerId] = @CustomerId, [InvoiceDate] = @InvoiceDate, [BillingAddress] = @BillingAddress, [BillingCity] = @BillingCity, [BillingState] = @BillingState, [BillingCountry] = @BillingCountry, [BillingPostalCode] = @BillingPostalCode, [Total] = @Total WHERE [InvoiceId] = @InvoiceId"))
            {
                try
                {
                    PopConnection(command);
                    System.Data.SqlClient.SqlParameter pInvoiceId = command.Parameters.Add("@InvoiceId", System.Data.SqlDbType.Int);
                    pInvoiceId.Value = entity.InvoiceId;

                    System.Data.SqlClient.SqlParameter pCustomerId = command.Parameters.Add("@CustomerId", System.Data.SqlDbType.Int);
                    pCustomerId.Value = entity.CustomerId;

                    System.Data.SqlClient.SqlParameter pInvoiceDate = command.Parameters.Add("@InvoiceDate", System.Data.SqlDbType.DateTime);
                    pInvoiceDate.Value = entity.InvoiceDate;

                    System.Data.SqlClient.SqlParameter pBillingAddress = command.Parameters.Add("@BillingAddress", System.Data.SqlDbType.NVarChar);
                    if (entity.BillingAddress == null)
                    {
                        pBillingAddress.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pBillingAddress.Value = entity.BillingAddress;
                    }

                    System.Data.SqlClient.SqlParameter pBillingCity = command.Parameters.Add("@BillingCity", System.Data.SqlDbType.NVarChar);
                    if (entity.BillingCity == null)
                    {
                        pBillingCity.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pBillingCity.Value = entity.BillingCity;
                    }

                    System.Data.SqlClient.SqlParameter pBillingState = command.Parameters.Add("@BillingState", System.Data.SqlDbType.NVarChar);
                    if (entity.BillingState == null)
                    {
                        pBillingState.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pBillingState.Value = entity.BillingState;
                    }

                    System.Data.SqlClient.SqlParameter pBillingCountry = command.Parameters.Add("@BillingCountry", System.Data.SqlDbType.NVarChar);
                    if (entity.BillingCountry == null)
                    {
                        pBillingCountry.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pBillingCountry.Value = entity.BillingCountry;
                    }

                    System.Data.SqlClient.SqlParameter pBillingPostalCode = command.Parameters.Add("@BillingPostalCode", System.Data.SqlDbType.NVarChar);
                    if (entity.BillingPostalCode == null)
                    {
                        pBillingPostalCode.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pBillingPostalCode.Value = entity.BillingPostalCode;
                    }

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

        public List<Invoice> SelectInvoiceByCustomerId(System.Int32 customerId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [InvoiceId], [CustomerId], [InvoiceDate], [BillingAddress], [BillingCity], [BillingState], [BillingCountry], [BillingPostalCode], [Total] FROM [dbo].[Invoice] WHERE [CustomerId] = @CustomerId"))
            {
                try
                {
                    PopConnection(command);
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

        #region Insert, Update, Delete, Select, Mapping - dbo.InvoiceLine

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

        public void InsertInvoiceLine(InvoiceLine entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("INSERT INTO [dbo].[InvoiceLine] ([InvoiceId], [TrackId], [UnitPrice], [Quantity]) VALUES (@InvoiceId, @TrackId, @UnitPrice, @Quantity); SET @InvoiceLineId = SCOPE_IDENTITY(); "))
            {
                try
                {
                    PopConnection(command);
                    System.Data.SqlClient.SqlParameter pInvoiceLineId = command.Parameters.Add("@InvoiceLineId", System.Data.SqlDbType.Int);
                    pInvoiceLineId.Direction = System.Data.ParameterDirection.Output;

                    System.Data.SqlClient.SqlParameter pInvoiceId = command.Parameters.Add("@InvoiceId", System.Data.SqlDbType.Int);
                    pInvoiceId.Value = entity.InvoiceId;

                    System.Data.SqlClient.SqlParameter pTrackId = command.Parameters.Add("@TrackId", System.Data.SqlDbType.Int);
                    pTrackId.Value = entity.TrackId;

                    System.Data.SqlClient.SqlParameter pUnitPrice = command.Parameters.Add("@UnitPrice", System.Data.SqlDbType.Decimal);
                    pUnitPrice.Value = entity.UnitPrice;

                    System.Data.SqlClient.SqlParameter pQuantity = command.Parameters.Add("@Quantity", System.Data.SqlDbType.Int);
                    pQuantity.Value = entity.Quantity;

                    if (command.ExecuteNonQuery() > 0)
                    {
                        entity.InvoiceLineId = (System.Int32)pInvoiceLineId.Value;
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

        public void UpdateInvoiceLine(InvoiceLine entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("UPDATE [dbo].[InvoiceLine] SET [InvoiceId] = @InvoiceId, [TrackId] = @TrackId, [UnitPrice] = @UnitPrice, [Quantity] = @Quantity WHERE [InvoiceLineId] = @InvoiceLineId"))
            {
                try
                {
                    PopConnection(command);
                    System.Data.SqlClient.SqlParameter pInvoiceLineId = command.Parameters.Add("@InvoiceLineId", System.Data.SqlDbType.Int);
                    pInvoiceLineId.Value = entity.InvoiceLineId;

                    System.Data.SqlClient.SqlParameter pInvoiceId = command.Parameters.Add("@InvoiceId", System.Data.SqlDbType.Int);
                    pInvoiceId.Value = entity.InvoiceId;

                    System.Data.SqlClient.SqlParameter pTrackId = command.Parameters.Add("@TrackId", System.Data.SqlDbType.Int);
                    pTrackId.Value = entity.TrackId;

                    System.Data.SqlClient.SqlParameter pUnitPrice = command.Parameters.Add("@UnitPrice", System.Data.SqlDbType.Decimal);
                    pUnitPrice.Value = entity.UnitPrice;

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

        public List<InvoiceLine> SelectInvoiceLineByInvoiceId(System.Int32 invoiceId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [InvoiceLineId], [InvoiceId], [TrackId], [UnitPrice], [Quantity] FROM [dbo].[InvoiceLine] WHERE [InvoiceId] = @InvoiceId"))
            {
                try
                {
                    PopConnection(command);
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

        #region Insert, Update, Delete, Select, Mapping - dbo.MediaType

        public static MediaType ReadMediaType(System.Data.SqlClient.SqlDataReader reader)
        {
            MediaType entity = new MediaType();
            entity.MediaTypeId = reader.GetInt32(0);
            entity.Name = reader.GetString(1);
            return entity;
        }

        public void InsertMediaType(MediaType entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("INSERT INTO [dbo].[MediaType] ([Name]) VALUES (@Name); SET @MediaTypeId = SCOPE_IDENTITY(); "))
            {
                try
                {
                    PopConnection(command);
                    System.Data.SqlClient.SqlParameter pMediaTypeId = command.Parameters.Add("@MediaTypeId", System.Data.SqlDbType.Int);
                    pMediaTypeId.Direction = System.Data.ParameterDirection.Output;

                    System.Data.SqlClient.SqlParameter pName = command.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar);
                    if (entity.Name == null)
                    {
                        pName.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pName.Value = entity.Name;
                    }

                    if (command.ExecuteNonQuery() > 0)
                    {
                        entity.MediaTypeId = (System.Int32)pMediaTypeId.Value;
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

        public void UpdateMediaType(MediaType entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("UPDATE [dbo].[MediaType] SET [Name] = @Name WHERE [MediaTypeId] = @MediaTypeId"))
            {
                try
                {
                    PopConnection(command);
                    System.Data.SqlClient.SqlParameter pMediaTypeId = command.Parameters.Add("@MediaTypeId", System.Data.SqlDbType.Int);
                    pMediaTypeId.Value = entity.MediaTypeId;

                    System.Data.SqlClient.SqlParameter pName = command.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar);
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

        public MediaType SelectMediaTypeByMediaTypeId(System.Int32 mediaTypeId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [MediaTypeId], [Name] FROM [dbo].[MediaType] WHERE [MediaTypeId] = @MediaTypeId"))
            {
                try
                {
                    PopConnection(command);
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

        #region Insert, Update, Delete, Select, Mapping - dbo.Playlist

        public static Playlist ReadPlaylist(System.Data.SqlClient.SqlDataReader reader)
        {
            Playlist entity = new Playlist();
            entity.PlaylistId = reader.GetInt32(0);
            entity.Name = reader.GetString(1);
            return entity;
        }

        public void InsertPlaylist(Playlist entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("INSERT INTO [dbo].[Playlist] ([Name]) VALUES (@Name); SET @PlaylistId = SCOPE_IDENTITY(); "))
            {
                try
                {
                    PopConnection(command);
                    System.Data.SqlClient.SqlParameter pPlaylistId = command.Parameters.Add("@PlaylistId", System.Data.SqlDbType.Int);
                    pPlaylistId.Direction = System.Data.ParameterDirection.Output;

                    System.Data.SqlClient.SqlParameter pName = command.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar);
                    if (entity.Name == null)
                    {
                        pName.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pName.Value = entity.Name;
                    }

                    if (command.ExecuteNonQuery() > 0)
                    {
                        entity.PlaylistId = (System.Int32)pPlaylistId.Value;
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

        public void UpdatePlaylist(Playlist entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("UPDATE [dbo].[Playlist] SET [Name] = @Name WHERE [PlaylistId] = @PlaylistId"))
            {
                try
                {
                    PopConnection(command);
                    System.Data.SqlClient.SqlParameter pPlaylistId = command.Parameters.Add("@PlaylistId", System.Data.SqlDbType.Int);
                    pPlaylistId.Value = entity.PlaylistId;

                    System.Data.SqlClient.SqlParameter pName = command.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar);
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

        public Playlist SelectPlaylistByPlaylistId(System.Int32 playlistId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [PlaylistId], [Name] FROM [dbo].[Playlist] WHERE [PlaylistId] = @PlaylistId"))
            {
                try
                {
                    PopConnection(command);
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

        #region Insert, Update, Delete, Select, Mapping - dbo.PlaylistTrack

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

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("INSERT INTO [dbo].[PlaylistTrack] ([PlaylistId], [TrackId]) VALUES (@PlaylistId, @TrackId);"))
            {
                try
                {
                    PopConnection(command);
                    System.Data.SqlClient.SqlParameter pPlaylistId = command.Parameters.Add("@PlaylistId", System.Data.SqlDbType.Int);
                    pPlaylistId.Value = entity.PlaylistId;

                    System.Data.SqlClient.SqlParameter pTrackId = command.Parameters.Add("@TrackId", System.Data.SqlDbType.Int);
                    pTrackId.Value = entity.TrackId;

                    if (command.ExecuteNonQuery() > 0)
                    {
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

        public void UpdatePlaylistTrack(PlaylistTrack entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("UPDATE [dbo].[PlaylistTrack] SET  WHERE [PlaylistId] = @PlaylistId AND [TrackId] = @TrackId"))
            {
                try
                {
                    PopConnection(command);
                    System.Data.SqlClient.SqlParameter pPlaylistId = command.Parameters.Add("@PlaylistId", System.Data.SqlDbType.Int);
                    pPlaylistId.Value = entity.PlaylistId;

                    System.Data.SqlClient.SqlParameter pTrackId = command.Parameters.Add("@TrackId", System.Data.SqlDbType.Int);
                    pTrackId.Value = entity.TrackId;

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

        public List<PlaylistTrack> SelectPlaylistTrackByTrackId(System.Int32 trackId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [PlaylistId], [TrackId] FROM [dbo].[PlaylistTrack] WHERE [TrackId] = @TrackId"))
            {
                try
                {
                    PopConnection(command);
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
                    System.Data.SqlClient.SqlParameter pPlaylistId = command.Parameters.Add("@PlaylistId", System.Data.SqlDbType.Int);
                    pPlaylistId.Value = playlistId;

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
                    System.Data.SqlClient.SqlParameter pPlaylistId = command.Parameters.Add("@PlaylistId", System.Data.SqlDbType.Int);
                    pPlaylistId.Value = playlistId;

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

        #region Insert, Update, Delete, Select, Mapping - dbo.Track

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

        public void InsertTrack(Track entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("INSERT INTO [dbo].[Track] ([Name], [AlbumId], [MediaTypeId], [GenreId], [Composer], [Milliseconds], [Bytes], [UnitPrice]) VALUES (@Name, @AlbumId, @MediaTypeId, @GenreId, @Composer, @Milliseconds, @Bytes, @UnitPrice); SET @TrackId = SCOPE_IDENTITY(); "))
            {
                try
                {
                    PopConnection(command);
                    System.Data.SqlClient.SqlParameter pTrackId = command.Parameters.Add("@TrackId", System.Data.SqlDbType.Int);
                    pTrackId.Direction = System.Data.ParameterDirection.Output;

                    System.Data.SqlClient.SqlParameter pName = command.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar);
                    pName.Value = entity.Name;

                    System.Data.SqlClient.SqlParameter pAlbumId = command.Parameters.Add("@AlbumId", System.Data.SqlDbType.Int);
                    if (entity.AlbumId == null)
                    {
                        pAlbumId.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pAlbumId.Value = entity.AlbumId;
                    }

                    System.Data.SqlClient.SqlParameter pMediaTypeId = command.Parameters.Add("@MediaTypeId", System.Data.SqlDbType.Int);
                    pMediaTypeId.Value = entity.MediaTypeId;

                    System.Data.SqlClient.SqlParameter pGenreId = command.Parameters.Add("@GenreId", System.Data.SqlDbType.Int);
                    if (entity.GenreId == null)
                    {
                        pGenreId.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pGenreId.Value = entity.GenreId;
                    }

                    System.Data.SqlClient.SqlParameter pComposer = command.Parameters.Add("@Composer", System.Data.SqlDbType.NVarChar);
                    if (entity.Composer == null)
                    {
                        pComposer.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pComposer.Value = entity.Composer;
                    }

                    System.Data.SqlClient.SqlParameter pMilliseconds = command.Parameters.Add("@Milliseconds", System.Data.SqlDbType.Int);
                    pMilliseconds.Value = entity.Milliseconds;

                    System.Data.SqlClient.SqlParameter pBytes = command.Parameters.Add("@Bytes", System.Data.SqlDbType.Int);
                    if (entity.Bytes == null)
                    {
                        pBytes.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pBytes.Value = entity.Bytes;
                    }

                    System.Data.SqlClient.SqlParameter pUnitPrice = command.Parameters.Add("@UnitPrice", System.Data.SqlDbType.Decimal);
                    pUnitPrice.Value = entity.UnitPrice;

                    if (command.ExecuteNonQuery() > 0)
                    {
                        entity.TrackId = (System.Int32)pTrackId.Value;
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

        public void UpdateTrack(Track entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("UPDATE [dbo].[Track] SET [Name] = @Name, [AlbumId] = @AlbumId, [MediaTypeId] = @MediaTypeId, [GenreId] = @GenreId, [Composer] = @Composer, [Milliseconds] = @Milliseconds, [Bytes] = @Bytes, [UnitPrice] = @UnitPrice WHERE [TrackId] = @TrackId"))
            {
                try
                {
                    PopConnection(command);
                    System.Data.SqlClient.SqlParameter pTrackId = command.Parameters.Add("@TrackId", System.Data.SqlDbType.Int);
                    pTrackId.Value = entity.TrackId;

                    System.Data.SqlClient.SqlParameter pName = command.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar);
                    pName.Value = entity.Name;

                    System.Data.SqlClient.SqlParameter pAlbumId = command.Parameters.Add("@AlbumId", System.Data.SqlDbType.Int);
                    if (entity.AlbumId == null)
                    {
                        pAlbumId.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pAlbumId.Value = entity.AlbumId;
                    }

                    System.Data.SqlClient.SqlParameter pMediaTypeId = command.Parameters.Add("@MediaTypeId", System.Data.SqlDbType.Int);
                    pMediaTypeId.Value = entity.MediaTypeId;

                    System.Data.SqlClient.SqlParameter pGenreId = command.Parameters.Add("@GenreId", System.Data.SqlDbType.Int);
                    if (entity.GenreId == null)
                    {
                        pGenreId.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pGenreId.Value = entity.GenreId;
                    }

                    System.Data.SqlClient.SqlParameter pComposer = command.Parameters.Add("@Composer", System.Data.SqlDbType.NVarChar);
                    if (entity.Composer == null)
                    {
                        pComposer.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pComposer.Value = entity.Composer;
                    }

                    System.Data.SqlClient.SqlParameter pMilliseconds = command.Parameters.Add("@Milliseconds", System.Data.SqlDbType.Int);
                    pMilliseconds.Value = entity.Milliseconds;

                    System.Data.SqlClient.SqlParameter pBytes = command.Parameters.Add("@Bytes", System.Data.SqlDbType.Int);
                    if (entity.Bytes == null)
                    {
                        pBytes.Value = System.DBNull.Value;
                    }
                    else
                    {
                        pBytes.Value = entity.Bytes;
                    }

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

        public List<Track> SelectTrackByAlbumId(System.Nullable<System.Int32> albumId)
        {
            using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT [TrackId], [Name], [AlbumId], [MediaTypeId], [GenreId], [Composer], [Milliseconds], [Bytes], [UnitPrice] FROM [dbo].[Track] WHERE [AlbumId] = @AlbumId"))
            {
                try
                {
                    PopConnection(command);
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
        
        public void BeginTransaction()
        {
            if (this.connection == null)
            {
                this.connection = new System.Data.SqlClient.SqlConnection(this.connectionString);
                this.connection.Open();
            }
        
            if (this.transaction == null)
            {
                this.transaction = this.connection.BeginTransaction();
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
