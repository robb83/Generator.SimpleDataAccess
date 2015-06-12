using Generator.SimpleDataAccess.Samples;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Generator.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
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
        }
    }
}
